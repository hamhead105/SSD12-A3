using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Camera used in the main menu scene. Can rotate a small angle which responds to mouse movement
 */
public class CinematicViewCamera : MonoBehaviour
{
    [SerializeField] private Vector3 baseRotation;
    private Vector3 addedRotations;
    [SerializeField] private float sensitivity;
    [SerializeField] private float maxMagnitude;

    // Update is called once per frame
    void Update()
    {
        addedRotations += new Vector3(-Input.GetAxis("Mouse Y") * sensitivity, Input.GetAxis("Mouse X") * sensitivity, 0);
        addedRotations = Vector3.ClampMagnitude(addedRotations, maxMagnitude);
        this.transform.eulerAngles = baseRotation + addedRotations;
    }
}
