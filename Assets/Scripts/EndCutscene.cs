using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCutscene : MonoBehaviour
{
    [Header("Inspector Object")]
    [SerializeField] private Transform spawnTransform;

    [Header("Listen Events")]
    [SerializeField] private FishEventChannelSO fishCaught;

    private Fish fish;

    private void OnEnable()
    {
        fishCaught.OnEventRaised += SetFish;
    }

    private void OnDisable()
    {
        fishCaught.OnEventRaised -= SetFish;
    }

    private void SetFish(Fish fishCaught)
    {
        fish = fishCaught;
    }

    public void SpawnFish()
    {
        GameObject fishObject = Instantiate(fish.Model, Vector3.zero, Quaternion.identity, spawnTransform);
        fishObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}
