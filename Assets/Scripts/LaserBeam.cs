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


    private void OnEnable()
    {
        disappearTimer = disappearTime;
    }

    public void SetUp(Vector3 targetPosition)
    {
        targetPos = targetPosition;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, disappearTime * Time.deltaTime);

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
