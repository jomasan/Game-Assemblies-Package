using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }
    public List<ResourceObject> allResources = new List<ResourceObject>();

    public List<ResourceUIBinding> resourcesToTrack = new List<ResourceUIBinding>();

    public int globalCapital = 0;

    public bool debug = false;
    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updatePanels();
        if (debug) Debug.Log("Global Capital: " + globalCapital);
    }

    public static int getGlobalCapital()
    {
        return Instance.globalCapital;
    }

    public void updatePanels()
    {
        for (int i = 0; i < resourcesToTrack.Count; i++)
        {
            Resource toTrack = resourcesToTrack[i].resourceType;
            resourceInfoManager resourceIM = resourcesToTrack[i].resourceUIPanel;
            int count = GetResourceCount(toTrack, null);
            resourceIM.resourceName.text = toTrack.resourceName.ToString();
            resourceIM.resourceAmount.text = count.ToString();
            //Debug.Log("Resource: " + toTrack.resourceName + " Amount: " + count + ", from: " + allResources.Count);
        }
    }

    // Methods to add and remove resources
    public void AddResource(ResourceObject resource)
    {
        allResources.Add(resource);
    }

    public void RemoveResource(ResourceObject resource)
    {
        //Debug.Log("Trying to remove resource: " + resource.resourceType.resourceName);
        if (allResources.Contains(resource))
        {   
            
            allResources.Remove(resource);
        }
    }

    /// <summary>Counts resources of the given type. When ownerOrNull is null, behavior depends on policy: if Policy Manager exists and ownership model is not Communal, counts only unowned (common) resources; otherwise counts all.</summary>
    public int GetResourceCount(Resource resourceType, playerController ownerOrNull)
    {
        if (ownerOrNull != null)
        {
            return allResources.Count(r => r != null && r.resourceType == resourceType && r.owner == ownerOrNull);
        }

        if (PolicyManager.Instance != null && PolicyManager.Instance.GetOwnershipModel() != OwnershipModel.Communal)
        {
            return allResources.Count(r => r != null && r.resourceType == resourceType && r.owner == null);
        }

        return allResources.Count(r => r != null && r.resourceType == resourceType);
    }

    /// <summary>Counts all resources of the given type (or policy-based when owner is null). See GetResourceCount(Resource, playerController).</summary>
    public int GetResourceCount(Resource resourceType)
    {
        return GetResourceCount(resourceType, null);
    }

    public int GetResourceCount2(Resource resourceType)
    {
        return GetResourceCount(resourceType, null);
    }

    /// <summary>Returns counts per resource type, optionally filtered by owner. When ownerOrNull is null, uses same policy rule as GetResourceCount (all vs unowned only).</summary>
    public Dictionary<Resource, int> GetAllResourceCounts(playerController ownerOrNull = null)
    {
        if (ownerOrNull != null)
        {
            return allResources
                .Where(r => r != null && r.owner == ownerOrNull)
                .GroupBy(r => r.resourceType)
                .ToDictionary(g => g.Key, g => g.Count());
        }
        if (PolicyManager.Instance != null && PolicyManager.Instance.GetOwnershipModel() != OwnershipModel.Communal)
        {
            return allResources
                .Where(r => r != null && r.owner == null)
                .GroupBy(r => r.resourceType)
                .ToDictionary(g => g.Key, g => g.Count());
        }
        return allResources
            .Where(r => r != null)
            .GroupBy(r => r.resourceType)
            .ToDictionary(g => g.Key, g => g.Count());
    }
    
}


