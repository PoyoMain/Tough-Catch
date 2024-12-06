using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Leaderboards;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine.SocialPlatforms.Impl;

namespace Assets.Scripts
{
    public class LeaderBoard : MonoBehaviour
    {
        [Header("Number of Player Display")]
        [SerializeField] public int PlayerCount = 5;
        const string leaderboardId = "FishingLeaderboard";

        /*private async void Awake()
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }*/

        private async void Awake()
        {
            try
            {
                // Initialize Unity Services
                await UnityServices.InitializeAsync();

                // Sign in anonymously
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    Debug.Log("Player signed in anonymously");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to initialize Unity Services: {e.Message}");
            }
        }

        public async void AddScore()
        {
            var playerEntry = await LeaderboardsService.Instance
                .AddPlayerScoreAsync(leaderboardId, 102);
            Debug.Log(JsonConvert.SerializeObject(playerEntry));
        }
    }
}