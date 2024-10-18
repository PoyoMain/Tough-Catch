using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "NewOptions", menuName = "Options")]
public class OptionsSO : ScriptableObject
{
    [Header("General Settings")]
    [SerializeField] private Difficulty _difficulty;
    [SerializeField] private bool _guideImages;
    [SerializeField] private bool _tutorialPopUps;
    [SerializeField] private bool _controlRumble;
    [SerializeField] private FullScreenMode _fullscreenMode;
    [SerializeField] private AudioMixer _mainAudioMixer;

    [Header("Minigame Settings")]
    [SerializeField] private TuggleOptions _easyTuggleMinigameOptions;
    [SerializeField] private TuggleOptions _mediumTuggleMinigameOptions;
    [SerializeField] private TuggleOptions _hardTuggleMinigameOptions;

    public Difficulty Difficulty => _difficulty;    
    public bool GuideImages => _guideImages;
    public bool ControlRumble => _controlRumble;
    public FullScreenMode FullScreenMode => _fullscreenMode;
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
