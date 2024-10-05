using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordPlayer : DiscHolder
{
    [SerializeField] private Button _myButton;

    public override void AttachDisc(Disc disc)
    {
        base.AttachDisc(disc);
        _myButton.enabled = true;
    }

    public override void DetachDisc(Disc disc)
    {
        _myButton.enabled = false;
        disc.StopDisc();
        base.DetachDisc(disc);
    }

    public void PlayMyDisc()
    {
        if (!GetDiscHeld()) return;
        base.GetDiscHeld().PlayDisc();
    }
}
