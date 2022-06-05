using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Base class for items in the world which the player can touch to collect
 */
public class Collectable : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;
    [SerializeField] private int scoreValue;
    [SerializeField] private GameObject collectEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Called at a fixed rate
    void FixedUpdate()
    {
        PlayRotateAnimation();
    }

    /*
     * Make the item spin around and look nice
     */
    private void PlayRotateAnimation()
    {
        this.transform.Rotate(rotateSpeed, rotateSpeed, rotateSpeed);
    }

    /*
     * Called when the item has been touched and collected by the player
     */
    public void Collect()
    {
        if (collectEffect) Instantiate(collectEffect, this.transform.position, Quaternion.identity);
        FindObjectOfType<GameManagement>().AddScore(scoreValue);
        Destroy(gameObject);
    }
}
