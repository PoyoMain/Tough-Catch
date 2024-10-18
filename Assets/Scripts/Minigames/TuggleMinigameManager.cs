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

    [Header("Listen Events")]
    [SerializeField] private VoidEventChannelSO tuggleSucceedSO;


    private void Start()
    {
        Invoke(nameof(StartNewMinigame), firstMinigameSpawnTime);

        for (int i = 1; i < numberOfMinigamesAtOnce; i++)
        {
            Invoke(nameof(StartNewMinigame), (minigameGapTime * i) + firstMinigameSpawnTime);
        }
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
            minigame.minigame.MinigameSuccessEvent.OnEventRaised -= MinigameFinished;

            if (minigame.destroyOnTuggleEnd) Destroy(minigame.minigame.gameObject);
            else minigame.Enabled = false;
        }
    }

    private void StartNewMinigame()
    {
        List<SpawnableMinigame> inactiveMinigames = new();
        float totalPercent = 0f;

        foreach (SpawnableMinigame minigame in minigames)
        {
            if (!minigame.Enabled && minigame.spawnChance > 0)
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
        Invoke(nameof(StartNewMinigame), minigameGapTime);
    }

    private void FinishPhase()
    {
        this.enabled = false;
        foreach (SpawnableMinigame minigame in minigames)
        {
            minigame.Enabled = false;
            minigame.minigame.MinigameSuccessEvent.OnEventRaised -= MinigameFinished;
        }
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
