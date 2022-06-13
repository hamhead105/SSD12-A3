using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Player touching this object will apply a velocity to them. This velocity is applied in a direction
 */
public class BouncePad : MonoBehaviour
{
    [SerializeField] float bounceVelocity;
    [SerializeField] private Transform bounceDirection;

    /*
     * Return a vector for the bounce in regards to the forward direction of an object transform and
     * a value for how strong the bounce is
     * 
     * @return - a vector for which the character will set their velocity on touching
     */
    public Vector3 GetBounceVector()
    {
        return bounceDirection.forward * bounceVelocity;
    }
}
