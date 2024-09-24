using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class TuggleMinigameManager : MonoBehaviour
{
    [Header("Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float laserMinigameSpawnChance;

    [Header("Listen Events")]
    [SerializeField] private VoidEventChannelSO tuggleSucceedSO;
    [SerializeField] private VoidEventChannelSO laserMinigameSucceedSO;

    private LaserMinigame laserMinigame;

    private bool LaserActive
    {
        get => laserMinigame.enabled;
        set => laserMinigame.enabled = value;
    }

    private void Awake()
    {
        TryGetComponent(out laserMinigame);
    }

    private void Start()
    {
        StartNewMinigame();
        Invoke(nameof(StartNewMinigame), 5f);
    }

    private void OnEnable()
    {
        tuggleSucceedSO.OnEventRaised += FinishPhase;

        laserMinigameSucceedSO.OnEventRaised += LaserSucceed;
    }

    private void OnDisable()
    {
        tuggleSucceedSO.OnEventRaised -= FinishPhase;

        laserMinigameSucceedSO.OnEventRaised -= LaserSucceed;
    }

    private void LaserSucceed()
    {
        LaserActive = false;
        print("Done");
        Invoke(nameof(StartNewMinigame), 0.05f);
    }

    private void StartNewMinigame()
    {
        List<MinigameBase> inactiveMinigames = new();

        float totalPercent = 0f;

        if (!LaserActive)
        {
            inactiveMinigames.Add(laserMinigame);
            totalPercent += laserMinigameSpawnChance;
        }

        if (inactiveMinigames.Count <= 0) return;

        float chosenPercent = Random.Range(0, totalPercent);

        if (chosenPercent <= laserMinigameSpawnChance)
        {
            LaserActive = true;
            return;
        }
        else chosenPercent -= laserMinigameSpawnChance;
    }

    private void FinishPhase()
    {
        LaserActive = false;
    }
}
