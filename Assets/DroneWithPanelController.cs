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
    [SerializeField] [Range(0.05f, 2)] private float _playerMovementThreshold = 0.1f;
    [SerializeField] [Range(0, 90)] private float _maxPlayerManouverAngle = 50f;
    [SerializeField] [Range(0, 1)] private float _robotMoveSmoothing = 0.3f;
    [SerializeField] [Range(0, 2)] private float _wanderingSpeed = 2f;
    [SerializeField] [Range(0, 360)] private float _maxWanderingAngleVariation = 30f;
    [SerializeField] [Range(0, 20)] private float _wanderingArea = 5f;
    [SerializeField] [Range(0, 20)] private float _warningRadius = 5f;

    [SerializeField] private GameObject _dummyPlayer, _target;

    #endregion

    #region Private Members and Constants

    private float _calibratedControllerDistance = 2;
    private CharacterController _currentCharacterController;
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
        Assert.IsNotNull(_panelController);

        _startPos = transform.position;
    }

    void Start()
    {
        _currentCharacterController =
            LocomotionManager.Instance.CurrentPlayerController.GetComponent<CharacterController>();
        Assert.IsNotNull(_currentCharacterController);
    }


    void Update()
    {
        _dummyPlayer.transform.DOMove(_target.transform.position, 5);

        UpdateStatus();
    }

    #endregion

    #region Public Methods

    public bool PlayerInRange()
    {
        return (actualPlayer.position - transform.position).magnitude < _warningRadius;
    }

    private bool PlayerInCloseRange()
    {
        return (actualPlayer.position - transform.position).magnitude < _aheadOfPlayer;
    }

    public Transform actualPlayer
    {
        get => LocomotionManager.Instance.CurrentPlayerController;
    }

    #endregion

    #region Helper Methods

    private void MoveRobot()
    {
    }

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
                wanderSpeed = Random.Range(wanderSpeed/3, wanderSpeed);
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
        float smooth=_robotMoveSmoothing;
        var pDeltaP = Vector3.zero;
        var tPos = transform.position;
        var tRot = transform.eulerAngles;
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
                lastPlayerPos = actualPlayer.position;
            }
            else
            {
                lastPlayerPos = actualPlayer.position;
                tPos = transform.position + transform.forward * 2 * _wanderingSpeed * Time.fixedDeltaTime;
                tPos.y = transform.position.y;
            }

            if (Mathf.Acos(Vector3.Dot(pDir, transform.forward)) > Mathf.Deg2Rad * _maxPlayerManouverAngle ||
               pDeltaP.magnitude < _playerMovementThreshold)
            {
                lastPlayerPos = actualPlayer.position;
                tPos = transform.position + transform.forward * _playerMovementThreshold;
                tPos.y = transform.position.y;
            }

            //if (inRange && Vector3.Dot(actualPlayer.position + pDir * _aheadOfPlayer - transform.position, transform.forward) <= 0)

            else
            if (PlayerInCloseRange())
            {
                //if (Vector3.Dot(tPos - transform.position, pDir) > 0)
                {
                    tPos = actualPlayer.position + pDir * _aheadOfPlayer;
                    tPos.y = transform.position.y;
                }

                tRot = transform.eulerAngles;
                tRot.y = Vector3.SignedAngle(transform.forward, pDir, Vector3.up);
                //transform.DOMove(tPos, _robotMoveSmoothing);
                //transform.DOBlendableMoveBy(tPos - transform.position, _robotMoveSmoothing).SetEase(Ease.InOutSine);
                //transform.DOBlendableRotateBy(tRot - transform.eulerAngles, _robotMoveSmoothing).SetEase(Ease.InQuad);
                //yield return new WaitForFixedUpdate();
            }

            smooth = Mathf.Lerp(_robotMoveSmoothing, 1, pDeltaP.magnitude.Remap(0, .17f, 0, 1));

            transform.position = Vector3.Lerp(transform.position, tPos, smooth);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pDir, Vector3.up), 0.15f);


            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    #endregion
}