using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoWindow : MonoBehaviour
{   
    public Image ownerSprite;
    public GameObject inputPanel;
    public GameObject outputPanel;

    public GameObject inputResource;
    public GameObject outputResource;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeResources(List<Resource> produces, List<Resource> consumes)
    {
        if(inputResource == null || outputResource == null) return;

        foreach (Resource c in consumes) //input
        {
            GameObject inputObjects = Instantiate(inputResource);
            inputObjects.transform.parent = inputPanel.transform;
            if (c.icon != null)
            {
                inputObjects.GetComponent<Image>().sprite = c.icon;
            }
        }

        foreach (Resource p in produces) //output
        {
            GameObject outputObjects = Instantiate(outputResource);
            outputObjects.transform.parent = outputPanel.transform;
            if (p.icon != null)
            {
                outputObjects.GetComponent<Image>().sprite = p.icon;
            }
        }
    }
}
