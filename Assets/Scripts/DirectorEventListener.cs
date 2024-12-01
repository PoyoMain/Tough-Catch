using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DirectorEventListener : MonoBehaviour
{
    [Header("Director")]
    [SerializeField] private PlayableDirector director;

    [Header("Broadcast Events")]
    [SerializeField] private VoidEventChannelSO finishEvent;

    private void OnEnable()
    {
        director.stopped += Activate;
    }

    private void OnDisable()
    {
        director.stopped -= Activate;
    }

    private void Activate(PlayableDirector obj)
    {
        finishEvent.RaiseEvent();
    }
}
