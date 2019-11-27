
/*
 * Custom template by Gabriele P.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PrattiToolkit;
using UnityEngine;

[RequireComponent(typeof(Level3Manager))]
public class StatisticsLoggerL3 : StatisticsLoggerBase
{
    #region Events

    #endregion

    #region Editor Visible

    [SerializeField] private float _anglethreshold = 45;
    [SerializeField] private PathDevAxis _pathDevAxis = PathDevAxis.X;
    #endregion

    #region Private Members and Constants

    private float _timeStart, _timeStop;
    private bool _uncoupledWalking = false, _pointWalking = false, _waveWalking = false;
    private uint _count;
    private List<float> _angles;
    private Vector3 _stopPos = Vector3.negativeInfinity;

    #endregion

    #region Properties

    #endregion

    #region MonoBehaviour

    protected override void Initialize()
    {
        _angles = new List<float>();
        base.Initialize();
    }

    public void StartLogUncoupledWalking(Destination d)
    {
        StartMasterLog("UW");
        _timeStart = Time.time;
        _uncoupledWalking = true;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
        _speeds.Clear();
        _timeStop = float.MinValue;
        _stopPos = Vector3.negativeInfinity;
        _errors = 0;
    }
    public void StopLogUncoupledWalking(Destination d)
    {
        _uncoupledWalking = false;
        _timeStop = Time.time - _timeStart;
        var values = new List<string>
        {
            "" + _timeStop,
            "" + GetAverageSpeed(),
            "" + _maxwalkdist,
            "" + _minwalkdist,
            "" + _diffsum,
            "" + _errors,
            "" + ((float)_count / (float)_angles.Count * 100),
            "" + GetAvgGazeWalkAngle()
        };
        WriteToCSV("UW", values, 1);
        StopMasterLog();
    }

    private float GetAvgGazeWalkAngle()
    {
        float v = 0.0f;
        foreach (var s in _angles)
        {
            v += s;
        }
        return v / _angles.Count;
    }

    public void StartLogPointWalking(Destination d)
    {
        StartMasterLog("PW");
        _timeStart = Time.time;
        _uncoupledWalking = false;
        _pointWalking = true;
        _timeStop = float.MinValue;
        _stopPos = Vector3.negativeInfinity;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
        _speeds.Clear();
        _angles.Clear();
        _count = 0;
        _errors = 0;
    }
    public void StopLogPointWalking(Destination d)
    {
        _pointWalking = false;
        _timeStop = Time.time - _timeStart;
        var values = new List<string>
        {
            "" + _timeStop,
            "" + GetAverageSpeed(),
            "" + _maxwalkdist,
            "" + _minwalkdist,
            "" + _diffsum,
            "" + _errors,
            "" + ((float)_count / (float)_angles.Count * 100),
            "" + GetAvgGazeWalkAngle() * 100,                             //just joking, it's hand to hand avg dist %
            "" + Level3Manager.Instance.RobotsCoinCollectorController.Score
        };
        WriteToCSV("PW", values, 2);
        StopMasterLog();
    }
       
    public void StartLogWaveHandWalking(Destination d)
    {
        StartMasterLog("WH");
        _timeStart = Time.time;
        _uncoupledWalking = false;
        _waveWalking = true;
        _timeStop = float.MinValue;
        _stopPos = Vector3.negativeInfinity;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
        _maxwalkdist = 0;
        _minwalkdist = 0;
        _diffsum = 0;
        _speeds.Clear();
        _angles.Clear();
        _count = 0;
        _errors = 0;
    }
    public void StopLogWaveHandWalking(Destination d)
    {
        _waveWalking = false;
        _timeStop = Time.time - _timeStart;
        var values = new List<string>
        {
            "" + _timeStop,
            "" + GetAverageSpeed(),
            "" + _maxwalkdist,
            "" + _minwalkdist,
            "" + _diffsum,
            "" + _errors,
            "" + ((float)_count / (float)_angles.Count * 100),
            "" + GetAvgGazeWalkAngle() * 100,
            "" + Level3Manager.Instance.DronesCoinCollectorController.Score
        };
        WriteToCSV("WH", values, 3);
        StopMasterLog();
    }

    #endregion

    #region Public Methods

    #endregion

    #region Helper Methods

    protected override void ComputeStatisticsStep()
    {
        if (_uncoupledWalking)
        {
            // compute Path Deviation metrics
            var diff = GetPathDev(Level3Manager.Instance._pathDevRef1, _pathDevAxis);
            _diffsum += diff * (1 / StatisticsLoggerData.SamplingRate);
            if (diff > _maxwalkdist)
                _maxwalkdist = diff;
            else if (diff < _minwalkdist)
                _minwalkdist = diff;

            // compute speed
            var t = Time.time - _lastsample;
            var d = Mathf.Abs(Vector3.Distance(LocomotionManager.Instance.CurrentPlayerController.position, _prevpos));
            var v = d / t;

            if (v > 0)
            {
                var v1 = LocomotionManager.Instance.CurrentPlayerController.position - _prevpos;
                var v2 = LocomotionManager.Instance.CameraEye.forward;
                var a = Vector3.Angle(v1, v2);
                if (a > _anglethreshold)
                    _count++;
                _angles.Add(a);
                if(_masterlog)
                    _gazewalkangles.Add(a);
                    
            }
            else
                _gazewalkangles.Add(float.NegativeInfinity);

            var currpos = LocomotionManager.Instance.CurrentPlayerController.position;
            if (currpos == _prevpos)
            {
                if (_timeStop == float.MinValue)
                    _timeStop = Time.time;
                else if (Time.time >= _timeStop + Level3Manager.Instance.TimeToStop)
                {
                    if (UnityExtender.NearlyEqual(_stopPos, Vector3.negativeInfinity))
                    {
                        _errors++;
                        _stopPos = currpos;
                    }
                }
            }
            else
            {
                _timeStop = float.MinValue;
                _stopPos = Vector3.negativeInfinity;
            }

            _speeds.Add(v);
        }
        else if (_pointWalking)
        {

            // compute Path Deviation metrics
            var diff = GetPathDev(Level3Manager.Instance._pathDevRef2, _pathDevAxis);
            _diffsum += diff * (1 / StatisticsLoggerData.SamplingRate);
            if (diff > _maxwalkdist)
                _maxwalkdist = diff;
            else if (diff < _minwalkdist)
                _minwalkdist = diff;

            // compute speed
            var t = Time.time - _lastsample;
            var d = Mathf.Abs(Vector3.Distance(LocomotionManager.Instance.CurrentPlayerController.position, _prevpos));
            var v = d / t;

            var cl = LocomotionManager.Instance.LeftController.transform.position;
            var cr = LocomotionManager.Instance.RightController.transform.position;
            var hd = Vector3.Distance(cl, cr);
            if (hd > .6f* LocomotionManager.Instance.CalibrationData.ControllerDistance)
                _count++;
            _angles.Add(hd / LocomotionManager.Instance.CalibrationData.ControllerDistance);

            var currpos = LocomotionManager.Instance.CurrentPlayerController.position;
            if (currpos == _prevpos)
            {
                if (_timeStop == float.MinValue)
                    _timeStop = Time.time;
                else if (Time.time >= _timeStop + Level3Manager.Instance.TimeToStop)
                {
                    if (UnityExtender.NearlyEqual(_stopPos, Vector3.negativeInfinity))
                    {
                        _errors++;
                        _stopPos = currpos;
                    }
                }
            }
            else
            {
                _timeStop = float.MinValue;
                _stopPos = Vector3.negativeInfinity;
            }


            _speeds.Add(v);
        }
        else if (_waveWalking)
        {

            // compute Path Deviation metrics
            var diff = GetPathDev(Level3Manager.Instance._pathDevRef3, _pathDevAxis);
            _diffsum += diff * (1 / StatisticsLoggerData.SamplingRate);
            if (diff > _maxwalkdist)
                _maxwalkdist = diff;
            else if (diff < _minwalkdist)
                _minwalkdist = diff;

            // compute speed
            var t = Time.time - _lastsample;
            var d = Mathf.Abs(Vector3.Distance(LocomotionManager.Instance.CurrentPlayerController.position, _prevpos));
            var v = d / t;

            var cl = LocomotionManager.Instance.LeftController.transform.position;
            var cr = LocomotionManager.Instance.RightController.transform.position;
            var hd = Vector3.Distance(cl, cr);
            if (hd > .6f * LocomotionManager.Instance.CalibrationData.ControllerDistance)
                _count++;
            _angles.Add(hd / LocomotionManager.Instance.CalibrationData.ControllerDistance);

            var currpos = LocomotionManager.Instance.CurrentPlayerController.position;
            if (currpos == _prevpos)
            {
                if (_timeStop == float.MinValue)
                    _timeStop = Time.time;
                else if (Time.time >= _timeStop + Level3Manager.Instance.TimeToStop)
                {
                    if (UnityExtender.NearlyEqual(_stopPos, Vector3.negativeInfinity))
                    {
                        _errors++;
                        _stopPos = currpos;
                    }
                }
            }
            else
            {
                _timeStop = float.MinValue;
                _stopPos = Vector3.negativeInfinity;
            }


            _speeds.Add(v);

        }
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;

        base.ComputeStatisticsStep();
    }

    #endregion

    #region Events Callbacks

    #endregion

    #region Coroutines

    #endregion

}
