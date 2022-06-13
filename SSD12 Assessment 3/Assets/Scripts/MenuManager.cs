using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Class responsible for handling transitioning of scenes
 */
public class MenuManager : MonoBehaviour
{
    /*
     * Load the scene requested
     */
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    /*
     * Closes the game
     */
    public void CloseGame()
    {
        Application.Quit();
    }
}
