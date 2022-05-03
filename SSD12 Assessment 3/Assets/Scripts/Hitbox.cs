using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private Human player;
    [SerializeField] private float damageMultiplier;

    public Human Player { get => player; set => player = value; }

    public float getDamage(float rawDamage)
    {
        return rawDamage * damageMultiplier;
    }
}
    