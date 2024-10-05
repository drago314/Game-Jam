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

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private int[] myWordIndeces;

    private bool dragging;
    private Vector3 dragOffset;

    private Vector3 defaultScale;
    private float currentScaleMult = 1;

    private Vector2 goToPos;
    private float snapSpeed = 10;

    // Start is called before the first frame update
    void Start()
    {
        SetHolder(currentHolder);
        defaultScale = transform.localScale;
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

        Collider2D targetObject = Physics2D.OverlapPoint(mousePosition);

        if (targetObject && targetObject.transform.gameObject == this.gameObject)
        {
            currentScaleMult = 1.15f;
            if (Input.GetMouseButtonDown(0))
            {
                dragging = true;
                dragOffset = transform.position - mousePosition;
            }
        }
        else { currentScaleMult = 1; }

        if (dragging)
        {
            transform.position = mousePosition + dragOffset;
            currentScaleMult = 1;
        }

        transform.localScale = Vector3.Lerp(transform.localScale, currentScaleMult * defaultScale, 10*Time.deltaTime);

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

        if (!dragging) transform.position = Vector2.Lerp(transform.position, goToPos, snapSpeed * Time.deltaTime);
    }

    public float GetRotation()
    {
        return transform.rotation.eulerAngles.z;
    }

    private void Spin()
    {
        transform.Rotate(new Vector3(0, 0, spinSpeed * Time.deltaTime));
    }

    public void SetHolder(DiscHolder discHolder)
    {
        currentHolder.DetachDisc(this);
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
        goToPos = currentHolder.transform.position;
    }

    public void PlayDisc()
    {
        spinning = true;
        audioSource.Play();
        StartCoroutine(StartPlayingClips());
    }

    // plays every word in 2 second intervals to allow for other records to play in the blank space
    IEnumerator StartPlayingClips()
    {
        for (int i = 0; i < myWordIndeces.Length; i++)
        {
            GameManager gm = GameManager.Inst;

            audioSource.clip = gm.sentenceClips[myWordIndeces[i]];
            audioSource.Play();

            if (gm.currentConstructedString.Count < gm.currentSentence.Length) gm.currentConstructedString.Add(gm.currentSentence[myWordIndeces[i]]);
            else
            {
                gm.currentConstructedString.RemoveAt(0);
                gm.currentConstructedString.Add(gm.currentSentence[myWordIndeces[i]]);
            }
            if (i < myWordIndeces.Length - 1) yield return new WaitForSeconds(Mathf.Abs(myWordIndeces[i] - myWordIndeces[i + 1]));
            else { yield return null; }
        }
    }

    public void StopDisc()
    {
        spinning = false;
        transform.rotation = Quaternion.identity;
        audioSource.Stop();
        StopCoroutine(StartPlayingClips());
    }
}
