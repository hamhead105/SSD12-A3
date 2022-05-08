using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    private CharacterController charController;
    private float flightZero_kP = 0.2f;
    private Vector3 velocity;

    [SerializeField] private Transform[] barrelPoints;
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject muzzleEffect;

    [SerializeField] private float attackSpeed;
    private bool readyToFire;
    
    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
        readyToFire = true;
    }

    // Update is called once per frame
    public new void Update()
    {
        base.Update();
        //Idle();
        Attack();
        UpdateMovement();
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
        transform.LookAt(FindObjectOfType<Player>().transform.position);
        if (readyToFire) Shoot();
    }

    public void Shoot()
    {
        foreach(Transform barrelPoint in barrelPoints)
        {
            Instantiate(projectile, barrelPoint.position, transform.rotation);
        }
    }
}
