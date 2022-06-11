using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * An extension of the Enemy class, for enemies that fly.
 */
public class FlyingEnemy : Enemy
{
    private CharacterController charController;
    private static float flightZero_kP = 0.2f;
    private static float playerAim_kP = 0.1f;
    private Vector3 velocity;

    [SerializeField] private Projectile projectile;
    [SerializeField] private Transform[] barrelPoints;
    [SerializeField] private float damage;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileSpread;
    [SerializeField] private float attackRange;
    [SerializeField] private float aimYOffset;

    [SerializeField] private GameObject muzzleEffect;

    [SerializeField] private float attackCooldown;
    [SerializeField] private bool readyToFire;
    
    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
        readyToFire = true;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        UpdateMovement();
    }

    // Called at a fixed rate
    public void FixedUpdate()
    {
        if (Vector3.Distance(this.transform.position, FindObjectOfType<Player>().transform.position) <= attackRange)
        {
            Attack();
        } 
        else
        {
            Idle();
        }
    }

    /*
     * Behaviour for when the enemy is in its idle state
     */
    public void Idle()
    {
        //velocity += transform.forward * speed;
    }

    /*
     * Update the movement of the enemy, i.e apply its velocity to its position
     */
    public void UpdateMovement()
    {
        velocity += (Vector3.zero - velocity) * flightZero_kP;
        charController.Move(velocity);
    }

    /*
     * Behaviour for when the enemy is attacking the player
     */
    public void Attack()
    {
        Player player = FindObjectOfType<Player>();
        Vector3 aimPoint = player.transform.position + new Vector3(0, aimYOffset, 0);
        SmoothLookAt(aimPoint);
        RaycastHit hit;
        if (Physics.Raycast(barrelPoints[0].transform.position, transform.forward, out hit))
        {
            print("hit " + hit.transform.name);
            if (hit.transform.GetComponent<Player>() || hit.transform.GetComponent<Hitbox>())
            {
                if (readyToFire) Shoot();
            }
        }
    }

    /*
     * Smoothly interpolate the angle of the enemy to a new rotation
     */
    void SmoothLookAt(Vector3 newDirection)
    {
        Player player = FindObjectOfType<Player>();
        Vector3 currentRot = this.transform.eulerAngles;
        transform.LookAt(newDirection);
        Vector3 targRot = this.transform.eulerAngles;
        transform.eulerAngles = currentRot;

        this.transform.eulerAngles = new Vector3(
            Mathf.LerpAngle(currentRot.x, targRot.x, playerAim_kP),
            Mathf.LerpAngle(currentRot.y, targRot.y, playerAim_kP),
            Mathf.LerpAngle(currentRot.z, targRot.z, playerAim_kP)
        );
        
    }


    /*
     *  Fire the designated projectile
     */
    public void Shoot()
    {
        foreach(Transform barrelPoint in barrelPoints)
        {
            if (readyToFire)
            {
                Projectile newProjectile = Instantiate(projectile, barrelPoint.position, barrelPoint.transform.rotation);
                newProjectile.transform.Rotate(Random.Range(-projectileSpread / 2, projectileSpread / 2), Random.Range(-projectileSpread / 2, projectileSpread / 2), 0);
                newProjectile.SetProjectileStats(projectileSpeed, damage, false, 30);
                newProjectile.Init();
                Invoke("Rechamber", attackCooldown);
                readyToFire = false;
            }
        }
    }


    /*
     * Reset this enemy's ability to attack
     */
    public void Rechamber()
    {
        readyToFire = true;
    }
}
