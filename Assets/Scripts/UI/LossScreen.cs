using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LossScreen : MonoBehaviour
{
    [Header("Images")]
    [SerializeField] private Image fishermanImage;

    [Header("ListenEvent")]
    [SerializeField] private FishEventChannelSO GameLostEventSO;

    [Header("Fisherman Image")]
    [SerializeField] private Sprite fisherMan;

    private void OnEnable()
    {
        GameLostEventSO.OnEventRaised += InitializeLossScreen;
    }

    private void OnDisable()
    {
        GameLostEventSO.OnEventRaised -= InitializeLossScreen;
    }

    private void InitializeLossScreen(Fish fish)
    {
        
        if (fishermanImage != null)
        {
            fishermanImage.sprite = fisherMan;
        }
        else
        {
            print("Image not found");
        }
        
    }


    public void ReturnToHome()
    {
        SceneManager.LoadScene("AndresScene");
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        print("The Button is working");
    }
}
