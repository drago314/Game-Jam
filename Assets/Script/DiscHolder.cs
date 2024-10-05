using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscHolder : MonoBehaviour
{
    private Disc discHeld = null;

    public Disc GetDiscHeld()
    {
        return discHeld;
    }

    public virtual void AttachDisc(Disc disc)
    {
        discHeld = disc;
    }

    public virtual void DetachDisc(Disc disc)
    {
        if (discHeld == disc)
            discHeld = null;
    }
}
