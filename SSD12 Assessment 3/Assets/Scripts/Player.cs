using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * Class for player, the character that the player controls.
 */
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

    // Start is called before the first frame update
    public override void Start()
    {
        Init();
    }

    /*
     * Initialize the player
     */
    public override void Init()
    {
        deathFlag = false;
        LookPoint = PlayerCamera.LookPoint.transform;
        InputEnabled = true;
        playerSensitivity = PlayerPrefs.GetFloat("Sensitivity", 0.5f);
        base.Init();
        LoadItem("Railgun", true);
        if (Item is PlayerFirearm) (Item as PlayerFirearm).CompleteReload();
        UpdateMenu();
    }

    /*
     * Reset the player, i.e respawn them
     */
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

    /*
     * Update called after all other processes, which is useful for overriding animations
     */
    public void LateUpdate()
    {
        if (!Active) return;
        UpdateAnimations();
        UpdateFeetIK();
        UpdateCharacterControllerDimensions();
    }

    /*
     * Called at a fixed rate
     */
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

    /*
     * Update the physics and movement of the player,
     * feed this movement to the item that this character is holding
     */
    public override void UpdateMovement()
    {
        base.UpdateMovement();
        Item.UpdateItemShift(RightInput, Velocity.y + FloorGravity);
    }

    /*
     * Register key inputs for the player, such as to move and jump
     */
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

    /*
     * Register inputs from the player which should be able to be called even if the player is dead, paused, inactive etc
     */
    public void PlayerMiscInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuOpened = !MenuOpened;
            UpdateMenu();
        }
    }

    /*
     * Called when player is killed, spawn ragdoll and hide player
     */
    public override void Death()
    {
        if (deathFlag) return;
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
        Invoke("InitGameLoss", 5f);
        deathFlag = true;
    }

    /*
     * Input mouse movement for the player to look around
     */
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

    /*
     * Add screen recoil values to the amount the player's point of aim should move
     */
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

    /*
     * Update whether the settings and menu should be opened or not
     */
    public void UpdateMenu()
    {
        if (MenuOpened)
        {
            InputEnabled = false;
            FindObjectOfType<UserInterface>().OpenSettings();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ResetInputs();
        }
        else
        {
            InputEnabled = true;
            FindObjectOfType<UserInterface>().CloseSettings();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    /*
     * Disable all the hitboxes of this character
     */
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

    /*
     * Enable all the hitboxes of this character
     */
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

    /*
     * Update camera states, sensitivity etc
     */
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

    /*
     * Update weapon sway. This is how the item the player is holding, is not held perfectly still
     */
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

    /*
     * Used when the player wants to interact with a world element
     * 
     * @param isInteract - whether the player has pressed the key to interact
     */
    private void CastInteractionRay(bool isInteract)
    {
        RaycastHit hit;
        if (Physics.Raycast(neck.transform.position, neck.transform.forward, out hit))
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

    /*
     * Check whether the player's item has queued a reload. This is used to run animations
     */
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

    /*
     * Update the active mouse input sensitivity the player has set
     * 
     * @param sensitivity - the value of sensitivity, higher sensitivity means more angular movement with less mouse movement.
     */
    public void UpdateSensitivity(float sensitivity)
    {
        PlayerSensitivity = sensitivity;
    }

    /*
     * Update the game volume
     * 
     * @param volume - how loud the game should be
     */
    public void UpdateVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    /*
     * Cast a ray forward from the item perspective, to see if the player's item is obstructed
     */
    private void CastForwardSpaceRay()
    {
        RaycastHit hit;
        if (Physics.Raycast(spine1.transform.position, spine1.transform.forward, out hit, 20, 7))
        {
            if (Item is PlayerFirearm) (Item as PlayerFirearm).SetWeaponDraw(hit.distance);
        }
        else
        {
            if (Item is PlayerFirearm) (Item as PlayerFirearm).SetWeaponDraw(20);
        }
    }

    /*
     * Called when the character controller collides with other colliders
     * 
     * @param other - the collider of that is intersecting
     */
    public void OnControllerColliderHit(ControllerColliderHit other)
    {
        if (other.transform.tag == "Teleporter") 
        {
            InitGameWin();
        }
        else if (other.transform.tag == "PointCube")
        {
            other.transform.GetComponent<Collectable>().Collect();
        }
    }

    /*
     * Called when the game is to be ended since the player has completed the level
     */
    public void InitGameWin()
    {
        InputEnabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        FindObjectOfType<GameManagement>().EndGame(true);
    }

    /*
     * Called when the game is to be ended since the player has completed the level
     */
    public void InitGameLoss()
    {
        InputEnabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        FindObjectOfType<GameManagement>().EndGame(false);
    }
}
