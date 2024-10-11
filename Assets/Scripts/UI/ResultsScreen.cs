using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultsScreen : MonoBehaviour
{
    
    [Header("Text & Image Fields")]
    [SerializeField] private TextMeshProUGUI fishName;
    [SerializeField] private TextMeshProUGUI fishDescription;
    [SerializeField] private Image fishImage;
    [SerializeField] private TextMeshProUGUI fishLength;
    [SerializeField] private TextMeshProUGUI fishWeight;
    [SerializeField] private TextMeshProUGUI fishWeightClass;

    [Header("Weight Class Sprites")]
    [SerializeField] private Sprite smallWeightSprite;
    [SerializeField] private Sprite mediumWeightSprite;
    [SerializeField] private Sprite largeWeightSprite;
    [SerializeField] private Sprite giganticWeightSprite;

    [Header("Fish Model Display")]
    [SerializeField] private GameObject fishModelDisplay;

    [Header("ListenEvent")]
    [SerializeField] private FishEventChannelSO fishFoundEventSO;

    private void OnEnable()
    {
        fishFoundEventSO.OnEventRaised += InitializeResultsScreen;
    }

    private void OnDisable()
    {
        fishFoundEventSO.OnEventRaised -= InitializeResultsScreen;
    }

    private void InitializeResultsScreen(Fish fish)
    {
        fishName.text = fish.Name;
        fishDescription.text = fish.Description;
        fishImage.sprite = fish.Image;
        fishLength.text = fish.Length.ToString() + "ft";
        fishWeight.text = fish.Weight.ToString() + "lbs";
        

        UpdateWeightClass(fish.WeightClass);

        if (fishModelDisplay!= null)
        {
            fishModelDisplay.SetActive(true);
        }
    }

    // Weight speicfication is temporary can change this depending on the fishes 
    private void UpdateWeightClass(FishWeightClass weightClass)
    {
        switch (weightClass)
        {
            case FishWeightClass.Small:
                fishWeightClass.text = "Small";
                fishImage.sprite = smallWeightSprite;
                break;
            case FishWeightClass.Medium:
                fishWeightClass.text = "Medium";
                fishImage.sprite = mediumWeightSprite;
                break;
            case FishWeightClass.Large:
                fishWeightClass.text = "Large";
                fishImage.sprite = largeWeightSprite;
                break;
            case FishWeightClass.Gigantic:
                fishWeightClass.text = "Gigantic";
                fishImage.sprite = giganticWeightSprite;
                break;
        }
    }

    public void ReturnToHome()
    {
        SceneManager.LoadScene("AndresScene");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        print("The Button is working");
    }
}
