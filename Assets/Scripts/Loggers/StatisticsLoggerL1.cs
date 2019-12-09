
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

    private float _timeStart, _timeStop;

    private bool _walking = false, _running = false, _chasing = false;
    private uint _overshoots, _inside = 0;
    private bool _stopped = false;
    private bool _errorCounted = false;
    private OvershootingDestination _overshotingtarget = null;

    #endregion

    #region Properties

    #endregion

    #region MonoBehaviour

    public void StartLogWalking(Destination d)
    {
        StartMasterLog("W");
        _timeStart = Time.time;
        _walking = true;
    }
    public void StopLogWalking(Destination d)
    {
        _walking = false;
        _timeStop = Time.time - _timeStart;
        var values = new List<string>
        {
            "" + _timeStop,
            "" + _maxwalkdist,
            "" + _diffsum,
            "" + Collisions
        };
        WriteToCSV("W", values, 1);
        StopMasterLog();
    }

    public override void LogCollisions(HitType type)
    {
        if (_running || _walking)
        {
            if (type == HitType.Player)
                Collisions++;
            LocomotionManager.Instance.LeftController.GetComponent<VibrationController>().ShortVibration(.5f);
            LocomotionManager.Instance.RightController.GetComponent<VibrationController>().ShortVibration(.5f);
        }
    }

    public void StartLogOvershooting(OvershootingDestination target)
    {
        if (_overshoots == 0)
            StartMasterLog("O");
        _overshotingtarget = target;
        _peakvel = _peakacc = 0;
        _peakdec = 0;
        _distpeakvel = _distpeakacc = _distpeakdec = -1;
        _errors = 0;
        _timeStart = Time.time;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
        _prevvel = 0;
        _overshoots++;
    }
    public void StopLogOvershooting()
    {
        _timeStop = Time.time - _timeStart;
        var values = new List<string>
        {

        "" + _timeStop,
        "" + Mathf.Abs(Vector3.Distance(_overshotingtarget.transform.position, LocomotionManager.Instance.CurrentPlayerController.position)),
        "" + _peakvel,
        "" + _peakacc,
        "" + _peakdec,
        "" + _distpeakvel / _peakvel,
        "" + _distpeakacc / _peakacc,
        "" + _distpeakdec / _peakdec,
        "" + (_timetopeakvel - _timeStart) / _timeStop * 100,
        "" + _errors
        };

        WriteToCSV("O" + _overshoots, values, 2);
        _overshotingtarget = null;
        if (_overshoots == 3)
        {
            StopMasterLog();
        }
    }

    public void StartLogRunning()
    {
        StartMasterLog("R");
        _timeStart = Time.time;
        _running = true;
        Collisions = 0;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
    }
    public void StopLogRunning(Destination d)
    {
        _running = false;
        _timeStop = Time.time - _timeStart;
        var values = new List<string>
        {
            "" + _timeStop,
            "" + GetAverageSpeed(),
            "" + Collisions
        };
        WriteToCSV("R", values, 4);
        this.OnLogFinalized += (ix) =>
        {
            if (ix == 0)
                Invoke("Quit",5);
        };
        StopMasterLog();
    }

    public void StartLogChasing()
    {
        StartMasterLog("C");
        _timeStart = Time.time;
        _timeStop = float.MinValue;
        _chasing = true;
        _errors = -1;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
    }
    public void StopLogChasing()
    {
        _chasing = false;
        _timeStop = Time.time - _timeStart;
        if (_errors == -1)
            _errors = 0;
        var values = new List<string>
        {
            "" + GetPercTimeInside(),
            "" + GetAverageDist(),
            "" + _errors,
        };
        WriteToCSV("C", values, 3);
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
        _errors++;
    }

    protected override void ComputeStatisticsStep()
    {
        if (_walking)
        {
            var diff = GetPathDev(Level1Manager.Instance._pathDevRef, _pathDevAxis);
            _diffsum += diff * (1 / StatisticsLoggerData.SamplingRate);
            if (diff > _maxwalkdist)
                _maxwalkdist = diff;
            else if (diff < _minwalkdist)
                _minwalkdist = diff;
        }
        else if (_overshotingtarget != null)
        {
            var t = (Time.time - _lastsample);
            var d = Mathf.Abs(Vector3.Distance(LocomotionManager.Instance.CurrentPlayerController.position, _prevpos));
            var v = d / t;

            if (_peakvel < v)
            {
                _peakvel = v;
                _timetopeakvel = Time.time;
                _distpeakvel = Mathf.Abs(Vector3.Distance(LocomotionManager.Instance.CurrentPlayerController.position, _overshotingtarget.transform.position));
            }

            var a = (v - _prevvel) / t;
            if (a > 0 && _peakacc < a)
            {
                _peakacc = a;
                _distpeakacc = Mathf.Abs(Vector3.Distance(LocomotionManager.Instance.CurrentPlayerController.position, _overshotingtarget.transform.position));
            }
            else if (a < 0 && _peakdec > a)
            {
                _peakdec = a;
                _distpeakdec = Mathf.Abs(Vector3.Distance(LocomotionManager.Instance.CurrentPlayerController.position, _overshotingtarget.transform.position));
            }

            _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
            _prevvel = v;
        }
        else if (_chasing)
        {
            if (LocomotionManager.Instance.CurrentPlayerController.position == _prevpos && !_errorCounted)
            {
                if (_timeStop == float.MinValue)
                    _timeStop = Time.time;
                else if (Time.time >= _timeStop + Level1Manager.Instance.TimeToStop)
                {
                    _errors++;
                    _errorCounted = true;
                }
            }
            else if (LocomotionManager.Instance.CurrentPlayerController.position != _prevpos)
            {
                _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
                _errorCounted = false;
            }
        }
        else if (_running)
        {
            var t = (Time.time - _lastsample);
            var d = Mathf.Abs(Vector3.Distance(LocomotionManager.Instance.CurrentPlayerController.position, _prevpos));
            var v = d / t;

            _speeds.Add(v);
            _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
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
