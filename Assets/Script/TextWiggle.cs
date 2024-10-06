using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextWiggle : MonoBehaviour
{
    TextMeshProUGUI text;
    public TMP_FontAsset[] fonts;
    int i = 0;
    public float wiggleFreq;

    private void Start()
    {
        InvokeRepeating("Wiggle", wiggleFreq, wiggleFreq);
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Wiggle()
    {
        i++;
        if (i >= fonts.Length) { i = 0; }
        text.font = fonts[i];
    }
}
