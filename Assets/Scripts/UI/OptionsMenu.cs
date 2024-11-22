using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] private OptionsSO options;

    [Header("Scene Objects")]
    [SerializeField] private Slider audioSlider_Master;
    [SerializeField] private Slider audioSlider_Music;
    [SerializeField] private Slider audioSlider_SFX;
    [SerializeField] private Slider audioSlider_Ambience;
    [Space(10)]
    [SerializeField] private Toggle checkbox_ControllerRumble;
    //[SerializeField] private Toggle checkbox_GuideImages;
    [Space(10)]
    [SerializeField] private Toggle toggle_EasyDifficulty;
    [SerializeField] private Toggle toggle_MediumDifficulty;
    [SerializeField] private Toggle toggle_HardDifficulty;

    private const string MIXER_MASTER = "MasterVolume";
    private const string MIXER_MUSIC = "MusicVolume";
    private const string MIXER_SFX = "SFXVolume";
    private const string MIXER_AMBIENCE = "AmbienceVolume";
    private const float VOLUME_MAX = 20;
    private const float VOLUME_MIN = -80f;

    private void Awake()
    {
        audioSlider_Master.maxValue = audioSlider_Music.maxValue = audioSlider_SFX.maxValue = audioSlider_Ambience.maxValue = VOLUME_MAX;
        audioSlider_Master.minValue = audioSlider_Music.minValue = audioSlider_SFX.minValue = audioSlider_Ambience.minValue = VOLUME_MIN;

        options.MainAudioMixer.GetFloat(MIXER_MASTER, out float volume);
        audioSlider_Master.value = volume;

        options.MainAudioMixer.GetFloat(MIXER_MUSIC, out volume);
        audioSlider_Music.value = volume;

        options.MainAudioMixer.GetFloat(MIXER_SFX, out volume);
        audioSlider_SFX.value = volume;

        options.MainAudioMixer.GetFloat(MIXER_AMBIENCE, out volume);
        audioSlider_Ambience.value = volume;

        checkbox_ControllerRumble.isOn = options.ControlRumble;
        //checkbox_GuideImages.isOn = options.GuideImages;

        switch (options.Difficulty)
        {
            case Difficulty.Easy:
                toggle_EasyDifficulty.isOn = true;
                break;
            case Difficulty.Medium:
                toggle_MediumDifficulty.isOn = true;
                break;
            case Difficulty.Hard:
                toggle_HardDifficulty.isOn = true;
                break;
        }
    }
}
