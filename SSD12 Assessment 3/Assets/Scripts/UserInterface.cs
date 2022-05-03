using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    [SerializeField] private Text[] loadedAmmoTexts;
    [SerializeField] private Text[] availableAmmoTexts;
    [SerializeField] private Text eventNotification;
    [SerializeField] private Text secondaryNotification; // used for timer
    [SerializeField] private GameObject settings;

    [SerializeField] private Text team1Wins;
    [SerializeField] private Text team2Wins;
    [SerializeField] private Text healthText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider healthTraceSlider;

    [SerializeField] private Text itemNameBoard;
    [SerializeField] private RawImage itemIconBoard;

    private float healthTraceSlider_kP = 0.2f;

    private int lastAmmo = 0;
    private int lastAvailableAmmo = 0;

    private void Start()
    {

    }

    public void FixedUpdate()
    {
        if (healthTraceSlider) healthTraceSlider.value += (healthSlider.value - healthTraceSlider.value) * healthTraceSlider_kP;
    }

    public void SetAmmoTexts(int loadedAmmo, int availableAmmo)
    {
        foreach (Text text in loadedAmmoTexts)
        {
            text.text = loadedAmmo.ToString();
            if (loadedAmmo != lastAmmo) GlitchAmmoDisplayEffect();
        }

        foreach (Text text in availableAmmoTexts)
        {
            text.text = availableAmmo.ToString();
            if (availableAmmo != lastAvailableAmmo) GlitchAmmoDisplayEffect();
        }

        lastAmmo = loadedAmmo;
        lastAvailableAmmo = availableAmmo;
    }

    public void GlitchAmmoDisplayEffect()
    {
        // TODO implement text glitch effect
    }

    public void ToggleSettings(bool opened)
    {
        if (opened) settings.SetActive(opened);
        else settings.SetActive(opened);
    }

    public void ShowNotification(string notification)
    {
        eventNotification.text = notification;
        eventNotification.enabled = true;
        Invoke("HideNotification", 5f);
    }

    private void HideNotification()
    {
        eventNotification.enabled = false;
    }

    public void ShowSecondaryNotification()
    {
        secondaryNotification.enabled = true;
    }

    public void SetSecondaryNotificationText(string text)
    {
        secondaryNotification.text = text;
    }

    public void HideSecondaryNotification()
    {
        secondaryNotification.enabled = false;
    }

    public void UpdateWinsDisplay(int team1Wins, int team1RoundsWon, int team2Wins, int team2RoundsWon)
    {
        string team1Text = "team 1 wins: " + team1Wins.ToString();
        if (team1RoundsWon == 1) team1Text += ".";
        this.team1Wins.text = team1Text;
        string team2Text = "team 2 wins: " + team2Wins.ToString();
        if (team2RoundsWon == 1) team2Text += ".";
        this.team2Wins.text = team2Text;
    }

    public void UpdateHealthDisplay(float health)
    {
        healthText.text = "Health: " + Mathf.Round(health).ToString();
        if (healthSlider) healthSlider.value = health;   
    }

    public void UpdateItemDisplay(string itemName, Texture itemIcon)
    {
        if (!itemNameBoard) return;
        itemNameBoard.text = itemName;
        if (itemIcon != null)
        {
            itemIconBoard.texture = itemIcon;
            itemIconBoard.color = new Color(1, 1, 1, 1);
        } 
        else
        {
            itemIconBoard.color = new Color(1, 1, 1, 0);
        }
    }

    public void LeaveGame()
    {

    }
}
