using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string gameSceneName;

    private void Start()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);
    }

    public void GameStart()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void GameQuit()
    {
        Application.Quit();
    }
}
