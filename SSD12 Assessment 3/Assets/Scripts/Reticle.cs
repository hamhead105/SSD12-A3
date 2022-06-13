using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Controls the visiblity of the dynamic reticle
 */
public class Reticle : MonoBehaviour
{
    [SerializeField] private GameObject[] reticleParts;

    /*
     * Make the player's reticle visible
     */
    public void EnableVisiblity()
    {
        foreach (GameObject reticlePart in reticleParts)
        {
            reticlePart.SetActive(true);
        }
    }

    /*
     * Make the player's reticle invisible
     */
    public void HideVisiblity()
    {
        foreach (GameObject reticlePart in reticleParts)
        {
            reticlePart.SetActive(false);
        }
    }
}
