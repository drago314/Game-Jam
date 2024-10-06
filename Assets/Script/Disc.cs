using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;

public class Disc : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 1;
    [SerializeField] private double spinTime = 0.05;
    [SerializeField] private DiscHolder currentHolder;
    private double spinTimer = 0;
    private bool spinning;

    [SerializeField] private AudioSource audioSource, staticSource;
    public AudioSource foleySource;
    [SerializeField] private AudioClip placeClip, playClip, singleSource, blipClip, pickupClip;
    private float staticSourceVol;
    public Light2D myLight;

    [SerializeField] private int[] myWordIndeces;

    private bool dragging;
    private Vector3 dragOffset;

    private Vector3 defaultScale;
    private float currentScaleMult = 1;

    private Vector2 goToPos;
    private float snapSpeed = 10;

    [SerializeField] private int superSpecialInt;

    [SerializeField] private TextMeshProUGUI caption;

    // Start is called before the first frame update
    void Start()
    {
        SetHolder(currentHolder);
        defaultScale = transform.localScale;
        staticSourceVol = staticSource.volume;
    }

    // Update is called once per frame
    void Update()
    {
        // spin
        if (spinning) { Spin(); }
        myLight.enabled = spinning;

        //drag
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Collider2D targetObject = Physics2D.OverlapPoint(mousePosition);

        if (targetObject && targetObject.transform.gameObject == this.gameObject)
        {
            if (currentScaleMult < 1.1f && !dragging) { foleySource.PlayOneShot(blipClip, 0); }
            currentScaleMult = 1.15f;
            if (Input.GetMouseButtonDown(0))
            {
                foleySource.PlayOneShot(pickupClip, 0.7f);
                dragging = true;
                dragOffset = transform.position - mousePosition;
                if (spinning) { StopDisc(); }
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
        if (spinning) return;
        spinning = true;
        audioSource.Play();
        staticSource.Play();
        foleySource.PlayOneShot(playClip);
        if (singleSource) { audioSource.clip = singleSource; audioSource.Play(); Invoke("AbruptStop", singleSource.length); return; }
        StartCoroutine("StartPlayingClips");
        CancelInvoke("AbruptStop");
    }

    // plays every word in 2 second intervals to allow for other records to play in the blank space
    IEnumerator StartPlayingClips()
    {
        for (int i = 0; i < myWordIndeces.Length; i++)
        {
            GameManager gm = GameManager.Inst;

            string[] sourceArray;
            int index = myWordIndeces[i];
            if (index < 0) { index *= -1; sourceArray = gm.redHerringList; audioSource.clip = gm.redHerringClips[index]; }
            else { sourceArray = gm.currentSentence; audioSource.clip = gm.sentenceClips[index]; }

            audioSource.Play();

            staticSource.volume = staticSourceVol / 2;
            Invoke("ReupStatic", audioSource.clip.length);

            caption.text = sourceArray[index];
            Invoke("ResetCaption", 1);

            if (gm.currentConstructedString.Count < gm.checkingSentence.Count) gm.currentConstructedString.Add(gm.currentSentence[index]);
            else
            {
                gm.currentConstructedString.RemoveAt(0);
                if (!sourceArray[index].Equals(""))
                    gm.currentConstructedString.Add(sourceArray[index]);
            }
            if (i < myWordIndeces.Length - 1) yield return new WaitForSeconds(Mathf.Abs(index - Mathf.Abs(myWordIndeces[i + 1])));
            else 
            {
                yield return new WaitForSeconds(Mathf.Max(2, audioSource.clip.length));
                foleySource.PlayOneShot(playClip);
                if (superSpecialInt == 1) 
                {
                    gm.Blink();
                    string load = "SampleScene";
                    if (PlayerPrefs.GetString("level") != "") { load = PlayerPrefs.GetString("level"); }
                    gm.nextLoad = load; 
                }
                if (superSpecialInt == 2) { gm.Blink(); gm.nextLoad = "Credits"; }
                if (superSpecialInt == 3) { gm.Blink(); gm.nextLoad = "Menu"; }
                StopDisc();
            }
        }
    }
    private void ReupStatic() { staticSource.volume = staticSourceVol; }

    public void StopDisc()
    {
        spinning = false;
        //transform.rotation = Quaternion.identity;
        audioSource.Stop();
        staticSource.Stop();
        StopCoroutine("StartPlayingClips");
    }
    private void AbruptStop() { foleySource.PlayOneShot(playClip); StopDisc(); }

    public bool GetDragging() { return dragging; }

    private void ResetCaption() { caption.text = ""; }
}
