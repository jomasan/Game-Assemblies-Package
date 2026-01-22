using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using TMPro; // Commented out - Unity 6.3+ has TMPro integrated by default

public class playerInfoManager : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI capital;

    public int playerID = 0;
    public playersInfo playerManager;
    // Start is called before the first frame update
    void Start()
    {
        //playerManager = GameObject.FindObjectOfType<playersInfo>();
    }

    // Update is called once per frame
    void Update()
    {   
        
    }
}
