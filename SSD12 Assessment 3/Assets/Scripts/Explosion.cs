using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
