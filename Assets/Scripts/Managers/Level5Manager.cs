
/*
 * Custom template by Gabriele P.
 */
using System.Collections;
using System.Collections.Generic;
using PrattiToolkit;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(StatisticsLoggerL5))]
public class Level5Manager : UnitySingleton<Level5Manager>
{

    protected Level5Manager() { }

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
        EndManipulation;

    public GrabDestination Red, Green, Blue;
    public Destination Cyan;
    #endregion

    #region Private Members and Constants

    private uint _snaps = 0;
    public List<GenericItem> ManipulationStuff;

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
        EndManipulation.OnDisabled += EndLevel;
    }

    private void EndLevel()
    {
        StatisticsLogger.OnLogFinalized += (ix) =>
        {
            if (ix == 0)
                Application.Quit();
        };
       StatisticsLogger.StopLogManipulation();
    }

    private void CountSnap()
    {
        _snaps++;
        if (_snaps == 3)
        {
            foreach (var g in ManipulationStuff)
                g.CanInteract(true, LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>());
            Cyan.gameObject.SetActive(true);
            EndManipulation.gameObject.SetActive(true);
        }
    }

    void Start()
    {

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
