using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Base class for all character entities in the game.
 */
public abstract class GameCharacter : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
    private bool deathFlag;

    public float Health { get => health; set => health = value; }
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }

    /*
     * Called when the character starts or is reset etc
     */
    public abstract void Init();

    /*
     * Called when the character is killed
     */
    public abstract void Death();


    // Start is called before the first frame update
    void Start()
    {
        deathFlag = false;
        health = maxHealth;
    }
    
    /*
     * Deal damage to this character
     */
    public void DealDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Death();
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (health <= 0)
        {
            if (!deathFlag)
            {
                deathFlag = true;
                Death();
            }
        }
    }
}
