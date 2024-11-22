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
    [SerializeField] private TextMeshProUGUI timeToCatch;
    [SerializeField] private TextMeshProUGUI fishName;
    [SerializeField] private TextMeshProUGUI fishDescription;
    [SerializeField] private Image fishImage;
    [SerializeField] private TextMeshProUGUI fishLength;
    [SerializeField] private TextMeshProUGUI fishWeight;
    [SerializeField] private TextMeshProUGUI fishWeightClass;
    [SerializeField] private Image weightClassImage;

    [Header("Weight Class Sprites")]
    [SerializeField] private Sprite smallWeightSprite;
    [SerializeField] private Sprite mediumWeightSprite;
    [SerializeField] private Sprite largeWeightSprite;
    [SerializeField] private Sprite giganticWeightSprite;

    [Header("Fish Model Display")]
    [SerializeField] private GameObject fishModelDisplay;

    [Header("ListenEvent")]
    [SerializeField] private FishEventChannelSO fishFoundEventSO;
    [SerializeField] private FloatEventChannelSO timerStoppedEventSO;

    private void OnEnable()
    {
        fishFoundEventSO.OnEventRaised += InitializeResultsScreen;
        timerStoppedEventSO.OnEventRaised += SetTimeForFishCatch;
    }

    private void OnDisable()
    {
        fishFoundEventSO.OnEventRaised -= InitializeResultsScreen;
        timerStoppedEventSO.OnEventRaised += SetTimeForFishCatch;
    }

    private void InitializeResultsScreen(Fish fish)
    {
        fishName.text = fish.Name;
        fishDescription.text = fish.Description;
        fishImage.sprite = fish.FullImage;
        fishLength.text = fish.Length.ToString() + "ft";
        fishWeight.text = fish.Weight.ToString() + "lbs";

        UpdateWeightClass(fish.WeightClass);
    }

    private void UpdateWeightClass(FishWeightClass weightClass)
    {
        fishWeightClass.text = weightClass.ToString();

        weightClassImage.sprite = weightClass switch
        {
            FishWeightClass.Small => smallWeightSprite,
            FishWeightClass.Medium => mediumWeightSprite,
            FishWeightClass.Large => largeWeightSprite,
            FishWeightClass.Gigantic => giganticWeightSprite,
            _ => throw new System.NotImplementedException(),
        };
    }

    private void SetTimeForFishCatch(float time)
    {
        var minutes = (int)(time / 60);
        var remainingSeconds = (int)(time - minutes * 60);
        print(minutes.ToString() + ':' + remainingSeconds.ToString("00"));
        timeToCatch.text = minutes.ToString() + ":" + remainingSeconds.ToString("00");
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
