using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Base class which handles the player's camera. This involves first person and third person functionality.
 */
public class PlayerCamera : MonoBehaviour
{
    private static int playerLayerMask = 1 << 9;
    private static int hitboxLayerMask = 1 << 7;

    private GameObject playerCameraObject;
    [SerializeField] private Transform lookPoint;

    private static float maxLookPointDistance = 200f;

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
    public Transform LookPoint { get => lookPoint; set => lookPoint = value; }
    public bool FirstPersonMode { get => firstPersonMode; set => firstPersonMode = value; }

    // Start is called before the first frame update
    void Start()
    {
        playerCameraObject = transform.GetChild(0).gameObject;
        playerCamera = playerCameraObject.GetComponent<Camera>();
        baseFov = PlayerPrefs.GetFloat("FOV", 90);
        fov = baseFov;
        playerCamera.fieldOfView = fov;     
    }

    /*
     * Update camera positioning based on target positioning
     */
    private void UpdateAdditionalTransforms()
    {
        cameraShake += cameraShakeVelocity * Time.deltaTime * GameManagement.DeltaTimeFrameSpeedConstant;

        UpdateCameraControl();

        playerCameraObject.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0) + cameraShake;

        if (!FirstPersonMode)
        {
            RaycastHit hit;
            if (Physics.Linecast(this.transform.position, transform.TransformPoint(basePosition), out hit, ~(playerLayerMask | hitboxLayerMask)))
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

    /*
     * Toggle whether the camera is in first person mode or third person mode
     */
    public void ToggleFirstPerson()
    {
        FirstPersonMode = !FirstPersonMode;
    }

    /*
     * Called at a fixed rate
     */
    private void FixedUpdate()
    {
        UpdateFOV();
        UpdateCameraSway();
        UpdateCameraShake();
        UpdateAdditionalTransforms();
    }

    /*
     * Update the position of the look point object, which is set to the position of the point which the camera is looked at.
     */
    public void UpdateLookPoint()
    {
        RaycastHit hit;
       
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 1500, ~(playerLayerMask | hitboxLayerMask)))
        {
            if (hit.distance < maxLookPointDistance) LookPoint.transform.position = hit.point;
            else LookPoint.transform.position = this.transform.position + transform.forward * maxLookPointDistance;
        } 
        else
        {
            LookPoint.transform.position = this.transform.position + transform.forward * maxLookPointDistance;
        }
    }

    /*
     * Update the position of the camera based on pivoting
     */
    private void UpdateCameraControl()
    {

        this.transform.localEulerAngles = new Vector3(0, 0, 0);
        this.transform.eulerAngles += ControlRot;

        if (!freeLook)
        {
            ControlRot += new Vector3(
                (0 - ControlRot.x) * controlRot_kP * (Time.deltaTime * 60),
                (0 - ControlRot.y) * controlRot_kP * (Time.deltaTime * 60),
                0
            );
        }
    }

    // Called every frame
    private void Update()
    {
        UpdateAdditionalTransforms();
        UpdateLookPoint();
    }

    /*
     * Update the camera's field of view. Player actions such as aiming, will decrease the FOV to create a zoomed in effect.
     * Use a proportional controller to smoothly interpolate to the target FOV
     */
    private void UpdateFOV()
    {
        fov += (targFov - fov) * fov_kP;
        playerCamera.fieldOfView = fov;
    }

    /*
     * Set a target FOV
     * 
     * @param targFOV - the new target field of view to use
     * @param active - whether to use the baseFOV.
     */
    public void SetTargFOV(float targFov, bool active)
    {
        if (active) this.targFov = targFov;
        else this.targFov = baseFov;
    } 

    /*
     * Update camera shake effects and add to the camera shake vector which is applied to the camera transforms.
     * Camera shake is used to shake the screen when the player is near an explosion, shoots the weapon etc.
     */
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
    }

    /*
     * Update additional camera instability, which makes the camera slowly sway around for ambience.
     * Added to the camera sway vector which is added to the transform.
     */
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

    /*
     * Set camera sway values and magnitude. For example, player sprinting will increase the amount of
     * sway and the speed of the sway.
     */
    public void SetCameraSwayValues(float speed, float magnitude)
    {
        cameraSwaySpeed = speed;
        cameraSwayMagnitudeTarget = magnitude;
    }

    /*
     * Add camera shake velocity. For example, used when the player shoots a weapon.
     */
    public void AddCameraShakeVelocity(Vector3 addedCameraShakeVelocity)
    {
        print("add camera shake");
        cameraShakeVelocity += addedCameraShakeVelocity;
    }
}
