using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscShadow : MonoBehaviour
{
    [SerializeField] private Transform myDisc, shadow;
    Disc discScript;

    private void Start()
    {
        discScript = myDisc.GetComponent<Disc>();
        transform.parent = myDisc.parent;
    }

    private void Update()
    {
        transform.position = myDisc.position;
        Vector2 offset = new(0.8f, -0.8f);
        if (discScript.GetDragging()) { offset = new(1.7f, -1.7f); }
        shadow.localPosition = Vector2.Lerp(shadow.localPosition, offset, Time.deltaTime * 6);
    }
}
