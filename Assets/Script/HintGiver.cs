using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HintGiver : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private string solution;
    private List<string> wordList;

    private void Start()
    {
        wordList = new List<string>(solution.Split(" "));
    }

    public void HintButtonPressed()
    {
        if (wordList.Count == 0)
            return;
        text.text += " ";
        text.text += wordList[0];
        wordList.RemoveAt(0);
    }
}
