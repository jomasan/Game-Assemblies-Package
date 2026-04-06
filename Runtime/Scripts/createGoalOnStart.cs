using UnityEngine;

public class createGoalOnStart : MonoBehaviour
{
    public ScriptableObject goalTemplate;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (LevelManager.Instance != null)
            LevelManager.Instance.CreateGoal(goalTemplate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
