using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Leaderboards;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine.SocialPlatforms.Impl;

public class LeaderBoard : MonoBehaviour
{
    const string leaderboardId = "FishingLeaderboard";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void AddScore()
    {
        var playerEntry = await LeaderboardsService.Instance
            .AddPlayerScoreAsync(leaderboardId, 102);
        Debug.Log(JsonConvert.SerializeObject(playerEntry));
    }
}
