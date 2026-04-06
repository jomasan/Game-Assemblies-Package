using UnityEngine;

[CreateAssetMenu(fileName = "NewStationGoal", menuName = "Game Assemblies/Goals/Create Station Goal")]
public class StationGoalSO : ScriptableObject
{
    [Header("Goal Definition")]
    [Tooltip("Specific station data required to complete this goal.")]
    public StationDataSO stationType;
    [Tooltip("How many stations of this type must be created.")]
    public int requiredCount = 1;
    [Tooltip("The total time allowed to meet the goal (in seconds).")]
    public float timeLimit = 20f;

    [Header("Rewards")]
    [Tooltip("Points awarded when this goal is completed.")]
    public int rewardPoints = 0;
    public int penalty = 1;

    [Header("Runtime State (Not Serialized)")]
    [System.NonSerialized] public float remainingTime;
    [System.NonSerialized] public bool isCompleted;
    [System.NonSerialized] public bool isFailed;
    [System.NonSerialized] public int currentCount;
    [System.NonSerialized] public playerController lastContributor;

    public void ResetGoal()
    {
        remainingTime = timeLimit;
        isCompleted = false;
        isFailed = false;
        currentCount = 0;
        lastContributor = null;
    }

    public void UpdateGoaltime(float deltaTime)
    {
        if (isCompleted || isFailed) return;

        remainingTime -= deltaTime;
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            isFailed = true;
        }
    }

    public bool UpdateGoalObjective(StationDataSO createdStationType)
    {
        if (isCompleted || isFailed) return isCompleted;
        if (createdStationType == null || stationType == null) return isCompleted;
        if (createdStationType != stationType) return isCompleted;

        currentCount++;
        if (currentCount >= Mathf.Max(1, requiredCount))
            isCompleted = true;

        return isCompleted;
    }
}
