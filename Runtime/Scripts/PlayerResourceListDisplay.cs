using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Attach to a GameObject with a Vertical Layout Group (or as the content parent). Creates one instance
/// of the row prefab per resource in the list, each configured to show that resource's count for the selected player.
/// Assign a prefab that contains a ResourceCounterDisplay; the script sets resourceToTrack, displayMode, playerSlot, and playerList on each instance.
/// </summary>
public class PlayerResourceListDisplay : MonoBehaviour
{
    [Tooltip("Optional: label to show the selected player's name. Set at runtime from Player List.")]
    public TMP_Text playerNameText;

    [Tooltip("Prefab for each row; must contain a ResourceCounterDisplay component.")]
    public GameObject rowPrefab;

    [Tooltip("Which player (1–4) to show resources for. Uses Player List to resolve the player.")]
    [Range(0, 3)]
    public int playerSlot;

    [Tooltip("Source of players and names. If not set, finds the single playersInfo in the scene on Start.")]
    public playersInfo playerList;

    [Tooltip("Resource types to show one row each. Populate in the editor (e.g. via the custom inspector).")]
    public List<Resource> resourcesToDisplay = new List<Resource>();

    [Header("Backgrounds")]
    [Tooltip("Optional: image behind the player name area.")]
    public Image playerBackgroundImage;
    [Tooltip("Color tint for the player background image.")]
    public Color playerBackgroundColor = new Color(0f, 0f, 0f, 0.5f);
    [Tooltip("Show or hide the player background image.")]
    public bool showPlayerBackground = true;

    [Tooltip("Optional: image behind the resource list area.")]
    public Image listBackgroundImage;
    [Tooltip("Color tint for the list background image.")]
    public Color listBackgroundColor = new Color(0f, 0f, 0f, 0.5f);
    [Tooltip("Show or hide the list background image.")]
    public bool showListBackground = true;

    [Header("Row scaling")]
    [Tooltip("Scale factor applied to each instantiated row prefab (e.g. 1 = normal, 0.5 = half size).")]
    public float rowScaleFactor = 1f;

    private readonly List<GameObject> _instances = new List<GameObject>();

    private void Start()
    {
        if (playerList == null)
            playerList = FindObjectOfType<playersInfo>();
        RebuildList();
    }

    private void OnDestroy()
    {
        ClearInstances();
    }

    /// <summary>
    /// Rebuilds the list: clears existing rows and creates one row per resource, each tracking the selected player.
    /// </summary>
    public void RebuildList()
    {
        ClearInstances();

        if (rowPrefab == null || resourcesToDisplay == null || resourcesToDisplay.Count == 0)
            return;

        playersInfo source = playerList != null ? playerList : FindObjectOfType<playersInfo>();
        if (source == null)
            return;

        if (playerNameText != null && playerSlot >= 0 && playerSlot < source.playerNames.Count)
            playerNameText.text = source.playerNames[playerSlot];

        ApplyBackgrounds();

        foreach (Resource resource in resourcesToDisplay)
        {
            if (resource == null) continue;

            GameObject instance = Instantiate(rowPrefab, transform);
            instance.transform.localScale = Vector3.one * Mathf.Max(0.01f, rowScaleFactor);
            _instances.Add(instance);

            var display = instance.GetComponentInChildren<ResourceCounterDisplay>();
            if (display != null)
            {
                display.resourceToTrack = resource;
                display.displayMode = ResourceCountDisplayMode.OwnedByPlayer;
                display.playerSlot = playerSlot;
                display.playerList = source;
            }
        }
    }

    private void ApplyBackgrounds()
    {
        if (playerBackgroundImage != null)
        {
            playerBackgroundImage.enabled = showPlayerBackground;
            playerBackgroundImage.color = playerBackgroundColor;
        }
        if (listBackgroundImage != null)
        {
            listBackgroundImage.enabled = showListBackground;
            listBackgroundImage.color = listBackgroundColor;
        }
    }

    private void ClearInstances()
    {
        foreach (GameObject go in _instances)
        {
            if (go != null)
                Destroy(go);
        }
        _instances.Clear();
    }
}
