using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : Human
{
    // non serialized constants
    private static float
        swaySpeed_idle = 1,
        swaySpeed_walking = 6,
        swaySpeed_sprinting = 7,
        swaySpeed_focused = 0.2f,

        swayMagnitude_idle = 0.15f,
        swayMagnitude_walking = 0.3f,
        swayMagnitude_sprinting = 0.6f,
        swayMagnitude_focused = 0f,

        itemSwayMagnitude_idle = 2f,
        itemSwayMagnitude_walking = 2f,
        itemSwayMagnitude_sprinting = 5f,
        itemSwayMagnitude_focused = 0.04f;

    private float interactionDistance = 2f;

    [SerializeField] private float damageCameraShakeMultiplier;

    // control variables
    private bool menuOpened;
    private bool inputEnabled;
    [SerializeField] private float baseSensitivity;
    private float playerSensitivity;
    private float sensitivity;

    [SerializeField] private PlayerCamera playerCamera;
    private float aimDownSightSwingReduction = 3.5f;

    private bool focused;
    private bool toggleADS;
    private bool toggleLean;

    [SerializeField] private bool playerAuthority;

    [SerializeField] private GameObject spectatorModeCamera;

    private Transform lookPoint;

    public bool InputEnabled { get => inputEnabled; set => inputEnabled = value; }
    public bool MenuOpened { get => menuOpened; set => menuOpened = value; }
    public float PlayerSensitivity { get => playerSensitivity; set => playerSensitivity = value; }
    public Transform LookPoint { get => lookPoint; set => lookPoint = value; }
    public PlayerCamera PlayerCamera { get => playerCamera; set => playerCamera = value; }

    private void OnEnable()
    {
        //SceneManager.sceneLoaded += OnSceneLoaded;
        Active = false;
    }

    // Start is called before the first frame update
    public override void Start()
    {
        Init();
        DontDestroyOnLoad(gameObject);
    }

    public override void Init()
    {
        LookPoint = PlayerCamera.LookPoint.transform;
        InputEnabled = true;
        playerSensitivity = PlayerPrefs.GetFloat("Sensitivity", 0.5f);
        base.Init();
        LoadItem("Railgun", true);
        if (Item is PlayerFirearm) (Item as PlayerFirearm).CompleteReload();
    }

    public void Reset()
    {
        foreach (GameObject mesh in Meshes) mesh.SetActive(true);
        GetComponent<CharacterController>().detectCollisions = true;
        Init();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (!Active) return;
        if (Health > 0)
        {
            UpdateAnimatorValues();
            CheckFootsteps();
            CheckReloadAnimations();
            if (InputEnabled) RegisterInputs();
            if (InputEnabled) PlayerMouseAxisInputs();
           
        }
        UpdateCamera();
        PlayerMiscInputs();
    }

    public void LateUpdate()
    {
        if (!Active) return;
        UpdateAnimations();
        UpdateFeetIK();
        UpdateCharacterControllerDimensions();
    }

    public void FixedUpdate()
    {
        if (!Active) return;
        if (Health > 0)
        {
            UpdateMovement();
            ImplementAimRecoil();
            UpdatePControlIK();
            UpdateFixedAnimationValues();
            UpdateMoveStateAnimationsValues();
            UpdateSway();
            CastForwardSpaceRay();
        }
    }

    public override void UpdateMovement()
    {
        base.UpdateMovement();
        Item.UpdateItemShift(RightInput, Velocity.y + FloorGravity);
    }

    public void RegisterInputs()
    {
        if (Input.GetKey(KeyCode.W))
        {
            ForwardInput = 1;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Sprinting = true;
                Item.Sprinting = true;
            }
            else
            {
                Sprinting = false;
                Item.Sprinting = false;
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            ForwardInput = -1;
            Sprinting = false;
            Item.Sprinting = false;
        }
        else
        {
            ForwardInput = 0;
            Sprinting = false;
            Item.Sprinting = false;
            if (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                focused = true;
            }
            else
            {
                focused = false;
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            RightInput = -1;
            ForwardInput = ForwardInput / 2;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            RightInput = 1;
            ForwardInput = ForwardInput / 2;
        }
        else
        {
            RightInput = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpQueued = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (Item is PlayerManualFirearm)
            {
                if (!(Item as PlayerFirearm).Reloading && !(Item as PlayerManualFirearm).CurrentlyRecycling && (Item as PlayerFirearm).CanReload())
                {
                    (Item as PlayerManualFirearm).CallReload();
                }
            }
            else if (Item is PlayerFirearm)
            {
                if (!(Item as PlayerFirearm).Reloading && (Item as PlayerFirearm).CanReload())
                {
                    (Item as PlayerFirearm).CallReload();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            CastInteractionRay(true);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            PlayerCamera.ToggleFirstPerson();
        }
        else
        {
            CastInteractionRay(false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && HumanInventory.CurrentlySelected != 1)
        {
            SelectFromInventory(1, false, false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && HumanInventory.CurrentlySelected != 2)
        {
            SelectFromInventory(2, false, false);
        }
        Item.SecondaryUseItem(Input.GetMouseButton(1) || Input.GetKey(KeyCode.RightShift));
        if (Item.PrimaryUseItem(Input.GetMouseButton(0))) PlayerCamera.AddCameraShakeVelocity(new Vector3(0, 0, Random.Range(-Item.UseCameraShake, Item.UseCameraShake)));
    }

    public void PlayerMiscInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuOpened = !MenuOpened;
            UpdateMenu();
        }
    }

    public override void Death()
    {
        charController.detectCollisions = false;
        DisableHitboxes();
        GameObject ragdoll = Instantiate(Ragdoll, this.transform.position, transform.rotation);
        for (int i = 0; i < ragdollParts.Length; i++)
        { 
            ragdoll.GetComponent<Ragdoll>().RagdollParts[i].transform.position = ragdollParts[i].transform.position;
            ragdoll.GetComponent<Ragdoll>().RagdollParts[i].transform.eulerAngles = ragdollParts[i].transform.eulerAngles;
        }
        foreach (GameObject mesh in meshes)
        {
            if (mesh.GetComponent<MeshRenderer>()) mesh.GetComponent<MeshRenderer>().enabled = false;
            else if (mesh.GetComponent<SkinnedMeshRenderer>()) mesh.GetComponent<SkinnedMeshRenderer>().enabled = false;
        }
        LoadItem("Unarmed", true);
    }

    public void PlayerMouseAxisInputs()
    {
        float xMovement = Input.GetAxis("Mouse X") * sensitivity;
        float yMovement = Input.GetAxis("Mouse Y") * sensitivity;

        if (!AimingDownSights) Item.AddItemSwing(-xMovement, yMovement);
        else { Item.AddItemSwing(-xMovement / aimDownSightSwingReduction, yMovement / aimDownSightSwingReduction); }

        if (!Input.GetKey(KeyCode.LeftAlt) && !Input.GetMouseButton(2))
        {
            UpdatePlayerRotations(xMovement, yMovement);
            PlayerCamera.FreeLook = false;
        }
        else
        {
            PlayerCamera.ControlRot += new Vector3(-yMovement, xMovement);
            PlayerCamera.FreeLook = true;
        }
    }

    private void ImplementAimRecoil()
    {
        float xMovement = 0;
        float yMovement = 0;

        if (Item is PlayerFirearm)
        {
            xMovement += (Item as PlayerFirearm).AimRecoilVelocity.x;
            yMovement += (Item as PlayerFirearm).AimRecoilVelocity.y;
        }
        UpdatePlayerRotations(xMovement, yMovement);
    }

    private void UpdateMenu()
    {
        if (MenuOpened)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ResetInputs();

        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // TODO find a more efficient means instead of searching through all cases
    public void DisableHitboxes()
    {
        foreach (Hitbox hitbox in FindObjectsOfType<Hitbox>())
        {
            if (hitbox.GameCharacter == this)
            {
                hitbox.GetComponent<Collider>().enabled = false;
            }
        }
    }

    public void EnableHitboxes()
    {
        foreach (Hitbox hitbox in FindObjectsOfType<Hitbox>())
        {
            if (hitbox.GameCharacter == this)
            {
                hitbox.GetComponent<Collider>().enabled = true;
            }
        }
    }

    private void UpdateCamera()
    {
        if (Item is PlayerFirearm)
        {
            if ((Item as PlayerFirearm).ActiveSight)
            {
                PlayerCamera.SetTargFOV((Item as PlayerFirearm).ActiveSight.UseFOV, AimingDownSights);
                if (AimingDownSights)
                {
                    sensitivity = baseSensitivity * (Item as PlayerFirearm).ActiveSight.SensitivityMultiplier * playerSensitivity;
                }
                else
                {
                    sensitivity = baseSensitivity * playerSensitivity;
                }
            }
        }
        else
        {
            PlayerCamera.SetTargFOV(PlayerCamera.BaseFov, AimingDownSights);
            sensitivity = baseSensitivity * playerSensitivity;
        }
    }

    private void UpdateSway()
    {
        if (Walking)
        {
            if (Sprinting)
            {
                PlayerCamera.SetCameraSwayValues(swaySpeed_sprinting, swayMagnitude_sprinting);
                Item.SetItemSwayValues(swaySpeed_sprinting, itemSwayMagnitude_sprinting);
            }
            else
            {
                PlayerCamera.SetCameraSwayValues(swaySpeed_walking, swayMagnitude_walking);
                Item.SetItemSwayValues(swaySpeed_walking, itemSwayMagnitude_walking);
            }
        }
        else
        {
            if (focused)
            {
                PlayerCamera.SetCameraSwayValues(swaySpeed_walking, swayMagnitude_focused);
                Item.SetItemSwayValues(swaySpeed_focused, itemSwayMagnitude_focused);
            }
            else
            {
                PlayerCamera.SetCameraSwayValues(swaySpeed_idle, swayMagnitude_idle);
                Item.SetItemSwayValues(swaySpeed_idle, itemSwayMagnitude_idle);
            }
        }
    }

    private void CastInteractionRay(bool isInteract)
    {
        RaycastHit hit;
        if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out hit))
        {
            if (hit.distance <= interactionDistance)
            {
                if (isInteract)
                {
                    if (hit.transform.GetComponent<PickupItem>()) LoadItemFromPickup(hit.transform.gameObject, true);
                }
                else
                {
                    // give name of item 
                }
            }

        }
    }

    public void CheckReloadAnimations()
    {
        if (Item is PlayerFirearm)
        {
            if ((Item as PlayerFirearm).UseRepetitiveReload)
            {
                if ((Item as PlayerFirearm).RepetitiveReloading)
                {
                    LeaveRepetitiveReload = false;
                }
                else
                {
                    LeaveRepetitiveReload = true;
                }
            }
            else
            {
                if ((Item as PlayerFirearm).Reloading)
                {
                    if (!(Item as PlayerFirearm).ReloadingFlag)
                    {
                        RunReloadAnimation();
                        (Item as PlayerFirearm).ReloadingFlag = true;
                    }
                }
            }
        }
    }

    public void UpdateSensitivity(float sensitivity)
    {
        PlayerSensitivity = sensitivity;
    }

    public void UpdateVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void UpdateToggleLean(bool toggleLean)
    {
        this.toggleLean = toggleLean;
    }

    private void CastForwardSpaceRay()
    {
        RaycastHit hit;
        if (Physics.Raycast(ItemAlignment.transform.position, playerCamera.transform.forward, out hit, 20, 7))
        {
            if (Item is PlayerFirearm) (Item as PlayerFirearm).SetWeaponDraw(hit.distance);
        }
        else
        {
            if (Item is PlayerFirearm) (Item as PlayerFirearm).SetWeaponDraw(20);
        }
    }
}
