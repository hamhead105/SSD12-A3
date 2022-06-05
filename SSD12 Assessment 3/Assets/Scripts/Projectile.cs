using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Base class for projectiles such as bullets, lasers etc
 */
public class Projectile : MonoBehaviour
{
    protected int layerMask = ~(1 << 9);
    private float baseSpeed;
    private float baseDamage;
    private float gravity;

    private Vector3 velocity;

    [SerializeField] private GameObject obstacleImpactParticles;

    [SerializeField] private float projectileForce;
    private PlayerFirearm item; // item that instantiated this projectile
    private bool queueDestroy;

    private bool canInteract = true;
    private bool useLimbMultipliers;

    private float lifeTime = 5;

    public PlayerFirearm Item { get => item; set => item = value; }

    /*
     * Set the stats of this projectile
     * 
     * @param baseSpeed - the speed of the projectile
     * @param baseDamage - the damage of the projectile
     * @param gravity - how fast the projectile will accelerate down
     * @param useLimbMultipliers - if the projectile does more damage from hitting weak spots and damage is affected by the limb damage multipliers
     * @param lifeTime - how long this projectile lasts before being deleted
     */
    public void SetProjectileStats(float baseSpeed, float baseDamage, float gravity, bool useLimbMultipliers, float lifeTime)
    {
        this.baseSpeed = baseSpeed;
        this.baseDamage = baseDamage;
        this.gravity = gravity;
        this.useLimbMultipliers = useLimbMultipliers;
        this.lifeTime = lifeTime;
    }

    /*
     * Set the stats of this projectile
     * 
     * @param baseSpeed - the speed of the projectile
     * @param baseDamage - the damage of the projectile
     * @param useLimbMultipliers - if the projectile does more damage from hitting weak spots and damage is affected by the limb damage multipliers
     * @param lifeTime - how long this projectile lasts before being deleted
     */
    public void SetProjectileStats(float baseSpeed, float baseDamage, bool useLimbMultipliers, float lifeTime)
    {
        this.baseSpeed = baseSpeed;
        this.baseDamage = baseDamage;
        this.useLimbMultipliers = useLimbMultipliers;
        this.lifeTime = lifeTime;
    }

    /*
     * Called when the projectile is initialized or started
     */
    public void Init()
    {
        velocity = transform.forward * baseSpeed;
        Destroy(gameObject, lifeTime);
    }

    /*
     * Called at a fixed rate
     */
    void FixedUpdate()
    {
        UpdateProjectile();
    }

    /*
     * Update projectile movement and if it has hit something
     */
    private void UpdateProjectile()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, velocity.magnitude, layerMask))
        {
            print("hit hitbox " + hit.transform.name + " " + hit.transform.gameObject.layer);
            if (hit.transform.gameObject.layer == 7) // check if hitbox layer
            {
                float damage = baseDamage;
                hit.transform.GetComponent<Hitbox>().Hit(damage);
                if (item) item.HitAnotherTarget(hit.transform.GetComponent<Hitbox>().GameCharacter.Health <= 0);

                Impact(obstacleImpactParticles, hit);
                queueDestroy = true;
            }
            else
            {
                Impact(obstacleImpactParticles, hit);

                queueDestroy = true;
            }
            if (queueDestroy) DestroyProjectile();
        }
        Vector3 oldPosition = this.transform.position;
        velocity += new Vector3(0, -gravity, 0);
        this.transform.position += velocity;
    }

    /*
     * Called when a projectile hits a surface
     * 
     * @param effectSpawn - the effect spawned when the projectile hits a surface
     * @param hit - the hit type containing hit position, type etc
     */
    protected virtual void Impact(GameObject effectSpawn, RaycastHit hit)
    {
        if (effectSpawn)
        {
            GameObject impactParticle = Instantiate(effectSpawn, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal)) as GameObject;
            if (GameObject.Find("Temp")) impactParticle.transform.parent = GameObject.Find("Temp").transform;
        }
    }

    /*
     * Destroy this object
     */
    public void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
