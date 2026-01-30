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

    [Header("Consume (IN) - Produce (OUT)")]
    public bool consumeResource;
    public bool produceResource;
    public List<Resource> consumes = new List<Resource>();
    public List<Resource> produces = new List<Resource>();

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
    /// Applies this data to a Station component.
    /// </summary>
    public void ApplyToStation(Station station)
    {
        if (station == null) return;

        station.consumeResource = consumeResource;
        station.produceResource = produceResource;
        station.consumes.Clear();
        foreach (var r in consumes)
            if (r != null) station.consumes.Add(r);
        station.produces.Clear();
        foreach (var r in produces)
            if (r != null) station.produces.Add(r);

        station.isSingleUse = isSingleUse;
        station.destroyAfterSingleUse = destroyAfterSingleUse;

        station.capitalInput = capitalInput;
        station.capitalOutput = capitalOutput;
        station.capitalInputAmount = capitalInputAmount;
        station.capitalOutputAmount = capitalOutputAmount;

        station.completesGoals_consumption = completesGoals_consumption;
        station.completesGoals_production = completesGoals_production;

        station.canBeWorked = canBeWorked;
        station.workDuration = workDuration;
        station.productionInterval = productionInterval;
        station.typeOfProduction = canBeWorked ? typeOfProduction : Station.interactionType.automatic;
        station.typeOfConsumption = canBeWorked ? typeOfConsumption : Station.interactionType.automatic;

        if (!canBeWorked)
        {
            station.typeOfProduction = Station.interactionType.automatic;
            station.typeOfConsumption = Station.interactionType.automatic;
        }

        if (stationGraphic != null)
        {
            station.normalSprite = stationGraphic;
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
