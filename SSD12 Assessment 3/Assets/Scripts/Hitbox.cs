using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class which defines hitboxes, which are colliders which deal damage to the character they belong to when hit.
 */
public class Hitbox : MonoBehaviour
{
    [SerializeField] private GameCharacter gameCharacter;
    
    [SerializeField] private float damageMultiplier;

    public GameCharacter GameCharacter { get => gameCharacter; set => gameCharacter = value; }

    /*
     * return damage calculated based on hitbox
     */
    public float getDamage(float rawDamage)
    {
        return rawDamage * damageMultiplier;
    }

    /*
     * deal damage to the character this hitbox is attached
     */
    public void Hit(float damage)
    {
        gameCharacter.DealDamage(damage);
    }
}
    