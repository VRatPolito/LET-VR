
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
using UnityEngine.Serialization;

public class Level3Manager : UnitySingleton<Level3Manager>
{

    protected Level3Manager() { }

    #region Events

    #endregion

    #region Editor Visible

    public float TimeToStop = .5f;
    public WalkingDestination StartUnc, EndUnc, StartPoint, EndPoint;
    public RobotsCoinCollectorController RobotsCoinCollectorController;
    public DoorController Task1ToTask2DoorController;

    #endregion

    #region Private Members and Constants

    #endregion

    #region Properties

    public StatisticsLoggerL3 StatisticsLogger { get; private set; }

    private Transform _player => LocomotionManager.Instance.CurrentPlayerController;

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        StatisticsLogger = GetComponent<StatisticsLoggerL3>();
        Assert.IsNotNull(StatisticsLogger);

        StartUnc.OnDisabled += StatisticsLogger.StartLogUncoupledWalking;
        EndUnc.OnDisabled += StatisticsLogger.StopLogUncoupledWalking;
        StartPoint.OnDisabled += StatisticsLogger.StartLogPointWalking;
        StartPoint.OnDisabled += ShowPanel;
        EndPoint.OnDisabled += EndGame;
        Task1ToTask2DoorController.OnOpenGate += () =>
        {
            if (RobotsCoinCollectorController != null) RobotsCoinCollectorController.Introduce();
        };
    }

    private void HidePanel()
    {
        if (RobotsCoinCollectorController != null) RobotsCoinCollectorController.Outro();
    }

    private void ShowPanel()
    {
        if (RobotsCoinCollectorController != null) RobotsCoinCollectorController.StartCollecting();
    }

    private void EndGame()
    {
        HidePanel();
        StatisticsLogger.OnLogFinalized += (ix) =>
        {
            if (ix == 0)
                Invoke("Quit", 5);
        };
        StatisticsLogger.StopLogPointWalking();
    }

    private void Quit()
    {
        Application.Quit();
    }
    #endregion

    #region Public Methods

    #endregion

    #region Helper Methods

    #endregion

    #region Events Callbacks

    #endregion

    #region Coroutines

    #endregion

}
