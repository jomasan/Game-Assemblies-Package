using UnityEngine;
using TMPro;

/// <summary>
/// What score to show: a specific player, a specific team, or the global/level score.
/// </summary>
public enum TeamScoreDisplayType
{
    PlayerScore,
    TeamScore,
    GlobalScore
}

/// <summary>
/// Attach to a GameObject with a TextMeshPro text component to display a score from TeamManager.
/// Choose Player Score (with player dropdown), Team Score (with team dropdown), or Global Score.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class TeamScoreDisplay : MonoBehaviour
{
    [Tooltip("Which score to show: a player, a team, or the global/level score.")]
    public TeamScoreDisplayType displayType = TeamScoreDisplayType.GlobalScore;

    [Tooltip("When Display Type is Player Score, which player (1–4). Uses Player List to resolve.")]
    [Range(0, 3)]
    public int playerSlot;

    [Tooltip("When Display Type is Team Score, which team (1–3).")]
    [Range(0, 2)]
    public int teamSlot;

    [Tooltip("Optional: source of players for Player Score mode. If not set, finds the single playersInfo in the scene on Start.")]
    public playersInfo playerList;

    [Tooltip("If true, display as \"Score label\" + score (e.g. \"Score: 42\"). If false, display only the score.")]
    public bool addName = true;

    [Tooltip("Label shown before the score when Add Name is enabled.")]
    public string scoreLabel = "Score: ";

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
        RefreshScore();
    }

    /// <summary>
    /// Refreshes the displayed score from TeamManager. Call manually if needed outside Update.
    /// </summary>
    public void RefreshScore()
    {
        if (_text == null) return;
        if (TeamManager.Instance == null)
        {
            _text.text = "";
            return;
        }

        int score = GetScore();
        _text.text = addName ? scoreLabel + score.ToString() : score.ToString();
    }

    private int GetScore()
    {
        var tm = TeamManager.Instance;
        switch (displayType)
        {
            case TeamScoreDisplayType.GlobalScore:
                return tm.GetScoreForLevel();
            case TeamScoreDisplayType.PlayerScore:
                playerController pc = ResolvePlayer();
                return pc != null ? tm.GetPlayerScore(pc) : 0;
            case TeamScoreDisplayType.TeamScore:
                return tm.GetTeamScore(teamSlot);
            default:
                return tm.GetScoreForLevel();
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
