using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Manages the level, score, player and enemies. Runs events for the games when required.
 */
public class GameManagement : MonoBehaviour
{
    private static float deltaTimeFrameSpeedConstant = 50; // multiplied through any multiplications of time.deltaTime

    private int score;
    private int timePast;
    private bool gameWin;
    private UserInterface UI;
    private static string playerName = "";
    private bool gameFinishedFlag;

    public static string PlayerName { get => playerName; set => playerName = value; }
    public static float DeltaTimeFrameSpeedConstant { get => deltaTimeFrameSpeedConstant; set => deltaTimeFrameSpeedConstant = value; }

    // Start is called before the first frame update
    void Start()
    {
        UI = FindObjectOfType<UserInterface>();
        InvokeRepeating("IncrementTime", 1f, 1f);
        DontDestroyOnLoad(gameObject);

        if (PlayerName == "") playerName = "Player" + Random.Range(0, 9999);
        gameFinishedFlag = false;
    }

    /*
     * Increment the time elapsed and update the UI show of this.
     */
    public void IncrementTime()
    {
        timePast++;
    }

    /*
     * Called when the player reaches a game end state e.g reaches the teleporter or is killed
     */
    public void EndGame()
    {
        if (gameFinishedFlag) return;
        SceneManager.LoadScene("End Screen");
        gameFinishedFlag = true;
    }

    /*
     * Called when the player reaches a game end state e.g reaches the teleporter or is killed
     */
    public void EndGame(bool gameWin)
    {
        if (gameFinishedFlag) return;
        gameFinishedFlag = true;
        this.gameWin = gameWin;
        if (gameWin) score = Mathf.RoundToInt(score * 1.5f);
        print(playerName + " score: " + score);
        ScoreManager.AppendScore(playerName, score);
        SceneManager.LoadScene("End Screen");
    }

    public void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            Destroy(gameObject);
        }
        if (SceneManager.GetActiveScene().name == "End Screen")
        {
            FindObjectOfType<EndScreenController>().UpdateUI(gameWin, playerName, score, timePast);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    /*
     * Update UI elements to reflect game states
     */
    private void UpdateUI()
    {
        UI.UpdateScoreText(score);
        UI.UpdateTimeText(timePast);
    }

    public void AddScore(int score)
    {
        this.score += score;
    }
}
