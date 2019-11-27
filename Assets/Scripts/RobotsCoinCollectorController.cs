
/*
 * Custom template by Gabriele P.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BansheeGz.BGSpline.Curve;
using DG.Tweening;
using PrattiToolkit;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Assertions;

public class RobotsCoinCollectorController : MonoBehaviour
{
    #region Events

    #endregion

    #region Editor Visible

    [SerializeField] private Transform _from, _to;
    [SerializeField] private Transform _leftCoinRow, _rightCoinRow;
    [SerializeField] private Rigidbody _leftRobot, _rightRobot;
    [SerializeField] private TextMeshPro _coinsCollectedVisualizerText;
    [SerializeField] [Range(0, 3)] private float _aheadOfPlayer = 1;
    [SerializeField] [Range(0, 1)] private float _robotMoveSmoothing = 0.3f;

    #endregion

    #region Private Members and Constants

    private int _maxCoins = 0;
    private int _score = 0;
    private float _calibratedControllerDistance = 2;
    private Vector3 _movingDirection;
    private Vector3 _lastLeftRobotPosition;
    private Vector3 _lastRightRobotPosition;
    private Vector3 _lastPlayerPosition;
    private Tuple<Vector3, Vector3> _robotPositionLimit;

    private List<AudioSource> _coinAudioSource, _idleAudioSource;

    private Sequence _introducingSequence, _outroSequence;
    private Coroutine _collectorCor;

    #endregion

    #region Properties

    public float ControllerDistance { get; private set; }

    public int Score
    {
        get { return _score; }
        private set
        {
            _score = value;
            _coinsCollectedVisualizerText.text = $"Coins Collected\n{_score:###0}/{_maxCoins:###0}";
        }
    }

    public bool IsCollecting { get; private set; } = false;

    protected float NormalizedControllerDistance => Mathf.InverseLerp(0, _calibratedControllerDistance, ControllerDistance);

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        Assert.IsNotNull(_leftRobot);
        Assert.IsNotNull(_rightRobot);
        Assert.IsNotNull(_from);
        Assert.IsNotNull(_to);
        Assert.IsNotNull(_leftCoinRow);
        Assert.IsNotNull(_rightCoinRow);
        Assert.IsNotNull(_to);
        Assert.IsNotNull(_coinsCollectedVisualizerText);
        IsCollecting = false;

        _calibratedControllerDistance = LocomotionManager.Instance.CalibrationData.ControllerDistance;
        foreach (var cel in GetComponentsInChildren<ColliderEventsListener>())
            cel.OnTriggerEnterAction += Collect;

        _movingDirection = (_to.position - _from.position).normalized;

        _lastLeftRobotPosition = _leftRobot.position;
        _lastRightRobotPosition = _rightRobot.position;

        _coinAudioSource = new List<AudioSource>();
        _idleAudioSource = new List<AudioSource>();

        var audioS = _leftRobot.GetComponents<AudioSource>();
        _coinAudioSource.Add(audioS[0]);
        _idleAudioSource.Add(audioS[1]);

        audioS = _rightRobot.GetComponents<AudioSource>();
        _coinAudioSource.Add(audioS[0]);
        _idleAudioSource.Add(audioS[1]);

        _maxCoins = _leftCoinRow.transform.childCount + _rightCoinRow.transform.childCount;
        Score = 0;
        SetupCoreo();
    }


    #endregion

    #region Public Methods

    public void Introduce()
    {
        _introducingSequence.Play();
    }

    public void StartCollecting()
    {
        _introducingSequence.Kill();
        if (_collectorCor == null)
            _collectorCor = StartCoroutine(CollectorCoroutine());
    }

    public void Outro()
    {
        IsCollecting = false;
        StopCoroutine(CollectorCoroutine());
        _collectorCor = null;
        _outroSequence.Play();
    }

    #endregion

    #region Helper Methods

    private void SetupCoreo()
    {
        _introducingSequence = DOTween.Sequence();
        _introducingSequence.OnComplete(() =>
        {
            IsCollecting = true;
            if (_collectorCor == null)
                _collectorCor = StartCoroutine(CollectorCoroutine());
        });
        _introducingSequence.OnKill(() =>
        {
            IsCollecting = true;
            if (_collectorCor == null)
                _collectorCor = StartCoroutine(CollectorCoroutine());
        });
        _introducingSequence.AppendCallback(() => _idleAudioSource.ForEach(source => source.Play()));
        _introducingSequence.AppendInterval(.5f);
        _introducingSequence.Append(_leftRobot.transform.DOShakeRotation(2));
        _introducingSequence.Join(_rightRobot.transform.DOShakeRotation(2));
        _introducingSequence.Append(_leftRobot.transform.DOLocalRotate(new Vector3(0, -90, 0), 1.5f));
        _introducingSequence.Join(_rightRobot.transform.DOLocalRotate(new Vector3(0, -90, 0), 1.5f));
        _introducingSequence.AppendInterval(1.5f);
        _introducingSequence.Append(_leftRobot.transform.DOLocalRotate(new Vector3(0, 90, 0), .8f));
        _introducingSequence.Join(_rightRobot.transform.DOLocalRotate(new Vector3(0, 90, 0), .8f));
        _introducingSequence.Pause();

        _outroSequence = DOTween.Sequence();
        _outroSequence.AppendInterval(.5f);
        _outroSequence.Append(_leftRobot.DOMoveZ(22, 2));
        _outroSequence.Join(_rightRobot.DOMoveZ(17, 2));
        _outroSequence.Append(_leftRobot.transform.DOLocalRotate(new Vector3(0, 180, 0), 1));
        _outroSequence.Join(_rightRobot.transform.DOLocalRotate(new Vector3(0, 0, 0), 1));
        _outroSequence.AppendCallback(() => _idleAudioSource.ForEach(source => source.Stop()));
        _outroSequence.Pause();
    }

    #endregion

    #region Events Callbacks

    private void Collect(Collider coin)
    {
        if (coin.tag != "Collectable") return;
        GameObjectEntity otherGameObjectEntity = coin.GetComponent<GameObjectEntity>();
        if (!otherGameObjectEntity) return;

        Score++;
        Destroy(coin.gameObject);
        Debug.Log($"collect {coin.gameObject.name}");
        _coinAudioSource.ForEach(source => source.PlayOneShot(source.clip, 0.6f));
        //var entityManager = World.Active.GetExistingManager<EntityManager>();
        //entityManager.DestroyEntity(otherGameObjectEntity.Entity);
    }

    #endregion

    #region Coroutines

    IEnumerator CollectorCoroutine()
    {
        while (!IsCollecting || _introducingSequence.IsPlaying()) yield return null;

        yield return new WaitForFixedUpdate();

        _leftRobot.position = _lastLeftRobotPosition;
        _rightRobot.position = _lastRightRobotPosition;
        _leftRobot.rotation = _rightRobot.rotation = Quaternion.Euler(new Vector3(0, 90, 0));

        yield return new WaitForFixedUpdate();

        float rayLength = 30;
        int layerMask = LayerMask.GetMask(new[] { "Default" });
        RaycastHit hitL, hitR;

        var ray = new Ray(_leftRobot.transform.position, _leftRobot.transform.right * -1);
        Physics.Raycast(ray, out hitL, rayLength, layerMask);
        //Debug.DrawRay(ray.origin, ray.direction, Color.red);

        ray = new Ray(_rightRobot.transform.position, _rightRobot.transform.right);
        Physics.Raycast(ray, out hitR, rayLength, layerMask);
        //Debug.DrawRay(ray.origin, ray.direction, Color.blue);
        _robotPositionLimit = new Tuple<Vector3, Vector3>(hitL.point, hitR.point);
        Debug.Log(_robotPositionLimit);

        Vector3 currPos, p, lp, rp;
        float leftControllerDistance, rightControllerDistance, fromD, dzL, dzR;

        while (IsCollecting)
        {
            currPos = LocomotionManager.Instance.CurrentPlayerController.transform.position;
            leftControllerDistance = Mathf.Abs(LocomotionManager.Instance.LeftController.localPosition.x -
                                                   LocomotionManager.Instance.CameraEye.localPosition.x);
            rightControllerDistance = Mathf.Abs(LocomotionManager.Instance.RightController.localPosition.x -
                                                    LocomotionManager.Instance.CameraEye.localPosition.x);

            if (Vector3.Dot(currPos - _lastPlayerPosition, _movingDirection) >= 0)
                _lastPlayerPosition = LocomotionManager.Instance.CurrentPlayerController.transform.position;

            //robot can only move forward
            p = _lastPlayerPosition;
            p += _movingDirection * _aheadOfPlayer;
            fromD = Vector3.Dot(_from.position - p, _movingDirection);
            if (fromD > 0)
                p = _from.position + _aheadOfPlayer * _movingDirection;
            lp = p;
            rp = p;
            dzL = Mathf.Lerp(.75f, _leftCoinRow.position.z - _from.position.z,
                Mathf.InverseLerp(0, _calibratedControllerDistance / 2, leftControllerDistance));
            dzR = Mathf.Lerp(.75f, _from.position.z - _rightCoinRow.position.z,
                Mathf.InverseLerp(0, _calibratedControllerDistance / 2, rightControllerDistance));

            if (Vector3.Dot(lp - _lastLeftRobotPosition, _movingDirection) < 0)
                lp = p;
            if (Vector3.Dot(rp - _lastRightRobotPosition, _movingDirection) < 0)
                rp = p;

            lp.z = Mathf.Min(lp.z + dzL, _robotPositionLimit.Item1.z - 0.65f);
            rp.z = Mathf.Max(rp.z - dzR, _robotPositionLimit.Item2.z + 0.65f);

            _leftRobot.DOMove(lp, _robotMoveSmoothing);
            _rightRobot.DOMove(rp, _robotMoveSmoothing);

            _lastLeftRobotPosition = _leftRobot.position;
            _lastRightRobotPosition = _rightRobot.position;

            yield return null;
        }
    }

    #endregion

}
