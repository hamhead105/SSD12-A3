using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Contains information for a score entry for the score manager, i.e a player and what score they have
 */
public class ScoreEntry : MonoBehaviour
{
    private string playerName;
    private int score;

    public string PlayerName { get => playerName; set => playerName = value; }
    public int Score { get => score; set => score = value; }

    public ScoreEntry(string playerName, int score)
    {
        this.playerName = playerName;
        this.score = score;
    }
}
