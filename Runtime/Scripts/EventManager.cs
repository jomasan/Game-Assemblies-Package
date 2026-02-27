using System;
using System.Collections.Generic;
using UnityEngine;

///
/// Runtime event manager: holds event assets, fires and removes events, and tracks active events by modality (permanent or duration).
/// Other systems (Policy, speed, recipes, decay) will read from GetActiveEvents() when integration is added; this class does not yet apply effects.
public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    [Header("Event references")]
    [Tooltip("Optional: events that can auto-trigger by condition (e.g. TimeElapsed). Leave empty to fire events only via FireEvent().")]
    public List<EventSO> conditionEvents = new List<EventSO>();

    private struct ActiveEntry
    {
        public EventSO Event;
        public float EndTime; // Time.time when effect ends; float.MaxValue for permanent
    }

    private readonly List<ActiveEntry> _active = new List<ActiveEntry>();
    private readonly HashSet<EventSO> _firedByCondition = new HashSet<EventSO>();
    private float _levelStartTime;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        _levelStartTime = Time.time;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Update()
    {
        RemoveExpiredEvents();
        EvaluateConditions();
    }

    /// Removes active events whose duration has ended.
    private void RemoveExpiredEvents()
    {
        float now = Time.time;
        for (int i = _active.Count - 1; i >= 0; i--)
        {
            if (_active[i].EndTime <= now)
            {
                _active.RemoveAt(i);
            }
        }
    }

    /// Evaluates explicit triggers (e.g. TimeElapsed). Score, goal, and resource-count triggers require integration and are no-ops for now.
    private void EvaluateConditions()
    {
        if (conditionEvents == null) return;

        float elapsed = Time.time - _levelStartTime;

        foreach (EventSO evt in conditionEvents)
        {
            if (evt == null || _firedByCondition.Contains(evt)) continue;

            switch (evt.triggerType)
            {
                case EventTriggerType.TimeElapsed:
                    if (evt.triggerParamFloat > 0 && elapsed >= evt.triggerParamFloat)
                    {
                        _firedByCondition.Add(evt);
                        FireEvent(evt);
                    }
                    break;
                case EventTriggerType.ScoreThreshold:
                case EventTriggerType.GoalCompleted:
                case EventTriggerType.ResourceCountExceeds:
                    // Require integration with TeamManager, GoalManager, ResourceManager
                    break;
                case EventTriggerType.TriggeredByManager:
                default:
                    break;
            }
        }
    }

    /// Fires an event: adds it to active list. If modality is WithDuration, it will be removed after durationSeconds.
    public void FireEvent(EventSO evt)
    {
        if (evt == null) return;

        float endTime = evt.HasDuration ? Time.time + evt.durationSeconds : float.MaxValue;
        _active.Add(new ActiveEntry { Event = evt, EndTime = endTime });
    }

    /// Removes an event from the active list (effect stops).
    public void RemoveEvent(EventSO evt)
    {
        if (evt == null) return;
        _active.RemoveAll(e => e.Event == evt);
    }

    /// Returns a read-only list of currently active events. Other systems will use this to resolve modifiers (integration later).
    public IReadOnlyList<EventSO> GetActiveEvents()
    {
        return _active.ConvertAll(e => e.Event);
    }

    /// Returns whether the given event is currently active.
    public bool IsActive(EventSO evt)
    {
        if (evt == null) return false;
        return _active.Exists(e => e.Event == evt);
    }
}
