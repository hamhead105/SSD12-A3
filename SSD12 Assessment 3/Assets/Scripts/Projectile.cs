using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float baseSpeed;
    private float baseDamage;
    private float gravity;
    private bool active; // active when this has been spawned by the client

    private Vector3 velocity;

    [SerializeField] private GameObject stoneImpactParticles;

    [SerializeField] private float projectileForce;
    private PlayerFirearm item; // item that instantiated this projectile
    [SerializeField] private GameObject deformerBrush;
    private bool queueDestroy;

    private bool canInteract = true;
    private float deformationSize;
    private float penetration;
    private float deformationChance;
    private float distanceTravelled;

    private AnimationCurve penetrationDropOff;
    private AnimationCurve damageDropOff;
    private bool useLimbMultipliers;

    public PlayerFirearm Item { get => item; set => item = value; }
    public bool Active { get => active; set => active = value; }

    public void SetProjectileStats(float baseSpeed, float baseDamage, float gravity, float deformationSize, float penetration, float deformationChance, AnimationCurve damageDropOff, AnimationCurve penetrationDropOff, bool useLimbMultipliers)
    {
        this.baseSpeed = baseSpeed;
        this.baseDamage = baseDamage;
        this.gravity = gravity;
        this.deformationSize = deformationSize;
        this.penetration = penetration;
        this.deformationChance = deformationChance;
        this.penetrationDropOff = penetrationDropOff;
        this.damageDropOff = damageDropOff;
        this.useLimbMultipliers = useLimbMultipliers;
    }

    private void Start()
    {
        Destroy(gameObject, 3f);
        velocity = transform.forward * baseSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, velocity.magnitude))
        {
            distanceTravelled += Vector3.Distance(transform.position, hit.point);
            if (hit.transform.gameObject.layer == 7) // check if hitbox layer
            {
                // Instantiate player impact particles
                //if (GameObject.Find("Temp")) impactParticle.transform.parent = GameObject.Find("Temp").transform;
                if (damageDropOff != null)
                {
                    float damage = baseDamage * damageDropOff.Evaluate(distanceTravelled);
                    if (useLimbMultipliers) damage = hit.transform.GetComponent<Hitbox>().getDamage(baseDamage * damageDropOff.Evaluate(distanceTravelled));

                    item.HitAnotherTarget(hit.transform.GetComponent<Hitbox>().GameCharacter.Health <= 0);
                }
                else
                {
                    float damage = baseDamage;
                    item.HitAnotherTarget(hit.transform.GetComponent<Hitbox>().GameCharacter.Health <= 0);
                }
                
                queueDestroy = true;
            }
            else if (hit.transform.tag == "Stone")
            {

                if (stoneImpactParticles)
                {
                    GameObject impactParticle = Instantiate(stoneImpactParticles, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal)) as GameObject;
                    if (GameObject.Find("Temp")) impactParticle.transform.parent = GameObject.Find("Temp").transform;
                }

                queueDestroy = true;
            }
            else
            {
                if (stoneImpactParticles)
                {
                    GameObject impactParticle = Instantiate(stoneImpactParticles, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal)) as GameObject;
                    if (GameObject.Find("Temp")) impactParticle.transform.parent = GameObject.Find("Temp").transform;
                }
                
                queueDestroy = true;
            }
            if (queueDestroy) DestroyProjectile();
        }
        Vector3 oldPosition = this.transform.position;
        velocity += new Vector3(0, -gravity, 0);
        this.transform.position += velocity;
        distanceTravelled += Vector3.Distance(oldPosition, transform.position);
    }

    public void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
