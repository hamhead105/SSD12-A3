using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Controls the UI in the Game End Screen
 */
public class EndScreenController : MonoBehaviour
{
    [SerializeField] private Text title;
    [SerializeField] private Text playerName;
    [SerializeField] private Text scoreboard;
    [SerializeField] private Text timeboard;
    [SerializeField] private Text placement;

    /*
     * Updates the end of game screen UI
     * 
     * @param win - whether the player won the game
     * @param name - the name of the player
     * @param score - the score the player achieved
     * @param time - the elapsed time of the round
     */
    public void UpdateUI(bool win, string name, int score, int time)
    {
        if (win) title.text = "Congratulations you won!";
        else title.text = "Game Over!";
        playerName.text = name;
        scoreboard.text = "score: " + score.ToString();
        timeboard.text = "time: " + time.ToString();
    }
}
