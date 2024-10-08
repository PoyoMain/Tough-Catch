using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultsScreen : MonoBehaviour
{
   
    [SerializeField] private TextMeshProUGUI fishName;
    [SerializeField] private TextMeshProUGUI fishDescription;
    [SerializeField] private Image fishImage;
    [SerializeField] private TextMeshProUGUI fishLength;
    [SerializeField] private TextMeshProUGUI fishWeight;
    [SerializeField] private TextMeshProUGUI fishWeightClass;

  
    [SerializeField] private FishFoundEvent fishFoundEvent;

    private void OnEnable()
    {
        fishFoundEvent.OnEventRaised += UpdateResultsScreen;
    }

    private void OnDisable()
    {
        fishFoundEvent.OnEventRaised -= UpdateResultsScreen;
    }

    private void UpdateResultsScreen(Fish fish)
    {
        fishName.text = fish.Name;
        fishDescription.text = fish.Description;
        fishImage.sprite = fish.Image;
        fishLength.text = fish.Length.ToString() + "ft";
        fishWeight.text = fish.Weight.ToString() + "lbs";
        fishWeightClass.text = fish.WeightClass.ToString();
    }

}
