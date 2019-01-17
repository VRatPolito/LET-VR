
/*
 * Custom template by Gabriele P.
 */
using System.Collections;
using System.Collections.Generic;
using PrattiToolkit;
using UnityEditor.Experimental.Rendering;
using UnityEngine;
using UnityEngine.Assertions;

public class Level4Manager : UnitySingleton<Level4Manager>
{

    protected Level4Manager() { }

    #region Events

    #endregion

    #region Editor Visible
    public float TimeToStop = .5f;
    public WalkingDestination AgilityStart, AgilityEnd, PieStart;

    [SerializeField] private GameObject _pOPSystem;
    [SerializeField] private FPSPatternSystem _robot;

    [Header("In combo with CTRL")] [SerializeField]
    private KeyCode _popSystemSafeStop = KeyCode.H;

    #endregion

    #region Private Members and Constants

    #endregion

    #region Properties

    public StatisticsLoggerL4 StatisticsLogger { get; private set; }

    public int FPSHits => _robot.HitsCounter;

    private Transform _player => LocomotionManager.Instance.CurrentPlayerController;

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        StatisticsLogger = GetComponent<StatisticsLoggerL4>();
        Assert.IsNotNull(StatisticsLogger);


        AgilityStart.OnDisabled += () => {StatisticsLogger.StartLogAgility(); };
        AgilityEnd.OnDisabled += () =>
        {
            _pOPSystem.GetComponent<ProceduralObstacleSpawnerSystem>().DestroyRenderedElements();
            _pOPSystem.SetActive(false);
            StatisticsLogger.StopLogAgility();
        };
        PieStart.OnDisabled += OnPieStartOnDisabled;

        _robot.OnLastHit += OnLastBulletHit;
    }

    private void Update()
    {
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(_popSystemSafeStop))
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

    private void OnPieStartOnDisabled()
    {
        _robot.enabled = true;
        LocomotionManager.Instance.StopLocomotion();
        LocomotionManager.Instance.CurrentPlayerController.GetComponent<CharacterController>().enabled = false;
        LocomotionManager.Instance.CameraEye.GetComponent<SphereCollider>().enabled = true;
        LocomotionManager.Instance.RightController.GetComponent<Collider>().enabled = false;
        LocomotionManager.Instance.LeftController.GetComponent<Collider>().enabled = false;
        StatisticsLogger.StartLogPieGame();
    }

    private void OnLastBulletHit()
    {
        _robot.enabled = false;

        StatisticsLogger.OnLogFinalized += (ix) =>
        {
            Debug.Log($"Log {ix} finalized!");
            if(ix== 0)
                Invoke("Quit", 5);
        };
        StatisticsLogger.StopLogPieGame();
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
