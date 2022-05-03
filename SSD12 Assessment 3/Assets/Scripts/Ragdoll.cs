using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [SerializeField] private GameObject[] RagdollParts;
    [SerializeField] private GameObject camera;

    public GameObject[] RagdollPartsGet { get => RagdollParts;}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCameraState(bool enabled)
    {
        camera.SetActive(enabled);
    }
}
