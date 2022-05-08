using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    private CharacterController charController;
    private float flightZero_kP = 0.2f;
    private Vector3 velocity;
        
    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    public new void Update()
    {
        base.Update();
        Idle();
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
}
