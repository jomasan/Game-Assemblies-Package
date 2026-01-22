using UnityEngine;

public class ResourceObject : MonoBehaviour
{
    public Resource resourceType;
    public int amount = 1;
    public playerController owner;

    public ResourceManager rManager;
    public enum resourceBehavior
    {
        Static,
        Decays,
        Consumable
    }
    public resourceBehavior typeOfBehavior = resourceBehavior.Static;
    public bool hasOwner = true; //is common?
    public float lifespan = 2f;
    public float currentLife = 0;

    public GameObject particlePrefab;

    public void Start()
    {
        rManager = GameObject.FindAnyObjectByType<ResourceManager>();
        if(rManager != null) rManager.AddResource(this);
    }
    public void Update()
    {
        if (typeOfBehavior == resourceBehavior.Decays) growOldandDie(); 
    }

    public void OnDestroy()
    {
        if (rManager != null) rManager.RemoveResource(this);

        /*
        if (particlePrefab != null)
        {
            GameObject destroyObject = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            destroyObject.GetComponent<ParticleSystemRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
        }
        */
    }

    public void InstantiateParticles()
    {
        if (particlePrefab != null)
        {
            GameObject destroyObject = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            destroyObject.GetComponent<ParticleSystemRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
        }
    }

    public void setOwner(playerController pC)
    {
        owner = pC;
    }

    public void growOldandDie()
    {
        currentLife += Time.deltaTime;
        if (currentLife > lifespan)
        {
            Destroy(gameObject);
        }
    }

    // Initialization method
    public void Initialize(Resource resource, int amount)
    {
        this.resourceType = resource;
        this.amount = amount;
    }

    // Example interaction method
    void OnMouseDown()
    {
        // For example, pick up the resource when clicked
        //Debug.Log($"Picked up {amount} of {resourceType.resourceName}");
        //Destroy(gameObject);

        // Optionally, add the resource to a player's inventory or a node
        // PlayerInventory.Instance.AddResource(resourceType, amount);
    }

}
