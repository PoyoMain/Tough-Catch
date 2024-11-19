using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VoidEventBroadcaster : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO _channel = default;
    
    public void Broadcast()
    {
        _channel.RaiseEvent();
    }
}
