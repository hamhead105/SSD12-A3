using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameCharacter : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
    private bool deathFlag;

    public float Health { get => health; set => health = value; }
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }

    public abstract void Init();

    public abstract void Death();


    // Start is called before the first frame update
    void Start()
    {
        deathFlag = false;
        health = maxHealth;
    }
    
    public void DealDamage(float amount)
    {
        health -= amount;
    }

    // Update is called once per frame
    public void Update()
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
