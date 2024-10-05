using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordPlayer : DiscHolder
{
    public override void AttachDisc(Disc disc)
    {
        base.AttachDisc(disc);
        GetDiscHeld().PlayDisc(true);
    }

    public override void DetachDisc(Disc disc)
    {
        GetDiscHeld().PlayDisc(false);
        base.DetachDisc(disc);
    }
}
