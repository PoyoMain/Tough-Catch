using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultsScreen : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string mainMenuSceneName;
    [SerializeField] private string gameSceneName;

    [Header("Text & Image Fields")]
    [SerializeField] private TextMeshProUGUI fishName;
    [SerializeField] private TextMeshProUGUI fishDescription;
    [SerializeField] private Image fishImage;
    [SerializeField] private TextMeshProUGUI fishLength;
    [SerializeField] private TextMeshProUGUI fishWeight;
    [SerializeField] private TextMeshProUGUI fishWeightClass;

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
        fishWeightClass.text = fish.WeightClass.ToString();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
