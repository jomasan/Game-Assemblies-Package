using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.2D;
using System.Collections;
//using System;

public class Station : ResourceNode
{
    [Header("Managers")]
    public StationManager sManager;
    public ResourceManager rManager;
    public GoalManager gManager;

    public enum productionMode
    {
        Resource,
        Station,
        LootTable
    }
    [Header("Consume (IN) - Produce (OUT)")]
    public productionMode WhatToProduce = productionMode.Resource;

    public bool produceResource = false;
    public bool consumeResource = false;

    public List<Resource> produces = new List<Resource>();
    public List<Resource> consumes = new List<Resource>();

    public List<Station> produces_stations = new List<Station>();

    //public bool produceFromLootTable = false;
    public LootTable produceLootTable;

    //public int productionAmount = 1;          // Amount produced per cycle
    public float productionInterval = 5f;     // Time between production cycles in seconds
    private float productionTimer = 0f;       // count for the automatic production
    public bool spawnResourcePrefab = true;
    private Vector3 spawnOffset = Vector3.zero;
    public float spawnRadius = 1f;

    [Header("Capital Consume (IN) - Capital Produce (OUT)")]
    public bool capitalInput = false; // Does this station require capital to operate?
    public bool capitalOutput = false; // Does this station produce capital?
    public int capitalInputAmount = 0; // Amount of capital required to operate
    public int capitalOutputAmount = 0; // Amount of capital produced

    [Header("Input/Output Areas")]
    public bool useInputArea = true;
    public bool useOutputArea = true;
    public Area inputArea;
    public Area outputArea;

    [Header("Labor (INTERACTIONS)")]
    public bool canBeWorked = false;

    public enum interactionType
    {
        None,
        automatic,
        whenWorked,
        whenResourcesConsumed,
        cycle
    }
    public interactionType typeOfProduction = interactionType.None;
    public interactionType typeOfConsumption = interactionType.None;

    public float workDuration = 5f; // Time required to complete a work cycle in seconds
    public float workProgress = 0f; // Progress towards completing the work cycle
    public bool isBeingWorkedOn = false; // Is work currently being performed?
    public List<playerController> workerCount;
    private Coroutine workCoroutine; // Reference to the ongoing work coroutine

    [Header("Lifespan (DECAY / TIME)")] //This is for making the input resources a requirement for the station not to die
    //public bool decayWithoutInput = false;
    public int decayValue = 0;
    public int maxDecay = 5;
    public float decayTimer = 0f;
    public float decayCycle = 10.0f;

    public bool isSingleUse = false;
    public bool destroyAfterSingleUse = false;
    public bool isAlive = true;
    private SpriteRenderer spRender;
    public Sprite normalSprite;
    public Sprite deadSprite;
    public int age = 0;
    private float ageTimer = 0f;
    public float growthRate = 1.0f; //every how many seconds grow older
    public int maxAge = 100;
    public List<Sprite> ageSprites = new List<Sprite>();
    public bool useAgeSprites = false;
    public bool canDie = false;
    public bool canGrow = false;
    public bool randStartAge = false;

    [Header("Upgrades (TRANSFORMATION)")]
    public bool canBeUpgraded = false;
    public GameObject upgradePrefab;
    private int flaggedToUpgrade = -1;

    [Header("Goals (SCORE)")]
    public bool completesGoals_production = false; //allows output to complete goals
    public bool completesGoals_consumption = false;

    [Header("Property System (OWNER/WORKER)")]
    public playerController owner;
    public playerController worker;

    [Header("Inspect / Work UI (UI POP UP)")]
    public bool isBeingInspected = false; // Is the station being inspected?
    public Transform inspectionPoint; // Point where the inspection window should be displayed
    public GameObject sliderBar;
    public GameObject infoWindow;
    Slider progressSlider;
    public bool manualSliderPosition = true;
    public float offsetY = 2f;

    //public float worker_owner_distribution = 0.5f; //not in use yet
    public int purchasePrice = 0;

    [Header("Audio")]
    bool prevWork = false;
    public AudioSource outputAudio;
    public AudioClip workingSound;
    public AudioClip completeSound;
    public AudioClip notEnoughMaterialsSound;

    [Header("Particles")]
    public GameObject productionParticles;


    [Header("Debug Tools")]
    public bool doUpdate = false;
    public bool debug = false;


    private bool resourcesConsumed = false;
    private bool workCompleted = false;
    private bool coroutineRunning = false;


    // Start is called before the first frame update
    void Start()   
    {
        workerCount = new List<playerController>();

        if (!canBeWorked) sliderBar.SetActive(false);

        progressSlider = sliderBar.GetComponent<Slider>();
        if (inputArea != null) inputArea.requirements = consumes;

        sManager = GameObject.FindAnyObjectByType<StationManager>();
        rManager = GameObject.FindAnyObjectByType<ResourceManager>();
        gManager = GameObject.FindAnyObjectByType<GoalManager>();

        if (sManager != null) sManager.allStations.Add(this);

        spRender = transform.GetComponent<SpriteRenderer>();

        if (randStartAge)
        {
            age = Random.Range(0, maxAge);//make random age based on max age
            ageTimer = Random.Range(0, growthRate);
        }

        InitializeInforPanel();
        updateInfoPanel();

        prevWork = isBeingWorkedOn;
    }

    void onDestroy()
    {
        if (sManager != null) sManager.allStations.Remove(this);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("workCompleted: " + workCompleted);
        //CONTROL PANEL
        if (isAlive)
        {
            updateInfoPanel(); //show info panel if inspected
            if (typeOfProduction == interactionType.whenWorked || typeOfConsumption == interactionType.whenWorked) updateSlider(); //1 - update the slider for workable units
            if (typeOfConsumption == interactionType.cycle) updateSliderCycle();

            if (produceResource)
            {
                if (typeOfProduction == interactionType.automatic) AutomaticProduction(); //and capital for owner
                if (typeOfProduction == interactionType.whenWorked) ProduceOnWork();//and capital for worker
                if (typeOfProduction == interactionType.whenResourcesConsumed) ConsumedProduction(); //and capital for worker

            }
            if (consumeResource)
            {
                if (typeOfConsumption == interactionType.automatic) AutomaticConsumption(); //and capital for owner
                if (typeOfConsumption == interactionType.whenWorked) ConsumeOnWork();//and capital for worker
                if (typeOfConsumption == interactionType.cycle) ConsumeOnCycle();
            }

            growOlder();
            workCompleted = false;

            playWorkingSound();
            if (flaggedToUpgrade > -1) flaggedToUpgrade--;
            if (flaggedToUpgrade == 0) upgradeStation();
           
        }
    }

    public void playWorkingSound()
    {
        if (outputAudio == null) return;
        if (workingSound == null) return;

        //Debug.Log("AudioCalled");

        if (prevWork != isBeingWorkedOn && isBeingWorkedOn == true)
        {
            outputAudio.clip = workingSound;
            outputAudio.Play();
            prevWork = isBeingWorkedOn;
            //Debug.Log("PLAY");
        } else if (prevWork != isBeingWorkedOn && isBeingWorkedOn == false) 
        {
            outputAudio.Stop();
            prevWork = isBeingWorkedOn;
            //Debug.Log("STOP");
        } else {
            //Debug.Log("NOTHING");
        }
    }

    public void ConsumeOnCycle()
    {
        decayTimer += Time.deltaTime; //decay timer
        if (decayTimer >= decayCycle) //decay cycle
        {   
            //if resources are there, they are consumed, if they are not, the unit decays
            if (inputArea.AreAllRequirementsMet()) 
            {
                ConsumeResource();
                ConsumeCapital(worker);
                //Debug.Log("TRUE!");
            } else{
                if (decayValue < maxDecay)
                {
                    decayValue++;
                } else
                {
                    isAlive = false;
                    swapSprite();
                }
                
                //Debug.Log("FALSE!");
            }
            decayTimer = 0f;
        }
        
    }
    public void growOlder()
    {
        if (canGrow)
        {
            ageTimer += Time.deltaTime;
            if(ageTimer >= growthRate)
            {
                age++;
                ageTimer = 0f;
            }
        }

        if (useAgeSprites)
        {
            if (ageSprites.Count > 0)
            {
                if (age < ageSprites.Count)
                {
                    spRender.sprite = ageSprites[age];
                }
            }
        }

    }
    public void ConsumeCapital(playerController pC)
    {
        if (capitalInput)
        {
            Debug.Log("Capital consumption called");
            if (pC != null)
            {
                Debug.Log("Capital removed from worker");
                pC.capital -= capitalInputAmount;
            }
            if (rManager != null)
            {
                Debug.Log("Capital removed from global");
                rManager.globalCapital -= capitalInputAmount;
            }
        }
    }
    public void ProduceCapital(playerController pC)
    {
        if (capitalOutput)
        {
            if (pC != null) pC.capital += capitalOutputAmount;
            if (rManager != null) rManager.globalCapital += capitalOutputAmount;
        }
    }
    public void swapSprite()
    {
        if (isAlive)
        {
            spRender.sprite = normalSprite;
        } else
        {
            spRender.sprite = deadSprite;
            sliderBar.SetActive(false);

            if (destroyAfterSingleUse) Destroy(this.gameObject);
        }
    }
    public void InitializeInforPanel()
    {
        if(infoWindow != null)
        {
            infoWindow.GetComponent<InfoWindow>().InitializeResources(produces, consumes);
        }
        
    }
    public void updateInfoPanel()
    {
        if (isBeingInspected)
        {   
            if(infoWindow != null)
            {
                infoWindow.SetActive(true);
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * offsetY);
                infoWindow.transform.position = screenPosition;
                infoPanelAnimation();
            }
            
        } else
        {
            if (infoWindow != null) infoWindow.SetActive(false); // could be optimized
        }
    }

    public void infoPanelAnimation()
    {
        if (!coroutineRunning && doUpdate)
        {
            doUpdate = false;
            float startScale = infoWindow.transform.localScale.x + 0.5f;
            float endScale = infoWindow.transform.localScale.x;
            float duration = 0.5f;
            StartCoroutine(ScaleOverTime(infoWindow, new Vector3(startScale, startScale, startScale), new Vector3(endScale, endScale, endScale), duration));
        }
    }
    public void updateSliderCycle()
    {
        sliderBar.SetActive(true);
        if (manualSliderPosition)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * offsetY + Vector3.up * -0.46f);
            progressSlider.transform.position = screenPosition;
        }
        float sliderValue = decayTimer / decayCycle;
        progressSlider.value = sliderValue;
    }
    public void updateSlider()
    {
        if (isBeingWorkedOn)
        {
            sliderBar.SetActive(true);
            if (manualSliderPosition)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * offsetY + Vector3.up * -0.46f);
                progressSlider.transform.position = screenPosition;
            }
            float sliderValue = workProgress / workDuration;
            progressSlider.value = sliderValue;
        } else
        {
            sliderBar.SetActive(false); // could be optimized
            progressSlider.value = 0;
        }
    }
    public void executeLabor(playerController newWorker)
    {
        if(!workerCount.Contains(newWorker) ) { workerCount.Add(newWorker); } //only add the worker to the list if new entity

        isBeingWorkedOn = true;
        //workProgress += Time.deltaTime; //ONE PLAYER CAN WORK
        workProgress += Time.deltaTime * workerCount.Count;
        //if (debug) Debug.Log("Work Pogress: " + workProgress + "/" + workDuration);
        if (workProgress >= workDuration)
            {
                CompleteWork();
            }
    }
    public void cancelLabor(playerController newWorker)
    {
        if (workerCount.Contains(newWorker)) { workerCount.Remove(newWorker); }

        if(workerCount.Count == 0)
        {
            isBeingWorkedOn = false;
            workProgress = 0;
        }
        
    }
    private void CompleteWork()
    {
        if (debug) Debug.Log("Completed Labor Cycle.");

        // ON COMPLETED ->
        workCompleted = true;
        //if(produceResource) ProduceResource();
        //if(consumeResource) ConsumeResource();

        // Reset work progress
        workProgress = 0f;

        //if(canBeUpgraded) upgradeStation(); //0-----------------------------------------
    }
    void upgradeStation()
    {
        if(upgradePrefab != null)
        {
            GameObject newStation = Instantiate(upgradePrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
    void ProduceOnWork()
    {
        if (workCompleted)
        {
            ProduceResource();
            ProduceCapital(worker);
            //workCompleted = false;
        }
    }
    void ConsumeOnWork()
    {
        if(debug) Debug.Log("Consume on Work Called, workCompleted: " + workCompleted);
        if (workCompleted)
        {
            if (debug) Debug.Log("Consume on Work Called Completed Sequence");
            ConsumeResource();
            ConsumeCapital(worker);
            //workCompleted = false;
        }
    }
    void ConsumeResource()
    {
        if (inputArea == null) return;

        if (debug) Debug.Log("station check: " + inputArea.allRequirementsMet);
        if (inputArea.allRequirementsMet)
        {
            inputArea.RemoveMatchingResources();
            resourcesConsumed = true;

            if (completesGoals_consumption) gManager.goalContribution(consumes[0]); //IF IT CONTRIBUTES TO GOALS - CHECK AND PASS INFO TO THE GLOBAL SCORE

            if (canBeUpgraded) flaggedToUpgrade = 2;//upgradeStation();
        }
    }
    void ConsumedProduction()
    {
        if (debug) Debug.Log("Consumed Production Started - resources Consumed: " + resourcesConsumed);
        if (resourcesConsumed)
        {
            ProduceResource();
            ProduceCapital(worker);
            resourcesConsumed = false;
        }
    }
    void AutomaticConsumption()
    {
        if (debug) Debug.Log("Automatic Consumption Running - ");
        //productionTimer += Time.deltaTime;

        //if (productionTimer >= productionInterval)
        //{
            ConsumeResource();
            ConsumeCapital(owner);
            //productionTimer = 0f;
        //}
    }
    void AutomaticProduction()
    {
        if (debug) Debug.Log("Automatic Production Running - ");
        productionTimer += Time.deltaTime;

        if (productionTimer >= productionInterval)
        {
            ProduceResource();
            ProduceCapital(owner);

            productionTimer = 0f;
        }
    }

    public void playProductionSound()
    {
        if(outputAudio == null || completeSound == null) return;
        outputAudio.clip = completeSound;
        outputAudio.Play();
    }

    void ProduceResource()
    {
        if (WhatToProduce == productionMode.Resource)
        {
            for (int i = 0; i < produces.Count; i++)
            {
                // Add the produced resources to the local storage
                AddResource(produces[i], 1);

                // Optional: Visual or audio feedback
                //if (debug) 
                if (debug) Debug.Log($"{gameObject.name}: Produced {1} of {produces[i].resourceName}");

                if (spawnResourcePrefab)
                {  
                    InstantiateResourcePrefabs(produces[i]);// Instantiate the resource prefab
                }

                if (completesGoals_production) gManager.goalContribution(produces[0]); //CONTRIBUTES TO GOALS THROUGH PRODUCTION


                //AUDIO:
                playProductionSound();
                
            }

            if (isSingleUse)
            {
                isAlive = false;
                swapSprite();
                
            }
        } else if (WhatToProduce == productionMode.Station)
        {
            for (int i = 0; i < produces_stations.Count; i++)
            {
                InstantiateStationPrefabs(i);
            }
        }else if (WhatToProduce == productionMode.LootTable)
        {
            if(produceLootTable != null)
            {
                Resource output = produceLootTable.GetRandomDrop();
                AddResource(output, 1);
                if (debug) Debug.Log($"{gameObject.name}: Produced {1} of {output.resourceName}");
                if (spawnResourcePrefab)
                {     
                    InstantiateResourcePrefabs(output);// Instantiate the resource prefab
                }

                //AUDIO:
                playProductionSound();

                if (isSingleUse)
                {
                    isAlive = false;
                    swapSprite();
                    
                }
            } else
            {
                Debug.LogError("No Loot Table available");
            }
        }
    }
    void InstantiateStationPrefabs(int index)
    {
        if (produces_stations[index] != null)
        {
            Vector3 spawnPosition = outputArea.GetPosition();
            GameObject stationInstance = Instantiate(produces_stations[index].gameObject, spawnPosition, Quaternion.identity);
        }
    }
    void InstantiateResourcePrefabs(Resource rs)
    {
        if (rs.resourcePrefab != null)
        {
            if (useOutputArea)
            {
                //Vector3 spawnPosition = outputArea.GetPosition();
                Vector3 spawnPosition = outputArea.GetPositionWithRandomness(0.1f);
                GameObject resourceInstance = Instantiate(rs.resourcePrefab, spawnPosition, Quaternion.identity);

                if (productionParticles != null) Instantiate(productionParticles, spawnPosition, Quaternion.identity);

            } else
            {
                // Generate a random point inside a circle
                Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * spawnRadius;

                // Use the random circle coordinates for X and Z axes
                Vector3 randomOffset = new Vector3(randomCircle.x, randomCircle.y, 0);

                Vector3 spawnPosition = transform.position + randomOffset;
                GameObject resourceInstance = Instantiate(rs.resourcePrefab, spawnPosition, Quaternion.identity);


                if (productionParticles != null) Instantiate(productionParticles, spawnPosition, Quaternion.identity);

                //Set owner
                ResourceObject rsObj = resourceInstance.GetComponent<ResourceObject>();
                if (rsObj != null && owner != null)
                {
                    rsObj.setOwner(owner);
                }

            }
        } else
        {
            //Debug.LogWarning($"{gameObject.name}: Resource prefab is not assigned for {resourceToProduce.resourceName}");
        }
    }
    IEnumerator ScaleOverTime(GameObject obj, Vector3 startScale, Vector3 endScale, float duration)
    {
        float elapsed = 0f;
        coroutineRunning = true;
        while (elapsed < duration)
        {
            // Normalize the elapsed time
            float t = elapsed / duration;

            // Apply the easing function
            float scaleValue = Tween.EaseOutBack(t);

            // Interpolate the scale
            obj.transform.localScale = Vector3.LerpUnclamped(startScale, endScale, scaleValue);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the final scale is set
        obj.transform.localScale = endScale;
        coroutineRunning = false;
    }
}
