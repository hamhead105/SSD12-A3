using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Used for the unarmed player item, i.e when the player has no item equipped. This may not be implemented to the game
 */
public class PlayerEmptyItem : PlayerItem
{
    public override bool PrimaryUseItem(bool input)
    {
        return true;
    }

    public override void SecondaryUseItem(bool input)
    {

    }
}
