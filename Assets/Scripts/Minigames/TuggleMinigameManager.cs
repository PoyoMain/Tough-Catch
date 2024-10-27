using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class TuggleMinigameManager : MonoBehaviour
{
    [Header("Settings")]
    [Range(1, 3)]
    [SerializeField] private int numberOfMinigamesAtOnce = 2;
    [SerializeField] private float firstMinigameSpawnTime = 5;
    [SerializeField] private float minigameGapTime = 5;

    [Header("Minigames")]
    [SerializeField] private SpawnableMinigame[] minigames;

    [Header("Inspector Objects")]
    [SerializeField] private Animator startImage;
    [SerializeField] private Animator successImage;

    [Header("Broadcast Events")]
    [SerializeField] private VoidEventChannelSO tuggleStartSO;
    [SerializeField] private VoidEventChannelSO tuggleSucceedSO;

    [Header("Listen Events")]
    [SerializeField] private VoidEventChannelSO fishHealthDepleatedSO;

    private bool deactivate = false;

    private void Start()
    {
        startImage.gameObject.SetActive(true);
        StartCoroutine(nameof(TextDisplayCoroutine));
    }
    
    private IEnumerator TextDisplayCoroutine()
    {
        while (startImage.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) yield return null;

        startImage.gameObject.SetActive(false);
        Activate();

        while (deactivate == false) yield return null;

        successImage.gameObject.SetActive(true);

        while(successImage.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) yield return null;

        successImage.gameObject.SetActive(false);
        foreach (SpawnableMinigame minigame in minigames)
        {
            minigame.Enabled = false;
            minigame.minigame.MinigameSuccessEvent.OnEventRaised -= MinigameFinished;
        }
        this.enabled = false;

        yield break;
    }

    private void Activate()
    {
        tuggleStartSO.RaiseEvent();

        Invoke(nameof(StartNewMinigame), firstMinigameSpawnTime);

        for (int i = 1; i < numberOfMinigamesAtOnce; i++)
        {
            Invoke(nameof(StartNewMinigame), (minigameGapTime * i) + firstMinigameSpawnTime);
        }
    }

    private void OnEnable()
    {
        fishHealthDepleatedSO.OnEventRaised += FinishPhase;

        foreach (SpawnableMinigame minigame in minigames)
        {
            minigame.minigame.MinigameSuccessEvent.OnEventRaised += MinigameFinished;
        }
    }

    private void OnDisable()
    {
        fishHealthDepleatedSO.OnEventRaised -= FinishPhase;

        foreach (SpawnableMinigame minigame in minigames)
        {
            minigame.minigame.MinigameSuccessEvent.OnEventRaised -= MinigameFinished;

            if (minigame.destroyOnTuggleEnd) Destroy(minigame.minigame.gameObject);
            else minigame.Enabled = false;
        }
    }

    private void StartNewMinigame()
    {
        List<SpawnableMinigame> activeMinigames = new();
        List<SpawnableMinigame> inactiveMinigames = new();
        float totalPercent = 0f;

        foreach (SpawnableMinigame minigame in minigames)
        {
            if (!minigame.Enabled && minigame.spawnChance > 0)
            {
                inactiveMinigames.Add(minigame);
                totalPercent += minigame.spawnChance;
            }
            else if (minigame.Enabled)
            {
                activeMinigames.Add(minigame);
            }
        }

        if (activeMinigames.Count >= numberOfMinigamesAtOnce) return;
        if (inactiveMinigames.Count <= 0) return;

        float chosenPercent = UnityEngine.Random.Range(0, totalPercent);

        foreach (SpawnableMinigame minigame in inactiveMinigames)
        {
            if (chosenPercent <= minigame.spawnChance)
            {
                minigame.Enabled = true;
                return;
            }
            else chosenPercent -= minigame.spawnChance;
        }
    }

    private void MinigameFinished()
    {
        Invoke(nameof(StartNewMinigame), minigameGapTime);
    }

    private void FinishPhase()
    {
        deactivate = true;
    }

    [Serializable]
    private struct SpawnableMinigame
    {
        public MinigameBase minigame;
        [Range(0f, 1f)]
        public float spawnChance;

        public readonly bool Enabled { 
            get => minigame.enabled;
            set => minigame.enabled = value;
        }

        public bool destroyOnTuggleEnd;
    }
}
