using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class FishHealthBar : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private bool testing;
    [Tooltip("Equivalent to the # of seconds the minigame lasts until success")]
    [SerializeField] private float testHealth = 300;

    [Header("Settings")]
    [SerializeField] private float stunGunDamageAmount = 30f;

    [Header("Broadcast Events")]
    [SerializeField] private VoidEventChannelSO tugglePhaseSuccessSO;

    [Header("Listen Events")]
    [SerializeField] private FishEventChannelSO fishFoundSO;
    [SerializeField] private VoidEventChannelSO tugglePhaseStartSO;
    [SerializeField] private VoidEventChannelSO fishTakeDamageSO;

    private float _fishCurrentHealth;
    private Slider _healthSlider;
    private Fish _currentFish;

    private void Awake()
    {
        TryGetComponent(out _healthSlider);
    }

    private void OnEnable()
    {
        fishFoundSO.OnEventRaised += SetData;
        tugglePhaseStartSO.OnEventRaised += InitiateHealthBar;
        fishTakeDamageSO.OnEventRaised += TakeDamage;
    }

    private void OnDisable()
    {
        fishFoundSO.OnEventRaised -= SetData;
        tugglePhaseStartSO.OnEventRaised -= InitiateHealthBar;
        fishTakeDamageSO.OnEventRaised -= TakeDamage;
    }

    private void SetData(Fish fish)
    {
        _currentFish = fish;
    }

    private void InitiateHealthBar()
    {
        if (testing) 
        { 
            _fishCurrentHealth = _healthSlider.value = _healthSlider.maxValue = testHealth; 
            return; 
        }

        _fishCurrentHealth = _healthSlider.value = _healthSlider.maxValue = _currentFish.Health;
    }

    private void Update()
    {
        if (_fishCurrentHealth > 0)
        {
            _fishCurrentHealth -= Time.deltaTime;
            _healthSlider.value = _fishCurrentHealth;

            if (_fishCurrentHealth <= 0)
            {
                tugglePhaseSuccessSO.RaiseEvent();
                this.enabled = false;
            }
        }
    }

    private void TakeDamage()
    {
        _fishCurrentHealth -= stunGunDamageAmount;
    }
}
