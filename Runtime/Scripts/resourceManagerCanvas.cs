using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class resourceManagerCanvas : MonoBehaviour
{

    public GameObject timerModule;
    public GameObject goalTrackerModule;
    public TMP_Text globalScoreModule;

    [Tooltip("Optional list of score UI elements (max 4). When set, these are shown/hidden and updated based on TeamManager. If empty, globalScoreModule is used for a single score.")]
    public List<TMP_Text> scoreItems = new List<TMP_Text>();

    [Tooltip("When false, all score UI (scoreItems and globalScoreModule) is hidden. Set by Create Resource Management System or at runtime.")]
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
            if (globalScoreModule != null)
                globalScoreModule.gameObject.SetActive(false);
            if (scoreItems != null)
            {
                for (int i = 0; i < scoreItems.Count; i++)
                {
                    if (scoreItems[i] != null)
                        scoreItems[i].gameObject.SetActive(false);
                }
            }
            return;
        }

        bool useScoreItems = scoreItems != null && scoreItems.Count > 0;

        if (TeamManager.Instance != null)
        {
            int count = TeamManager.Instance.GetScoreDisplayCount();
            if (useScoreItems)
            {
                for (int i = 0; i < scoreItems.Count && i < TeamManager.MaxScoreDisplayCount; i++)
                {
                    if (scoreItems[i] == null) continue;
                    bool active = i < count;
                    scoreItems[i].gameObject.SetActive(active);
                    if (active)
                    {
                        scoreItems[i].text = TeamManager.Instance.GetScoreLabel(i) + ": " + TeamManager.Instance.GetScoreValue(i).ToString();
                    }
                }
            }
            else if (globalScoreModule != null)
            {
                globalScoreModule.gameObject.SetActive(true);
                globalScoreModule.text = "Score: " + TeamManager.Instance.GetScoreForLevel().ToString();
            }
        }
        else
        {
            int singleScore = ResourceManager.Instance != null ? ResourceManager.Instance.globalCapital : 0;
            if (useScoreItems && scoreItems[0] != null)
            {
                scoreItems[0].gameObject.SetActive(true);
                scoreItems[0].text = "Score: " + singleScore.ToString();
                for (int i = 1; i < scoreItems.Count && i < TeamManager.MaxScoreDisplayCount; i++)
                {
                    if (scoreItems[i] != null)
                        scoreItems[i].gameObject.SetActive(false);
                }
            }
            else if (globalScoreModule != null)
            {
                globalScoreModule.gameObject.SetActive(true);
                globalScoreModule.text = "Score: " + singleScore.ToString();
            }
        }
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
