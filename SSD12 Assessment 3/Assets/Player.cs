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

    private UserInterface UI;

    private bool focused;
    private bool toggleADS;
    private bool toggleLean;

    [SerializeField] private bool playerAuthority;

    [SerializeField] private GameObject spectatorModeCamera;

    public bool InputEnabled { get => inputEnabled; set => inputEnabled = value; }
    public bool MenuOpened { get => menuOpened; set => menuOpened = value; }
    public float PlayerSensitivity { get => playerSensitivity; set => playerSensitivity = value; }

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
        UpdateUI();
        if (Health > 0)
        {
            UpdateAnimatorValues();
            CheckFootsteps();
            CheckReloadAnimations();
            if (InputEnabled) RegisterInputs();
            if (InputEnabled) PlayerMouseAxisInputs();
            UpdateCamera();
        }
        PlayerMiscInputs();
    }

    public void LateUpdate()
    {
        if (!Active) return;

        UpdateFeetIK();
        UpdateCharacterControllerDimensions();
        UpdateAnimations();
        playerCamera.SetLean(Lean);
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
        Item.SecondaryUseItem(Input.GetMouseButton(1));
        if (Item.PrimaryUseItem(Input.GetMouseButton(0))) playerCamera.AddCameraShakeVelocity(new Vector3(0, 0, Random.Range(-Item.UseCameraShake, Item.UseCameraShake)));
    }

    public void PlayerMiscInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuOpened = !MenuOpened;
            UpdateMenu();
        }
    }

    public void PlayerMouseAxisInputs()
    {
        float xMovement = Input.GetAxis("Mouse X") * sensitivity;
        float yMovement = Input.GetAxis("Mouse Y") * sensitivity;

        if (!AimingDownSights) Item.AddItemSwing(-xMovement, yMovement);
        else { Item.AddItemSwing(-xMovement / aimDownSightSwingReduction, yMovement / aimDownSightSwingReduction); }
        UpdatePlayerRotations(xMovement, yMovement);
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
            if (hitbox.Player == this)
            {
                hitbox.GetComponent<Collider>().enabled = false;
            }
        }
    }

    public void EnableHitboxes()
    {
        foreach (Hitbox hitbox in FindObjectsOfType<Hitbox>())
        {
            if (hitbox.Player == this)
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
                playerCamera.SetTargFOV((Item as PlayerFirearm).ActiveSight.UseFOV, AimingDownSights);
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
            playerCamera.SetTargFOV(playerCamera.BaseFov, AimingDownSights);
            sensitivity = baseSensitivity * playerSensitivity;
        }
    }

    private void UpdateSway()
    {
        if (Walking)
        {
            if (Sprinting)
            {
                playerCamera.SetCameraSwayValues(swaySpeed_sprinting, swayMagnitude_sprinting);
                Item.SetItemSwayValues(swaySpeed_sprinting, itemSwayMagnitude_sprinting);
            }
            else
            {
                playerCamera.SetCameraSwayValues(swaySpeed_walking, swayMagnitude_walking);
                Item.SetItemSwayValues(swaySpeed_walking, itemSwayMagnitude_walking);
            }
        }
        else
        {
            if (focused)
            {
                playerCamera.SetCameraSwayValues(swaySpeed_walking, swayMagnitude_focused);
                Item.SetItemSwayValues(swaySpeed_focused, itemSwayMagnitude_focused);
            }
            else
            {
                playerCamera.SetCameraSwayValues(swaySpeed_idle, swayMagnitude_idle);
                Item.SetItemSwayValues(swaySpeed_idle, itemSwayMagnitude_idle);
            }
        }
    }

    private void CastInteractionRay(bool isInteract)
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit))
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

    private void UpdateUI()
    {
        if (!UI) return;

        UI.UpdateHealthDisplay(Health);
        if (Item is PlayerFirearm)
        {
            UI.SetAmmoTexts((Item as PlayerFirearm).Ammo, (Item as PlayerFirearm).MaxAmmo);
        }
        else
        {
            UI.SetAmmoTexts(0, 0);
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

}
