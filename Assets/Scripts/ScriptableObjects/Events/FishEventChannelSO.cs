using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewFishEvent", menuName = "Events/Fish Event")]
public class FishEventChannelSO : ScriptableObject
{
    public event UnityAction<Fish> OnEventRaised;
    public void RaiseEvent(Fish fish)
    {
        OnEventRaised?.Invoke(fish);
    }
}