using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string mainMenuSceneName;

    [Header("Inspector Objects")]
    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _firstSelectButton;

    [Header("Broadcast Events")]
    [SerializeField] private VoidEventChannelSO _gameUnpaused;

    [Header("Listen Events")]
    [SerializeField] private VoidEventChannelSO _gamePaused;

    public bool IsPaused { get; private set; }
    public bool OnPauseCoolDown => pauseTimer > 0;

    private float pauseTimer;

    private void OnEnable()
    {
        _gamePaused.OnEventRaised += Pause;
        _gameUnpaused.OnEventRaised += Pause;
    }

    private void OnDisable()
    {
        _gamePaused.OnEventRaised -= Pause;
        _gameUnpaused.OnEventRaised -= Pause;
    }

    public void Pause()
    {
        if (OnPauseCoolDown) return;
        pauseTimer = 1;

        IsPaused = !IsPaused;
        _menu.SetActive(IsPaused);
        Time.timeScale = IsPaused ? 0 : 1;
        if (IsPaused) EventSystem.current.SetSelectedGameObject(_firstSelectButton);
        else _gameUnpaused.RaiseEvent();

        
    }

    public void Pause(InputAction.CallbackContext _)
    {
        IsPaused = !IsPaused;
        _menu.SetActive(IsPaused);
        Time.timeScale = IsPaused ? 0 : 1;
        if (IsPaused) EventSystem.current.SetSelectedGameObject(_firstSelectButton);
        else _gameUnpaused.RaiseEvent();
    }

    public void Quit()
    {
        IsPaused = false;
        _menu.SetActive(IsPaused);
        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void Update()
    {
        if (OnPauseCoolDown)
        {
            pauseTimer -= Time.unscaledDeltaTime;
        }
    }
}
