using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Level of detail manager, decreases details based on distance to the player.
 * Disables effects of choice based on distance to the player.
 */
public class LODManager : MonoBehaviour
{
    private float highGraphicsMultiplier = 1;
    private float midGraphicsMultiplier = 0.7f;
    private float lowGraphicsMultiplier = 0.4f;

    [SerializeField] private GameObject[] disappearingObjects;
    [SerializeField] private float baseMaxDistance;
    private float maxDistance;
    // Start is called before the first frame update
    void Start()
    {
        if (QualitySettings.GetQualityLevel() == 0) maxDistance = baseMaxDistance * lowGraphicsMultiplier;
        if (QualitySettings.GetQualityLevel() == 1) maxDistance = baseMaxDistance * midGraphicsMultiplier;
        if (QualitySettings.GetQualityLevel() == 2) maxDistance = baseMaxDistance * highGraphicsMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDisappearingAssets();
    }

    /*
     * Hide objects which are too far from the player for game optimization 
     */
    private void UpdateDisappearingAssets()
    {
        if (Vector3.Distance(transform.position, FindObjectOfType<Player>().transform.position) > maxDistance)
        {
            foreach (GameObject item in disappearingObjects) item.SetActive(false);
        }
        else
        {
            foreach (GameObject item in disappearingObjects) item.SetActive(true);
        }
    }
}
