using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _firstSelectButton;

    public bool IsPaused { get; private set; }

    public void Pause()
    {
        IsPaused = !IsPaused;
        _menu.SetActive(IsPaused);
        Time.timeScale = IsPaused ? 0 : 1;
        if (IsPaused) EventSystem.current.SetSelectedGameObject(_firstSelectButton);
    }

    public void Pause(InputAction.CallbackContext _)
    {
        IsPaused = !IsPaused;
        _menu.SetActive(IsPaused);
        Time.timeScale = IsPaused ? 0 : 1;
        if (IsPaused) EventSystem.current.SetSelectedGameObject(_firstSelectButton);
    }

    public void Quit()
    {
        //SceneManager.LoadScene(1);
    }
}
