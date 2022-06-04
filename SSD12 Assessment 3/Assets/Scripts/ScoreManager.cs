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
    private static int maxScoreEntries = 10;

    // Start is called before the first frame update
    void Start()
    {
        InitializeDirectoryPaths();
        ClearScores();
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

    public void RepeatableDriver()
    {
        AppendScore("player" + UnityEngine.Random.Range(0, 9999), UnityEngine.Random.Range(0, 9999));
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
     * 
     * @param playerName - the name of the player for a new entry
     * @param score - the score of the player for a new entry
     */
    public static void AppendScore(string playerName, int score)
    {
        AppendToFile(playerName + ":" + score + ",");
        if (GetScoreEntryCount() > maxScoreEntries)
        {
            ReduceScoreFile(maxScoreEntries);
        }
    }

    /*
     * Override and replace the contents of the score file
     * 
     * @param scoreEntries - a list of scoreEntries to write in
     */
    public static void OverwriteScoreFile(ScoreEntry[] scoreEntries)
    {
        if (GetScoreEntryCount() > maxScoreEntries)
        {
            print("reduce score file A");
            ReduceScoreFile(maxScoreEntries);
        }
        else
        {
            foreach (ScoreEntry scoreEntry in scoreEntries)
            {
                OutputToFile(scoreEntry.PlayerName + ":" + scoreEntry.Score + ",");
            }
        }
    }

    /*
     * Write to a file, overwriting its current contents
     * 
     * @param text - the text to write into the file
     */
    public static void OutputToFile(string text)
    {
        using (StreamWriter outputFile = new StreamWriter(filePath, false))
        {
            outputFile.Write(text);
            outputFile.Close();
        }
    }

    /*
     * Append to a file, adding text to what is already written
     * 
     * @param text - the text to write
     */
    public static void AppendToFile(string text)
    {
        using (StreamWriter outputFile = new StreamWriter(filePath, true))
        {
            outputFile.Write(text);
            outputFile.Close();
        }
    }

    /*
     * Read the contents of a file to the end
     * 
     * @return - the contents of the file
     */
    public static string ReadFile()
    {
        string read;
        using (StreamReader inputFile = new StreamReader(filePath))
        {
            read = inputFile.ReadToEnd();
            inputFile.Close();
        }
        return read;
    }

    /*
     * Get the number of scores in the score file
     * 
     * @return - the number of score entries in the score file
     */
    public static int GetScoreEntryCount()
    {
        if (!Directory.Exists(directoryPath))
        {
            print("unable to find directory");
            return 0;
        }
        if (!File.Exists(filePath))
        {
            print("unable to find file");
            return 0;
        }

        string scores = ReadFile();
        string[] scoresArray = scores.Split(',');

        return scoresArray.Length;
    }


    /*
     * Delete excessive score entries in the file, preserve the highest scores
     * 
     * @param saveEntries - how many of the highest score entries should be saved
     */
    private static void ReduceScoreFile(int saveEntries)
    {
        List<ScoreEntry> sortedScoreEntries = GetHighScores();
        ClearScores();
        int i = 0;
        int loopBound; 
        if (saveEntries < sortedScoreEntries.Count) loopBound = saveEntries;
        else loopBound = sortedScoreEntries.Count;
        // if there are less save entries than entries allowed to be saved, just end when we reach the number of score entries
        while (i < loopBound)
        {
            AppendToFile(sortedScoreEntries[i].PlayerName + ":" + sortedScoreEntries[i].Score + ",");
            i++;
        }
    }

    /*
     * Add a score to the scoreFile based on a string and integer
    */
    public static void ClearScores()
    {
        print("clear scores");
        OutputToFile("");
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

        string scores = ReadFile();
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
    public static void DeleteScoresFile()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
