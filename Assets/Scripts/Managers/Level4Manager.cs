/*
 * Custom template by Gabriele P.
 */

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PrattiToolkit;
using UnityEngine;
using UnityEngine.Assertions;

public class Level4Manager : UnitySingleton<Level4Manager>
{
    protected Level4Manager()
    {
    }

    #region Events

    #endregion

    #region Editor Visible

    public float TimeToStop = .5f;
    [SerializeField] private WalkingDestination _agilityStart, _agilityEnd, _headShooterStart, _bodyShooterStart;

    [SerializeField] private GameObject _pOPSystem, _jailBalcony;
    [SerializeField] private FPSPatternSystem _headShooter, _bodyShooter;

    [Header("In combo with CTRL")] [SerializeField]
    private KeyCode _popSystemSafeStop = KeyCode.H;

    #endregion

    #region Private Members and Constants

    #endregion

    #region Properties

    public StatisticsLoggerL4 StatisticsLogger { get; private set; }

    public int HeadShooterHits => _headShooter.HitsCounter;
    public int BodyShooterHits => _bodyShooter.HitsCounter;

    private Transform _player => LocomotionManager.Instance.CurrentPlayerController;

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        StatisticsLogger = GetComponent<StatisticsLoggerL4>();
        Assert.IsNotNull(StatisticsLogger);


        _agilityStart.OnDisabled += (Destination d) => { StatisticsLogger.StartLogAgility(); };
        _agilityEnd.OnDisabled += (Destination d) =>
        {
            _pOPSystem.GetComponent<ProceduralObstacleSpawnerSystem>().DestroyRenderedElements();
            _pOPSystem.SetActive(false);
            StatisticsLogger.StopLogAgility();
        };
        _headShooterStart.OnDisabled += OnHeadShooterStartOnDisabled;
        _headShooter.OnLastBulletExpired += OnLastBulletHeadShooterBulletExpired;
        _headShooter.OnShuttedDown += HeadShooterOnShuttedDown;

        _bodyShooterStart.OnDisabled += OnBodyShooterStart;
        _bodyShooter.OnLastBulletExpired += OnLastBulletBodyShooterBulletExpired;
    }

    private void Update()
    {
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
            Input.GetKeyDown(_popSystemSafeStop))
        {
            _pOPSystem.GetComponent<ProceduralObstacleSpawnerSystem>().DestroyRenderedElements();
            _pOPSystem.SetActive(false);
        }
    }

    #endregion

    #region Public Methods

    #endregion

    #region Helper Methods

    #endregion

    #region Events Callbacks

    private void OnHeadShooterStartOnDisabled(Destination d)
    {
        _headShooter.enabled = true;
        LocomotionManager.Instance.StopLocomotion();
        switch(LocomotionManager.Instance.Locomotion)
        {
            case ControllerType.ArmSwing:
            case ControllerType.FootSwing:
            case ControllerType.Joystick:
                LocomotionManager.Instance.CurrentPlayerController.GetComponent<CircularLimitTracking>().DisableCollider();
                break;
            default:
                LocomotionManager.Instance.CurrentPlayerController.GetComponent<PlayerColliderManager>().DisableCollider();
                break;
        }
        LocomotionManager.Instance.CurrentPlayerController.position = new Vector3(LocomotionManager.Instance.CurrentPlayerController.position.x, 0, LocomotionManager.Instance.CurrentPlayerController.position.z);
        LocomotionManager.Instance.CameraEye.GetComponent<SphereCollider>().enabled = true;
        LocomotionManager.Instance.RightController.GetComponent<Collider>().enabled = false;
        LocomotionManager.Instance.LeftController.GetComponent<Collider>().enabled = false;
        StatisticsLogger.StartLogHeadShooter();
    }

    private void OnLastBulletHeadShooterBulletExpired()
    {
        _headShooter.Shutdown();
        StatisticsLogger.StopLogHeadShooter();
        LocomotionManager.Instance.CameraEye.GetComponent<SphereCollider>().enabled = false;
    }

    private void HeadShooterOnShuttedDown()
    {
        LocomotionManager.Instance.RightController.GetComponent<Collider>().enabled = true;
        LocomotionManager.Instance.LeftController.GetComponent<Collider>().enabled = true;

        _jailBalcony.GetComponent<Collider>().enabled = false;
        _jailBalcony.transform.DOMoveY(_jailBalcony.transform.position.y - 2, 5);
        LocomotionManager.Instance.StartLocomotion();
        switch (LocomotionManager.Instance.Locomotion)
        {
            case ControllerType.ArmSwing:
            case ControllerType.FootSwing:
            case ControllerType.Joystick:
                LocomotionManager.Instance.CurrentPlayerController.GetComponent<CircularLimitTracking>().EnableCollider();
                break;
            default:
                LocomotionManager.Instance.CurrentPlayerController.GetComponent<PlayerColliderManager>().EnableCollider();
                break;
        }
        LocomotionManager.Instance.CameraEye.GetComponent<SphereCollider>().enabled = true;
        LocomotionManager.Instance.RightController.GetComponent<Collider>().enabled = true;
        LocomotionManager.Instance.LeftController.GetComponent<Collider>().enabled = true;
    }

    private void OnBodyShooterStart(Destination d)
    {
        _bodyShooter.enabled = true;
        _headShooter.HitsCounter = 0;
        StatisticsLogger.StartLogBodyShooter();
    }

    private void OnLastBulletBodyShooterBulletExpired()
    {
        _bodyShooter.Shutdown();
        StatisticsLogger.OnLogFinalized += (ix) =>
        {
            Debug.Log($"Log {ix} finalized!");
            if (ix == 0)
                Invoke("Quit", 5);
        };
        StatisticsLogger.StopLogBodyShooter();
        LocomotionManager.Instance.RightController.GetComponent<Collider>().enabled = true;
        LocomotionManager.Instance.LeftController.GetComponent<Collider>().enabled = true;
    }

    private void Quit()
    {
        Application.Quit();
    }

    #endregion

    #region Coroutines

    #endregion
}