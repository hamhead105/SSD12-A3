using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Manages the level, score, player and enemies. Runs events for the games when required.
 */
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

    /*
     * Increment the time elapsed and update the UI show of this.
     */
    public void IncrementTime()
    {
        timePast++;
        UpdateUI();
    }

    /*
     * 
     */

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
     * Update UI elements to reflect game states
     */
    private void UpdateUI()
    {
        UI.UpdateScoreText(score);
        UI.UpdateTimeText(timePast);
    }
}
