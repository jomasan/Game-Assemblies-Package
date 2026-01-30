using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject containing the key configuration data for a station.
/// Can be used as a data template to spawn or configure stations at runtime.
/// </summary>
[CreateAssetMenu(fileName = "New Station Data", menuName = "Game Assemblies/Stations/Station Data")]
public class StationDataSO : ScriptableObject
{
    [Header("Identity")]
    public string stationName = "New Station";
    public Sprite stationGraphic;
    [Tooltip("Sprite shown when the station is inactive/dead. If null, stationGraphic is used.")]
    public Sprite deadSprite;
    [Tooltip("Reference to the prefab representing this station.")]
    public GameObject stationPrefab;

    [Header("Consume (IN) - Produce (OUT)")]
    public bool consumeResource;
    public bool produceResource;
    public List<Resource> consumes = new List<Resource>();
    public List<Resource> produces = new List<Resource>();
    [Tooltip("What this station outputs: Resource, Station, or LootTable.")]
    public Station.productionMode whatToProduce = Station.productionMode.Resource;
    [Tooltip("When true, spawns physical resource prefabs in the output area.")]
    public bool spawnResourcePrefab = true;
    [Tooltip("Radius for random spawn offset when not using output area.")]
    public float spawnRadius = 1f;

    [Header("Lifespan")]
    public bool isSingleUse;
    public bool destroyAfterSingleUse = true;

    [Header("Capital")]
    public bool capitalInput;
    public bool capitalOutput;
    public int capitalInputAmount = 1;
    public int capitalOutputAmount = 1;

    [Header("Goals")]
    public bool completesGoals_consumption;
    public bool completesGoals_production;

    [Header("Timing & Interaction")]
    public bool canBeWorked = true;
    public float workDuration = 5f;
    public float productionInterval = 5f;
    public Station.interactionType typeOfProduction = Station.interactionType.whenWorked;
    public Station.interactionType typeOfConsumption = Station.interactionType.whenWorked;

    /// <summary>
    /// Applies this data to a Station component. Assigns this SO as the station's data source
    /// and performs one-time setup (input/output areas, sprite). The station reads all config
    /// from stationData after this call.
    /// </summary>
    public void ApplyToStation(Station station)
    {
        if (station == null) return;

        station.stationData = this;

        if (stationGraphic != null)
        {
            var sr = station.GetComponentInChildren<SpriteRenderer>();
            if (sr != null && sr.GetComponent<Canvas>() == null && sr.transform.GetComponentInParent<Canvas>() == null)
            {
                sr.sprite = stationGraphic;
            }
        }

        if (station.inputArea != null)
        {
            station.inputArea.requirements.Clear();
            foreach (var r in consumes)
                if (r != null) station.inputArea.requirements.Add(r);
            station.inputArea.gameObject.SetActive(consumeResource);
        }
        if (station.outputArea != null)
        {
            station.outputArea.gameObject.SetActive(produceResource);
        }
    }
}
