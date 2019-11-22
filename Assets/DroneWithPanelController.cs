/*
 * Custom template by F. Gabriele Pratticò {filippogabriele.prattico@polito.it}
 */

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PrattiToolkit;
using UnityEngine;
using UnityEngine.Assertions;
using Valve.VR;
using Random = UnityEngine.Random;

public class DroneWithPanelController : MonoBehaviour
{
    public enum DroneStatus : byte
    {
        NONE,
        IDLE,
        WANDER,
        ESCAPE
    }

    #region Events

    #endregion

    #region Editor Visible

    [SerializeField] [Range(0, 3)] private float _aheadOfPlayer = 1.5f;
    [SerializeField] [Range(0.05f, 1)] private float _playerMovementThreshold = 0.1f;
    [SerializeField] [Range(0, 1)] private float _playerJitterSmooth = 0.95f;
    [SerializeField] [Range(0, 90)] private float _maxPlayerManouverAngle = 50f;
    [SerializeField] [Range(0, 1)] private float _robotMoveSmoothing = 0.3f;
    [SerializeField] [Range(0, 4)] private float _recoverySpeed = 1f;
    [SerializeField] [Range(0, 2)] private float _wanderingSpeed = 2f;
    [SerializeField] [Range(0, 360)] private float _maxWanderingAngleVariation = 30f;
    [SerializeField] [Range(0, 20)] private float _wanderingArea = 5f;
    [SerializeField] [Range(0, 20)] private float _warningRadius = 5f;

    [SerializeField] private GameObject _dummyPlayer, _target;

    #endregion

    #region Private Members and Constants

    private float _calibratedControllerDistance = 2;
    private CharacterController _controller;
    private MIPanelController _panelController;

    private Vector3 _dir, _startPos;
    private DroneStatus _prevState = DroneStatus.NONE, _currentState = DroneStatus.IDLE;

    #endregion

    #region Properties

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _calibratedControllerDistance = LocomotionManager.Instance.CalibrationData.ControllerDistance;
        _panelController = GetComponentInChildren<MIPanelController>();
        _controller = GetComponent<CharacterController>();
        Assert.IsNotNull(_panelController);
        Assert.IsNotNull(_controller);
        Assert.IsTrue(_warningRadius > _aheadOfPlayer);
        _startPos = transform.position;

      
    }

    void Update()
    {
        if (_dummyPlayer != null && _target != null)
        {
            _dir = (_target.transform.position - _dummyPlayer.transform.position);
            _dummyPlayer.GetComponent<CharacterController>().SimpleMove(_dir);
        }

        UpdateStatus();
    }

    #endregion

    #region Public Methods

    public bool PlayerInRange()
    {
        return (actualPlayer.position - transform.position).magnitude <
               Mathf.Max(_aheadOfPlayer, _warningRadius); //warning
    }

    private bool PlayerInCloseRange()
    {
        return (actualPlayer.position - transform.position).magnitude <
               Mathf.Lerp(_aheadOfPlayer, _warningRadius, 0.2f); //ahead
    }

    public Transform actualPlayer
    {
        get => LocomotionManager.Instance.CurrentPlayerController;
    }

    #endregion

    #region Helper Methods

    protected void UpdateStatus()
    {
        if (PlayerInRange())
        {
            transitionTo(DroneStatus.ESCAPE);
        }
        else
        {
            transitionTo(DroneStatus.WANDER);
        }
    }

    protected void transitionTo(DroneStatus nextState)
    {
        if (nextState == _currentState)
        {
            return;
        }

        _prevState = _currentState;
        _currentState = nextState;
        OnStatusExit();
        OnStatusEnter();
    }

    private void OnStatusExit()
    {
        switch (_prevState)
        {
            case DroneStatus.NONE:
                break;
            case DroneStatus.IDLE:
                break;
            case DroneStatus.WANDER:
                transform.DOKill();
                break;
            case DroneStatus.ESCAPE:
                transform.DOKill();
                break;
        }
    }

    private void OnStatusEnter()
    {
        switch (_currentState)
        {
            case DroneStatus.NONE:
                break;
            case DroneStatus.IDLE:
                break;
            case DroneStatus.WANDER:
                StartCoroutine(Wander());
                break;
            case DroneStatus.ESCAPE:
                StartCoroutine(Escape());
                break;
        }
    }

    #endregion

    #region Events Callbacks

    #endregion

    #region Coroutines

    private IEnumerator Wander()
    {
        float heading = transform.rotation.y;
        float wanderSpeed = _wanderingSpeed;
        Vector3 targetRotation = transform.rotation.eulerAngles;
        float headingTimeout = Time.time - 1;
        float areaTimeout = Time.time - 1;

        while (_currentState == DroneStatus.WANDER)
        {
            if (Time.time > areaTimeout && (this.transform.position - _startPos).magnitude > _wanderingArea)
            {
                areaTimeout = Time.time + 2;
                headingTimeout = Time.time + Random.Range(2f, 4f);
                targetRotation = _startPos;
                targetRotation.y = transform.position.y;
                transform.DOLookAt(targetRotation, 1);
            }

            else if (Time.time > headingTimeout)
            {
                var floor = Mathf.Clamp(heading - _maxWanderingAngleVariation, 0, 360);
                var ceil = Mathf.Clamp(heading + _maxWanderingAngleVariation, 0, 360);
                heading = Random.Range(floor, ceil);
                wanderSpeed = Random.Range(wanderSpeed / 2, wanderSpeed);
                targetRotation = new Vector3(0, heading, 0);

                headingTimeout = Time.time + Random.Range(0.5f, 4f);
                transform.DOBlendableRotateBy(targetRotation - transform.eulerAngles,
                        Mathf.Max((targetRotation.y - transform.eulerAngles.y) / 5, headingTimeout - Time.deltaTime))
                    .SetEase(Ease.InOutQuad);
            }


            //transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation,
            //    Time.deltaTime * _robotMoveSmoothing);
            //var forward = transform.TransformDirection(Vector3.forward);
            transform.DOBlendableMoveBy(transform.forward * wanderSpeed * Time.fixedDeltaTime, _robotMoveSmoothing);
            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    private IEnumerator Escape()
    {
        float smooth = _robotMoveSmoothing;
        var pDeltaP = Vector3.zero;
        var tPos = transform.position;
        var tRot = transform.eulerAngles;
        var tForward = Quaternion.LookRotation(transform.forward, Vector3.up);
        var pDir = transform.forward;
        var lastPlayerPos = actualPlayer.position;

        while (_currentState == DroneStatus.ESCAPE)
        {
            pDeltaP = actualPlayer.position - lastPlayerPos;
            if (pDeltaP.magnitude > _playerMovementThreshold / 2)
            {
                pDir = pDeltaP;
                pDir.y = 0;
                pDir.Normalize();
                tForward = Quaternion.LookRotation(pDir, Vector3.up);
            }
            else
            {
                // ----> pDir = pDir;
                tPos = transform.position + transform.forward * _recoverySpeed * _wanderingSpeed * Time.fixedDeltaTime;
                tPos.y = transform.position.y;
                tForward = Quaternion.LookRotation(transform.forward, Vector3.up);
            }

            //if (Mathf.Acos(Vector3.Dot(pDir, transform.forward)) > Mathf.Deg2Rad * _maxPlayerManouverAngle ||
            //    pDeltaP.magnitude < _playerMovementThreshold)
            //{
            //    tPos = transform.position + transform.forward * _recoverySpeed;
            //    tPos.y = transform.position.y;
            //}

            //if (inRange && Vector3.Dot(actualPlayer.position + pDir * _aheadOfPlayer - transform.position, transform.forward) <= 0)

            //else
            if (PlayerInCloseRange())
            {
                tPos = Vector3.Lerp(lastPlayerPos, actualPlayer.position, _playerJitterSmooth) + pDir * _aheadOfPlayer;
                tPos.y = transform.position.y;
                if ((tPos - transform.position).magnitude > Mathf.Lerp(0, _aheadOfPlayer, 0.6f))
                    tPos = Vector3.Lerp(transform.position, tPos, 0.25f);


                tForward = Quaternion.LookRotation(pDir, Vector3.up);
            }
            else if (PlayerInRange())
            {
                tPos = transform.position + pDir * _recoverySpeed * _wanderingSpeed * Time.fixedDeltaTime;
                tPos.y = transform.position.y;
                tForward = Quaternion.LookRotation(pDir, Vector3.up);
            }

            lastPlayerPos =
                Vector3.Lerp(lastPlayerPos, actualPlayer.position, _playerJitterSmooth); //actualPlayer.position;

            smooth = Mathf.Lerp(_robotMoveSmoothing, .75f, Mathf.Clamp(pDeltaP.magnitude.Remap(0, .15f, 0, 1), 0, 1));

            transform.position = Vector3.Lerp(transform.position, tPos, smooth);
            transform.rotation = Quaternion.Slerp(transform.rotation, tForward, 0.15f);

            //transform.DOMove(tPos, _robotMoveSmoothing);
            //transform.DOBlendableMoveBy(tPos - transform.position, _robotMoveSmoothing).SetEase(Ease.InOutSine);
            //transform.DOBlendableRotateBy(tRot - transform.eulerAngles, _robotMoveSmoothing).SetEase(Ease.InQuad);


            yield return new WaitForFixedUpdate();
        }


        yield return null;
    }

    #endregion
}