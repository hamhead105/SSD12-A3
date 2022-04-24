using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandIKTarget : MonoBehaviour
{
    //constants
    [SerializeField] private Vector3 startPosition = new Vector3(0, -0.031f, 0);
    [SerializeField] private Human player;

    // control variable
    [SerializeField] private Transform startRelateVector;
    [SerializeField] private Transform playerPivotPoint;

    void Update()
    {
        UpdateTransforms();
    }

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