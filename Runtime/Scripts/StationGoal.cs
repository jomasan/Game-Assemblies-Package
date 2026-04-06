using UnityEngine;

public class StationGoal
{
    // The station data we want to see created.
    public StationDataSO stationType;
    // How many stations of this type are required.
    public int requiredCount;
    // Total time allowed (in seconds).
    public float totalTime;
    // Remaining time (in seconds).
    public float remainingTime;
    // Goal status.
    public bool isCompleted;
    public bool isFailed;

    private int currentCount;

    public StationGoal(StationDataSO stationType, int requiredCount, float timeLimit)
    {
        this.stationType = stationType;
        this.requiredCount = Mathf.Max(1, requiredCount);
        this.totalTime = timeLimit;
        this.remainingTime = timeLimit;
        isCompleted = false;
        isFailed = false;
        currentCount = 0;
    }

    // Call this when a matching station is created.
    public void RegisterStationCreated(StationDataSO createdStationType)
    {
        if (isCompleted || isFailed) return;
        if (createdStationType != stationType) return;

        currentCount++;
        if (currentCount >= requiredCount)
            isCompleted = true;
    }

    // Call this each frame to update timer and completion/failure state.
    public void UpdateGoal(float deltaTime)
    {
        if (isCompleted || isFailed) return;

        remainingTime -= deltaTime;
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            isFailed = true;
        }
    }
}
