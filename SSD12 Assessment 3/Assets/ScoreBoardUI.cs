using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Class which controls the display of the scoreboard
 */
public class ScoreBoardUI : MonoBehaviour
{
    private static int numberofScores = 5;
    [SerializeField] private Text[] nameDisplays = new Text[numberofScores];
    [SerializeField] private Text[] scoreDisplays = new Text[numberofScores];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScoreboard();
    }

    /*
     * Update the contents of the scoreboard
     */
    private void UpdateScoreboard()
    {
        ScoreEntry[] scoreEntries = ScoreManager.GetHighScores().ToArray();

        int i = 0; // iterator
        while (i < numberofScores)
        {
            if (i < scoreEntries.Length)
            {
                nameDisplays[i].text = scoreEntries[i].PlayerName;
                scoreDisplays[i].text = scoreEntries[i].Score.ToString();
            } 
            else
            {
                // if the score doesn't exist just set the UI to ---
                nameDisplays[i].text = "---";
                scoreDisplays[i].text = "---";
            }
            i++;
        }
    }
}
