using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class TuggleMinigameManager : MinigameBase
{
    [Header("Laser Minigame Settings")]
    [SerializeField] private float minSpawnTime = 5f;
    [SerializeField] private float maxSpawnTime = 15f;
    [SerializeField] private float laserCooldownTime = 2f;
    private float _leftSpawnTimer;
    private float _rightSpawnTimer;
    private float _leftCooldownTimer;
    private float _rightCooldownTimer;

    private bool LeftLaserCanShoot
    {
        get { return _leftCooldownTimer <= 0; }
    }

    private bool RightLaserCanShoot
    {
        get { return _rightCooldownTimer <= 0; }
    }

    [Space(5)]
    [SerializeField] private SplineContainer leftSpline;
    [SerializeField] private SplineContainer rightSpline;

    [Space(5)]
    [SerializeField] private Trash testPrefab;
    private Trash _leftObject;
    private Trash _rightObject;

    public override void OnEnable()
    {
        base.OnEnable();

        InitializeLaserMinigame();
    }

    private void InitializeLaserMinigame()
    {
        Controls.LaserShootLeft.performed += ShootLeft;
        Controls.LaserShootRight.performed += ShootRight;

        ResetSpawnTimer(Direction.Left);
        ResetSpawnTimer(Direction.Right);
    }

    private void Update()
    {
        CheckLaserTimers();
    }

    #region Laser Methods

    private void CheckLaserTimers()
    {
        _leftSpawnTimer -= Time.deltaTime;
        if (_leftSpawnTimer <= 0)
        {
            SpawnTrash(Direction.Left);
            ResetSpawnTimer(Direction.Left);
        }

        _rightSpawnTimer -= Time.deltaTime;
        if (_rightSpawnTimer <= 0)
        {
            SpawnTrash(Direction.Right);
            ResetSpawnTimer(Direction.Right);
        }

        if (!LeftLaserCanShoot) _leftCooldownTimer -= Time.deltaTime;
        if (!RightLaserCanShoot) _rightCooldownTimer -= Time.deltaTime;
    }

    private void ResetSpawnTimer(Direction dir)
    {
        if (dir == Direction.Left) _leftSpawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
        else _rightSpawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
    }

    private void SpawnTrash(Direction dir)
    {
        if (dir == Direction.Left)
        {
            _leftObject = Instantiate(testPrefab);
            _leftObject.Spline = leftSpline;
        }
        else
        {
            _rightObject = Instantiate(testPrefab);
            _rightObject.Spline = rightSpline;
        }
    }

    private void ShootLeft(InputAction.CallbackContext _)
    {
        if (!LeftLaserCanShoot) return;

        if (_leftObject != null) Destroy(_leftObject.gameObject);
        _leftCooldownTimer = laserCooldownTime;
    }

    private void ShootRight(InputAction.CallbackContext _)
    {
        if (!RightLaserCanShoot) return; 

        if (_rightObject != null) Destroy(_rightObject.gameObject);
        _rightCooldownTimer = laserCooldownTime;
    }

    #endregion

    private enum Direction
    {
        Left,
        Right
    }

    private void OnDisable()
    {
        Controls.LaserShootLeft.performed -= ShootLeft;
        Controls.LaserShootRight.performed -= ShootRight;

        if (_leftObject != null) Destroy(_leftObject.gameObject);
        if (_rightObject != null) Destroy(_rightObject.gameObject);
    }
}
