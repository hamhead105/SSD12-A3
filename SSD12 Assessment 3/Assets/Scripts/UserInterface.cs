using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Base class for the player's user interface or GUI.
 */
public class UserInterface : MonoBehaviour
{
    private static float healthTrace_kP = 0.2f;

    private Player player;

    [SerializeField] private Text itemName;
    [SerializeField] private RawImage itemIcon;
    [SerializeField] private Text ammoText;
    [SerializeField] private Text maxAmmoText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider healthTraceSlider;
    [SerializeField] private GameObject settings;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text timeText;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateItemShow();
        UpdateAmmoShow();
    }

    /*
     * Called at a fixed rate
     */
    private void FixedUpdate()
    {
        UpdateHealthShow();
    }


    /*
     * Update the UI element which shows what item the player is holding
     */
    public void UpdateItemShow()
    {
        itemName.text = player.Item.ItemName;
        itemIcon.texture = player.Item.ItemIcon;
    }

    /*
     * Update the UI element which shows how much ammo the player has left
     */
    public void UpdateAmmoShow()
    {
        if (player.Item is PlayerFirearm)
        {
            ammoText.text = (player.Item as PlayerFirearm).Ammo.ToString();
            maxAmmoText.text = (player.Item as PlayerFirearm).MaxAmmo.ToString();
        }
        else
        {
            ammoText.text = "-";
            maxAmmoText.text = "-";
        }
    }

    /*
     * Update the health bar for the player
     */
    public void UpdateHealthShow()
    {
        if (player)
        {
            healthSlider.value = player.Health;
            healthTraceSlider.value += (healthSlider.value - healthTraceSlider.value) * healthTrace_kP;
        }
    }

    /*
     * Close the player's menu which is opened with the escape key
     */
    public void CloseMenu()
    {
        FindObjectOfType<Player>().MenuOpened = false;
        FindObjectOfType<Player>().UpdateMenu();
        print("close menu");
    }

    /*
     * Update the time indicator
     * 
     * @param time - the new time to put on the indicator
     */
    public void UpdateTimeText(int time)
    {
        timeText.text = time.ToString();
    }

    /*
     * Update the scoreboard
     * 
     * @param score - the score to put on the scoreboard
     */
    public void UpdateScoreText(int score)
    {
        scoreText.text = score.ToString();
    }

    /*
     * Open the settings menu which contains, settings, leave button etc
     */
    public void OpenSettings()
    {
        settings.SetActive(true);
    }


    /*
     * Close the settings menu 
     */
    public void CloseSettings()
    {
        settings.SetActive(false);
    }
}
