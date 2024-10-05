using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Disc[] discs;
    [SerializeField] DiscHolder[] discHolders;
    [SerializeField] double snapDistance;
    public GameObject blinker;
    public static GameManager Inst = null;

    public List<string> currentConstructedString;
    public string[] currentSentence;
    public AudioClip[] sentenceClips;

    private void Awake()
    {
        if (Inst != null)
            Destroy(this);
        Inst = this;

        currentConstructedString = new List<string>();
    }

    // Start is called before the first frame update
    void Start()
    {
        discs = FindObjectsOfType<Disc>();
        discHolders = FindObjectsOfType<DiscHolder>();

        DebugNodes();
    }

    // Update is called once per frame
    void Update()
    {
        bool levelCompleted = true;
        if (currentConstructedString.Count == currentSentence.Length)
        {
            for (int i = 0; i < currentSentence.Length; i++)
            {
                if (!currentSentence[i].Equals(currentConstructedString[i]))
                    levelCompleted = false;
            }
        }
        else
            levelCompleted = false;

        if (levelCompleted)
        {
            blinker.SetActive(true);
            Invoke("LoadNextScene", 0.6f);
        }
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
