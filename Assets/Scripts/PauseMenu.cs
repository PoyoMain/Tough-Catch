using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
        IsPaused = !IsPaused;
        menu.SetActive(IsPaused);
        Time.timeScale = IsPaused ? 0 : 1;
    }

    public void Pause(InputAction.CallbackContext context)
    {
        IsPaused = !IsPaused;
        menu.SetActive(IsPaused);
        Time.timeScale = IsPaused ? 0 : 1;
    }

    public void Quit()
    {
        //SceneManager.LoadScene(1);
    }
}
