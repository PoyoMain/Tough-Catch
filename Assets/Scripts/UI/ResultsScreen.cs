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
    [SerializeField] private FishSO bigFish;

    [Header("Krey")]
    [SerializeField] private Animator kreyAnim;
    [SerializeField] private Transform objectSpawnTransform;

    [Header("Star Settings")]
    [SerializeField] private Animation starPrefab;
    [SerializeField] private Transform starParent;

    [Header("ListenEvent")]
    [SerializeField] private VoidEventChannelSO resultsShowEvent;
    [SerializeField] private FishEventChannelSO fishFoundEventSO;
    [SerializeField] private FloatEventChannelSO timerStoppedEventSO;
    [SerializeField] private VoidEventChannelSO starGainedEventSO;

    

    [Range(0, 3)]
    private int starsGained;

    private Coroutine starCoroutine;
    private Fish caughtFish;

    private void OnEnable()
    {
        resultsShowEvent.OnEventRaised += Activate;
        fishFoundEventSO.OnEventRaised += InitializeResultsScreen;
        timerStoppedEventSO.OnEventRaised += SetTimeForFishCatch;
        starGainedEventSO.OnEventRaised += GainStar;
    }

    private void OnDisable()
    {
        resultsShowEvent.OnEventRaised -= Activate;
        fishFoundEventSO.OnEventRaised -= InitializeResultsScreen;
        timerStoppedEventSO.OnEventRaised -= SetTimeForFishCatch;
        starGainedEventSO.OnEventRaised -= GainStar;
    }

    private void InitializeResultsScreen(Fish fish)
    {
        caughtFish = fish;
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

    private void GainStar()
    {
        starsGained++;
    }

    private void Activate()
    {
        if (caughtFish.Name == bigFish.name) kreyAnim.SetTrigger("VictoryBig");
        else
        {
            kreyAnim.SetTrigger("VictorySmall");
            GameObject fishObject = Instantiate(caughtFish.Model, objectSpawnTransform);
            fishObject.transform.localPosition = Vector3.zero;
            fishObject.transform.localRotation = Quaternion.identity;
        }

        if (starCoroutine != null) StopCoroutine(starCoroutine);
        starCoroutine = StartCoroutine(nameof(StarCoroutine));
    }

    private IEnumerator StarCoroutine()
    {
        if (starsGained < 1) yield break;

        float timer = 0;

        for (int i = 1; i <= starsGained; i++)
        {
            Animation star = Instantiate(starPrefab, starParent);

            while (timer < star.clip.averageDuration)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            timer = 0;
        }

        yield break;
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
