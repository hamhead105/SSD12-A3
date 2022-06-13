using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Manages the graphics settings for the game for each scene
 */
public class GraphicsManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.SetQualityLevel(SettingsManager.GraphicsSetting);
    }
}
