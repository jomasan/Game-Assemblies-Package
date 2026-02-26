using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// One input or output slot in a recipe (resource type + quantity).
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
/// A recipe defines a single alternative method of production: a list of inputs and a list of outputs.
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
    [Tooltip("Resources produced (and quantities) when this recipe runs.")]
    public List<RecipeSlot> outputs = new List<RecipeSlot>();

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

    /// <summary>Expands outputs to a flat list of Resource (each repeated by amount). Used for production iteration.</summary>
    public List<Resource> GetOutputsExpanded()
    {
        var list = new List<Resource>();
        if (outputs == null) return list;
        foreach (var slot in outputs)
        {
            if (slot?.resource == null) continue;
            for (int i = 0; i < slot.amount; i++)
                list.Add(slot.resource);
        }
        return list;
    }
}
