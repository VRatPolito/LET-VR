/*
 * Custom template by Gabriele P.
 */

using System.Collections;
using System.Collections.Generic;
using PrattiToolkit;
using UnityEngine;

[RequireComponent(typeof(Scenario4Manager))]
public class StatisticsLoggerS4 : StatisticsLoggerBase
{
    #region Events

    #endregion

    #region Editor Visible

    #endregion

    #region Private Members and Constants

    private float _timeStart, _timeStop;
    private bool _dynamicAgility = false, _stationaryAgility = false, _evasion = false;
    private Vector3 _stopPos = Vector3.negativeInfinity;
    private int _numObsColl = 0;

    #endregion

    #region Properties

    #endregion

    #region MonoBehaviour

    public void StartLogDynamicAgility()
    {
        StartMasterLog("DA");
        _timeStart = Time.time;
        _dynamicAgility = true;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
    }

    public void StopLogDynamicAgility()
    {
        _dynamicAgility = false;
        _timeStop = Time.time - _timeStart;
        var values = new List<string>
        {
            "" + _timeStop,
            "" + _numObsColl
        };
        WriteToCSV("DA", values, 1);
        StopMasterLog();
    }

    public void StartLogStationaryAgility()
    {
        StartMasterLog("SA");
        _stationaryAgility = true;
    }

    public void StopLogStationaryAgility()
    {
        _stationaryAgility = false;
        var NumHits = Scenario4Manager.Instance.HeadShooterHits;
        var values = new List<string>
        {
            "" + NumHits
        };
        WriteToCSV("SA", values, 2);
        StopMasterLog();
    }

    public void StartLogBodyShooter()
    {
        StartMasterLog("E");
        _evasion = true;
    }

    public void StopLogBodyShooter()
    {
        _evasion = false;
        var NumHits = Scenario4Manager.Instance.BodyShooterHits;
        var values = new List<string>
        {
            "" + NumHits
        };
        WriteToCSV("E", values, 3);
        StopMasterLog();
    }

    public override void LogCollisions(HitType type)
    {
        if (_dynamicAgility)
        {
            _numObsColl++;
        }
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