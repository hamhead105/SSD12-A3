using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BillboardCanvas : MonoBehaviour
{
    public void Update()
    {
        // TODO OPTIMISATION
        foreach (Camera camera in FindObjectsOfType<Camera>())
        {
            if (camera.gameObject.activeSelf)
            {
                transform.LookAt(camera.transform.position);
            } 
        }
    }
}
