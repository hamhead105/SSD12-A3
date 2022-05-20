using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
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

    public void SetProjectileStats(float baseSpeed, float baseDamage, float gravity, bool useLimbMultipliers, float lifeTime)
    {
        this.baseSpeed = baseSpeed;
        this.baseDamage = baseDamage;
        this.gravity = gravity;
        this.useLimbMultipliers = useLimbMultipliers;
        this.lifeTime = lifeTime;
    }

    public void SetProjectileStats(float baseSpeed, float baseDamage, bool useLimbMultipliers, float lifeTime)
    {
        this.baseSpeed = baseSpeed;
        this.baseDamage = baseDamage;
        this.useLimbMultipliers = useLimbMultipliers;
        this.lifeTime = lifeTime;
    }

    public void Init()
    {
        velocity = transform.forward * baseSpeed;
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, velocity.magnitude))
        {
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

    protected virtual void Impact(GameObject effectSpawn, RaycastHit hit)
    {
        if (effectSpawn)
        {
            GameObject impactParticle = Instantiate(effectSpawn, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal)) as GameObject;
            if (GameObject.Find("Temp")) impactParticle.transform.parent = GameObject.Find("Temp").transform;
        }
    }

    public void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
