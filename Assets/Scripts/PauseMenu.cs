using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject firstSelectButton;

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
        if (IsPaused) EventSystem.current.SetSelectedGameObject(firstSelectButton);
    }

    public void Pause(InputAction.CallbackContext _)
    {
        IsPaused = !IsPaused;
        menu.SetActive(IsPaused);
        Time.timeScale = IsPaused ? 0 : 1;
        if (IsPaused) EventSystem.current.SetSelectedGameObject(firstSelectButton);
    }

    public void Quit()
    {
        //SceneManager.LoadScene(1);
    }
}
