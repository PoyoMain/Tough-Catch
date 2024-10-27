using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

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
    [SerializeField] private LaserMinigameSettings _easyLaserMinigameOptions;
    [SerializeField] private LaserMinigameSettings _mediumLaserMinigameOptions;
    [SerializeField] private LaserMinigameSettings _hardLaserMinigameOptions;
    [Space(10)]
    [SerializeField] private FishingRodMinigameSettings _easyFishingRodMinigameOptions;
    [SerializeField] private FishingRodMinigameSettings _mediumFishingRodMinigameOptions;
    [SerializeField] private FishingRodMinigameSettings _hardFishingRodMinigameOptions;

    public Difficulty Difficulty => _difficulty;    
    public bool GuideImages => _guideImages;
    public bool ControlRumble => _controlRumble;
    public FullScreenMode FullScreenMode => _fullscreenMode;

    public bool ControllerConnected => Gamepad.current != null;
    public LaserMinigameSettings LaserMinigameOptions => Difficulty switch
    {
        Difficulty.Easy => _easyLaserMinigameOptions,
        Difficulty.Medium => _mediumLaserMinigameOptions,
        Difficulty.Hard => _hardLaserMinigameOptions,
        _ => _mediumLaserMinigameOptions
    };

    public FishingRodMinigameSettings FishingRodMinigameOptions => Difficulty switch
    {
        Difficulty.Easy => _easyFishingRodMinigameOptions,
        Difficulty.Medium => _mediumFishingRodMinigameOptions,
        Difficulty.Hard => _hardFishingRodMinigameOptions,
        _ => _mediumFishingRodMinigameOptions
    };

    [Serializable]
    public struct LaserMinigameSettings
    {
        public float minSpawnTime;
        public float maxSpawnTime;
        public float laserHoldTime;
        public float objectHitTime;
    }

    [Serializable]
    public struct FishingRodMinigameSettings
    {
        public float minDriftTime;
        public float maxDriftTime;
        public float minigameHoldTime;
        public float minigameFailTime;
    }

}

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}
