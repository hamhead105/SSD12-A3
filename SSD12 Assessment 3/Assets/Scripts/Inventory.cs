using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Base class for player inventory. Note this will probably not be used for the game to a large extent, as implementing 
 * secondary items and such is not a priority.
 */
public class Inventory : MonoBehaviour
{
    private PlayerItem primaryItem;
    private PlayerItem secondaryItem;
    private int currentlySelected;

    public PlayerItem SecondaryItem { get => secondaryItem; set => secondaryItem = value; }
    public PlayerItem PrimaryItem { get => primaryItem; set => primaryItem = value; }
    public int CurrentlySelected { get => currentlySelected; set => currentlySelected = value; }

    /*
     * Initialize Player inventory
     */
    public void Init()
    {
        currentlySelected = 1;
        primaryItem = (Resources.Load("Weapon_Unarmed") as GameObject).GetComponent<PlayerItem>();
        secondaryItem = (Resources.Load("Weapon_Unarmed") as GameObject).GetComponent<PlayerItem>();
        print("assign unarmed");
    }

    /*
     * Set inventory to a set of items
     * 
     * @param primary - the player's primary item
     * @param secondary - the player's secondary item
     * @param fullAmmo - whether to set ammunition to full
     */
    public void SetInventory(PlayerItem primary, PlayerItem secondary, bool fullAmmo)
    {
        primaryItem = primary;
        secondaryItem = secondary;
        if (primaryItem is PlayerFirearm) (primaryItem as PlayerFirearm).Chambered = true;
        if (secondaryItem is PlayerFirearm) (secondaryItem as PlayerFirearm).Chambered = true;
        if (primaryItem is PlayerFirearm && fullAmmo) (primaryItem as PlayerFirearm).Ammo = (primaryItem as PlayerFirearm).MaxAmmo;
        if (secondaryItem is PlayerFirearm && fullAmmo) (secondaryItem as PlayerFirearm).Ammo = (secondaryItem as PlayerFirearm).MaxAmmo;
    }

    /*
     * Set this inventory to another
     */
    public void SetInventory(Inventory inventory)
    {
        primaryItem = inventory.PrimaryItem;
        secondaryItem = inventory.SecondaryItem;
    }

    /*
     * Give an item to this inventory at the primary slot
     * 
     * @param item - the item type
     * @param fullAmmo - whether this item has full Ammunition or uses
     */
    public void GiveItemAsPrimary(PlayerItem item, bool fullAmmo)
    {
        primaryItem = item;
        if (item is PlayerFirearm && fullAmmo) (item as PlayerFirearm).Ammo = (item as PlayerFirearm).MaxAmmo;
    }

    /*
     * Give an item to this inventory at the secondary slot
     * 
     * @param item - the item type
     * @param fullAmmo - whether this item has full Ammunition or uses
     */
    public void GiveItemAsSecondary(PlayerItem item, bool fullAmmo)
    {
        secondaryItem = item;
        if (item is PlayerFirearm && fullAmmo) (item as PlayerFirearm).Ammo = (item as PlayerFirearm).MaxAmmo;
    }


     /*
     * Give an item to this inventory, to the slot the player is currently using
     * 
     * @param item - the item type
     * @param fullAmmo - whether this item has full Ammunition or uses
     */
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
