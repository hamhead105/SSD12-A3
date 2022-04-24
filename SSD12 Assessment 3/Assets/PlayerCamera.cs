using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private static float fixedUpdateCallRate = 0.02f;
    // FOV control
    [SerializeField] private float fov;
    private float targFov = 80;
    private float baseFov = 80;

    private float fov_kP = 0.3f;
    private Camera playersCamera;

    // shake control
    [SerializeField]
    private Vector3 
        cameraShake,
        cameraShakeVelocity,
        targCameraShakeVelocity;

    [SerializeField] private float cameraShake_kP;
    [SerializeField] private float cameraShakeVelocity_kP;

    // sway control
    private Vector3 cameraSway;
    private float cameraSwayMagnitudeTarget;
    private float cameraSwayMagnitude;
    private float cameraSwaySpeed;
    private static float cameraSwayTransition_kP = 0.2f;

    private float swayTime;
    private float lean;

    // accessors and mutators
    public float TargFOV { get => targFov; set => targFov = value; }
    public float BaseFov { get => baseFov; set => baseFov = value; }

    // Start is called before the first frame update
    void Start()
    {
        playersCamera = GetComponent<Camera>();
        baseFov = PlayerPrefs.GetFloat("FOV", 90);
        fov = baseFov;
        playersCamera.fieldOfView = fov;     
    }

    private void UpdateAdditionalTransforms()
    {
        this.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0) + cameraShake;    
    }

    private void FixedUpdate()
    {
        UpdateFOV();
        UpdateCameraShake();
        UpdateCameraSway();
    }

    private void LateUpdate()
    {
        UpdateAdditionalTransforms();
    }

    private void UpdateFOV()
    {
        fov += (targFov - fov) * fov_kP;
        playersCamera.fieldOfView = fov;
    }

    public void SetTargFOV(float targFov, bool active)
    {
        if (active) this.targFov = targFov;
        else this.targFov = baseFov;
    } 

    public void UpdateCameraShake()
    {
        targCameraShakeVelocity = new Vector3(
            (0 - cameraShake.x) * cameraShake_kP,
            (0 - cameraShake.y) * cameraShake_kP,
            (0 - cameraShake.z) * cameraShake_kP
        );

        cameraShakeVelocity += new Vector3(
            (targCameraShakeVelocity.x - cameraShakeVelocity.x) * cameraShakeVelocity_kP,
            (targCameraShakeVelocity.y - cameraShakeVelocity.y) * cameraShakeVelocity_kP,
            (targCameraShakeVelocity.z - cameraShakeVelocity.z) * cameraShakeVelocity_kP
        );

        cameraShake += cameraShakeVelocity;
    }

    public void UpdateCameraSway()
    {
        swayTime += fixedUpdateCallRate * cameraSwaySpeed;
        if (swayTime > Mathf.Deg2Rad * 360) swayTime = Mathf.Deg2Rad * (swayTime - 360);
        cameraSwayMagnitude += (cameraSwayMagnitudeTarget - cameraSwayMagnitude) * cameraSwayTransition_kP;

        cameraSway = new Vector3(
            cameraSwayMagnitude * Mathf.Sin(swayTime * 2),
            cameraSwayMagnitude * Mathf.Sin(swayTime),
            0
        );
    }

    public void SetCameraSwayValues(float speed, float magnitude)
    {
        cameraSwaySpeed = speed;
        cameraSwayMagnitudeTarget = magnitude;
    }

    public void AddCameraShakeVelocity(Vector3 addedCameraShakeVelocity)
    {
        print("add camera shake");
        cameraShakeVelocity += addedCameraShakeVelocity;
    }

    public void SetLean(float lean)
    {
        this.lean = lean;
    }
}
