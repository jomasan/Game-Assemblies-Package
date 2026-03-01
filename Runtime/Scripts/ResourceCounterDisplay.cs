using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// What to count: all of the resource type, all owned by a specific player, or all un-owned.
/// </summary>
public enum ResourceCountDisplayMode
{
    AllOfType,
    OwnedByPlayer,
    Unowned
}

/// <summary>
/// Attach to a GameObject with a TextMeshPro text component to display the current count
/// of a chosen resource. Requires a ResourceManager in the scene; the counter updates each frame.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class ResourceCounterDisplay : MonoBehaviour
{
    [Tooltip("Resource type to count. Assign via the Resource dropdown in the inspector.")]
    public Resource resourceToTrack;

    [Tooltip("What to count: all of this type, all owned by a chosen player, or all un-owned.")]
    public ResourceCountDisplayMode displayMode = ResourceCountDisplayMode.AllOfType;

    [Tooltip("When Display Mode is OwnedByPlayer, which player (1–4). Requires Player List to be set or a playersInfo in the scene.")]
    [Range(0, 3)]
    public int playerSlot;

    [Tooltip("Optional: source of players for OwnedByPlayer mode. If not set, finds the single playersInfo in the scene on Start.")]
    public playersInfo playerList;

    [Tooltip("If true, display as \"Resource name: count\". If false, display only the count.")]
    public bool includeName;

    [Tooltip("If true, show the resource icon in the assigned Image. The Image field appears below when this is on.")]
    public bool showIcon;

    [Tooltip("Image used to display the resource icon. Only used when Show Icon is enabled.")]
    public Image iconImage;

    private TMP_Text _text;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
        if (_text == null)
            _text = GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
        if (playerList == null)
            playerList = FindObjectOfType<playersInfo>();
    }

    private void Update()
    {
        RefreshCount();
    }

    /// <summary>
    /// Refreshes the displayed count from ResourceManager. Call manually if you need to update outside Update.
    /// </summary>
    public void RefreshCount()
    {
        if (_text == null) return;
        if (ResourceManager.Instance == null)
        {
            _text.text = "";
            if (iconImage != null) iconImage.enabled = false;
            return;
        }
        if (resourceToTrack == null)
        {
            _text.text = "";
            if (iconImage != null) iconImage.enabled = false;
            return;
        }

        int count = GetCount();
        _text.text = includeName ? $"{resourceToTrack.resourceName}: {count}" : count.ToString();

        if (showIcon && iconImage != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = resourceToTrack.icon;
            iconImage.color = resourceToTrack.iconTint;
        }
        else if (iconImage != null)
        {
            iconImage.enabled = false;
        }
    }

    private int GetCount()
    {
        var rm = ResourceManager.Instance;
        switch (displayMode)
        {
            case ResourceCountDisplayMode.AllOfType:
                return rm.GetResourceCountAll(resourceToTrack);
            case ResourceCountDisplayMode.OwnedByPlayer:
                playerController pc = ResolvePlayer();
                return pc != null ? rm.GetResourceCount(resourceToTrack, pc) : 0;
            case ResourceCountDisplayMode.Unowned:
                return rm.GetResourceCountUnowned(resourceToTrack);
            default:
                return rm.GetResourceCountAll(resourceToTrack);
        }
    }

    private playerController ResolvePlayer()
    {
        playersInfo source = playerList != null ? playerList : FindObjectOfType<playersInfo>();
        if (source == null || source.allControllers == null || playerSlot < 0 || playerSlot >= source.allControllers.Count)
            return null;
        return source.allControllers[playerSlot];
    }
}
