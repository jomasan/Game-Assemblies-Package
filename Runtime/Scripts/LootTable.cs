using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LootEntry
{
    public Resource resource;
    [Range(0.01f, 100f)]
    public float dropPercentage;
}

[CreateAssetMenu(fileName = "New Loot Table", menuName = "Detroit at Play/Loot Table")]
public class LootTable : ScriptableObject
{
    [SerializeField]
    private List<LootEntry> possibleLoot = new List<LootEntry>();

    [SerializeField]
    private bool validateOnEnable = true;

    private void OnEnable()
    {
        if (validateOnEnable)
        {
            //ValidatePercentages();
        }
    }

    public void ValidatePercentages()
    {
        float totalPercentage = 0f;

        // Calculate total
        foreach (var entry in possibleLoot)
        {
            if (entry.resource != null)
            {
                totalPercentage += entry.dropPercentage;
            }
        }

        // Check if percentages need adjustment
        if (Mathf.Abs(totalPercentage - 100f) > 0.01f)
        {
            Debug.LogWarning($"Loot table {name} percentages don't sum to 100%. Current sum: {totalPercentage}. Normalizing values.");

            // If there are no valid entries, we can't normalize
            if (totalPercentage <= 0)
            {
                Debug.LogError($"Loot table {name} has no valid entries or all percentages are 0.");
                return;
            }

            // Normalize values to sum to 100%
            float normalizationFactor = 100f / totalPercentage;

            foreach (var entry in possibleLoot)
            {
                if (entry.resource != null)
                {
                    entry.dropPercentage *= normalizationFactor;
                }
            }
        }
    }

    public Resource GetRandomDrop()
    {
        float randomValue = UnityEngine.Random.Range(0f, 100f);
        float cumulativePercentage = 0f;

        foreach (var entry in possibleLoot)
        {
            if (entry.resource == null)
                continue;

            cumulativePercentage += entry.dropPercentage;

            if (randomValue <= cumulativePercentage)
            {
                return entry.resource;
            }
        }

        // Fallback - return the last valid resource if something went wrong
        for (int i = possibleLoot.Count - 1; i >= 0; i--)
        {
            if (possibleLoot[i].resource != null)
            {
                Debug.LogWarning("Loot table calculation failed to select an item properly. Returning last valid item.");
                return possibleLoot[i].resource;
            }
        }

        Debug.LogError("No valid resources found in loot table!");
        return null;
    }

    // Helper method to add a resource with a specified drop percentage
    public void AddResource(Resource resource, float dropPercentage)
    {
        if (resource == null)
        {
            Debug.LogError("Cannot add null resource to loot table.");
            return;
        }

        possibleLoot.Add(new LootEntry
        {
            resource = resource,
            dropPercentage = Mathf.Clamp(dropPercentage, 0.01f, 100f)
        });

        ValidatePercentages();
    }

    // Helper method to clear all resources
    public void ClearResources()
    {
        possibleLoot.Clear();
    }

#if UNITY_EDITOR
    // Editor utility for balancing percentages
    public void BalancePercentages()
    {
        int validEntries = 0;

        foreach (var entry in possibleLoot)
        {
            if (entry.resource != null)
            {
                validEntries++;
            }
        }

        if (validEntries == 0)
            return;

        float equalPercentage = 100f / validEntries;

        foreach (var entry in possibleLoot)
        {
            if (entry.resource != null)
            {
                entry.dropPercentage = equalPercentage;
            }
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}