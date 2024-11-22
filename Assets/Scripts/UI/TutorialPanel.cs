using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialPanel : MonoBehaviour
{
    [Header("Inspector Objects")]
    [SerializeField] private InputReaderSO inputReader;


    [Header("Broadcast Events")]
    [SerializeField] private VoidEventChannelSO endEventSO;

    private bool active = false;

    private void OnEnable()
    {
        inputReader.Controls.Confirm.performed += Deactivate;

        Activate();
    }

    private void OnDisable()
    {
        inputReader.Controls.Confirm.performed -= Deactivate;
    }

    private void Activate()
    {
        if (active) return;
        active = true;

        Time.timeScale = 0;
    }

    private void Deactivate(InputAction.CallbackContext _)
    {
        if (!active) return;

        Time.timeScale = 1;

        endEventSO.RaiseEvent();

        this.enabled = false;
    }
}
