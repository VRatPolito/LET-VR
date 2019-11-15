/*
 * Custom template by Gabriele P.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PrattiToolkit;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class FPSPatternSystem : MonoBehaviour
{
    #region Events

    public event Action OnBulletFinished, OnLastBulletExpired;

    #endregion

    #region Editor Visible


    [SerializeField] private bool _followPlayer = false;
    [SerializeField] [Range(0, 1)] private float _robotMoveSmoothing = 0.5f;

    [SerializeField] private TextMeshPro _hitsVisualizer;
    [SerializeField] private Transform _referenceTransform;
    [Expandable] [SerializeField] private FPSPatternSO _bulletPattern;
    [SerializeField] private GameObject _robot;
    [SerializeField] private Color _coolColor = Color.cyan, _hotColor = Color.red;

    #endregion

    #region Private Members and Constants

    private Vector3 _rp = Vector3.zero;

    private Transform _spawnPoint;
    private Renderer _aimRenderer;

    [SerializeField] private int _bulletIdx = 0;
    private bool _shooterReady = false;
    private Sequence _aimAndShootSequence;
    private int _hitsCounter = 0;
    private bool _bulletFinished = false;
    private  List<GameObject> _spawnedBullets = new List<GameObject>();

    #endregion

    #region Properties

    public int HitsCounter
    {
        get { return _hitsCounter; }

        set
        {
            _hitsCounter = value;
            if (_hitsVisualizer != null) _hitsVisualizer.text = $"Hits = {_hitsCounter:00}";
        }
    }

    #endregion

    #region MonoBehaviour

    private void OnEnable()
    {
        _bulletFinished = false;
        _bulletIdx = _hitsCounter = 0;
        var startup = DOTween.Sequence();
        startup.PrependInterval(1);
        startup.Append(_robot.transform.DOMoveY(1.7f, 4).SetEase(Ease.OutBack, 3));
        startup.Append(DOTween.To(() => _aimRenderer.sharedMaterial.GetColor("_TintColor"),
            x => _aimRenderer.sharedMaterial.SetColor("_TintColor", x), _coolColor, 3));
        startup.OnComplete(() =>
        {
            _shooterReady = true;
            _rp = _robot.transform.position;
        });
        startup.Play();
    }

    private void OnDisable()
    {
        DOTween.PauseAll();
        DOTween.KillAll();
        _shooterReady = false;
        _aimRenderer.sharedMaterial.SetColor("_TintColor", _coolColor);
    }

    void Awake()
    {
        _aimRenderer = _robot.transform.GetChildRecursive("Aim2").gameObject.GetComponent<Renderer>();
        _aimRenderer.sharedMaterial.SetColor("_TintColor", new Color32(10, 10, 10, 200));
        _spawnPoint = _robot.transform.GetChildRecursive("SpawnPoint");
        Assert.IsNotNull(_spawnPoint);
    }

    void Update()
    {
        LoadNextBullet();
        if (_shooterReady && _followPlayer)
            FollowPlayer();
        //_coolDownTimer += Time.deltaTime;
        //if (_coolDownTimer > 10)
        //{
        //    _coolDownTimer = 0;
        //    _idleSequence.Pause();
        //    _robot.transform.DOLookAt(_test.position,1);
        //}
    }

    #endregion

    #region Public Methods

    public void DestroySpawnedBullets()
    {
        foreach (var bullet in _spawnedBullets)
        {
            Destroy(bullet);
        }
        _spawnedBullets.Clear();
    }

    #endregion

    #region Helper Methods

    private void FollowPlayer()
    {
        _rp.y = LocomotionManager.Instance.CameraEye.position.y;
        _rp.z = LocomotionManager.Instance.CameraEye.position.z;
        _robot.transform.DOMove(_rp, _robotMoveSmoothing);
    }

    private void LoadNextBullet()
    {
        if (!_shooterReady || _bulletFinished) return;

        if (_bulletIdx == _bulletPattern.Bullets.Count)
        {
            _bulletIdx = 0;
            _bulletFinished = true;
            OnBulletFinished.RaiseEvent();
        }

        var bullet = _bulletPattern.Bullets[_bulletIdx];
        if (_aimAndShootSequence != null)
        {
            _aimAndShootSequence.Kill();
            _aimAndShootSequence = null;
        }

        _shooterReady = false;

        _aimAndShootSequence = DOTween.Sequence();
        _aimAndShootSequence.Append(_robot.transform.DOPunchRotation(new Vector3(5, 0, 0), bullet.LoadingTime, 20));
        _aimAndShootSequence.Append(_robot.transform.DOLookAt(bullet.GetTarget(_referenceTransform),
            bullet.AimingTime));
        _aimAndShootSequence.Join(DOTween.To(() => _aimRenderer.sharedMaterial.GetColor("_TintColor"),
            x => _aimRenderer.sharedMaterial.SetColor("_TintColor", x), _hotColor, bullet.AimingTime));
        _aimAndShootSequence.AppendCallback(Shoot);
        _aimAndShootSequence.Append(_robot.transform.DOPunchPosition(Vector3.left * 0.3f, bullet.CoolDownTime, 1,
            0.2f));
        _aimAndShootSequence.Join(DOTween.To(() => _aimRenderer.sharedMaterial.GetColor("_TintColor"),
            x => _aimRenderer.sharedMaterial.SetColor("_TintColor", x), _coolColor, bullet.CoolDownTime));
        _aimAndShootSequence.OnComplete(() => _shooterReady = true);
        _aimAndShootSequence.Play();
    }

    private void Shoot()
    {
        var bullet = _bulletPattern.Bullets[_bulletIdx];

        var bgo = Instantiate(
            _bulletPattern.BulletPrefab,
            _spawnPoint.position,
            _spawnPoint.rotation);

        _spawnedBullets.Add(bgo);
        var bulletCollision = bgo.GetComponent<CollisionDetect>();
        if (bulletCollision != null)
        {
            bulletCollision.OnHit += OnBulletCollisionHit;
        }

        // Add velocity to the bullet
        bgo.GetComponent<Rigidbody>().velocity =
            (bullet.GetTarget(_referenceTransform) - _spawnPoint.position).normalized * bullet.Speed;
        //bgo.GetComponent<Rigidbody>().AddForce((bullet.GetTarget(_referenceTransform) - _spawnPoint.position).normalized * bullet.Speed, ForceMode.VelocityChange);
        _bulletIdx++; //TODO Maybe Find Better Position
        // Destroy the bullet after 2 seconds
        //Destroy(b, 2.0f);
    }

    private void OnBulletCollisionHit(CollisionDetect collisionDetect, CollisionDetect.HitType hitType)
    {
        switch (hitType)
        {
            case CollisionDetect.HitType.Player:
                HitsCounter++;
                break;
            case CollisionDetect.HitType.NotPlayer:
                //
                break;
        }
        collisionDetect.ResetHitEventListener();
        if (_bulletFinished) OnLastBulletExpired.RaiseEvent();
    }


    #endregion

    #region Events Callbacks

    #endregion

    #region Coroutines

    #endregion
}