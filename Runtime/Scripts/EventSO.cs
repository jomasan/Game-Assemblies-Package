using System;
using System.Collections.Generic;
using UnityEngine;

///
/// How an event is triggered: by the Event Manager (or other systems) or by an explicit condition.
/// When using a condition, the Event Manager evaluates trigger parameters (e.g. time, score, goal, resource count).
public enum EventTriggerType
{
    /// Fired only when EventManager.FireEvent is called (schedule, signal, or other system).
    TriggeredByManager,
    /// Trigger after this many seconds (use triggerParamFloat for seconds).
    TimeElapsed,
    /// Trigger when score reaches threshold (use triggerParamFloat; team/scoring source TBD).
    ScoreThreshold,
    /// Trigger when a goal is completed (use triggerGoal).
    GoalCompleted,
    /// Trigger when resource count exceeds value (use triggerResource + triggerParamInt).
    ResourceCountExceeds,
}

///
/// Whether the event's effect lasts until removed or for a fixed duration.
public enum EventModality
{
    /// Effect stays until explicitly removed or overwritten.
    Permanent,
    /// Effect lasts for durationSeconds, then is reverted.
    WithDuration,
}

///
/// Type of effect an event applies. Integration with systems (Policy, speed, recipes, decay) is applied when those systems read from EventManager.
public enum EventEffectType
{
    /// Override or push active policy (use effectPolicy).
    ChangePolicy,
    /// Global movement speed multiplier (use effectParamFloat).
    MultiplySpeed,
    /// Scale recipe input quantities (use effectParamFloat; optional effectRecipe or effectStation to target).
    ScaleRecipeInputs,
    /// Scale recipe output quantities (use effectParamFloat; optional effectRecipe or effectStation).
    ScaleRecipeOutputs,
    /// Scale decay rate for decayable resources (use effectParamFloat).
    ScaleDecay,
    /// Scale station work duration (use effectParamFloat; optional effectStation).
    ScaleWorkDuration,
    /// Custom or future effect; parameters TBD.
    Other,
}

///
/// One effect applied by an event: type plus parameters. Systems read these when resolving effective modifiers (integration later).
[Serializable]
public class EventEffect
{
    [Tooltip("Kind of effect (policy override, speed multiplier, recipe scale, etc.).")]
    public EventEffectType effectType = EventEffectType.MultiplySpeed;

    [Tooltip("Multiplier or generic float parameter (e.g. 2 = double, 0.5 = half).")]
    public float effectParamFloat = 1f;

    [Tooltip("Optional: policy to apply when effectType is ChangePolicy.")]
    public PolicyDataSO effectPolicy;

    [Tooltip("Optional: recipe to target for ScaleRecipeInputs/Outputs (null = all recipes).")]
    public RecipeSO effectRecipe;

    [Tooltip("Optional: station to target for recipe/work duration scaling (null = all stations).")]
    public StationDataSO effectStation;
}

///
/// An event asset: name, icon, trigger (condition or manager-triggered), modality (permanent or duration), and a list of effects.
/// EventManager fires events and tracks active ones; other systems will read active events for modifiers (integration later).
[CreateAssetMenu(fileName = "New Event", menuName = "Game Assemblies/Events/Event")]
public class EventSO : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("Display name for UI and cards.")]
    public string eventName = "New Event";
    [Tooltip("Optional icon for UI and card presentation.")]
    public Sprite icon;
    [TextArea(2, 4)]
    [Tooltip("Optional description for tooltips or cards.")]
    public string description;

    [Header("Trigger")]
    [Tooltip("Whether this event is fired by the manager/schedule or by an explicit condition.")]
    public EventTriggerType triggerType = EventTriggerType.TriggeredByManager;
    [Tooltip("For TimeElapsed: seconds. For ScoreThreshold: score value.")]
    public float triggerParamFloat;
    [Tooltip("For ResourceCountExceeds: required count.")]
    public int triggerParamInt;
    [Tooltip("For ResourceCountExceeds: which resource to count.")]
    public Resource triggerResource;
    [Tooltip("For GoalCompleted: which goal must be completed.")]
    public ResourceGoalSO triggerGoal;

    [Header("Modality")]
    [Tooltip("Permanent = effect until removed; WithDuration = effect for durationSeconds then revert.")]
    public EventModality modality = EventModality.WithDuration;
    [Tooltip("When modality is WithDuration, effect lasts this many seconds.")]
    public float durationSeconds = 30f;

    [Header("Effects")]
    [Tooltip("List of effects applied while this event is active.")]
    public List<EventEffect> effects = new List<EventEffect>();

    /// Whether this event uses a time-based duration (modality WithDuration).
    public bool HasDuration => modality == EventModality.WithDuration;
}
