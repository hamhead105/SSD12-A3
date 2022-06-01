using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Controls the player's hand IK position vectors. These are what the IK algorithms will try "reach"
 * This class is important for tracking the item that the player is holding.
 */
public class HandIKTarget : MonoBehaviour
{
    //constants
    [SerializeField] private Vector3 startPosition = new Vector3(0, -0.031f, 0);
    [SerializeField] private Human player;

    // control variable
    [SerializeField] private Transform startRelateVector;
    [SerializeField] private Transform playerPivotPoint;

    // Called once per frame
    void Update()
    {
        UpdateTransforms();
    }

    /*
     * Update the transforms of this object. Ensure correct pivoting around the player as the item rotates and moves
     */
    public void UpdateTransforms()
    {
        this.transform.localPosition = startPosition;
        this.transform.localEulerAngles = new Vector3(0, 0, 0);

        if (player.Item)
        {
            PlayerItem item = player.Item;
            this.transform.RotateAround(player.ItemAlignment.transform.position, player.ItemAlignment.transform.right, item.transform.localEulerAngles.x);
            this.transform.RotateAround(player.ItemAlignment.transform.position, player.ItemAlignment.transform.forward, item.transform.localEulerAngles.z);
            this.transform.RotateAround(player.ItemAlignment.transform.position, player.ItemAlignment.transform.up, item.transform.localEulerAngles.y);
            this.transform.position += CalculateIKTargetPositionFromItem();
        }
    }

    /*
     * Calculate the new desired position of this target
     */
    private Vector3 CalculateIKTargetPositionFromItem()
    {
        PlayerItem item = player.Item;
        if (item && item.BaseTransform)
        {
            Vector3 basePositionOffset = startRelateVector.transform.TransformPoint(startPosition) + item.transform.TransformPoint(item.transform.localPosition) - item.transform.TransformPoint(0, 0, 0);
            basePositionOffset = item.BaseTransform.position - item.GetZeroedBaseTransformPosition();
            return basePositionOffset;
        }
        return new Vector3(0,0,0);
    }
} 