using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private void FixedUpdate()
    {
        UpdateHealthShow();
    }

    public void UpdateItemShow()
    {
        itemName.text = player.Item.ItemName;
        itemIcon.texture = player.Item.ItemIcon;
    }

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

    public void UpdateHealthShow()
    {
        if (player)
        {
            healthSlider.value = player.Health;
            healthTraceSlider.value += (healthSlider.value - healthTraceSlider.value) * healthTrace_kP;
        }
    }

    public void UpdateTimeText(int time)
    {
        timeText.text = time.ToString();
    }

    public void UpdateScoreText(int score)
    {
        scoreText.text = score.ToString();
    }

    public void OpenSettings()
    {
        settings.SetActive(true);
    }

    public void CloseSettings()
    {
        settings.SetActive(false);
    }
}
