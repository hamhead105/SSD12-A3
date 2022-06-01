using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A bone that tracks another bone and rotates correctly around it in a pivot manner
 */
public class TrackedLimb : MonoBehaviour
{
    [SerializeField] private Transform targetLimb;
    [SerializeField] private ConfigurableJoint configurableJoint;
    private Quaternion targetInitialRotation;

    // Start is called before the first frame update
    void Start()
    {
        this.configurableJoint = this.GetComponent<ConfigurableJoint>();
        this.targetInitialRotation = this.targetLimb.transform.localRotation;
    }

    /*
     * Called at a fixed rate
     */
    private void FixedUpdate()
    {
        this.configurableJoint.targetRotation = CopyRotation();
    }

    /*
     * Copy the rotation of a target bone
     */
    private Quaternion CopyRotation()
    {
        return Quaternion.Inverse(this.targetLimb.localRotation) * this.targetInitialRotation;
    }
}
