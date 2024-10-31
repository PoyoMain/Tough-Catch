using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [Space(10)]
    [SerializeField] private StunMinigameSettings _easyStunMinigameOptions;
    [SerializeField] private StunMinigameSettings _mediumStunMinigameOptions;
    [SerializeField] private StunMinigameSettings _hardStunMinigameOptions;

    public bool GuideImages 
    {
        get => _guideImages; 
        set => _guideImages = value;
    }
    public bool ControlRumble
    {
        get => _controlRumble;
        set => _controlRumble = value;
    }
    public bool ControllerConnected => Gamepad.current != null;
    public AudioMixer MainAudioMixer => _mainAudioMixer;
    public Difficulty Difficulty
    {
        get => _difficulty;
        set => _difficulty = value;
    } 
    public FullScreenMode FullScreenMode
    {
        get => _fullscreenMode;
        set => _fullscreenMode = value;
    }
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
    public StunMinigameSettings StunMinigameOptions => Difficulty switch
    {
        Difficulty.Easy => _easyStunMinigameOptions,
        Difficulty.Medium => _mediumStunMinigameOptions,
        Difficulty.Hard => _hardStunMinigameOptions,
        _ => _mediumStunMinigameOptions
    };

    private const string MIXER_MASTER = "MasterVolume";
    private const string MIXER_MUSIC = "MusicVolume";
    private const string MIXER_SFX = "SFXVolume";
    private const string MIXER_AMBIENCE = "AmbienceVolume";

    public void SetDifficulty(Toggle toggle)
    {
        _difficulty = toggle.name switch
        {
            "Toggle_Easy" => Difficulty.Easy,
            "Toggle_Medium" => Difficulty.Medium,
            "Toggle_Hard" => Difficulty.Hard,
            _ => throw new NotImplementedException(),
        };
    }

    public void SetGuideImages()
    {
        _guideImages = !_guideImages;
    }

    public void SetControllerRumble()
    {
        _controlRumble = !_controlRumble;
    }

    

    public void SetMasterVolume(Slider slider)
    {
        _mainAudioMixer.SetFloat(MIXER_MASTER, slider.value);
    }

    public void SetMusicVolume(Slider slider)
    {
        _mainAudioMixer.SetFloat(MIXER_MUSIC, slider.value);
    }

    public void SetSFXVolume(Slider slider)
    {
        _mainAudioMixer.SetFloat(MIXER_SFX, slider.value);
    }

    public void SetAmbienceVolume(Slider slider)
    {
        _mainAudioMixer.SetFloat(MIXER_AMBIENCE, slider.value);
    }


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

    [Serializable]
    public struct StunMinigameSettings
    {
        public int numberOfButtonsForCombo;
        public float minigameCompleteTime;
    }

}

[Serializable]
public enum Difficulty
{
    Easy,
    Medium,
    Hard
}
