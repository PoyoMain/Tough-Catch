using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string gameSceneName;

    public void GameStart()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void GameQuit()
    {
        Application.Quit();
    }
}
