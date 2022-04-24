using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : Item
{
    [SerializeField] private int sightNumber;
    [SerializeField] private int currentAmmo;
    [SerializeField] private int sightAttachment;

    [SerializeField] private GameObject sightParent;

    public int SightAttachment { get => sightAttachment; set => sightAttachment = value; }

    // Start is called before the first frame update
    void Start()
    {
        if (sightParent)
        {
            sightParent.transform.GetChild(SightAttachment).gameObject.SetActive(true);
        }
    }
}
