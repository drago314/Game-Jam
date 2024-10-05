using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] List<Disc> discs = new List<Disc>();
    [SerializeField] DiscHolder[] discHolders;

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
        discHolders = FindObjectsOfType<DiscHolder>();
        foreach (Disc disc in discs) {
            disc.SetSpinning(true);
        }
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
        return closest;
    }
}
