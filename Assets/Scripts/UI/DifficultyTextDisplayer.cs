using System;
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
            string substring = "";

            if (tmp.text.Contains("{Difficulty}"))
            {
                substring = "{Difficulty}";
            }
            else
            {
                foreach (string difficulty in Enum.GetNames(typeof(Difficulty)))
                {
                    if (tmp.text.Contains (difficulty))
                    {
                        substring = difficulty;
                        break;
                    }
                }
            }

            string newText = tmp.text.Replace(substring, options.Difficulty.ToString());
            tmp.text = newText;
        }
    }
}
