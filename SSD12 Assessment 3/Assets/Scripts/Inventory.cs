using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private PlayerItem primaryItem;
    private PlayerItem secondaryItem;
    private int currentlySelected;

    public PlayerItem SecondaryItem { get => secondaryItem; set => secondaryItem = value; }
    public PlayerItem PrimaryItem { get => primaryItem; set => primaryItem = value; }
    public int CurrentlySelected { get => currentlySelected; set => currentlySelected = value; }

    public void Init()
    {
        currentlySelected = 1;
        primaryItem = (Resources.Load("Weapon_Unarmed") as GameObject).GetComponent<PlayerItem>();
        secondaryItem = (Resources.Load("Weapon_Unarmed") as GameObject).GetComponent<PlayerItem>();
        print("assign unarmed");
    }

    public void SetInventory(PlayerItem primary, PlayerItem secondary, bool fullAmmo)
    {
        primaryItem = primary;
        secondaryItem = secondary;
        if (primaryItem is PlayerFirearm) (primaryItem as PlayerFirearm).Chambered = true;
        if (secondaryItem is PlayerFirearm) (secondaryItem as PlayerFirearm).Chambered = true;
        if (primaryItem is PlayerFirearm && fullAmmo) (primaryItem as PlayerFirearm).Ammo = (primaryItem as PlayerFirearm).MaxAmmo;
        if (secondaryItem is PlayerFirearm && fullAmmo) (secondaryItem as PlayerFirearm).Ammo = (secondaryItem as PlayerFirearm).MaxAmmo;
    }

    public void SetInventory(Inventory inventory)
    {
        primaryItem = inventory.PrimaryItem;
        secondaryItem = inventory.SecondaryItem;
    }

    public void GiveItemAsPrimary(PlayerItem item, bool fullAmmo)
    {
        primaryItem = item;
        if (item is PlayerFirearm && fullAmmo) (item as PlayerFirearm).Ammo = (item as PlayerFirearm).MaxAmmo;
    }

    public void GiveItemAsSecondary(PlayerItem item, bool fullAmmo)
    {
        secondaryItem = item;
        if (item is PlayerFirearm && fullAmmo) (item as PlayerFirearm).Ammo = (item as PlayerFirearm).MaxAmmo;
    }

    public void AssignNewItem(PlayerItem item, bool fullAmmo)
    {
        if (CurrentlySelected == 1)
        {
            GiveItemAsPrimary(item, fullAmmo);
        }
        else if (CurrentlySelected == 2)
        {
            GiveItemAsSecondary(item, fullAmmo);
        }
    }
}
