using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;

    public bool IsPaused
    {
        get;
        private set;
    }

    public void Pause()
    {
        IsPaused = true;
        menu.SetActive(true);
        Time.timeScale = 0;
    }

    public void UnPause()
    {
        IsPaused = false;
        menu.SetActive(false);
        Time.timeScale = 1;
    }

    public void Quit()
    {
        //SceneManager.LoadScene(1);
    }
}
