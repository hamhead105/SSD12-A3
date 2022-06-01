using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class holding a list of limb parts
 */
public class Ragdoll : MonoBehaviour
{
    [SerializeField] private GameObject[] ragdollParts;

    public GameObject[] RagdollParts { get => ragdollParts; }
}
