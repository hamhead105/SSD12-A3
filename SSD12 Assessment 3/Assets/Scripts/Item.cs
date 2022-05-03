using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private string itemName;
    [SerializeField] private Texture itemIcon;

    public string ItemName { get => itemName; set => itemName = value; }
    public Texture ItemIcon { get => itemIcon; set => itemIcon = value; }
}
