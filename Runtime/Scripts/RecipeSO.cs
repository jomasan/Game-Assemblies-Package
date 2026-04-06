using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// One input slot in a recipe (resource type + quantity).
/// </summary>
[System.Serializable]
public class RecipeSlot
{
    [Tooltip("Resource type for this slot.")]
    public Resource resource;
    [Tooltip("Quantity required (input) or produced (output).")]
    public int amount = 1;
}

/// <summary>
/// Whether a recipe output line produces a resource or spawns a station instance.
/// </summary>
public enum RecipeOutputKind
{
    Resource,
    Station
}

/// <summary>
/// One output line: either a resource (with amount) or a station definition (spawn from StationDataSO prefab, amount = count).
/// </summary>
[System.Serializable]
public class RecipeOutputSlot
{
    [Tooltip("Resource output or station spawn.")]
    public RecipeOutputKind kind = RecipeOutputKind.Resource;
    [Tooltip("When kind is Resource, the resource type produced.")]
    public Resource resource;
    [Tooltip("How many resources to add/spawn, or how many station instances to spawn.")]
    public int amount = 1;
    [Tooltip("When kind is Station, spawns stationPrefab from this data (ApplyToStation runs on the new instance).")]
    public StationDataSO stationData;
}

/// <summary>
/// A recipe defines a single alternative method of production: inputs (resources) and outputs (resources and/or stations).
/// Stations can reference multiple recipes and choose one as active at runtime.
/// </summary>
[CreateAssetMenu(fileName = "New Recipe", menuName = "Game Assemblies/Recipes/Recipe")]
public class RecipeSO : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("Display name for this recipe (e.g. for UI or station builder).")]
    public string recipeName = "New Recipe";
    [Tooltip("Optional icon for this recipe.")]
    public Sprite icon;
    [Tooltip("Optional work duration override for this recipe. 0 = use station default.")]
    public float workDurationOverride;

    [Header("Inputs")]
    [Tooltip("Resources required (and quantities) for this recipe. Consumed when the recipe runs.")]
    public List<RecipeSlot> inputs = new List<RecipeSlot>();

    [Header("Outputs")]
    [Tooltip("Each line is either a resource output or a station to instantiate.")]
    public List<RecipeOutputSlot> outputs = new List<RecipeOutputSlot>();

    /// <summary>Expands inputs to a flat list of Resource (each repeated by amount). Used for Area.requirements.</summary>
    public List<Resource> GetInputsExpanded()
    {
        var list = new List<Resource>();
        if (inputs == null) return list;
        foreach (var slot in inputs)
        {
            if (slot?.resource == null) continue;
            for (int i = 0; i < slot.amount; i++)
                list.Add(slot.resource);
        }
        return list;
    }

    /// <summary>Resource outputs only (expanded), for UI and logic that only cares about resources.</summary>
    public List<Resource> GetOutputsExpanded()
    {
        var list = new List<Resource>();
        if (outputs == null) return list;
        foreach (var slot in outputs)
        {
            if (slot == null || slot.kind != RecipeOutputKind.Resource || slot.resource == null) continue;
            for (int i = 0; i < slot.amount; i++)
                list.Add(slot.resource);
        }
        return list;
    }

    /// <summary>Number of output icons/cells to show (resources count expanded + one cell per station line × amount).</summary>
    public int GetOutputDisplayCount()
    {
        int n = 0;
        if (outputs == null) return 0;
        foreach (var slot in outputs)
        {
            if (slot == null) continue;
            if (slot.kind == RecipeOutputKind.Resource && slot.resource != null)
                n += Mathf.Max(0, slot.amount);
            else if (slot.kind == RecipeOutputKind.Station && slot.stationData != null)
                n += Mathf.Max(0, slot.amount);
        }
        return n;
    }

    /// <summary>True if this recipe has at least one output line.</summary>
    public bool HasOutputs()
    {
        return outputs != null && outputs.Count > 0;
    }
}
