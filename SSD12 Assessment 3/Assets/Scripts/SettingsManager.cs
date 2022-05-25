using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class SettingsManager : MonoBehaviour
{
    private string filePath;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private Text sensitivityText;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Text volumeText;
    [SerializeField] private int settingResolutionIndex;
    private bool toggleLeanValue;
    Resolution[] resolutions;
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    private bool settingFullscreen;

    private float sensitivity;
    private float volume;

    public int SettingResolutionIndex { get => settingResolutionIndex; set => settingResolutionIndex = value; }
    public Resolution[] Resolutions { get => resolutions; set => resolutions = value; }
    public Dropdown ResolutionDropdown { get => resolutionDropdown; set => resolutionDropdown = value; }
    public Toggle FullscreenToggle { get => fullscreenToggle; set => fullscreenToggle = value; }
    public bool SettingFullscreen { get => settingFullscreen; set => settingFullscreen = value; }

    private void Start()
    {
        filePath = Application.dataPath + "/settings/";

        SetUpSettings();
        InitializeResolutionSettings();
    }

    public void SetUpSettings()
    {
        print("set up settings");
        UpdateSensitivity(PlayerPrefs.GetFloat("Input_Sensitivity", 0.5f));
        UpdateVolume(PlayerPrefs.GetFloat("Audio_Volume", 0.5f));
    }

    public void InitializeResolutionSettings()
    {
        if (!ResolutionDropdown) return;
        ResolutionDropdown.ClearOptions();
        Resolutions = Screen.resolutions;
        List<string> options = new List<string>();
        for (int i = 0; i < Resolutions.Length; i++)
        {
            string option = Resolutions[i].width + " x " + Resolutions[i].height + " " + Resolutions[i].refreshRate + "hz";
            options.Add(option);
        }

        ResolutionDropdown.AddOptions(options);
        SettingResolutionIndex = PlayerPrefs.GetInt("Display_ResolutionSettings", 0);

        ResolutionDropdown.value = SettingResolutionIndex;
        if (PlayerPrefs.GetInt("Display_Fullscreen", 1) == 1)
        {
            SettingFullscreen = true;
            FullscreenToggle.isOn = true;
        }
        else
        {
            SettingFullscreen = false;
            FullscreenToggle.isOn = false;
        }
    }

    public void UpdateSensitivity()
    {
        sensitivityText.text = sensitivitySlider.value.ToString();
        foreach (Player player in FindObjectsOfType<Player>()) { player.UpdateSensitivity(sensitivitySlider.value); }
        PlayerPrefs.SetFloat("Input_Sensitivity", sensitivitySlider.value);
    }

    public void UpdateSensitivity(float sensitivity)
    {
        sensitivityText.text = sensitivity.ToString();
        sensitivitySlider.value = sensitivity;
        foreach (Player player in FindObjectsOfType<Player>()) { player.UpdateSensitivity(sensitivitySlider.value); }
        PlayerPrefs.SetFloat("Input_Sensitivity", sensitivitySlider.value);
    }
    public void UpdateVolume()
    {
        volumeText.text = volumeSlider.value.ToString();
        foreach (Player player in FindObjectsOfType<Player>()) { player.UpdateVolume(volumeSlider.value); }
        PlayerPrefs.SetFloat("Audio_Volume", volumeSlider.value);
    }

    public void UpdateVolume(float volume)
    {
        volumeText.text = volume.ToString();
        volumeSlider.value = volume;
        foreach (Player player in FindObjectsOfType<Player>()) { player.UpdateVolume(volumeSlider.value); }
        PlayerPrefs.SetFloat("Audio_Volume", volume);
    }

    public void SetResolution()
    {
        SettingResolutionIndex = resolutionDropdown.value;
    }

    public void FullScreenToggle()
    {
        SettingFullscreen = fullscreenToggle.isOn;
    }

    public void ApplySettings()
    {
        print("applied settings");
        PlayerPrefs.SetInt("Display_ResolutionSettings", SettingResolutionIndex);
        if (SettingFullscreen)
        {
            PlayerPrefs.SetInt("Display_Fullscreen", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Display_Fullscreen", 0);
        }
        Screen.SetResolution(Resolutions[SettingResolutionIndex].width, Resolutions[SettingResolutionIndex].height, SettingFullscreen);  
    }

    public void SaveSettings()
    {
        string settings = GenerateSettings();
        if (!Directory.Exists(Application.dataPath + "/Settings/")) 
        {
            Directory.CreateDirectory(Application.dataPath + "/Settings/");
        }
        if (!File.Exists(filePath))
        {
            File.Create(filePath);
        }
        File.WriteAllText(filePath, settings); // write into file
        //File.AppendAllText();
    }

    public void ReadSettings()
    {
        if (!Directory.Exists(Application.dataPath + "/Settings/"))
        {
            print("unable to find directory");
            return;
        }
        if (!File.Exists(filePath))
        {
            print("unable to find file");
            return;
        }

        string settings = File.ReadAllText(filePath);
        string[] settingsArray = settings.Split(' ');
        sensitivity = Convert.ToInt32(settingsArray[0]);
        volume = Convert.ToInt32(settingsArray[1]);
    }

    public string GenerateSettings()
    {
        return sensitivity + " " + volume;
    }
}
