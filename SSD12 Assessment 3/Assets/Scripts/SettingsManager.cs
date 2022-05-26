using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class SettingsManager : MonoBehaviour
{
    private string filePath;
    private string directoryPath;

    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private Text sensitivityText;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Text volumeText;

    Resolution[] resolutions;
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    private static float sensitivity;
    private static float volume;
    private static int resolutionIndex;
    private static int fullScreen;

    private static float defaultSensitivity = 0.5f;
    private static float defaultVolume = 1;

    private void Start()
    {
        InitializeDirectoryPaths();
        if (!ReadSettings()) LoadDefaultSettings();
        InitializeResolutionSettings();
        UpdateSettings();
    }

    public void InitializeDirectoryPaths()
    {
        if (Application.isEditor)
        {
            filePath = "Assets/_GameFiles/Settings.txt";
            directoryPath = "Assets/_GameFiles";
        }
        else
        {
            filePath = Application.dataPath + "/GameFiles/settings";
            directoryPath = Application.dataPath + "/GameFiles";
        }
    }

    private void UpdateSettings()
    {
        volumeSlider.value = volume;
        sensitivitySlider.value = sensitivity;

        Player player = FindObjectOfType<Player>();
        player.UpdateSensitivity(sensitivity);
        player.UpdateVolume(volume); 

        if (resolutionDropdown) resolutionDropdown.value = resolutionIndex;
        if (fullscreenToggle) fullscreenToggle.isOn = fullScreen == 0 ? false : true; // if fullscreen is 0, the toggle is set to false, else true
    }

    private void LoadDefaultSettings()
    {
        sensitivity = defaultSensitivity;
        volume = defaultVolume;
        SaveSettings();
    }

    public void InitializeResolutionSettings()
    {
        if (!resolutionDropdown) return;
        resolutionDropdown.ClearOptions();
        resolutions = Screen.resolutions;
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " " + resolutions[i].refreshRate + "hz";
            options.Add(option);
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = resolutionIndex;

        if (fullScreen == 1)
        {
            fullscreenToggle.isOn = true;
        }
        else
        {
            fullscreenToggle.isOn = false;
        }
    }

    public void UpdateSensitivity()
    {
        sensitivityText.text = sensitivitySlider.value.ToString();
        foreach (Player player in FindObjectsOfType<Player>()) { player.UpdateSensitivity(sensitivitySlider.value); }
        SaveSettings();        
    }

    public void UpdateSensitivity(float newSenseitivity)
    {
        sensitivityText.text = newSenseitivity.ToString();
        sensitivitySlider.value = newSenseitivity;
        foreach (Player player in FindObjectsOfType<Player>()) { player.UpdateSensitivity(sensitivitySlider.value); }
        SaveSettings();
    }
    public void UpdateVolume()
    {
        volumeText.text = volumeSlider.value.ToString();
        foreach (Player player in FindObjectsOfType<Player>()) { player.UpdateVolume(volumeSlider.value); }
    }

    public void UpdateVolume(float newVolume)
    {
        volumeText.text = newVolume.ToString();
        volumeSlider.value = newVolume;
        foreach (Player player in FindObjectsOfType<Player>()) { player.UpdateVolume(volumeSlider.value); }
    }

    public void ApplySettings()
    {
        volume = volumeSlider.value;
        sensitivity = sensitivitySlider.value;

        bool fullScreenBool = fullscreenToggle.isOn;
        if (fullScreenBool) fullScreen = 1;
        else fullScreen = 0;

        print("applied settings");
        resolutionIndex = resolutionDropdown.value;
        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, fullScreenBool);   // return false when fullscreen = 0 and true for fullscreen = 1
        SaveSettings();
    }

    public void SaveSettings()
    {
        print("save settings");
        string settings = GenerateSettings();
        if (!Directory.Exists(directoryPath)) 
        {
            Directory.CreateDirectory(directoryPath);
        }
        if (!File.Exists(filePath))
        {
            File.Create(filePath);
        }
        StreamWriter writer = new StreamWriter(filePath);
        writer.Write(settings);
        writer.Close();
    }

    /*
     * Open the settings file and set all variables to the settings in file
     * 
     * @return - whether the save file exists and is valid
     */
    public bool ReadSettings()
    {
        if (!Directory.Exists(directoryPath))
        {
            print("unable to find directory");
            return false;
        }
        if (!File.Exists(filePath))
        {
            print("unable to find file");
            return false;
        }

        StreamReader reader = new StreamReader(filePath);
        string settings = reader.ReadToEnd(); ;
        string[] settingsArray = settings.Split(' ');
        reader.Close();

        if (settingsArray.Length == 4)
        {
            sensitivity = (float) Convert.ToDouble(settingsArray[0]);
            volume = (float) Convert.ToDouble(settingsArray[1]);
            resolutionIndex = Convert.ToInt32(settingsArray[2]);
            fullScreen = Convert.ToInt32(settingsArray[3]);
            return true;
        } 
        else
        {
            return false;
        }
    }

    public string GenerateSettings()
    {
        return sensitivity + " " + volume + " " + resolutionIndex + " " + fullScreen;
    }
}
