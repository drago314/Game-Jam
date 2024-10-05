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

    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private string[] clipValues;

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

    private void Spin()
    {
        transform.Rotate(new Vector3(0, 0, spinSpeed));
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
        transform.position = currentHolder.transform.position;
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
        for (int i = 0; i < audioClips.Length; i++)
        {
            audioSource.clip = audioClips[i];
            audioSource.Play();

            GameManager gm = GameManager.Inst;
            if (gm.currentConstructedString.Count < gm.currentSentenceLength) gm.currentConstructedString.Add(clipValues[i]);
            else
            {
                gm.currentConstructedString.RemoveAt(0);
                gm.currentConstructedString.Add(clipValues[i]);
            }
            Debug.Log(gameObject.name + "played: " + clipValues[i]);

            yield return new WaitForSeconds(2);
        }
    }

    public void StopDisc()
    {
        spinning = false;
        audioSource.Stop();
        StopCoroutine(StartPlayingClips());
    }
}
