using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineAnimate))]
public class Trash : MonoBehaviour
{
    private SplineAnimate _sAnim;

    public SplineContainer Spline
    {
        get => _sAnim.Container;
        set => _sAnim.Container = value;
    }

    public bool Hit => _sAnim.NormalizedTime >= 1f;

    public void SetTime(float time)
    {
        _sAnim.Duration = time;
    }

    private void Awake()
    {
        _sAnim = GetComponent<SplineAnimate>();
    }
}
