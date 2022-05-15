using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingProjectile : Projectile
{
    [SerializeField] private GameObject explosion;
    [SerializeField] private float damage;
    [SerializeField] private AnimationCurve damageDropOff;
    [SerializeField] private bool useImpactRotation;

    protected override void Impact(GameObject effectSpawn, RaycastHit hit)
    {
        if (explosion)
        {
            base.Impact(effectSpawn, hit);
            GameObject spawnedExplosion;
            if (useImpactRotation)
            {
                spawnedExplosion = Instantiate(explosion, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal)) as GameObject;
            }
            else
            {
                spawnedExplosion = Instantiate(explosion, hit.point, Quaternion.identity) as GameObject;
            }
            if (GameObject.Find("Temp")) spawnedExplosion.transform.parent = GameObject.Find("Temp").transform;
        } 
        else
        {
            Debug.LogWarning("The explosive projectile has no explosion");
        }
    }
}
