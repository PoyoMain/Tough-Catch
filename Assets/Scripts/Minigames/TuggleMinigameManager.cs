using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class TuggleMinigameManager : MinigameBase
{
    [SerializeField] private bool _useOptionValues;

    [Header("Laser Minigame Settings")]
    [SerializeField] private float _minSpawnTime = 5f;
    [SerializeField] private float _maxSpawnTime = 15f;
    [SerializeField] private float _laserCooldownTime = 2f;
    [SerializeField] private float _objectHitTime = 3f;

    [Header("Splines")]
    [SerializeField] private SplineContainer _leftSpline;
    [SerializeField] private SplineContainer _rightSpline;

    [Header("Prefabs")]
    [SerializeField] private Trash _testPrefab;

    [Header("Broadcast Events")]
    [SerializeField] private VoidEventChannelSO _onTakeDamage;

    [Header("Listen Events")]
    [SerializeField] private VoidEventChannelSO _onTuggleSucceed;

    private float _leftSpawnTimer;
    private float _rightSpawnTimer;
    private float _leftCooldownTimer;
    private float _rightCooldownTimer;
    private Trash _leftObject;
    private Trash _rightObject;

    private float MinSpawnTime  
    {
        get
        {
            if (_useOptionValues) return Options.TuggleMinigameOptions.minSpawnTime;
            else return _minSpawnTime;
        }
    }
    private float MaxSpawnTime
    {
        get
        {
            if (_useOptionValues) return Options.TuggleMinigameOptions.maxSpawnTime;
            else return _maxSpawnTime;
        }
    }
    private float LaserCooldownTime
    {
        get
        {
            if (_useOptionValues) return Options.TuggleMinigameOptions.laserCooldownTime;
            else return _laserCooldownTime;
        }
    }
    private float ObjectHitTime
    {
        get
        {
            if (_useOptionValues) return Options.TuggleMinigameOptions.objectHitTime;
            else return _objectHitTime;
        }
    }
    private bool LeftLaserCanShoot => _leftCooldownTimer <= 0;
    private bool RightLaserCanShoot => _rightCooldownTimer <= 0;



    public override void OnEnable()
    {
        base.OnEnable();

        _onTuggleSucceed.OnEventRaised += DisableMinigame;

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
        if (_leftObject != null && _leftObject.Hit)
        {
            _onTakeDamage.RaiseEvent();
            Destroy(_leftObject.gameObject);
        }
        if (_rightObject != null && _rightObject.Hit)
        {
            _onTakeDamage.RaiseEvent();
            Destroy(_rightObject.gameObject);
        }

        _leftSpawnTimer -= Time.deltaTime;
        if (_leftSpawnTimer <= 0 && _leftObject == null)
        {
            SpawnTrash(Direction.Left);
            ResetSpawnTimer(Direction.Left);
        }

        _rightSpawnTimer -= Time.deltaTime;
        if (_rightSpawnTimer <= 0 && _rightObject == null)
        {
            SpawnTrash(Direction.Right);
            ResetSpawnTimer(Direction.Right);
        }

        if (!LeftLaserCanShoot) _leftCooldownTimer -= Time.deltaTime;
        if (!RightLaserCanShoot) _rightCooldownTimer -= Time.deltaTime;
    }

    private void ResetSpawnTimer(Direction dir)
    {
        if (dir == Direction.Left) _leftSpawnTimer = Random.Range(MinSpawnTime, MaxSpawnTime);
        else _rightSpawnTimer = Random.Range(MinSpawnTime, MaxSpawnTime);
    }

    private void SpawnTrash(Direction dir)
    {
        if (dir == Direction.Left)
        {
            _leftObject = Instantiate(_testPrefab);
            _leftObject.Spline = _leftSpline;
            _leftObject.SetTime(ObjectHitTime);
        }
        else
        {
            _rightObject = Instantiate(_testPrefab);
            _rightObject.Spline = _rightSpline;
            _rightObject.SetTime(ObjectHitTime);
        }
    }

    private void ShootLeft(InputAction.CallbackContext _)
    {
        if (!LeftLaserCanShoot) return;

        if (_leftObject != null) Destroy(_leftObject.gameObject);
        _leftCooldownTimer = LaserCooldownTime;
    }

    private void ShootRight(InputAction.CallbackContext _)
    {
        if (!RightLaserCanShoot) return; 

        if (_rightObject != null) Destroy(_rightObject.gameObject);
        _rightCooldownTimer = LaserCooldownTime;
    }

    #endregion

    private enum Direction
    {
        Left,
        Right
    }

    private void DisableMinigame()
    {
        this.enabled = false;
    }

    private void OnDisable()
    {
        Controls.LaserShootLeft.performed -= ShootLeft;
        Controls.LaserShootRight.performed -= ShootRight;

        _onTuggleSucceed.OnEventRaised -= DisableMinigame;

        if (_leftObject != null) Destroy(_leftObject.gameObject);
        if (_rightObject != null) Destroy(_rightObject.gameObject);
    }
}
