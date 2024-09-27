using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TuggleMinigameManager : MonoBehaviour
{
    [Header("Minigames")]
    [SerializeField] private SpawnableMinigame[] minigames;

    [Header("Listen Events")]
    [SerializeField] private VoidEventChannelSO tuggleSucceedSO;

    private void Start()
    {
        StartNewMinigame();
        Invoke(nameof(StartNewMinigame), 3f);
    }

    private void OnEnable()
    {
        tuggleSucceedSO.OnEventRaised += FinishPhase;

        foreach (SpawnableMinigame minigame in minigames)
        {
            minigame.minigame.MinigameSuccessEvent.OnEventRaised += MinigameFinished;
        }
    }

    private void OnDisable()
    {
        tuggleSucceedSO.OnEventRaised -= FinishPhase;

        foreach (SpawnableMinigame minigame in minigames)
        {
            minigame.Enabled = false;
            minigame.minigame.MinigameSuccessEvent.OnEventRaised -= MinigameFinished;
        }
    }

    private void StartNewMinigame()
    {
        List<SpawnableMinigame> inactiveMinigames = new();
        float totalPercent = 0f;

        foreach (SpawnableMinigame minigame in minigames)
        {
            if (!minigame.Enabled)
            {
                inactiveMinigames.Add(minigame);
                totalPercent += minigame.spawnChance;
            }
        }

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
        Invoke(nameof(StartNewMinigame), 0.05f);
    }

    private void FinishPhase()
    {
        this.enabled = false;
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

    }
}
