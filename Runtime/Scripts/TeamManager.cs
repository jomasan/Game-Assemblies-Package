using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Determines which players are on which team or working solo, and routes score updates
/// to a shared pool (EveryoneOneTeam), per-team (Teams), or per-player (Solo).
/// Provides a single "level score" for results and star brackets.
/// </summary>
public class TeamManager : MonoBehaviour
{
    public static TeamManager Instance { get; private set; }

    public enum Mode
    {
        EveryoneOneTeam,
        Teams,
        Solo
    }

    public enum LevelScoreAggregate
    {
        SumAll,
        Max
    }

    [Header("Configuration")]
    [Tooltip("EveryoneOneTeam = one shared score (ResourceManager.globalCapital). Teams = per-team scores. Solo = per-player scores.")]
    public Mode mode = Mode.EveryoneOneTeam;
    [Tooltip("When mode is Teams, how to derive the level score for results/stars.")]
    public LevelScoreAggregate levelScoreAggregate = LevelScoreAggregate.SumAll;

    [Header("Player list (optional)")]
    [Tooltip("If set, TeamManager registers these players in Start and applies default team assignment.")]
    public playersInfo playerList;

    private Dictionary<playerController, int> playerToTeamId = new Dictionary<playerController, int>();
    private Dictionary<int, int> teamScores = new Dictionary<int, int>();
    private Dictionary<playerController, int> playerScores = new Dictionary<playerController, int>();

    public bool debug = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (playerList != null && playerList.allControllers != null)
        {
            foreach (playerController pc in playerList.allControllers)
            {
                if (pc == null) continue;
                RegisterPlayer(pc);
            }
            ApplyDefaultAssignments();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary>Register a player and assign default team based on mode.</summary>
    public void RegisterPlayer(playerController pc)
    {
        if (pc == null) return;
        if (mode == Mode.Solo)
        {
            if (!playerScores.ContainsKey(pc))
                playerScores[pc] = 0;
        }
        else if (mode == Mode.Teams)
        {
            if (!playerToTeamId.ContainsKey(pc))
                playerToTeamId[pc] = 0;
            int tid = playerToTeamId[pc];
            if (!teamScores.ContainsKey(tid))
                teamScores[tid] = 0;
        }
    }

    /// <summary>Apply default assignments: EveryoneOneTeam = all team 0; Teams = all team 0; Solo = each player already has own score entry.</summary>
    private void ApplyDefaultAssignments()
    {
        if (mode != Mode.Teams || playerList?.allControllers == null) return;
        for (int i = 0; i < playerList.allControllers.Count; i++)
        {
            playerController pc = playerList.allControllers[i];
            if (pc != null)
                SetTeamAssignment(pc, 0);
        }
    }

    /// <summary>Add score (positive or negative). In EveryoneOneTeam writes to ResourceManager.globalCapital; in Teams/Solo updates the appropriate team or player.</summary>
    public void AddScore(int amount, playerController contributor = null)
    {
        if (mode == Mode.EveryoneOneTeam)
        {
            if (ResourceManager.Instance != null)
                ResourceManager.Instance.globalCapital += amount;
            if (debug) Debug.Log($"[TeamManager] AddScore({amount}) -> globalCapital");
            return;
        }

        if (mode == Mode.Solo)
        {
            if (contributor != null)
            {
                if (!playerScores.ContainsKey(contributor))
                    playerScores[contributor] = 0;
                playerScores[contributor] += amount;
                if (debug) Debug.Log($"[TeamManager] AddScore({amount}) -> player {contributor.playerID}");
            }
            return;
        }

        if (mode == Mode.Teams)
        {
            int teamId = GetTeamId(contributor);
            if (!teamScores.ContainsKey(teamId))
                teamScores[teamId] = 0;
            teamScores[teamId] += amount;
            if (debug) Debug.Log($"[TeamManager] AddScore({amount}) -> team {teamId}");
        }
    }

    /// <summary>Returns the score to use for level complete, results screen, and star brackets.</summary>
    public int GetScoreForLevel()
    {
        if (mode == Mode.EveryoneOneTeam)
        {
            if (ResourceManager.Instance != null)
                return ResourceManager.Instance.globalCapital;
            return 0;
        }

        if (mode == Mode.Solo)
        {
            int sum = 0;
            foreach (var v in playerScores.Values)
                sum += v;
            return sum;
        }

        if (mode == Mode.Teams)
        {
            if (levelScoreAggregate == LevelScoreAggregate.Max)
            {
                int max = 0;
                foreach (var v in teamScores.Values)
                    if (v > max) max = v;
                return max;
            }
            int sum = 0;
            foreach (var v in teamScores.Values)
                sum += v;
            return sum;
        }

        return 0;
    }

    public int GetTeamId(playerController pc)
    {
        if (pc == null || !playerToTeamId.ContainsKey(pc))
            return 0;
        return playerToTeamId[pc];
    }

    public void SetTeamAssignment(playerController pc, int teamId)
    {
        if (pc == null) return;
        playerToTeamId[pc] = teamId;
        if (mode == Mode.Teams && !teamScores.ContainsKey(teamId))
            teamScores[teamId] = 0;
    }

    public List<playerController> GetPlayersInTeam(int teamId)
    {
        var list = new List<playerController>();
        foreach (var kv in playerToTeamId)
            if (kv.Value == teamId)
                list.Add(kv.Key);
        return list;
    }

    public int GetTeamScore(int teamId)
    {
        return teamScores.ContainsKey(teamId) ? teamScores[teamId] : 0;
    }

    public int GetPlayerScore(playerController pc)
    {
        return playerScores.ContainsKey(pc) ? playerScores[pc] : 0;
    }

    /// <summary>Number of score slots to show in UI (1 for EveryoneOneTeam, up to 4 for Teams/Solo).</summary>
    public const int MaxScoreDisplayCount = 4;

    public int GetScoreDisplayCount()
    {
        if (mode == Mode.EveryoneOneTeam)
            return 1;
        if (mode == Mode.Solo)
        {
            if (playerList == null || playerList.allControllers == null)
                return 1;
            return Mathf.Min(MaxScoreDisplayCount, playerList.allControllers.Count);
        }
        if (mode == Mode.Teams)
        {
            var ids = GetOrderedTeamIds();
            return Mathf.Min(MaxScoreDisplayCount, ids.Count);
        }
        return 1;
    }

    /// <summary>Label for the score at the given index (e.g. "Score", "Player 1", "Team 0").</summary>
    public string GetScoreLabel(int index)
    {
        if (mode == Mode.EveryoneOneTeam)
            return "Score";
        if (mode == Mode.Solo && playerList != null && playerList.allControllers != null && index >= 0 && index < playerList.allControllers.Count)
            return "Player " + (index + 1);
        if (mode == Mode.Teams)
        {
            var ids = GetOrderedTeamIds();
            if (index >= 0 && index < ids.Count)
                return "Team " + ids[index];
        }
        return "Score " + (index + 1);
    }

    /// <summary>Score value for the given display index.</summary>
    public int GetScoreValue(int index)
    {
        if (mode == Mode.EveryoneOneTeam)
            return GetScoreForLevel();
        if (mode == Mode.Solo && playerList != null && playerList.allControllers != null && index >= 0 && index < playerList.allControllers.Count)
        {
            var pc = playerList.allControllers[index];
            return GetPlayerScore(pc);
        }
        if (mode == Mode.Teams)
        {
            var ids = GetOrderedTeamIds();
            if (index >= 0 && index < ids.Count)
                return GetTeamScore(ids[index]);
        }
        return 0;
    }

    /// <summary>Ordered list of team IDs that have score entries (for UI display).</summary>
    public List<int> GetOrderedTeamIds()
    {
        if (teamScores == null || teamScores.Count == 0)
            return new List<int>();
        var ids = teamScores.Keys.ToList();
        ids.Sort();
        return ids;
    }
}
