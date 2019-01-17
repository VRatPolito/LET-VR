
/*
 * Custom template by Gabriele P.
 */
using System.Collections;
using System.Collections.Generic;
using PrattiToolkit;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(StatisticsLoggerTutorial))]
public class TutorialManager : UnitySingleton<TutorialManager>
{

    protected TutorialManager() { }

    #region Events

    #endregion

    #region Editor Visible

    #endregion

    #region Private Members and Constants

    #endregion

    #region Properties


    public StatisticsLoggerTutorial StatisticsLogger { get; private set; }
    public Transform Player => LocomotionManager.Instance.CurrentPlayerController;

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        StatisticsLogger = GetComponent<StatisticsLoggerTutorial>();
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
