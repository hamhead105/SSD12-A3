using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private GameCharacter gameCharacter;
    
    [SerializeField] private float damageMultiplier;

    public GameCharacter GameCharacter { get => gameCharacter; set => gameCharacter = value; }

    public float getDamage(float rawDamage)
    {
        return rawDamage * damageMultiplier;
    }
}
    