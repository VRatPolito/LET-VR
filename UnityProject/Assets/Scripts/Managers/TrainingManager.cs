
/*
 * Custom template by Gabriele P.
 */
using System.Collections;
using System.Collections.Generic;
using PrattiToolkit;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(StatisticsLoggerTraining))]
public class TrainingManager : UnitySingleton<TrainingManager>
{

    protected TrainingManager() { }

    #region Events

    #endregion

    #region Editor Visible

    #endregion

    #region Private Members and Constants

    #endregion

    #region Properties


    public StatisticsLoggerTraining StatisticsLogger { get; private set; }
    public Transform Player => LocomotionManager.Instance.CurrentPlayerController;

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        StatisticsLogger = GetComponent<StatisticsLoggerTraining>();
        Assert.IsNotNull(StatisticsLogger);
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
