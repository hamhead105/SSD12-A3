using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : GameCharacter
{
    // serialized constants
    [SerializeField]
    private float
        baseMovementSpeed,
        sprintSpeedMultiplier,
        groundFriction,
        groundMovingFriction,
        airFriction,
        gravity,
        jumpStrength,
        pivotLimit;

    // internal parts
    [SerializeField]
    private GameObject
        spine1,
        spine2,
        spine3,
        neck,
        itemAlignment,
        leftFootTargBase,
        rightFootTargBase,
        leftFootTarg,
        rightFootTarg;

    [SerializeField]
    protected GameObject[]
        ragdollParts,
        meshes;

    [SerializeField] private GameObject ragdoll;

    // hidden constants
    private float
        sprintTransition_kP = 0.1f,
        floorGravity = 0.1f,
        feetIKCastHeight = 1f,
        feetIKOffset = 0.95f,
        feetCastRadius = 0.09f,
        feetIK_kP = 0.42f,
        feetIK_max = 0.8f,
        characterControllerBaseHeight = 1.7f,
        characterControllerBaseCenterY = 1;

    private int
        hitboxLayer = 7;

    // control variables
    [SerializeField] private bool active;

    private float
        forwardInput,
        rightInput,
        movementSpeed,
        spine2Multiplier,
        neckMultiplier;

    [SerializeField] private float aimY;
    [SerializeField] private bool sprinting;

    private float pivot;
    [SerializeField]  private float syncedPivot;

    private bool
        walking,
        jumpQueued,
        aimingDownSights,
        callDeathFlag,
        crouch;

    private Animator animator;
    protected CharacterController charController;

    [SerializeField] private GameObject weaponParent;
    [SerializeField] private PlayerItem item;
    [SerializeField]  protected string itemString;

    [SerializeField] private Vector3 velocity;
    [SerializeField] protected string oldItem = "";

    protected int leanState; // left = 1, right = 2
    [SerializeField] private GameObject leanLimb;
    [SerializeField] private float lean;
    private float lean_kP = 0.2f;

    private int moveAnimationState;
    private int armAnimationState;
    private bool leaveRepetitiveReload;

    [SerializeField]  private string humanName;
    [SerializeField]  private int team;
    private Inventory humanInventory = new Inventory();
    protected Player localPlayer;

    [SerializeField] protected GameObject headwearParent;
    [SerializeField] protected GameObject equipmentParent;

    [Header("Footsteps")]
    [SerializeField] private GameObject[] FootstepGrassSprintSounds;
    [SerializeField] private GameObject[] FootstepGrassSounds;
    private int lastStepSound;
    private bool stepSound;
    private float stepCoolTime = 0.5f;
    [SerializeField] private float rightFootCastDistance;
    [SerializeField] private float leftFootCastDistance;
    [SerializeField] private float pControlledRightFootPos = 0;
    [SerializeField] private float pControlledLeftFootPos = 0;

    public float AimY { get => aimY; set => aimY = value; }
    public GameObject ItemAlignment { get => itemAlignment; set => itemAlignment = value; }
    public bool AimingDownSights { get => aimingDownSights; set => aimingDownSights = value; }
    public PlayerItem Item { get => item; set => item = value; }
    public GameObject[] Meshes { get => meshes; set => meshes = value; }
    public GameObject Ragdoll { get => ragdoll; set => ragdoll = value; }
    public bool Sprinting { get => sprinting; set => sprinting = value; }
    public float ForwardInput { get => forwardInput; set => forwardInput = value; }
    public float RightInput { get => rightInput; set => rightInput = value; }
    public bool JumpQueued { get => jumpQueued; set => jumpQueued = value; }
    public Animator Animator { get => animator; set => animator = value; }
    public bool Crouch { get => crouch; set => crouch = value; }
    public bool Walking { get => walking; set => walking = value; }
    public Vector3 Velocity { get => velocity; set => velocity = value; }
    public float Gravity { get => gravity; set => gravity = value; }
    public float FloorGravity { get => floorGravity; set => floorGravity = value; }
    public float Lean { get => lean; set => lean = value; }
    public bool LeaveRepetitiveReload { get => leaveRepetitiveReload; set => leaveRepetitiveReload = value; }
    public int Team { get => team; set => team = value; }
    public Inventory HumanInventory { get => humanInventory; set => humanInventory = value; }
    public bool Active { get => active; set => active = value; }
    public string HumanName { get => humanName; set => humanName = value; }
    public float Pivot { get => pivot; set => pivot = value; }

    // Start is called before the first frame update
    public virtual void Start()
    {

    }

    public override void Init()
    {
        Active = true;
        Animator = GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
    }

    public virtual void Update()
    {
       // UpdatePivotValues();
    }

    public void UpdatePlayerRotations(float x, float y)
    {
        pivot += x;
        //spine1.transform.localEulerAngles = new Vector3(spine1.transform.localEulerAngles.x, pivot, spine1.transform.localEulerAngles.z);
        AimY += y;
        AimY = Mathf.Clamp(AimY, -60, 70);
    }

    public virtual void UpdateMovement()
    {
        if (Sprinting) movementSpeed = baseMovementSpeed * sprintSpeedMultiplier;
        else movementSpeed = baseMovementSpeed;

        if (charController.isGrounded)
        {
            Velocity += (transform.forward * ForwardInput * movementSpeed) + (transform.right * RightInput * movementSpeed);

            if (Walking || Sprinting) Velocity = new Vector3(Velocity.x / groundMovingFriction, -FloorGravity, Velocity.z / groundMovingFriction);
            else Velocity = new Vector3(Velocity.x / groundFriction, -FloorGravity, Velocity.z / groundFriction);

            if (forwardInput == 0 && rightInput == 0) Walking = false;
            else Walking = true;

            if (JumpQueued)
            {
                Jump(jumpStrength);
            }
        }
        else
        {
            Velocity = new Vector3(Velocity.x / airFriction, Velocity.y - Gravity, Velocity.z / airFriction);
        }
        JumpQueued = false;
        charController.Move(Velocity);
    }

    public void ResetInputs()
    {
        ForwardInput = 0;
        RightInput = 0;
        Sprinting = false;
        AimingDownSights = false;
    }

    protected void Jump(float strength)
    {
        if (charController.isGrounded)
        {
            Velocity += new Vector3(0, strength, 0);
        }
    }

    public void UpdateFixedAnimationValues()
    {
        if (!Sprinting)
        {
            spine2Multiplier += (1 - spine2Multiplier) * sprintTransition_kP;
            neckMultiplier += (0 - neckMultiplier) * sprintTransition_kP;
        }
        else
        {
            spine2Multiplier += (0 - spine2Multiplier) * sprintTransition_kP;
            neckMultiplier += (1 - neckMultiplier) * sprintTransition_kP;
        }
    }

    protected void UpdateAnimations()
    {
        leftFootTarg.transform.position = new Vector3(leftFootTargBase.transform.position.x, leftFootTargBase.transform.position.y - pControlledLeftFootPos, leftFootTargBase.transform.position.z);
        rightFootTarg.transform.position = new Vector3(rightFootTargBase.transform.position.x, rightFootTargBase.transform.position.y - pControlledRightFootPos, rightFootTargBase.transform.position.z);
     
        spine2.transform.Rotate(this.transform.right * (-AimY / 2) * spine2Multiplier, Space.World);
        spine3.transform.Rotate(this.transform.right * (-AimY / 2), Space.World);
        neck.transform.Rotate(this.transform.right * (-AimY / 2) * neckMultiplier, Space.World);

        if (Item is PlayerFirearm) UpdateAimDownSightsState((Item as PlayerFirearm).AimedDownSights);
        UpdatePivot();
        UpdateAnimatorValues();
    }

    protected void RunReloadAnimation()
    {
        print("reload animation");
        Animator.SetTrigger("Reload");
    }

    public void UpdatePivot()
    {
        float pivotTranslate = (0 - pivot) * 0.1f;
        this.transform.Rotate(0, -pivotTranslate, 0);
        pivot += pivotTranslate;
        spine1.transform.localEulerAngles += new Vector3(spine1.transform.localEulerAngles.x, pivot, spine1.transform.localEulerAngles.z);
    }

    public void UpdateAnimatorValues()
    {
        Animator.SetInteger("MoveState", moveAnimationState);
        Animator.SetInteger("ArmState", armAnimationState);
    }

    protected void UpdateAimDownSightsState(bool aimedDownSights)
    {
        if (aimedDownSights)
        {
            armAnimationState = 1;
            AimingDownSights = true;
        }
        else
        {
            armAnimationState = 0;
            AimingDownSights = false;
        }
    }

    public void UpdateFeetIK()
    {
        if (charController.isGrounded)
        {
            RaycastHit rightFootHit;
            Vector3 rightFootTransformCastPosition = new Vector3(rightFootTarg.transform.position.x, this.transform.position.y + feetIKCastHeight, rightFootTarg.transform.position.z);
            Debug.DrawRay(rightFootTransformCastPosition, Vector3.down * 1.5f, Color.red, 0.01f);
            if (Physics.SphereCast(rightFootTransformCastPosition, feetCastRadius, Vector3.down, out rightFootHit, 1.5f, hitboxLayer))
            {
                rightFootCastDistance = Mathf.Clamp(rightFootHit.distance - feetIKOffset + feetCastRadius, -feetIK_max, feetIK_max);
            }

            RaycastHit leftFootHit;
            Vector3 leftFootTransformCastPosition = new Vector3(leftFootTarg.transform.position.x, this.transform.position.y + feetIKCastHeight, leftFootTarg.transform.position.z);
            Debug.DrawRay(leftFootTransformCastPosition, Vector3.down * 1.5f, Color.red, 0.01f);
            if (Physics.SphereCast(leftFootTransformCastPosition, feetCastRadius, Vector3.down, out leftFootHit, 1.5f, hitboxLayer))
            {
                leftFootCastDistance = Mathf.Clamp(leftFootHit.distance - feetIKOffset + feetCastRadius, -feetIK_max, feetIK_max);
            }
        }
    }

    public override void Death()
    {
        GameObject ragdoll = Instantiate(Ragdoll, this.transform.position, transform.rotation);
        int i = 0;
        foreach (GameObject ragdollPart in ragdollParts)
        {
            ragdoll.GetComponent<Ragdoll>().RagdollParts[i] = ragdollPart;
        }
        Destroy(gameObject);
    }

    public void UpdatePControlIK()
    {
        pControlledLeftFootPos += (leftFootCastDistance - pControlledLeftFootPos) * feetIK_kP;
        pControlledRightFootPos += (rightFootCastDistance - pControlledRightFootPos) * feetIK_kP;
    }

    public void UpdateCharacterControllerDimensions()
    {
        float castDistanceDiff = Mathf.Abs(pControlledRightFootPos - pControlledLeftFootPos);
        if (Mathf.Abs(leftFootCastDistance) < feetIKCastHeight && Mathf.Abs(rightFootCastDistance) < feetIKCastHeight)
        {
            charController.height = characterControllerBaseHeight - castDistanceDiff;
        }
        else
        {
            charController.height = characterControllerBaseHeight;
        }
        
        charController.center = new Vector3(0, characterControllerBaseCenterY + castDistanceDiff / 4);
    } 

    private void UpdateLeaning(float leanState)
    {
        float targetLean = 0;
        if (leanState == 1) targetLean = 18;
        else if (leanState == 2) targetLean = -18;

        Lean += (targetLean - Lean) * lean_kP * (Time.deltaTime * 60f);
        leanLimb.transform.localEulerAngles = new Vector3(leanLimb.transform.localEulerAngles.x, leanLimb.transform.localEulerAngles.y, Lean + leanLimb.transform.localEulerAngles.z);
    }

    protected void UpdateMoveStateAnimationsValues()
    {
        if (ForwardInput != 0)
        {
            if (Sprinting)
            {
                moveAnimationState = 4;
                crouch = false;
            }
            else
            {
                if (!crouch) moveAnimationState = 1;
                else moveAnimationState = 6;
            }
        }
        else if (RightInput != 0)
        {
            if (!crouch)
            {
                if (RightInput > 0) moveAnimationState = 2; // direction - right
                else moveAnimationState = 3; // direction - left
            }
            else
            {
                if (RightInput > 0) moveAnimationState = 7; // direction - right
                else moveAnimationState = 8; // direction - left
            }
        }
        else
        {
            if (!crouch) moveAnimationState = 0;
            else moveAnimationState = 5;
        }
    }

    protected void LoadItem(GameObject item, bool localPlayer)
    {
        if (item.GetComponent<Item>().ItemName != oldItem)
        {
            print("equip run gameObject " + item.name);
            if (weaponParent.transform.GetChild(1)) Destroy(weaponParent.transform.GetChild(1).gameObject);
            GameObject newItem = Instantiate(item, transform.position, transform.rotation);
            newItem.transform.parent = weaponParent.transform;

            newItem.transform.localPosition = new Vector3(0, 0, 0);
            newItem.transform.localEulerAngles = new Vector3(0, 0, 0);
            newItem.transform.localScale = newItem.transform.lossyScale;
            this.item = newItem.GetComponent<PlayerItem>();
            newItem.GetComponent<PlayerItem>().User = this;

            Animator.runtimeAnimatorController = Item.GetComponent<PlayerItem>().AnimatorController;
            Animator.avatar = Item.GetComponent<PlayerItem>().Avatar;

            if (localPlayer && newItem.GetComponent<PlayerFirearm>())
            {
                newItem.GetComponent<PlayerFirearm>().SetAsLocalPlayer();
            }
        }
        else
        {
            // update item stats
        }
        oldItem = item.GetComponent<Item>().ItemName;
    }

    protected void LoadItemFromPickup(GameObject item, bool localPlayer)
    {
        if (item.GetComponent<Item>().ItemName != oldItem)
        {
            GameObject newItem = Instantiate(Resources.Load("Weapon_" + item.GetComponent<PickupItem>().ItemName) as GameObject, transform.position, transform.rotation);
            newItem.transform.localPosition = new Vector3(0, 0, 0);
            newItem.transform.localEulerAngles = new Vector3(0, 0, 0);
            newItem.transform.localScale = newItem.transform.lossyScale;
            newItem.GetComponent<PlayerItem>().User = this;
            if (newItem.GetComponent<PlayerItem>() is PlayerFirearm)
            {
                newItem.GetComponent<PlayerFirearm>().SetSightAttachment(item.GetComponent<PickupItem>().SightAttachment);
            }

            if (localPlayer && newItem.GetComponent<PlayerFirearm>())
            {
                newItem.GetComponent<PlayerFirearm>().SetAsLocalPlayer();
                newItem.GetComponent<PlayerFirearm>().Chambered = true;
            }

            HumanInventory.AssignNewItem(newItem.GetComponent<PlayerItem>(), true);
            LoadItem(newItem.GetComponent<PlayerItem>(), true, true);
            Destroy(newItem);
        }
        else
        {
            if (this.item is PlayerFirearm)
            {
                (this.item as PlayerFirearm).SetSightAttachment(item.GetComponent<PickupItem>().SightAttachment);
                (this.item as PlayerFirearm).SetActiveSight();
                (this.item as PlayerFirearm).SetAsLocalPlayer();
            }
            HumanInventory.AssignNewItem(this.item.GetComponent<PlayerItem>(), true);
            LoadItem(this.item.GetComponent<PlayerItem>(), true, true);
        }
        oldItem = item.GetComponent<Item>().ItemName;
    }

    protected void LoadItem(string item, bool localPlayer)
    {
        print("player: " + this.transform.name + "item: " + item);
        if (item != oldItem && Animator != null)
        {
            Destroy(Item.gameObject); // destroy old item
            print("equip run string " + item);
            if (item == "" || item == null)
            {
                print("no item given");
                item = "Unarmed";
            }

            GameObject newItem = Instantiate(Resources.Load("Weapon_" + item) as GameObject, transform.position, transform.rotation);
            newItem.transform.parent = weaponParent.transform;

            newItem.transform.localPosition = new Vector3(0, 0, 0);
            newItem.transform.localEulerAngles = new Vector3(0, 0, 0);
            newItem.transform.localScale = newItem.transform.lossyScale;
            this.Item = newItem.GetComponent<PlayerItem>();
            print("assign user " + this.transform.name);
            newItem.GetComponent<PlayerItem>().User = this;

            Animator.runtimeAnimatorController = Item.GetComponent<PlayerItem>().AnimatorController;
            Animator.avatar = Item.GetComponent<PlayerItem>().Avatar;

            if (localPlayer && newItem.GetComponent<PlayerFirearm>())
            {
                newItem.GetComponent<PlayerFirearm>().SetAsLocalPlayer();
            }
        }
        else
        {
            // update item stats
            if (Animator == null) print("no animator");
            if (item == oldItem) print("item = olditem");
        }
        oldItem = item;
    }

    public void LoadItem(PlayerItem item, bool localPlayer, bool init)
    {
        if (item.ItemName != oldItem && Animator != null)
        {
            Destroy(Item.gameObject); // destroy old item
            GameObject newItem = Instantiate(Resources.Load("Weapon_" + item.ItemName) as GameObject, transform.position, transform.rotation);
            newItem.transform.parent = weaponParent.transform;

            newItem.transform.localPosition = new Vector3(0, 0, 0);
            newItem.transform.localEulerAngles = new Vector3(0, 0, 0);
            if (localPlayer && newItem.GetComponent<PlayerFirearm>())
            {
                newItem.GetComponent<PlayerFirearm>().Ammo = (item as PlayerFirearm).Ammo;
                newItem.GetComponent<PlayerFirearm>().SetSightAttachment((item as PlayerFirearm).SightAttachment);
                newItem.GetComponent<PlayerFirearm>().SetAsLocalPlayer();
            }

            newItem.transform.localScale = newItem.transform.lossyScale;
            newItem.GetComponent<PlayerItem>().User = this;
            if (newItem.GetComponent<PlayerFirearm>()) newItem.GetComponent<PlayerFirearm>().Chambered = (item as PlayerFirearm).Chambered;

            this.Item = newItem.GetComponent<PlayerItem>();
            Animator.runtimeAnimatorController = Item.GetComponent<PlayerItem>().AnimatorController;
            Animator.avatar = Item.GetComponent<PlayerItem>().Avatar;
            Animator.SetBool("Init", init);
        }
        else
        {
            // update item stats
            if (Animator == null) print("no animator");
            if (item.ItemName == oldItem) print("item = olditem");
        }
        oldItem = item.ItemName;
    }

    /*
     * @param equipForNewItem avoids setting the current slot as unarmed
     */
    public void SelectFromInventory(int selection, bool equipForNewItem, bool init)
    {
        if (!equipForNewItem) humanInventory.AssignNewItem(this.item, false);

        if (selection == 1) LoadItem(HumanInventory.PrimaryItem, true, init);
        else if (selection == 2) LoadItem(HumanInventory.SecondaryItem, true, init);
        HumanInventory.CurrentlySelected = selection;
    }

    public void CheckFootsteps()
    {
        if (!active || !animator) return;
        //Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), transform.TransformDirection(Vector3.down) * 2.5f, Color.red, 0.1f);
        if (animator.GetInteger("MoveState") == 4 && stepSound)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + new Vector3(0, 1f, 0), transform.TransformDirection(Vector3.down), out hit, 1.5f))
            {
                this.stepSound = false;
                //if (hit.transform.tag == "Terrain")
                //{
                    int stepSound = Random.Range(0, FootstepGrassSprintSounds.Length - 1);
                    if (stepSound == lastStepSound)
                    {
                        stepSound += 1;
                        if (stepSound > FootstepGrassSprintSounds.Length) stepSound = 0;
                    }
                    lastStepSound = stepSound;
                    Instantiate(FootstepGrassSprintSounds[stepSound], this.transform.position, transform.rotation);
               // }
                Invoke("StepSoundCoolDown", stepCoolTime / 1.3f);
            }
        }
        else if ((animator.GetInteger("MoveState") == 1 || moveAnimationState == 2 || moveAnimationState == 3 || 
            animator.GetInteger("MoveState") == 6 || animator.GetInteger("MoveState") == 7 || animator.GetInteger("MoveState") == 8) && stepSound)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + new Vector3(0, 1f, 0), transform.TransformDirection(Vector3.down), out hit, 1.5f))
            {
                print("raycast hit " + hit.transform.name);
                this.stepSound = false;
                //if (hit.transform.tag == "Terrain")
                //{
                    int stepSound = Random.Range(0, FootstepGrassSounds.Length - 1);
                    if (stepSound == lastStepSound)
                    {
                        stepSound += 1;
                        if (stepSound > FootstepGrassSounds.Length) stepSound = 0;
                    }
                    lastStepSound = stepSound;
                    Instantiate(FootstepGrassSounds[stepSound], this.transform.position, transform.rotation);
                //}
                Invoke("StepSoundCoolDown", stepCoolTime);
            }
        }
    }

    public void StepSoundCoolDown()
    {
        stepSound = true;
    }
}
