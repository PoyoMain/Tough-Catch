using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "NewOptions", menuName = "Options")]
public class ScriptableOptions : ScriptableObject
{
    [SerializeField] private Difficulty _difficulty;
    public Difficulty Difficulty => _difficulty;


    [SerializeField] private bool _guideImages;
    public bool GuideImages => _guideImages;


    [SerializeField] private bool _controlRumble;
    public bool ControlRumble => _controlRumble;


    [SerializeField] private FullScreenMode _fullscreenMode;
    public FullScreenMode FullScreenMode => _fullscreenMode;


    [SerializeField] private AudioMixer _mainAudioMixer;

    [Header("Settings")]
    [SerializeField] private TuggleOptions _easyTuggleMinigameOptions;
    [SerializeField] private TuggleOptions _mediumTuggleMinigameOptions;
    [SerializeField] private TuggleOptions _hardTuggleMinigameOptions;
    public TuggleOptions TuggleMinigameOptions => Difficulty switch
    {
        Difficulty.Easy => _easyTuggleMinigameOptions,
        Difficulty.Medium => _mediumTuggleMinigameOptions,
        Difficulty.Hard => _hardTuggleMinigameOptions,
        _ => _mediumTuggleMinigameOptions
    };

    [Serializable]
    public struct TuggleOptions
    {
        [Header("Laser Minigame Settings")]
        public float minSpawnTime;
        public float maxSpawnTime;
        public float laserCooldownTime;
        public float objectHitTime;
    }
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}
