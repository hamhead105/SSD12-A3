using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Base class for all enemies in the game. Different enemies may behave differently and employ different tactics to defeat the player.
 */
public class Enemy : GameCharacter
{
    [SerializeField] protected GameObject[] deathInstantiations;
    [SerializeField] protected float speed;
    [SerializeField] protected int pointsAwarded;

    /* 
     * Initialize the enemy
     */
    public override void Init()
    {
       
    }

    /*
     * Called when the enemy's health falls below zero. Destroy the enemy
     */
    public override void Death()
    {
        foreach (GameObject deathSpawn in deathInstantiations)
        {
            GameObject newObject = Instantiate(deathSpawn, this.transform.position, transform.rotation) as GameObject;
            if (GameObject.Find("Temp"))
            {
                newObject.transform.parent = GameObject.Find("Temp").transform;
            }
        }
        FindObjectOfType<GameManagement>().AddScore(pointsAwarded);
        Destroy(gameObject);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
