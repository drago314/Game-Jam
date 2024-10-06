using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HintGiver : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private string solution;
    private List<string> wordList;

    [SerializeField] AudioSource source;

    public Transform button;
    float scaleMult = 1;
    Vector2 defaultScale;

    private void Start()
    {
        wordList = new List<string>(solution.Split(" "));
        defaultScale = button.localScale;
    }

    private void Update()
    {
        button.localScale = Vector2.Lerp(button.localScale, defaultScale * scaleMult, Time.deltaTime * 10);
    }

    public void HintButtonPressed()
    {
        if (wordList.Count == 0)
            return;
        text.text += " ";
        text.text += wordList[0];
        wordList.RemoveAt(0);
        source.pitch = Random.Range(0.8f, 1.2f);
        source.Play();
    }

    public void ChangeScaleMult(float mult) { scaleMult = mult; Debug.Log(mult); }
}
