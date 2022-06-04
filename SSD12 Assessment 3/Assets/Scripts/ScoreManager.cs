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
    private static string filePath;
    private static string directoryPath;

    // Start is called before the first frame update
    void Start()
    {
        InitializeDirectoryPaths();
        //RunDriver();
    }

    /*
     * A driver alghorithm to teset sorting and players
     */
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
     * @return - a list of sorted scoreEntry from the score file
     */
    public static List<ScoreEntry> GetHighScores()
    {
        return InsertionSortScores(ReadScores());
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
        if (!File.Exists(filePath))
        {
            ClearScores(); // clearing scores also creates a new file
        }
    }

    /*
     * Add a score to the scoreFile based on a string and integer
     */
    public static void AppendScore(string playerName, int score)
    {
        StreamWriter writer = new StreamWriter(filePath, true); // Open file for append
        writer.Write(playerName + ":" + score + ",");
        writer.Close();
    }

    /*
     * Add a score to the scoreFile based on a string and integer
    */
    public static void ClearScores()
    {
        StreamWriter writer = new StreamWriter(filePath, false); // Open file for append
        writer.Write("");
        writer.Close();
    }

    /*
     * Open the scores file and return a list of scores 
     * entry list will look like [playerName]:[score],[playerName]:[score],
     * 
     * @return - a list of string where each entry contains the name of the player and the score they achieved
     */
    public static List<ScoreEntry> ReadScores()
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
    public static List<ScoreEntry> InsertionSortScores(List<ScoreEntry> scores)
    {
        int y;
        List<ScoreEntry> sortedScores = scores;

        // keeps repeating until the list is sorted
        for (int x = 1; x < sortedScores.Count; x++) 
        {
            ScoreEntry key = sortedScores[x];
            y = x - 1;

            // move elements that are greater than the key, along the array
            while (y >= 0 && sortedScores[y].Score < key.Score)
            {
                sortedScores[y + 1] = sortedScores[y];
                y--;
            }
            sortedScores[y + 1] = key;
        }

        return sortedScores;
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
}
