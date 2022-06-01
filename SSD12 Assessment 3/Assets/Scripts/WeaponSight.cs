using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * An optic that is on a player weapon
 */
public class WeaponSight : MonoBehaviour
{
    [SerializeField] private string attachmentName;
    [SerializeField] private Texture itemIcon;

    [SerializeField] private float useFOV;
    [SerializeField] private Vector3 positionalOffset;
    [SerializeField] private float sensitivityMultiplier;
    [SerializeField] private GameObject localObjects;

    public float UseFOV { get => useFOV; set => useFOV = value; }
    public Vector3 PositionalOffset { get => positionalOffset; set => positionalOffset = value; }
    public float SensitivityMultiplier { get => sensitivityMultiplier; set => sensitivityMultiplier = value; }
    public Texture ItemIcon { get => itemIcon; set => itemIcon = value; }

    /*
     * Update whether the local components are enabled. For example, the camera attached to scopes is disabled for sights on 
     * non local items. This is to improve performace.
     */
    public void EnableCamera()
    {
        if (localObjects) localObjects.SetActive(true);
    }
}
