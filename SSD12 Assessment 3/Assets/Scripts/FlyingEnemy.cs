using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void FixedUpdate()
    {
        Attack();
    }

    public void Idle()
    {
        velocity += transform.forward * speed;
    }

    public void UpdateMovement()
    {
        velocity += (Vector3.zero - velocity) * flightZero_kP;
        charController.Move(velocity);
    }

    public void Attack()
    {
        Player player = FindObjectOfType<Player>();
        if (readyToFire) Shoot();
        SmoothLookAt(player.transform.position);
    }

    void SmoothLookAt(Vector3 newDirection)
    {
        Player player = FindObjectOfType<Player>();
        Vector3 currentRot = this.transform.eulerAngles;
        transform.LookAt(player.transform.position);
        Vector3 targRot = this.transform.eulerAngles;
        transform.eulerAngles = currentRot;

        this.transform.eulerAngles = new Vector3(
            Mathf.LerpAngle(currentRot.x, targRot.x, playerAim_kP),
            Mathf.LerpAngle(currentRot.y, targRot.y, playerAim_kP),
            Mathf.LerpAngle(currentRot.z, targRot.z, playerAim_kP)
        );
        
    }

    public void Shoot()
    {
        foreach(Transform barrelPoint in barrelPoints)
        {
            if (readyToFire)
            {
                Projectile newProjectile = Instantiate(projectile, barrelPoint.position, barrelPoint.transform.rotation);
                newProjectile.SetProjectileStats(projectileSpeed, damage, false, 30);
                newProjectile.Init();
                Invoke("Rechamber", attackCooldown);
                readyToFire = false;
            }
        }
    }

    public void Rechamber()
    {
        readyToFire = true;
    }
}
