using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewVoidEvent", menuName = "Events/Void Event")]
public class VoidEventChannelSO : ScriptableObject
{
    public event UnityAction OnEventRaised;
    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }
}
