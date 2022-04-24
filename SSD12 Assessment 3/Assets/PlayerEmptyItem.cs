using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
