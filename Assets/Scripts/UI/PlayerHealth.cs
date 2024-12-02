using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Settings")]

    [Header("Broadcast Events")]
    [SerializeField] private VoidEventChannelSO gameOverSO;
    [SerializeField] private VoidEventChannelSO starGainedSO;

    [Header("Listen Events")]
    [SerializeField] private VoidEventChannelSO tugglePhaseStartSO;
    [SerializeField] private VoidEventChannelSO tugglePhaseSucceedSO;
    [SerializeField] private VoidEventChannelSO playerTakeDamageSO;

    private List<Image> healthImages;
    private int Health => healthImages.Count;

    private bool hasTakenDamage = false;

    private void Awake()
    {
        List<Image> heartList = new();

        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out Image image))
            {
                heartList.Add(image);
            }
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
        Destroy(healthImages[Health - 1].gameObject);
        healthImages.RemoveAt(Health - 1);

        if (!hasTakenDamage) hasTakenDamage = true;

        if (Health <= 0)
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
