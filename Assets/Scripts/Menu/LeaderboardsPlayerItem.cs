using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine.UI;
using System;

public class LeaderboardsPlayerItem : MonoBehaviour
{

    [SerializeField] public TextMeshProUGUI dateText = null;
    [SerializeField] public TextMeshProUGUI nameText = null;
    [SerializeField] public TextMeshProUGUI weightText = null;
    [SerializeField] public TextMeshProUGUI timeText = null;
    [SerializeField] private Button selectButton = null;
    
    private LeaderboardEntry player = null;
    
    public void Initialize(LeaderboardEntry player)
    {
        int gameTime = 0, fishWeight = 0;
        DateTime dt = DateTime.Now;
        this.player = player;
        nameText.text = player.PlayerName;
        dateText.text = dt.ToString();
        weightText.text = fishWeight.ToString();
        timeText.text = gameTime.ToString();
    }
        
}