using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DifficultyTextDisplayer : MonoBehaviour
{
    [Header("Opitions")]
    [SerializeField] private OptionsSO options;

    [Header("Text Fields")]
    [SerializeField] private TextMeshProUGUI[] textFields;


    private void OnEnable()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        foreach (TextMeshProUGUI tmp in textFields)
        {
            string newText = tmp.text.Replace("{Difficulty}", options.Difficulty.ToString());
            tmp.text = newText;
        }
    }
}
