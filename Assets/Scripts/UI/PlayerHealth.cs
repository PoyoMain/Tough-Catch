using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Settings")]

    [Header("Broadcast Events")]
    [SerializeField] private VoidEventChannelSO gameOverSO;

    [Header("Listen Events")]
    [SerializeField] private VoidEventChannelSO tugglePhaseStartSO;
    [SerializeField] private VoidEventChannelSO playerTakeDamageSO;

    private List<Image> healthImages;
    private int Health => healthImages.Count;

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
    }

    private void OnDisable()
    {
        playerTakeDamageSO.OnEventRaised -= TakeDamage;
    }

    private void TakeDamage()
    {
        if (Health <= 0)
        {
            gameOverSO.RaiseEvent();
            return;
        }

        Destroy(healthImages[Health - 1].gameObject);
        healthImages.RemoveAt(Health - 1);
    }
}
