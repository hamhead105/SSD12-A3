using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * @author - Peter Jung
 * 
 * Base class for billboard type objects, for example world UI elements
 */
public class Billboard : MonoBehaviour
{
    private static float baseFOV = 60f; 

    [SerializeField] private bool maintainSize;
    [SerializeField] private bool disableForPromode;
    private Transform cameraTransform;

    [SerializeField] private float objectScale = 1.0f;
    private Vector3 initialScale;

    public bool DisableForPromode { get => disableForPromode; set => disableForPromode = value; }

    // Start is called before the first frame update
    void Start()
    {
        initialScale = this.transform.localScale;
        foreach (Camera camera in FindObjectsOfType<Camera>())
        {
            if (camera.gameObject.activeSelf)
            {
                cameraTransform = camera.transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + cameraTransform.transform.rotation * Vector3.forward,
        cameraTransform.transform.rotation * Vector3.up);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);

        Plane plane = new Plane(cameraTransform.transform.forward, cameraTransform.transform.position);
        float dist = plane.GetDistanceToPoint(transform.position);
        transform.localScale = initialScale * dist * objectScale * (cameraTransform.gameObject.GetComponent<Camera>().fieldOfView / baseFOV);
    }
}
