using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewFloatEvent", menuName = "Events/Float Event")]
public class FloatEventChannelSO : ScriptableObject
{
    public event UnityAction<float> OnEventRaised;
    public void RaiseEvent(float floatValue)
    {
        OnEventRaised?.Invoke(floatValue);
    }
}
