using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Disc[] discs;
    [SerializeField] DiscHolder[] discHolders;

    [SerializeField] double snapDistance;

    public static GameManager Inst = null;

    private void Awake()
    {
        if (Inst != null)
            Destroy(this);
        Inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        discs = FindObjectsOfType<Disc>();
        discHolders = FindObjectsOfType<DiscHolder>();
        foreach (Disc disc in discs) {
            disc.SetSpinning(true);
        }

        DebugNodes();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public DiscHolder GetClosestDiscHolderToPosition(Vector2 position)
    {
        double min = Mathf.Infinity;
        DiscHolder closest = null;
        foreach (DiscHolder discHolder in discHolders)
        {
            double dist = Vector3.Distance(position, discHolder.transform.position);
            if (dist < min)
            {
                min = dist;
                closest = discHolder;
            }
        }
        if (min < snapDistance)
            return closest;
        else
            return null;
    }


    public void DebugNodes()
    {
        string print = "";
        foreach (DiscHolder holder in discHolders)
            if (holder.GetDiscHeld() != null)
                print += holder.gameObject.name + " is holding " + holder.GetDiscHeld().gameObject.name + "\n";

        foreach (Disc disc in discs)
                print += disc.gameObject.name + " is on " + disc.GetHolder().gameObject.name + "\n";

        Debug.Log(print);
    }
}
