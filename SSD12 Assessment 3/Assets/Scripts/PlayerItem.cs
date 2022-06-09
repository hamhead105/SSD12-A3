using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Base class for items used by the player 
 */
public abstract class PlayerItem : Item
{
    // constants
    [SerializeField] private int useCooldown;
    [SerializeField] private float drawTime;
    protected bool itemEnabled;

    private static float
        fixedUpdateCallRate = 0.02f,

        itemSwayTransition_kP = 0.3f,
        itemShift_kP = 0.15f,

        itemShiftAmount_x = 0.02f,
        itemShiftAmount_y = 0.2f,
        itemShiftCap = 0.2f;

    private Vector3
        rotationVector;

    // control variables
    private Vector3
        positionOffset,
        rotationOffset;

    [SerializeField]
    private Transform
        baseTransform,
        forwardTransform;

    [Header("Animations and Avatars")]
    [SerializeField] RuntimeAnimatorController animatorController;
    [SerializeField] Avatar avatar;

    [Header("Item Stats")]
    [SerializeField] private float itemSwingAmount;
    [SerializeField] private float itemRecovery_kP;

    private float itemSwing_x;
    private float itemSwing_y;

    [SerializeField] private float useCameraShake;
    [SerializeField] private float itemSwayMultiplier;

    private Vector3
        itemSway,
        targItemShift,
        itemShift;

    private float
        swayTime,
        itemSwaySpeed,
        itemSwayMagnitude,
        itemSwayMagnitudeTarget;

    private bool sprinting;
    [SerializeField] private Human user;

    public Vector3 RotationOffset { get => rotationOffset; set => rotationOffset = value; }
    public Vector3 RotationVector { get => rotationVector; set => rotationVector = value; }
    public Transform BaseTransform { get => baseTransform; set => baseTransform = value; }
    public float ItemSwing_x { get => itemSwing_x; set => itemSwing_x = value; }
    public float ItemSwing_y { get => itemSwing_y; set => itemSwing_y = value; }
    public float ItemSwingAmount { get => itemSwingAmount; set => itemSwingAmount = value; }
    public float ItemRecovery_kP { get => itemRecovery_kP; set => itemRecovery_kP = value; }
    public Vector3 PositionOffset { get => positionOffset; set => positionOffset = value; }
    public Transform ForwardTransform { get => forwardTransform; set => forwardTransform = value; }
    public RuntimeAnimatorController AnimatorController { get => animatorController; set => animatorController = value; }
    public Avatar Avatar { get => avatar; set => avatar = value; }
    public float UseCameraShake { get => useCameraShake; set => useCameraShake = value; }
    public Vector3 ItemSway { get => itemSway; set => itemSway = value; }
    public Vector3 TargItemShift { get => targItemShift; set => targItemShift = value; }
    public Vector3 ItemShift { get => itemShift; set => itemShift = value; }
    public static float ItemSwayTransition_kP { get => itemSwayTransition_kP; set => itemSwayTransition_kP = value; }
    public static float ItemShift_kP { get => itemShift_kP; set => itemShift_kP = value; }
    public static float ItemShiftAmount_x { get => itemShiftAmount_x; set => itemShiftAmount_x = value; }
    public static float ItemShiftAmount_y { get => itemShiftAmount_y; set => itemShiftAmount_y = value; }
    public static float ItemShiftCap { get => itemShiftCap; set => itemShiftCap = value; }
    public bool Sprinting { get => sprinting; set => sprinting = value; }
    public Human User { get => user; set => user = value; }

    // Called when the item is created
    public virtual void Start()
    {
        itemEnabled = false;
        Invoke("EnableItem", drawTime);
    }

    /*
     * Enable the item
     */
    public void EnableItem()
    {
        itemEnabled = true;
    }

    /*
     * Called at a fixed rate
     */
    public virtual void FixedUpdate()
    {
       
    }

    /*
     * Called once per frame
     */
    public virtual void Update()
    {
        UpdateItemSway();
        UpdateTransforms();
    }

    /*
     * Update the transforms - displacement, rotation of the item
     */
    public virtual void UpdateTransforms()
    {
        rotationOffset = new Vector3(ItemSwing_y + ItemSway.x, 0, ItemSwing_x + ItemSway.y);
        ItemSwing_x += (0 - ItemSwing_x) * Mathf.Min(ItemRecovery_kP * (Time.deltaTime * 50), 1);
        ItemSwing_y += (0 - ItemSwing_y) * Mathf.Min(ItemRecovery_kP * (Time.deltaTime * 50), 1);

        positionOffset = new Vector3(ItemShift.x, ItemShift.y, 0);

        this.transform.localPosition = ConvertVectorDirections(PositionOffset);
        this.transform.localEulerAngles = ConvertVectorDirections(RotationOffset);
    }

    /**
     *  rotate a given Vector to match the weapon rotation
     *  
     *  @param baseVector - the base vector which is to be rotated to be corrected for this item
     */
    public Vector3 ConvertVectorDirections(Vector3 baseVector)
    {
        return Quaternion.Euler(RotationVector) * baseVector; 
    }

    public Vector3 GetZeroedBaseTransformPosition()
    {
        if (baseTransform)
        {
            Vector3 startTransform = this.transform.position;
            this.transform.localPosition = new Vector3(0, 0, 0);
            Vector3 baseTransformPosition = baseTransform.TransformPoint(0, 0, 0);
            this.transform.position = startTransform;
            return baseTransformPosition;
        } else
        {
            Vector3 startTransform = this.transform.position;
            this.transform.localPosition = new Vector3(0, 0, 0);
            Vector3 baseTransformPosition = this.transform.TransformPoint(0, 0, 0);
            this.transform.position = startTransform;
            return baseTransformPosition;
        }
    }

    /*
     * Add to the swing movement of the item. The item swings around when the player looks around to add a sense of inertia.
     * 
     * @param x - additional xMovement
     * @param y - additional yMovement
     */
    public void AddItemSwing(float x, float y)
    {
        ItemSwing_x += x * ItemSwingAmount;
        ItemSwing_y += y * ItemSwingAmount;
    }

    /*
     * Update the amount of the item swaying. This is to simulate the user being unable to hold the item completely still
     * due to breathing etc.
     */
    public void UpdateItemSway()
    {
        swayTime += fixedUpdateCallRate * itemSwaySpeed * Time.deltaTime * 50;
        if (swayTime > Mathf.Deg2Rad * 360) swayTime = Mathf.Deg2Rad * (swayTime - 360);
        itemSwayMagnitude += (itemSwayMagnitudeTarget - itemSwayMagnitude) * Mathf.Min(ItemSwayTransition_kP * Time.deltaTime * 50, 1);

        ItemSway = new Vector3(
            itemSwayMagnitude * itemSwayMultiplier * Mathf.Sin(swayTime * 2),
            itemSwayMagnitude * itemSwayMultiplier * Mathf.Sin(swayTime),
            0
        );
    }

    /*
     * Update the item shifting around due to user inertia and movement
     * 
     * @param x - x movement component
     * @param y - y movement component
     */
    public virtual void UpdateItemShift(float x, float y)
    {
        TargItemShift = new Vector3(
            Mathf.Clamp(x * ItemShiftAmount_x, -ItemShiftCap, ItemShiftCap),
            Mathf.Clamp(y * ItemShiftAmount_y, -ItemShiftCap, ItemShiftCap),
            0
        );

        ItemShift += new Vector3(
            (TargItemShift.x - ItemShift.x) * ItemShift_kP,
            (TargItemShift.y - ItemShift.y) * ItemShift_kP,
            0
        );
    }

    /*
     * Set the amount of sway the item experiences
     * 
     * @param speed - the speed of the swaying
     * @param magnitude - how far the item will sway around
     */
    public void SetItemSwayValues(float speed, float magnitude)
    {
        itemSwaySpeed = speed;
        itemSwayMagnitudeTarget = magnitude;
    }

    /*
     * The primary use of the item
     * 
     * @param input - the input of the player
     * @return whether the item usage was successful
     */
    public abstract bool PrimaryUseItem(bool input);

    /*
     * Called on the secondary use of the item
     * 
     * @param input - whether this input is being used
     */
    public abstract void SecondaryUseItem(bool input);
}
