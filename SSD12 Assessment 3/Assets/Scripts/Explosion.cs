using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Base class for explosions, i.e objects which cause damage to objects in a radius
 */
public class Explosion : MonoBehaviour
{
    [SerializeField] private float baseDamage;
    [SerializeField] private AnimationCurve damageMultiplierDropOff;
    [SerializeField] private float maxRange;

    // Start is called before the first frame update
    void Start()
    {
        DealExplosionDamage();
    }

    /*
     * Check all hitboxes and their distance to the explosion. This could be optimized using selection of hitboxes, but for the 
     * purposes of this game, where there are never many entities this approach is fine. Deals damage based on distance.
     */
    private void DealExplosionDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, maxRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.GetComponent<GameCharacter>())
            {
                if (hitCollider.GetComponent<GameCharacter>().Health > 0)
                {
                    float damage = damageMultiplierDropOff.Evaluate(Vector3.Distance(this.transform.position, hitCollider.transform.position)) * baseDamage;
                    hitCollider.GetComponent<GameCharacter>().DealDamage(damage);
                }
            }
        }
    }
}
