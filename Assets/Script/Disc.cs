using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disc : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 1;
    [SerializeField] private double spinTime = 0.05;
    [SerializeField] private DiscHolder currentHolder;
    private double spinTimer = 0;
    private bool spinning;

    private bool dragging;
    private Vector3 dragOffset;

    // Start is called before the first frame update
    void Start()
    {
        SetHolder(currentHolder);
    }

    // Update is called once per frame
    void Update()
    {
        // spin
        spinTimer -= Time.deltaTime;
        if (spinTimer < 0 && spinning)
        {
            spinTimer = spinTime;
            Spin();
        }

        //drag
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            Collider2D targetObject = Physics2D.OverlapPoint(mousePosition);

            if (targetObject && targetObject.transform.gameObject == this.gameObject)
            {
                dragging = true;
                dragOffset = transform.position - mousePosition;
            }
        }

        if (dragging)
        {
            transform.position = mousePosition + dragOffset;
        }

        if (Input.GetMouseButtonUp(0) && dragging)
        {
            DiscHolder discHolder = GameManager.Inst.GetClosestDiscHolderToPosition(transform.position);
            if (discHolder != null)
            {
                if (discHolder.GetDiscHeld() != null)
                    discHolder.GetDiscHeld().SetHolder(currentHolder);
                SetHolder(discHolder);
            }

            SnapToHolder();

            GameManager.Inst.DebugNodes();

            dragging = false;
        }
    }

    public float GetRotation()
    {
        return transform.rotation.eulerAngles.z;
    }

    public void SetSpinning(bool spinning)
    {
        this.spinning = spinning;
    }

    private void Spin()
    {
        transform.Rotate(new Vector3(0, 0, spinSpeed));
    }

    public void SetHolder(DiscHolder discHolder)
    {
        currentHolder.DetatchDisc(this);
        currentHolder = discHolder;
        currentHolder.AttachDisc(this);
        SnapToHolder();
    }

    public DiscHolder GetHolder()
    {
        return currentHolder;
    }

    private void SnapToHolder()
    {
        transform.position = currentHolder.transform.position;
    }
}
