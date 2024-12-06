using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Leaderboards;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Assets.Scripts;
using System.Runtime.CompilerServices;
using UnityEngine.UI;


public class LeaderBoard : MonoBehaviour
{
    public GameObject[] entry = new GameObject[5];

    private Text entryText;

    private bool isInitialized = false;
    const string leaderboardId = "Fish_Leaderboard";

    private float weight, time;
    private string playerName, date;

    private void start()
    {
        
    }

    private async void Awake()
    {
        // Initialize Unity Services
        await UnityServices.InitializeAsync();

        // Sign in anonymously
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Player signed in anonymously");
            isInitialized = true;
        }
    }

    public async void AddScore()
    {
        var playerEntry = await LeaderboardsService.Instance
            .AddPlayerScoreAsync(leaderboardId, 102);
        Debug.Log(JsonConvert.SerializeObject(playerEntry));
    }

    public async void GetScore()
    {



     }

    private async void Start()
    {
        while (!isInitialized)
        {
            await Task.Delay(100);
        }
        
    }
}
