
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
    public WalkingDestination StartUnc, EndUnc, StartPointHandFar, EndPointHandFar, StartWavingHand, EndPointWavingHand;
    public RobotsCoinCollectorController RobotsCoinCollectorController;
    public DronesCoinCollectorController DronesCoinCollectorController;
    public DoorController Task1ToTask2DoorController, T2ToT3DoorController;

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
        StartPointHandFar.OnDisabled += StatisticsLogger.StartLogPointHandFarWalking;
        StartPointHandFar.OnDisabled += StartRobotT2;
        //TODO ??? EndPointHandFar.OnDisabled += LOGGER;
        EndPointHandFar.OnDisabled += StopRobotT2;
        StartWavingHand.OnDisabled += StartDronesT3;
        EndPointWavingHand.OnDisabled += EndGame;

        Task1ToTask2DoorController.OnOpenGate += () =>
        {
            if (RobotsCoinCollectorController != null) RobotsCoinCollectorController.Introduce();
        };

        T2ToT3DoorController.OnOpenGate += () =>
        {
            //TODO
        };
    }

    private void StopRobotT2()
    {
        if (RobotsCoinCollectorController != null) RobotsCoinCollectorController.Outro();
    }

    private void StartRobotT2()
    {
        if (RobotsCoinCollectorController != null) RobotsCoinCollectorController.StartCollecting();
    }

    private void StartDronesT3()
    {
        if(DronesCoinCollectorController!=null) DronesCoinCollectorController.StartCollecting();
    }

    private void StopDronesT3()
    {
        if(DronesCoinCollectorController != null) DronesCoinCollectorController.Outro();
    }

    private void EndGame()
    {
        StopDronesT3();
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
