using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private GameObject playerCameraObject;
    private static float fixedUpdateCallRate = 0.02f;
    private bool firstPersonMode;

    // Pos control
    private Vector3 basePosition = new Vector3(0, 0.7f, -4);
    private float minDistancePoint = 0.8f;
    private Vector3 fpsPosition = new Vector3(0, 0.01f , 0.025f);
    private Vector3 targPosition;
    private float pos_kp = 0.3f;
    private float bufferDistance = 0.2f;

    // FOV control
    [SerializeField] private float fov;
    private float targFov = 80;
    private float baseFov = 80;

    private float fov_kP = 0.3f;
    [SerializeField] private Camera playerCamera;

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

    // control point
    private bool freeLook;
    private float controlRot_kP = 0.1f;
    private Vector3 controlRot;
    private float minView = -60;
    private float maxView = 60;

    // accessors and mutators
    public float TargFOV { get => targFov; set => targFov = value; }
    public float BaseFov { get => baseFov; set => baseFov = value; }
    public Vector3 ControlRot { get => controlRot; set => controlRot = value; }
    public bool FreeLook { get => freeLook; set => freeLook = value; }

    // Start is called before the first frame update
    void Start()
    {
        playerCameraObject = transform.GetChild(0).gameObject;
        playerCamera = playerCameraObject.GetComponent<Camera>();
        baseFov = PlayerPrefs.GetFloat("FOV", 90);
        fov = baseFov;
        playerCamera.fieldOfView = fov;     
    }

    private void UpdateAdditionalTransforms()
    {
        UpdateCameraControl();

        playerCameraObject.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0) + cameraShake;

        if (!firstPersonMode)
        {
            RaycastHit hit;
            if (Physics.Linecast(this.transform.position, transform.TransformPoint(basePosition), out hit, 7))
            {
                if (hit.distance < basePosition.magnitude)
                {
                    print("hit " + hit.transform.gameObject);
                    if (hit.distance > minDistancePoint)
                    {
                        targPosition = basePosition * ((hit.distance - bufferDistance) / basePosition.magnitude);
                    }
                    else
                    {
                        targPosition = fpsPosition;
                    }
                }
                else
                {
                    targPosition = basePosition;
                }
            }
            else
            {
                targPosition = basePosition;
            }
        }
        else
        {
            targPosition = fpsPosition;
        }

        playerCameraObject.transform.localPosition += new Vector3(
            (targPosition.x - playerCameraObject.transform.localPosition.x) * pos_kp * Time.deltaTime * 60,
            (targPosition.y - playerCameraObject.transform.localPosition.y) * pos_kp * Time.deltaTime * 60,
            (targPosition.z - playerCameraObject.transform.localPosition.z) * pos_kp * Time.deltaTime * 60
        );
    }

    public void ToggleFirstPerson()
    {
        firstPersonMode = !firstPersonMode;
    }

    private void FixedUpdate()
    {
        UpdateFOV();
        UpdateCameraShake();
        UpdateCameraSway();
        UpdateAdditionalTransforms();
    }

    private void UpdateCameraControl() // pivot point
    {

        this.transform.localEulerAngles = new Vector3(0, 0, 0);
        this.transform.eulerAngles += ControlRot;
        //ControlRot = new Vector3(Mathf.Clamp(ControlRot.x, minView + this.transform.eulerAngles.x, maxView + this.transform.eulerAngles.x), ControlRot.y, 0);

        if (!freeLook)
        {
            ControlRot += new Vector3(
                (0 - ControlRot.x) * controlRot_kP * (Time.deltaTime * 60),
                (0 - ControlRot.y) * controlRot_kP * (Time.deltaTime * 60),
                0
            );
        }
    }

    private void Update()
    {
        UpdateAdditionalTransforms();
    }

    private void UpdateFOV()
    {
        fov += (targFov - fov) * fov_kP;
        playerCamera.fieldOfView = fov;
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
}
