using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    private int score;
    private int timePast;
    private UserInterface UI;

    // Start is called before the first frame update
    void Start()
    {
        UI = FindObjectOfType<UserInterface>();
        InvokeRepeating("IncrementTime", 1f, 1f);
    }

    public void IncrementTime()
    {
        timePast++;
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateUI()
    {
        UI.UpdateScoreText(score);
        UI.UpdateTimeText(timePast);
    }
}
