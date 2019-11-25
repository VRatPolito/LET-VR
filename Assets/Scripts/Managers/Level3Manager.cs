
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

    private LookAtMeCatalizer _lookAtMe;
    
    #endregion

    #region Properties

    public StatisticsLoggerL3 StatisticsLogger { get; private set; }

    private Transform _player => LocomotionManager.Instance.CurrentPlayerController;

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        StatisticsLogger = GetComponent<StatisticsLoggerL3>();
        _lookAtMe = FindObjectOfType<LookAtMeCatalizer>();
        Assert.IsNotNull(StatisticsLogger);
        Assert.IsNotNull(_lookAtMe);

        StartUnc.OnDisabled += StatisticsLogger.StartLogUncoupledWalking;
        EndUnc.OnDisabled += d => _lookAtMe.End();
        EndUnc.OnDisabled += StatisticsLogger.StopLogUncoupledWalking;
        StartPointHandFar.OnDisabled += StatisticsLogger.StartLogPointWalking;
        StartPointHandFar.OnDisabled += StartRobotT2;
        EndPointHandFar.OnDisabled += StatisticsLogger.StopLogPointWalking;
        EndPointHandFar.OnDisabled += StopRobotT2;
        StartWavingHand.OnDisabled += StatisticsLogger.StartLogWaveHandWalking;
        StartWavingHand.OnDisabled += StartDronesT3;       
        EndPointWavingHand.OnDisabled += EndGame;

        Task1ToTask2DoorController.OnOpenGate += () =>
        {
            if (RobotsCoinCollectorController != null) RobotsCoinCollectorController.Introduce();
        };

        T2ToT3DoorController.OnOpenGate += () =>
        {
            if (DronesCoinCollectorController != null) DronesCoinCollectorController.Introduce();
        };
    }

    private void StartRobotT2(Destination d)
    {
        if (RobotsCoinCollectorController != null) RobotsCoinCollectorController.StartCollecting();
    }

    private void StopRobotT2(Destination d)
    {
        if (RobotsCoinCollectorController != null) RobotsCoinCollectorController.Outro();
    }

    private void StartDronesT3(Destination d)
    {
        if(DronesCoinCollectorController!=null) DronesCoinCollectorController.StartCollecting();
    }
    private void StopDronesT3(Destination d)
    {
        if(DronesCoinCollectorController != null) DronesCoinCollectorController.Outro();
    }


    private void EndGame(Destination d)
    {
        StopDronesT3(d);
        StatisticsLogger.OnLogFinalized += (ix) =>
        {
            Debug.Log($"Log {ix} finalized!");
            if (ix == 0)
                Invoke("Quit", 5);
        };
        //StatisticsLogger.StopLogPointWalking();
        StatisticsLogger.StopLogWaveHandWalking(d); //TODO SURE??
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
