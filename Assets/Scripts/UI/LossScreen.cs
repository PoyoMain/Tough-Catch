using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LossScreen : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string mainMenuSceneName;
    [SerializeField] private string gameSceneName;

    [Header("Inspector Objects")]
    [SerializeField] private Image fishermanImage;

    [Header("ListenEvent")]
    [SerializeField] private VoidEventChannelSO GameLostEventSO;

    [Header("Lose Sprites")]
    [SerializeField] private Sprite[] loseSprites;

    private void OnEnable()
    {
        GameLostEventSO.OnEventRaised += InitializeLossScreen;
    }

    private void OnDisable()
    {
        GameLostEventSO.OnEventRaised -= InitializeLossScreen;
    }

    private void Start()
    {
        InitializeLossScreen();
    }

    private void InitializeLossScreen()
    {
        
        if (fishermanImage != null)
        {
            fishermanImage.sprite = loseSprites[Random.Range(0,loseSprites.Length - 1)];
        }
        else
        {
            print("Image not found");
        }
        
    }


    public void ReturnToHome()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
