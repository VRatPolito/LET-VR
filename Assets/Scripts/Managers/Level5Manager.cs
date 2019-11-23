/*
 * Custom template by Gabriele P.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PrattiToolkit;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(StatisticsLoggerL5))]
public class Level5Manager : UnitySingleton<Level5Manager>
{
    protected Level5Manager()
    {
    }

    #region Events

    #endregion

    #region Editor Visible

    public DisableOnContactDestination StartGrab1,
        EndGrab1,
        StartGrab2,
        EndGrab2,
        StartGrab3,
        EndGrab3,
        StartManipulation,
        EndManipulation,
        StartMovingInteraction;

    public GrabDestination Red, Green, Blue;
    public Destination Cyan;
    public GameObject Link;
    [SerializeField] protected DroneWithPanelController _drone;
    [SerializeField] protected BillBoardManager _plate;

    #endregion

    #region Private Members and Constants

    private uint _snaps = 0;
    public List<GenericItem> ManipulationStuff;
    private bool _firstTime = true;
    #endregion

    #region Properties

    public StatisticsLoggerL5 StatisticsLogger { get; private set; }


    public Transform Player => LocomotionManager.Instance.CurrentPlayerController;

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        StatisticsLogger = GetComponent<StatisticsLoggerL5>();
        Assert.IsNotNull(StatisticsLogger);
        Assert.IsNotNull(Link);

        StartGrab1.OnDisabled += StatisticsLogger.StartLogGrabbing;
        StartGrab2.OnDisabled += StatisticsLogger.StartLogGrabbing;
        StartGrab3.OnDisabled += StatisticsLogger.StartLogGrabbing;

        EndGrab1.OnDisabled += StatisticsLogger.StopLogGrabbing;
        EndGrab2.OnDisabled += StatisticsLogger.StopLogGrabbing;
        EndGrab3.OnDisabled += StatisticsLogger.StopLogGrabbing;
        Red.OnDisabled += CountSnap;
        Green.OnDisabled += CountSnap;
        Blue.OnDisabled += CountSnap;
        StartManipulation.OnDisabled += StatisticsLogger.StartLogManipulation;
        StartMovingInteraction.OnDisabled += StatisticsLogger.StopLogManipulation;
        StartMovingInteraction.OnDisabled += StartMovingInteractionLevel;
        _drone.PlayerInRange.AddListener(PlayerInRange);
        _drone.PlayerOutRange.AddListener(StatisticsLogger.LogPlayerOutRange);
        _plate.InteractionError.AddListener(StatisticsLogger.LogInteractionError);
        _plate.AllInteractionsDone.AddListener(StopDrone);
        _plate.AllInteractionsDone.AddListener(EndLevel);
    }

    private void StopDrone()
    {
        _drone.enabled = false;
    }

    private void PlayerInRange()
    {
        if(_firstTime)
        {
            StatisticsLogger.StartLogMovingInteraction();
            _firstTime = false;
        }
    }

    #endregion

    #region Public Methods

    #endregion

    #region Helper Methods

    #endregion

    #region Events Callbacks

    private void CountSnap()
    {
        _snaps++;
        if (_snaps == 3)
        {
            foreach (var g in ManipulationStuff)
                g.CanInteract(true,
                    LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>());
            Cyan.gameObject.SetActive(true);
            EndManipulation.gameObject.SetActive(true);
        }
    }

    private void StartMovingInteractionLevel()
    {
        var antifall = Link.transform.GetChildRecursive("AntiFalling").gameObject;
        foreach (var c in antifall.GetComponents<BoxCollider>())
            c.enabled = true;
        
        LocomotionManager.Instance.StopLocomotion();
        LocomotionManager.Instance.CurrentPlayerController.parent = Link.transform;
        Link.transform.DOMoveY(3.5f, 5).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            LocomotionManager.Instance.CurrentPlayerController.parent = null;
            LocomotionManager.Instance.StartLocomotion();
            antifall.SetActive(false);
            //Destoy(Link);
        });
        //TODO ROBOT
    }

    private void EndLevel()
    {
        StatisticsLogger.OnLogFinalized += (ix) =>
        {
            if (ix == 0)
                Application.Quit();
        };
        StatisticsLogger.StopLogMovingInteraction();
    }

    #endregion

    #region Coroutines

    #endregion
}