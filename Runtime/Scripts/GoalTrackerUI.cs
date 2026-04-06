using UnityEngine;
using UnityEngine.UI;

public class GoalTrackerUI : MonoBehaviour
{
    [Header("UI References")]
    // Assign the Image component that will show the resource icon.
    [SerializeField] private Image resourceIcon;
    // Assign the Slider that will represent the remaining time.
    [SerializeField] private Slider timeSlider;

    // The goal this UI tracker is currently displaying.
    private ResourceGoalSO currentResourceGoal;
    private StationGoalSO currentStationGoal;

    

    [Header("Color Settings")]
    // The Image used as the slider's fill area whose color will change.
    [SerializeField] private Image sliderFillImage;
    // A gradient to define the color ramp (e.g., green when full, red when nearly out).
    [SerializeField] private Gradient timeColorRamp;

    /// <summary>
    /// Initializes the GoalTrackerUI with a specific goal.
    /// </summary>
    /// <param name="goal">The ResourceGoalSO to track.</param>
    public void Initialize(ResourceGoalSO goal)
    {
        currentResourceGoal = goal;
        currentStationGoal = null;

        // If the resource type has an associated icon, assign it to the UI Image.
        if (goal.resourceType != null && resourceIcon != null)
        {
            resourceIcon.sprite = goal.resourceType.icon;
            resourceIcon.color = GetIconTint(goal.resourceType);
        }

        // Set up the slider. We set the maximum value to the goal's time limit
        // and initialize it with the remaining time.
        if (timeSlider != null)
        {
            timeSlider.maxValue = goal.timeLimit;
            timeSlider.value = goal.remainingTime;
        }

        // Update the fill color based on the current time.
        float ratio = goal.timeLimit > 0f ? (goal.remainingTime / goal.timeLimit) : 0f;
        if (sliderFillImage != null)
            sliderFillImage.color = timeColorRamp.Evaluate(ratio);
    }

    /// <summary>
    /// Initializes the GoalTrackerUI with a station creation goal.
    /// </summary>
    public void Initialize(StationGoalSO goal)
    {
        currentStationGoal = goal;
        currentResourceGoal = null;

        if (goal.stationType != null && resourceIcon != null)
        {
            resourceIcon.sprite = goal.stationType.stationGraphic;
            resourceIcon.color = goal.stationType.stationSpriteTint.a < 0.01f ? Color.white : goal.stationType.stationSpriteTint;
        }

        if (timeSlider != null)
        {
            timeSlider.maxValue = goal.timeLimit;
            timeSlider.value = goal.remainingTime;
        }

        float ratio = goal.timeLimit > 0f ? (goal.remainingTime / goal.timeLimit) : 0f;
        if (sliderFillImage != null)
            sliderFillImage.color = timeColorRamp.Evaluate(ratio);
    }

    private static Color GetIconTint(Resource r)
    {
        if (r == null) return Color.white;
        return r.iconTint.a < 0.01f ? Color.white : r.iconTint;
    }

    private void Update()
    {
        if (currentResourceGoal != null)
        {
            // Update the slider to represent the remaining time.
            if (timeSlider != null)
                timeSlider.value = currentResourceGoal.remainingTime;

            // Calculate the ratio of remaining time to total time.
            float ratio = currentResourceGoal.timeLimit > 0f ? (currentResourceGoal.remainingTime / currentResourceGoal.timeLimit) : 0f;
            // Update the fill color using the gradient.
            if (sliderFillImage != null)
                sliderFillImage.color = timeColorRamp.Evaluate(ratio);
        }
        else if (currentStationGoal != null)
        {
            if (timeSlider != null)
                timeSlider.value = currentStationGoal.remainingTime;

            float ratio = currentStationGoal.timeLimit > 0f ? (currentStationGoal.remainingTime / currentStationGoal.timeLimit) : 0f;
            if (sliderFillImage != null)
                sliderFillImage.color = timeColorRamp.Evaluate(ratio);
        }
    }
}
