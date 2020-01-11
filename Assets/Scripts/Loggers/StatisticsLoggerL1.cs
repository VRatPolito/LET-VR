
/*
 * Custom template by Gabriele P.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Level1Manager))]
public class StatisticsLoggerL1 : StatisticsLoggerBase
{
    #region Events

    #endregion

    #region Editor Visible
    [SerializeField] private PathDevAxis _pathDevAxis = PathDevAxis.X;
    #endregion

    #region Private Members and Constants

    private float _timeStart;
    private float _timeStop = float.MinValue;
    private bool _strLineWalking = false, _sprinting = false, _chasing = false;
    private uint _overshoots, _inside = 0;
    private bool _stopped = false;
    private bool _errorCounted = false;
    private OvershootingDestination _overshotingtarget = null;
    private int _numExits = 0;
    private int _numInterr = 0;
    private int _numWallColl = 0;
    #endregion

    #region Properties

    #endregion

    #region MonoBehaviour

    public void StartLogStrLineWalking(Destination d)
    {
        StartMasterLog("SW");
        _timeStart = Time.time;
        _strLineWalking = true;
    }
    public void StopLogStrLineWalking(Destination d)
    {
        _strLineWalking = false;
        var ComplTime = Time.time - _timeStart;
        var values = new List<string>
        {
            "" + ComplTime,
            "" + _pathDev,
            "" + _numWallColl
        };
        WriteToCSV("SW", values, 1);
        StopMasterLog();
    }

    public override void LogCollisions(HitType type)
    {
        if (_sprinting || _strLineWalking)
        {
            if (type == HitType.Player)
                _numWallColl++;
            LocomotionManager.Instance.LeftController.GetComponent<VibrationController>().ShortVibration(.5f);
            LocomotionManager.Instance.RightController.GetComponent<VibrationController>().ShortVibration(.5f);
        }
    }

    public void StartLogOvershooting(OvershootingDestination target)
    {
        if (_overshoots == 0)
            StartMasterLog("OU");
        _overshotingtarget = target;
        _timeStart = Time.time;
        _overshoots++;
    }
    public void StopLogOvershooting()
    {
        var ComplTime = Time.time - _timeStart;
        var TargetDist = Mathf.Abs(Vector3.Distance(_overshotingtarget.transform.position, LocomotionManager.Instance.CurrentPlayerController.position));
        var values = new List<string>
        {
        "" + ComplTime,
        "" + TargetDist,
        "" + _numExits
        };

        WriteToCSV("OU" + _overshoots, values, 2);
        _overshotingtarget = null;
        if (_overshoots == 3)
        {
            StopMasterLog();
        }
    }

    public void StartLogChasing()
    {
        StartMasterLog("C");
        _timeStart = Time.time;
        _chasing = true;
        _numInterr = -1;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
    }
    public void StopLogChasing()
    {
        _chasing = false;
        if (_numInterr == -1)
            _numInterr = 0;
        var TimeInside = GetPercTimeInside();
        var AvgDist = GetAverageDist();
        var values = new List<string>
        {
            "" + TimeInside,
            "" + AvgDist,
            "" + _numInterr,
        };
        WriteToCSV("C", values, 3);
        StopMasterLog();
    }

    public void StartLogSprinting()
    {
        StartMasterLog("S");
        _timeStart = Time.time;
        _sprinting = true;
        _numWallColl = 0;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
    }
    public void StopLogSprinting(Destination d)
    {
        _sprinting = false;
        var ComplTime = Time.time - _timeStart;
        var values = new List<string>
        {
            "" + ComplTime,
            "" + _numWallColl
        };
        WriteToCSV("S", values, 4);
        this.OnLogFinalized += (ix) =>
        {
            if (ix == 0)
                Invoke("Quit",5);
        };
        StopMasterLog();
    }

    private float GetPercTimeInside()
    {
        if (_inside == 0) return 0;
        return ((float)_inside / (float)(_targetpositions.Count)) * 100;
    }

    #endregion

    #region Public Methods

    public void LogOvershootError()
    {
        _numExits++;
    }

    protected override void ComputeStatisticsStep()
    {
        if (_strLineWalking)
        {
            var diff = GetPathDev(Level1Manager.Instance._pathDevRef, _pathDevAxis);
            _pathDev += diff * (1 / StatisticsLoggerData.SamplingRate);
        }
        else if (_chasing)
        {
            if (LocomotionManager.Instance.CurrentPlayerController.position == _prevpos && !_errorCounted)
            {
                if (_timeStop == float.MinValue)
                    _timeStop = Time.time;
                else if (Time.time >= _timeStop + Level1Manager.Instance.TimeToStop)
                {
                    _numInterr++;
                    _errorCounted = true;
                }
            }
            else if (LocomotionManager.Instance.CurrentPlayerController.position != _prevpos)
            {
                _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
                _errorCounted = false;
            }
        }

        if (_chasing && _masterlog)
        {
            _targetpositions.Add(Level1Manager.Instance.ChasingDest.transform.position);
            if (Level1Manager.Instance.ChasingDest.PlayerInside)
                _inside++;
        }
        base.ComputeStatisticsStep();
    }

    #endregion

    #region Helper Methods
    protected float GetAverageDist()
    {
        float v = 0.0f;
        for (int i = 0; i < _positions.Count; i++)
        {
            v += Mathf.Abs(Vector3.Distance(_positions[i], _targetpositions[i]));
        }
        return v / _positions.Count;
    }


    protected void Quit()
    {
        Application.Quit();
    }

    #endregion

    #region Events Callbacks

    #endregion

    #region Coroutines

    #endregion

}
