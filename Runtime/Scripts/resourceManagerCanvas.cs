using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class resourceManagerCanvas : MonoBehaviour
{

    public GameObject timerModule;
    public GameObject goalTrackerModule;
    [FormerlySerializedAs("globalScoreModule")]
    [Tooltip("Text used for independent/general score display (shared total).")]
    public TMP_Text totalScoreText;

    [FormerlySerializedAs("scoreItems")]
    [Tooltip("Optional list of player score texts (max 4), one slot per player.")]
    public List<TMP_Text> playerScoreTexts = new List<TMP_Text>();

    [Tooltip("When false, all score UI (playerScoreTexts and totalScoreText) is hidden. Set by Create Resource Management System or at runtime.")]
    public bool scoreDisplayEnabled = true;

    //game states:
    public GameObject pauseScreen;
    public GameObject endOfChallengeScreeen;

    public List<GameObject> playerJoinPanels;

    public playersInfo playerInfoManager;

    void Start()
    {
        playerInfoManager = GameObject.FindAnyObjectByType<playersInfo>();
    }

    void Update()
    {
        updatePlayerInvites();
        UpdateScoreDisplay();
    }

    /// <summary>Refreshes score UI from TeamManager (or ResourceManager when no TeamManager). Turns on/off score items based on how many scores are tracked.</summary>
    public void UpdateScoreDisplay()
    {
        if (!scoreDisplayEnabled)
        {
            SetTotalScoreVisible(false);
            SetAllPlayerScoresVisible(false);
            return;
        }

        if (TeamManager.Instance != null)
        {
            bool showPlayerScores = TeamManager.Instance.UsesPerPlayerScores();

            if (showPlayerScores)
            {
                ShowPlayerScoresFromTeamManager();
            }
            else
            {
                SetAllPlayerScoresVisible(false);
                SetTotalScoreVisible(true);
                if (totalScoreText != null)
                    totalScoreText.text = "Total Score: " + TeamManager.Instance.GetScoreForLevel().ToString();
            }
        }
        else
        {
            SetAllPlayerScoresVisible(false);
            SetTotalScoreVisible(true);
            int singleScore = ResourceManager.Instance != null ? ResourceManager.Instance.globalCapital : 0;
            if (totalScoreText != null)
                totalScoreText.text = "Total Score: " + singleScore.ToString();
        }
    }

    private void ShowPlayerScoresFromTeamManager()
    {
        int displayCount = TeamManager.Instance.GetScoreDisplayCount();
        List<TMP_Text> usableScoreTexts = GetUsablePlayerScoreTexts();
        bool totalTextIsUsedAsPlayerSlot = totalScoreText != null && usableScoreTexts.Contains(totalScoreText);

        // If total score text is also used as a player slot in the scene, keep it visible here.
        if (!totalTextIsUsedAsPlayerSlot) SetTotalScoreVisible(false);

        for (int i = 0; i < usableScoreTexts.Count; i++)
        {
            if (usableScoreTexts[i] != null)
                usableScoreTexts[i].gameObject.SetActive(false);
        }

        int visibleSlots = Mathf.Min(Mathf.Min(TeamManager.MaxScoreDisplayCount, displayCount), usableScoreTexts.Count);
        for (int i = 0; i < visibleSlots; i++)
        {
            TMP_Text scoreText = usableScoreTexts[i];
            if (scoreText == null) continue;
            scoreText.gameObject.SetActive(true);
            scoreText.text = "Player " + (i + 1) + ": " + TeamManager.Instance.GetScoreValue(i).ToString();
        }
    }

    private void SetTotalScoreVisible(bool visible)
    {
        if (totalScoreText != null)
            totalScoreText.gameObject.SetActive(visible);
    }

    private void SetAllPlayerScoresVisible(bool visible)
    {
        if (playerScoreTexts == null) return;
        for (int i = 0; i < playerScoreTexts.Count; i++)
        {
            if (playerScoreTexts[i] != null)
                playerScoreTexts[i].gameObject.SetActive(visible);
        }
    }

    private List<TMP_Text> GetUsablePlayerScoreTexts()
    {
        var usable = new List<TMP_Text>();
        if (playerScoreTexts == null) return usable;

        for (int i = 0; i < playerScoreTexts.Count; i++)
        {
            TMP_Text txt = playerScoreTexts[i];
            if (txt == null) continue;
            if (!usable.Contains(txt)) usable.Add(txt);
        }
        return usable;
    }

    public void updatePlayerInvites()
    {
        if (playerInfoManager != null)
        {
            for (int i = 0; i < playerJoinPanels.Count; i++)
            {
                if (i >= playerInfoManager.allPlayers.Count)
                {
                    playerJoinPanels[i].SetActive(true);
                } else
                {
                    playerJoinPanels[i].SetActive(false);
                }
            }
        }
    }
}
