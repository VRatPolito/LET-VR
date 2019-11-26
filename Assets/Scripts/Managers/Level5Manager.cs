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
    private TowerManager _tower;
    public GameObject Link;
    [SerializeField] protected DroneWithPanelController _drone;
    [SerializeField] protected BillBoardManager _plate;
    public List<GenericItem> ManipulationStuff;


    #endregion

    #region Private Members and Constants

    private uint _snaps = 0;
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
        Assert.IsNotNull(Red);
        Assert.IsNotNull(Green);
        Assert.IsNotNull(Blue);
        Assert.IsNotNull(Cyan);
        Assert.IsNotNull(Link);
        _tower = Cyan.GetComponent<TowerManager>();

        StartGrab1.OnDisabled.AddListener(StatisticsLogger.StartLogGrabbing);
        StartGrab2.OnDisabled.AddListener(StatisticsLogger.StartLogGrabbing);
        StartGrab3.OnDisabled.AddListener(StatisticsLogger.StartLogGrabbing);

        EndGrab1.OnDisabled.AddListener(StatisticsLogger.StopLogGrabbing);
        EndGrab2.OnDisabled.AddListener(StatisticsLogger.StopLogGrabbing);
        EndGrab3.OnDisabled.AddListener(StatisticsLogger.StopLogGrabbing);
        Red.OnDisabled.AddListener(CountSnap);
        Green.OnDisabled.AddListener(CountSnap);
        Blue.OnDisabled.AddListener(CountSnap);
        StartManipulation.OnDisabled.AddListener(StatisticsLogger.StartLogManipulation);
        EndManipulation.OnDisabled.AddListener(StopManipulation);
        StartMovingInteraction.OnDisabled.AddListener(StartMovingInteractionLevel);
        _drone.PlayerInRange.AddListener(PlayerInRange);
        _drone.PlayerInRange.AddListener(StatisticsLogger.PlayerInRange);
        _drone.PlayerOutRange.AddListener(StatisticsLogger.PlayerOutRange);
        _plate.InteractionError.AddListener(StatisticsLogger.LogInteractionError);
        _plate.AllInteractionsDone.AddListener(StopDrone);
        _plate.AllInteractionsDone.AddListener(EndLevel);
    }

    private void StopManipulation(Destination d)
    {
        StatisticsLogger.LogTowerPrecision(_tower.GetPosDiff(), _tower.GetRotDiff());
        StatisticsLogger.StopLogManipulation(d);
    }

    private void StopDrone()
    {
        _drone.End();
    }

    private void PlayerInRange()
    {
        if (_firstTime)
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

    private void CountSnap(Destination d)
    {
        _snaps++;
        GrabDestination gd = (GrabDestination)d;
        StatisticsLogger.LogSetupPrecision(gd.GetPosDiff(), gd.GetRotDiff());
        if (_snaps == 3)
        {
            foreach (var g in ManipulationStuff)
                g.CanInteract(true,
                    LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>());
            Cyan.gameObject.SetActive(true);
            EndManipulation.gameObject.SetActive(true);
        }
    }

    private void StartMovingInteractionLevel(Destination d)
    {
        var antifall = Link.transform.GetChildRecursive("AntiFalling").gameObject;
        foreach (var c in antifall.GetComponents<BoxCollider>())
            c.enabled = true;

        if(!LocomotionManager.Instance.IsAutoFreezable)
            LocomotionManager.Instance.StopLocomotionPublic();

        LocomotionManager.Instance.CurrentPlayerController.parent = Link.transform;
        Link.transform.DOMoveY(3.5f, 5).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            LocomotionManager.Instance.CurrentPlayerController.parent = null;
            if (!LocomotionManager.Instance.IsAutoFreezable)
                LocomotionManager.Instance.StartLocomotionPublic();
            antifall.SetActive(false);
        });
    }

    private void EndLevel()
    {
        StatisticsLogger.OnLogFinalized += (ix) =>
        {
            if (ix == 0)
                Invoke("Quit", 5);
        };
        StatisticsLogger.StopLogMovingInteraction();
    }

    private void Quit()
    {
        Application.Quit();
    }

    #endregion

    #region Coroutines

    #endregion
}