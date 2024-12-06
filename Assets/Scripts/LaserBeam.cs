using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LaserBeam : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float disappearTime = 5f;

    private float disappearTimer;
    private Vector3 targetPos;
    private Vector3 startPos;

    private float lerpTime;
    private float lerpTimer;


    private void OnEnable()
    {
        disappearTimer = disappearTime;
        lerpTime = disappearTime;
    }

    public void SetUp(Vector3 targetPosition)
    {
        targetPos = targetPosition;
        startPos = transform.position;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(startPos, targetPos, lerpTimer / lerpTime);

        if (lerpTimer < lerpTime) lerpTimer += Time.deltaTime;

        if (disappearTimer > 0)
        {
            disappearTimer -= Time.deltaTime;

            if (disappearTimer <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
