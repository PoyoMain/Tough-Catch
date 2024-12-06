using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private OptionsSO options;
    [SerializeField] private bool testing;
    [Space(10)]
    [SerializeField] private Image heartPrefab;
    [SerializeField] private Transform healthParent;

    [Header("Health")]
    [SerializeField] private int testHealth;

    [Header("Broadcast Events")]
    [SerializeField] private VoidEventChannelSO gameOverSO;
    [SerializeField] private VoidEventChannelSO starGainedSO;

    [Header("Listen Events")]
    [SerializeField] private VoidEventChannelSO tugglePhaseStartSO;
    [SerializeField] private VoidEventChannelSO tugglePhaseSucceedSO;
    [SerializeField] private VoidEventChannelSO playerTakeDamageSO;

    private List<Image> healthImages;
    private int StartHealth
    {
        get => testing ? testHealth : options.GeneralOptions.health;
    }

    private int CurrentHealth => healthImages.Count;

    private bool hasTakenDamage = false;

    private void Awake()
    {
        List<Image> heartList = new();

        for (int i = 0; i < StartHealth; i++)
        {
            Image heart = Instantiate(heartPrefab, healthParent);
            heartList.Add(heart); 
        }

        healthImages = heartList;
    }

    private void OnEnable()
    {
        playerTakeDamageSO.OnEventRaised += TakeDamage;
        tugglePhaseSucceedSO.OnEventRaised += CheckForNoDamage;
    }

    private void OnDisable()
    {
        playerTakeDamageSO.OnEventRaised -= TakeDamage;
        tugglePhaseSucceedSO.OnEventRaised -= CheckForNoDamage;
    }

    private void TakeDamage()
    {
        Destroy(healthImages[CurrentHealth - 1].gameObject);
        healthImages.RemoveAt(CurrentHealth - 1);

        if (!hasTakenDamage) hasTakenDamage = true;

        if (CurrentHealth <= 0)
        {
            gameOverSO.RaiseEvent();
            return;
        }
    }

    private void CheckForNoDamage()
    {
        if (!hasTakenDamage) starGainedSO.RaiseEvent();
    }
}
