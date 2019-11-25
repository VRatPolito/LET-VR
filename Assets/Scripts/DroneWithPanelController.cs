/*
 * Custom template by F. Gabriele Pratticò {filippogabriele.prattico@polito.it}
 */

using DG.Tweening;
using PrattiToolkit;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

public class DroneWithPanelController : MonoBehaviour
{
    #region Events
    public UnityEvent PlayerInRange, PlayerOutRange;
    #endregion

    #region Editor Visible

    [SerializeField] [Range(0, 3)] private float _aheadOfPlayer = 1.5f;
    [SerializeField] [Range(0, 20)] private float _warningRadius = 5f;
    [SerializeField] private float _escapeSpeed = 3;
    [SerializeField] private float _rotateSpeed = 15;

    [SerializeField] private GameObject _dummyPlayer, _target;

    #endregion

    #region Private Members and Constants

    private GameObject _panel;
    private bool _inRange = false;
    private CharacterController _controller;
    private Vector3 _lastDir, _dir, _pDir, _rDir;
    private Quaternion _rot;
    private float _currSpeed = 0, _targetSpeed = 0, _boost = 1, _pGap;
    private object[] _playingCoreo;

    #endregion

    #region Properties

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

        PlayerInRange.AddListener(() =>
        {
            if (_playingCoreo != null) DOTween.KillAll();
            //.ForEach(o => DOTween.Kill(o));
            _playingCoreo = null;
        });
    }

    private void Start()
    {
        _playingCoreo = WaitForPlayerCoreo();
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

        if (_pGap < _aheadOfPlayer) //InCloseRange
        {
            _boost = (1 + Mathf.InverseLerp(_aheadOfPlayer, 0, _pGap)).Remap(1, 2, 1, _escapeSpeed);
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

    public Transform actualPlayer
    {
        get => LocomotionManager.Instance.CurrentPlayerController;
    }

    #endregion

    #region Helper Methods   

    private object[] WaitForPlayerCoreo()
    {
        var currPos = transform.position;
        var currBillRot = _panel.transform.rotation;
        var endCall = new TweenCallback(() =>
        {
            transform.position = currPos;
            _panel.transform.rotation = currBillRot;
        });

        var seq1 = DOTween.Sequence();
        var seq2 = DOTween.Sequence();
        seq1.Append(transform.DOMoveY(transform.position.y + 0.25f, 2.0f).SetEase(Ease.InOutSine));
        seq1.SetLoops(-1, LoopType.Yoyo);
        seq1.OnKill(endCall);

        seq2.Append(_panel.transform.DORotate(_panel.transform.eulerAngles + new Vector3(1, 0, 0) * 20, .75f).SetEase(Ease.InOutSine));
        seq2.SetLoops(-1, LoopType.Yoyo);
        seq2.OnKill(endCall);
        return new[] { seq1.id, seq2.id };
    }

    #endregion

    #region Events Callbacks

    #endregion

    #region Coroutines

    #endregion
}