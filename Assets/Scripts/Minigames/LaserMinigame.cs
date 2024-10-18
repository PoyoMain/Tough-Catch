using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class LaserMinigame : MinigameBase
{
    [Header("Settings")]
    [SerializeField] private float _activeTime = 20f;
    [SerializeField] private float _minSpawnTime = 5f;
    [SerializeField] private float _maxSpawnTime = 15f;
    [SerializeField] private float _laserCooldownTime = 2f;
    [SerializeField] private float _objectHitTime = 3f;

    [Header("Splines")]
    [SerializeField] private SplineContainer _leftSpline;
    [SerializeField] private SplineContainer _rightSpline;

    [Header("Laser Fields")]
    [SerializeField] private Transform _leftLaser;
    [SerializeField] private Transform _rightLaser;

    [Header("Prefabs")]
    [SerializeField] private Trash _testPrefab;
    [SerializeField] private LaserBeam _laserPrefab;

    [Header("Broadcast Events")]
    [SerializeField] private VoidEventChannelSO _damagePlayer;
    [SerializeField] private VoidEventChannelSO _laserLeftCharge;
    [SerializeField] private VoidEventChannelSO _laserRightCharge;
    [SerializeField] private VoidEventChannelSO _laserLeftDecharge;
    [SerializeField] private VoidEventChannelSO _laserRightDecharge;
    [SerializeField] private VoidEventChannelSO _laserLeftFire;
    [SerializeField] private VoidEventChannelSO _laserRightFire;
    [SerializeField] private VoidEventChannelSO _trashDestroyed;


    private float _activeTimer;
    private float _leftSpawnTimer;
    private float _rightSpawnTimer;
    private float _leftCooldownTimer;
    private float _rightCooldownTimer;
    private Trash _leftObject;
    private Trash _rightObject;
    private bool _leftLaserFired;
    private bool _rightLaserFired;
    private bool _leftLaserCharging;
    private bool _rightLaserCharging;

    private float MinSpawnTime
    {
        get
        {
            if (_useOptionValues) return Options.LaserMinigameOptions.minSpawnTime;
            else return _minSpawnTime;
        }
    }
    private float MaxSpawnTime
    {
        get
        {
            if (_useOptionValues) return Options.LaserMinigameOptions.maxSpawnTime;
            else return _maxSpawnTime;
        }
    }
    private float LaserCooldownTime
    {
        get
        {
            if (_useOptionValues) return Options.LaserMinigameOptions.laserCooldownTime;
            else return _laserCooldownTime;
        }
    }
    private float ObjectHitTime
    {
        get
        {
            if (_useOptionValues) return Options.LaserMinigameOptions.objectHitTime;
            else return _objectHitTime;
        }
    }
    private bool LeftLaserCanShoot => _leftCooldownTimer <= 0;
    private bool RightLaserCanShoot => _rightCooldownTimer <= 0;

    private void Start()
    {
        Controls.LaserShootLeft.started += ChargeLeft;
        Controls.LaserShootLeft.performed += ShootLeft;
        Controls.LaserShootLeft.canceled += DechargeLeft;
        Controls.LaserShootRight.started += ChargeRight;
        Controls.LaserShootRight.performed += ShootRight;
        Controls.LaserShootRight.canceled += DechargeRight;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        _activeTimer = _activeTime;

        

        ResetSpawnTimer(Direction.Left);
        ResetSpawnTimer(Direction.Right);
    }

    private void OnDisable()
    {
        //Controls.LaserShootLeft.started -= ChargeLeft;
        //Controls.LaserShootLeft.performed -= ShootLeft;
        //Controls.LaserShootLeft.canceled -= DechargeLeft;
        //Controls.LaserShootRight.started -= ChargeRight;
        //Controls.LaserShootRight.performed -= ShootRight;
        //Controls.LaserShootRight.canceled -= DechargeRight;

        if (_leftObject != null) Destroy(_leftObject.gameObject);
        if (_rightObject != null) Destroy(_rightObject.gameObject);
    }

    private void OnDestroy()
    {
        Controls.LaserShootLeft.started -= ChargeLeft;
        Controls.LaserShootLeft.performed -= ShootLeft;
        Controls.LaserShootLeft.canceled -= DechargeLeft;
        Controls.LaserShootRight.started -= ChargeRight;
        Controls.LaserShootRight.performed -= ShootRight;
        Controls.LaserShootRight.canceled -= DechargeRight;
    }

    private void Update()
    {
        if (isPaused) return;

        if (_leftObject == null && _rightObject == null && _activeTimer <= 0)
        {
            _minigameSuccess.RaiseEvent();
            this.enabled = false;
        }
        CheckTimers();
    }

    private void CheckTimers()
    {
        if (_activeTimer > 0) _activeTimer -= Time.deltaTime;

        if (_leftObject != null && _leftObject.Hit)
        {
            _damagePlayer.RaiseEvent();
            Destroy(_leftObject.gameObject);
        }
        if (_rightObject != null && _rightObject.Hit)
        {
            _damagePlayer.RaiseEvent();
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

    private void ChargeLeft(InputAction.CallbackContext _)
    {
        if (!LeftLaserCanShoot) return;

        _leftLaserCharging = true;
        _laserLeftCharge.RaiseEvent();
    }
    private void ChargeRight(InputAction.CallbackContext _)
    {
        if (!RightLaserCanShoot) return;

        _rightLaserCharging = true;
        _laserRightCharge.RaiseEvent();
    }

    private void ShootLeft(InputAction.CallbackContext _)
    {
        if (!_leftLaserCharging) return;
        if (!LeftLaserCanShoot) return;

        _leftCooldownTimer = LaserCooldownTime;
        _leftLaserCharging = false;
        _leftLaserFired = true;

        Vector3 target = _leftObject != null ? _leftObject.transform.position : _leftSpline.transform.position;
        Vector3 dir = target - _leftLaser.position;
        Quaternion lookDir = Quaternion.LookRotation(dir);
        LaserBeam laserBeam = Instantiate(_laserPrefab, _leftLaser.position, lookDir);

        laserBeam.SetUp(target);

        _laserLeftFire.RaiseEvent();
        if (_leftObject != null) _trashDestroyed.RaiseEvent();

        Invoke(nameof(DestroyLeftObject), 0.2f);
    }    

    private void ShootRight(InputAction.CallbackContext _)
    {
        if (!_rightLaserCharging) return;
        if (!RightLaserCanShoot) return;

        _rightCooldownTimer = LaserCooldownTime;
        _rightLaserCharging = false;
        _rightLaserFired = true;


        Vector3 target = _rightObject != null ? _rightObject.transform.position : _rightSpline.transform.position;
        Vector3 dir = target - _rightLaser.position;
        Quaternion lookDir = Quaternion.LookRotation(dir);
        LaserBeam laserBeam = Instantiate(_laserPrefab, _rightLaser.position, lookDir);

        laserBeam.SetUp(target);

        _laserRightFire.RaiseEvent();
        if (_rightObject != null) _trashDestroyed.RaiseEvent();

        Invoke(nameof(DestroyRightObject), 0.4f);
    }
    private void DestroyLeftObject()
    {
        if (_leftObject != null)
        {
            _leftObject.Destroy();
            Destroy(_leftObject.gameObject);
        }
    }

    private void DestroyRightObject()
    {
        if (_rightObject != null)
        {
            _rightObject.Destroy();
            Destroy(_rightObject.gameObject);
        }
    }

    private void DechargeLeft(InputAction.CallbackContext _)
    {
        if (!_leftLaserCharging) return;

        if (!_leftLaserFired) _laserLeftDecharge.RaiseEvent();
        else _leftLaserFired = false;
    }

    private void DechargeRight(InputAction.CallbackContext _)
    {
        if (!_rightLaserCharging) return;

        if (!_rightLaserFired) _laserRightDecharge.RaiseEvent();
        else _rightLaserFired = false;
    }

    
}

public enum Direction
{
    Left,
    Right
}
