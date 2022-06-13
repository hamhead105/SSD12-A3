using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class which handles the online help screen
 */
public class HelpUIManager : MonoBehaviour
{
    [SerializeField] private GameObject startingAGamePanel;
    [SerializeField] private GameObject playingTheGamePanel;
    [SerializeField] private GameObject accessingSettingsPanel;
    [SerializeField] private GameObject accessingScoreboardPanel;

    /*
     * Disable all panels
     */
    public void CloseAllPanels()
    {
        startingAGamePanel.SetActive(false);
        playingTheGamePanel.SetActive(false);
        accessingSettingsPanel.SetActive(false);
        accessingScoreboardPanel.SetActive(false);
    }

    /*
     * Open the starting a game panel
     */
    public void OpenStartingAGamePanel()
    {
        CloseAllPanels();
        startingAGamePanel.SetActive(true);
    }

    /*
     * Open the playing the game panel
     */
    public void OpenPlayingTheGamePanel()
    {
        CloseAllPanels();
        playingTheGamePanel.SetActive(true);
    }

    /*
     * Open the playing the accessing the settings panel
     */
    public void OpenAccessingTheSettingsPanel()
    {
        CloseAllPanels();
        accessingSettingsPanel.SetActive(true);
    }

    /*
     * Open the playing the accessing the settings panel
     */
    public void OpenAccessingTheScoreboardPanel()
    {
        CloseAllPanels();
        accessingScoreboardPanel.SetActive(true);
    }
}
