/*
 * Custom template by F. Gabriele Pratticò {filippogabriele.prattico@polito.it}
 */

using System;
using System.Collections;
using Boo.Lang;
using DG.Tweening;
using PrattiToolkit;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

public class DroneWithPanelController : MonoBehaviour
{
    [Serializable]
    struct TimeFactor
    {
        public float FromTime;
        public float Factor;
    }

    #region Events
    public UnityEvent PlayerInRange, PlayerOutRange;
    #endregion

    #region Editor Visible

    [SerializeField] [Range(0, 3)] private float _aheadOfPlayer = 1.5f;
    [SerializeField] [Range(0, 20)] private float _warningRadius = 5f;
    [SerializeField] private float _escapeSpeed = 3;
    [SerializeField] private float _rotateSpeed = 15;
    [SerializeField] private float _boostFactor = 3;

    [SerializeField] private GameObject _dummyPlayer, _target;
    [SerializeField] private TimeFactor[] _speedScalingFactorOnTimeElapsed;

    #endregion

    #region Private Members and Constants

    private GameObject _panel;
    private bool _inRange = false;
    private CharacterController _controller;
    private Vector3 _lastDir, _dir, _pDir, _rDir;
    private Quaternion _rot;
    private float _currSpeed = 0, _targetSpeed = 0, _targetAhead = 0, _boost = 1, _pGap;
    private Sequence[] _playingCoreo;
    private bool _shootdown = false;
    private float _startChasingTime = 0;

    #endregion

    #region Properties

    public Transform actualPlayer
    {
        get => LocomotionManager.Instance.CameraEye;
    }

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _panel = transform.GetChildRecursive("Panel").gameObject;
        Assert.IsNotNull(_panel);
        Assert.IsTrue(_aheadOfPlayer < _warningRadius);
        _lastDir = transform.forward;
        _rot = transform.rotation;
        _targetSpeed = _escapeSpeed;
        _targetAhead = (LocomotionManager.Instance.CalibrationData.ControllerDistance * 0.55f).Clamp(0.90f, 1.25f);
        Debug.Log($"Drone ahead of {_targetAhead}");


        PlayerInRange.AddListener(() =>
        {
            //if (_playingCoreo != null) DOTween.KillAll();
            _playingCoreo?.ForEach(s => s.Kill());
            _playingCoreo = null;
        });
    }

    private void Start()
    {
        StartCoroutine(HeightCalibration());
    }

    private void FixedUpdate()
    {
        if (_dummyPlayer != null && _target != null)
        {
            _dir = (_target.transform.position - _dummyPlayer.transform.position);
            if (_dummyPlayer.GetComponent<CharacterController>() != null)
            {
                _dummyPlayer.GetComponent<CharacterController>().SimpleMove(_dir);
            }
        }


        _pGap = Vector3.Distance(actualPlayer.position.vector3XZOnly(), transform.position.vector3XZOnly());
        _pDir = (transform.position - actualPlayer.position).normalized;
        _pDir.y = 0;

        if(_shootdown) return;

        CorrectSpeedFactor();

        if (_pGap < _targetAhead) //InCloseRange
        {
            _boost = (1 + Mathf.InverseLerp(_targetAhead, 0, _pGap)).Remap(1, 2, 1, _boostFactor);
            _currSpeed = Mathf.Lerp(_currSpeed, _targetSpeed, .5f);
            transform.Translate(_pDir * _boost * _currSpeed * Time.fixedDeltaTime, Space.World);
            _lastDir = _pDir;

            _rot = Quaternion.LookRotation(_lastDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, _rot, Time.fixedDeltaTime * _rotateSpeed);
            if (!_inRange)
            {
                _inRange = true;
                PlayerInRange?.Invoke();
            }

        }
        else if (_pGap < _warningRadius) //InRange
        {
            _currSpeed = Mathf.Lerp(_currSpeed, _targetSpeed, .5f);
            transform.Translate(_lastDir * _currSpeed * Time.fixedDeltaTime, Space.World);
            transform.rotation = Quaternion.Slerp(transform.rotation, _rot, Time.fixedDeltaTime * _rotateSpeed);
            if (!_inRange)
            {
                _inRange = true;
                PlayerInRange?.Invoke();
            }
        }
        else
        {
            _currSpeed = Mathf.Lerp(_currSpeed, 0, .01f);
            transform.Translate(_lastDir * _currSpeed * Time.fixedDeltaTime, Space.World);
            if (_inRange)
            {
                _inRange = false;
                PlayerOutRange?.Invoke();
            }
        }
    }

    #endregion

    #region Public Methods

    public void End()
    {
        var shutdownSequence = DOTween.Sequence();
        var seq2 = DOTween.Sequence();

        _shootdown = true;
        _pDir = (actualPlayer.position- transform.position).normalized;
        _pDir.y = 0;
        _pDir.Normalize();
        var look = actualPlayer.position;
        look.y = transform.position.y;

        shutdownSequence.Append(transform.DOLookAt(look, 1f));
        shutdownSequence.AppendCallback(() => seq2.Play());
        shutdownSequence.Append(this.transform.DOMove(transform.position + _pDir*100, 7).SetEase(Ease.InCubic));
        shutdownSequence.Join(transform.DOScale(Vector3.zero, 8).SetEase(Ease.InCubic));
        shutdownSequence.Play();

        seq2.Append(_panel.transform.DOLocalRotateQuaternion(_panel.transform.localRotation * Quaternion.AngleAxis(20, Vector3.right), .25f).SetEase(Ease.InOutSine));
        seq2.SetLoops(-1, LoopType.Yoyo);
        seq2.Pause();
    }

    #endregion

    #region Helper Methods   

    private void CorrectSpeedFactor()
    {
        if(_startChasingTime < 0.1f) return;

        foreach (var th in _speedScalingFactorOnTimeElapsed)
        {
            if ((Time.time - _startChasingTime) > th.FromTime)
            {
                _targetSpeed = _escapeSpeed * th.Factor;
                _targetAhead = (LocomotionManager.Instance.CalibrationData.ControllerDistance * 0.55f * th.Factor).Clamp(0.90f*th.Factor, 1.25f);
            }
        }
        
    }

    private Sequence[] WaitForPlayerCoreo()
    {
        var currPos = transform.position;
        var currBillRot = _panel.transform.rotation;
        var endCall = new TweenCallback(() =>
        {
            transform.position = currPos;
            _panel.transform.rotation = currBillRot;
            _startChasingTime = Time.deltaTime;
        });

        var seq1 = DOTween.Sequence();
        var seq2 = DOTween.Sequence();
        seq1.Append(transform.DOMoveY(transform.position.y + 0.25f, 2.0f).SetEase(Ease.InOutSine));
        seq1.SetLoops(-1, LoopType.Yoyo);
        seq1.OnKill(endCall);

        seq2.Append(_panel.transform.DOLocalRotateQuaternion(_panel.transform.localRotation * Quaternion.AngleAxis(20, Vector3.right), .75f).SetEase(Ease.InOutSine));
        seq2.SetLoops(-1, LoopType.Yoyo);
        seq2.OnKill(endCall);
        return new Sequence[] { seq1, seq2 };
    }

    #endregion

    #region Events Callbacks

    #endregion

    #region Coroutines

    private IEnumerator HeightCalibration()
    {
        yield return new WaitForFixedUpdate();

        var newPos = transform.position;
        float rayLength = 30;
        int layerMask = LayerMask.GetMask(new[] { "Default" });
        RaycastHit hitH;

        var ray = new Ray(this.transform.position, Vector3.down);
        Physics.Raycast(ray, out hitH, rayLength, layerMask);
        Debug.DrawRay(ray.origin, ray.direction, Color.red,5);

        newPos.y = hitH.point.y + (LocomotionManager.Instance.CalibrationData.HeadHeight * 0.8f).Clamp(0.5f, 1.8f) + (transform.position.y-_panel.transform.position.y);
        transform.position = newPos;

        Debug.Log($"Floor Y = {hitH.point.y}, Drone height From floor = {newPos.y - hitH.point.y}");

        yield return null;

        _playingCoreo = WaitForPlayerCoreo();
    }

    #endregion
}