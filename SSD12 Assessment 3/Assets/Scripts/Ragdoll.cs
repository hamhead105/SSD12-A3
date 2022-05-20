using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [SerializeField] private GameObject[] ragdollParts;

    public GameObject[] RagdollParts { get => ragdollParts; }
}
