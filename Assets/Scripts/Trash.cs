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
        get
        {
            return _sAnim.Container;
        }

        set
        {
            _sAnim.Container = value;
        }
    }

    public void SetTime(float time)
    {
        _sAnim.Duration = time;
    }

    private void Awake()
    {
        _sAnim = GetComponent<SplineAnimate>();
    }

    private void Update()
    {
        if (_sAnim.NormalizedTime >= 1f)
        {
            Destroy(gameObject);
            GameManager.Instance.TakeDamage();
        }
    }
}
