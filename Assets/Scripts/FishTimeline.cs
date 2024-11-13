using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class FishTimeline : MonoBehaviour
{
    [SerializeField] PlayableDirector director;
    [SerializeField] float minTime;
    [SerializeField] float maxTime;

    void Start()
    {
        director.stopped += delegate { StartCoroutine(PlayTimeline()); };
        StartCoroutine(PlayTimeline());
    }

    IEnumerator PlayTimeline()
    {

        yield return new WaitForSeconds(Random.Range(minTime, maxTime));

        director.Play();

    }
}
