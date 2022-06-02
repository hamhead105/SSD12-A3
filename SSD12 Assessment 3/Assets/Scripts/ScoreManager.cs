using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

/*
 * Controls the scores of players as well as writing and reading scrores from a file
 */
public class ScoreManager : MonoBehaviour
{
    private string filePath;
    private string directoryPath;

    // Start is called before the first frame update
    void Start()
    {
        InitializeDirectoryPaths();
        RunDriver();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RunDriver()
    {
        ClearScores();
        AppendScore("p1", 10);
        AppendScore("p2", 50);
        AppendScore("p3", 30);
        AppendScore("p4", 45);
        AppendScore("p5", 21);
        AppendScore("p6", 19);

        List<ScoreEntry> sortedScores = InsertionSortScores(ReadScores());
        foreach (ScoreEntry sortedScore in sortedScores)
        {
            print(sortedScore.PlayerName + " " + sortedScore.Score);
        }
    }

    /*
     * Get the directory paths to write file to. Based on whether the game is being run from the editor or the built version
     * this path will differ.
     */
    public void InitializeDirectoryPaths()
    {
        if (Application.isEditor)
        {
            filePath = "Assets/_GameFiles/Scores.txt";
            directoryPath = "Assets/_GameFiles";
        }
        else
        {
            filePath = Application.dataPath + "/_GameFiles/Scores.txt";
            directoryPath = Application.dataPath + "/_GameFiles";
        }

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    /*
     * Add a score to the scoreFile based on a string and integer
     */
    public void AppendScore(string playerName, int score)
    {
        StreamWriter writer = new StreamWriter(filePath, true); // Open file for append
        writer.Write(playerName + ":" + score + ",");
        writer.Close();
    }

    /*
     * Add a score to the scoreFile based on a string and integer
    */
    public void ClearScores()
    {
        StreamWriter writer = new StreamWriter(filePath, false); // Open file for append
        writer.Write("");
        writer.Close();
    }

    /*
    * Delete the scores file
    */
    public void DeleteScoresFile()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    /*
     * Open the scores file and return a list of scores 
     * 
     * @return - a list of string where each entry contains the name of the player and the score they achieved
     */
    public List<ScoreEntry> ReadScores()
    {
        if (!Directory.Exists(directoryPath))
        {
            print("unable to find directory");
            return new List<ScoreEntry>();
        }
        if (!File.Exists(filePath))
        {
            print("unable to find file");
            return new List<ScoreEntry>();
        }

        StreamReader reader = new StreamReader(filePath);
        string scores = reader.ReadToEnd();
        string[] scoresArray = scores.Split(',');

        List<ScoreEntry> scoreEntries = new List<ScoreEntry>();
        for (int i = 0; i < scoresArray.Length - 1; i++) // don't count the last comma as there is no score entry there, since each new entry ends with a comma
        {
            string score = scoresArray[i];
            string[] splitScore = score.Split(':');
            if (splitScore.Length == 2)
            {
                ScoreEntry scoreEntry = new ScoreEntry(splitScore[0], Convert.ToInt32(splitScore[1]));
                scoreEntries.Add(scoreEntry);
            } else
            {
                print("irregular split " + score);
            }
        }
        reader.Close();

        return scoreEntries;
    }

    /*
     * Use insertion sorting techniques to sort a list of scoreEntry types
     * 
     * @param scores - a list of scores to be sorted
     */
    public List<ScoreEntry> InsertionSortScores(List<ScoreEntry> scores)
    {
        int y;
        List<ScoreEntry> sortedScores = scores;
        for (int x = 1; x < sortedScores.Count; x++) 
        {
            ScoreEntry key = sortedScores[x];
            y = x - 1;

            while (y >= 0 && sortedScores[y].Score > key.Score)
            {
                sortedScores[y + 1] = sortedScores[y];
                y--;
            }
            sortedScores[y + 1] = key;
        }

        return sortedScores;
    }

}
