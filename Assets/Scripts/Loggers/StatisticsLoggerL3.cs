
/*
 * Custom template by Gabriele P.
 */

using PrattiToolkit;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Level3Manager))]
public class StatisticsLoggerL3 : StatisticsLoggerBase
{
    #region Events

    #endregion

    #region Editor Visible
    
    [SerializeField] private float _anglethresholdOffset = 2;
    [SerializeField] private PathDevAxis _pathDevAxis = PathDevAxis.X;
    #endregion

    #region Private Members and Constants

    private float _timeStart;
    private bool _decoupledGaze = false, _strOutHands = false, _decoupledHands = false;
    private int _numInterr = 0;
    private int _count = 0;
    private int _inCount = 0;
    private float _timeStop = float.MinValue;
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

    public void StartLogDecoupledGaze(Destination d)
    {
        StartMasterLog("DG");
        _timeStart = Time.time;
        _decoupledGaze = true;
        _stopPos = Vector3.negativeInfinity;
    }
    public void StopLogDecoupledGaze(Destination d)
    {
        _decoupledGaze = false;
        var ComplTime = Time.time - _timeStart;
        var TimeGazeUnc = ((float)_inCount / (float)_count * 100);
        var values = new List<string>
        {
            "" + ComplTime,
            "" + _pathDev,
            "" + TimeGazeUnc,
            "" + _numInterr
        };
        WriteToCSV("DG", values, 1);
        StopMasterLog();
    }

    public void StartLogStrcOutHands(Destination d)
    {
        StartMasterLog("SH");
        _timeStart = Time.time;
        _decoupledGaze = false;
        _strOutHands = true;
        _stopPos = Vector3.negativeInfinity;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
        _count = 0;
        _inCount = 0;
        _numInterr = 0;
    }
    public void StopLogStrcOutHands(Destination d)
    {
        _strOutHands = false;
        var ComplTime = Time.time - _timeStart;
        var TimStrOut = ((float)_inCount / (float)_count * 100);
        var values = new List<string>
        {
            "" + ComplTime,
            "" + _pathDev,
            "" + TimStrOut,
            "" + _numInterr
        };
        WriteToCSV("SH", values, 2);
        StopMasterLog();
    }
       
    public void StartLogDecoupledHands(Destination d)
    {
        StartMasterLog("DH");
        _timeStart = Time.time;
        _decoupledGaze = false;
        _decoupledHands = true;
        _stopPos = Vector3.negativeInfinity;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
        _pathDev = 0;
        _numInterr = 0;
    }
    public void StopLogDecoupledHands(Destination d)
    {
        _decoupledHands = false;
        var ComplTime = Time.time - _timeStart;
        var values = new List<string>
        {
            "" + ComplTime,
            "" + _pathDev,
            "" + Level3Manager.Instance.DronesCoinCollectorController.Score,
            "" + _numInterr
        };
        WriteToCSV("DH", values, 3);
        StopMasterLog();
    }

    #endregion

    #region Public Methods

    #endregion

    #region Helper Methods

    protected override void ComputeStatisticsStep()
    {
        if (_decoupledGaze)
        {
            // compute Path Deviation metrics
            var diff = GetPathDev(Level3Manager.Instance._pathDevRef1, _pathDevAxis);
            _pathDev += diff * (1 / StatisticsLoggerData.SamplingRate);
                       
            var v1 = LocomotionManager.Instance.CurrentPlayerController.position - _prevpos;
            var v2 = LocomotionManager.Instance.CameraEye.forward;
            var a = Vector3.Angle(v1, v2);
            if (a > Level3Manager.Instance.Sphere.Angle - _anglethresholdOffset && a < Level3Manager.Instance.Sphere.Angle + _anglethresholdOffset)
                _inCount++;
            _count++;
            if(_masterlog)
                _gazewalkangles.Add(a);

            var currpos = LocomotionManager.Instance.CurrentPlayerController.position;
            if (currpos == _prevpos)
            {
                if (_timeStop == float.MinValue)
                    _timeStop = Time.time;
                else if (Time.time >= _timeStop + Level3Manager.Instance.TimeToStop)
                {
                    if (UnityExtender.NearlyEqual(_stopPos, Vector3.negativeInfinity))
                    {
                        _numInterr++;
                        _stopPos = currpos;
                    }
                }
            }
            else
            {
                _timeStop = float.MinValue;
                _stopPos = Vector3.negativeInfinity;
            }
        }
        else if (_strOutHands)
        {
            // compute Path Deviation metrics
            var diff = GetPathDev(Level3Manager.Instance._pathDevRef2, _pathDevAxis);
            _pathDev += diff * (1 / StatisticsLoggerData.SamplingRate);
            
            var cl = LocomotionManager.Instance.LeftController.transform.position;
            var cr = LocomotionManager.Instance.RightController.transform.position;
            var hd = Vector3.Distance(cl, cr);
            if (hd > .6f* LocomotionManager.Instance.CalibrationData.ControllerDistance)
                _inCount++;
            _count++;

            var currpos = LocomotionManager.Instance.CurrentPlayerController.position;
            if (currpos == _prevpos)
            {
                if (_timeStop == float.MinValue)
                    _timeStop = Time.time;
                else if (Time.time >= _timeStop + Level3Manager.Instance.TimeToStop)
                {
                    if (UnityExtender.NearlyEqual(_stopPos, Vector3.negativeInfinity))
                    {
                        _numInterr++;
                        _stopPos = currpos;
                    }
                }
            }
            else
            {
                _timeStop = float.MinValue;
                _stopPos = Vector3.negativeInfinity;
            }            
        }
        else if (_decoupledHands)
        {

            // compute Path Deviation metrics
            var diff = GetPathDev(Level3Manager.Instance._pathDevRef3, _pathDevAxis);
            _pathDev += diff * (1 / StatisticsLoggerData.SamplingRate);
            
            var currpos = LocomotionManager.Instance.CurrentPlayerController.position;
            if (currpos == _prevpos)
            {
                if (_timeStop == float.MinValue)
                    _timeStop = Time.time;
                else if (Time.time >= _timeStop + Level3Manager.Instance.TimeToStop)
                {
                    if (UnityExtender.NearlyEqual(_stopPos, Vector3.negativeInfinity))
                    {
                        _numInterr ++;
                        _stopPos = currpos;
                    }
                }
            }
            else
            {
                _timeStop = float.MinValue;
                _stopPos = Vector3.negativeInfinity;
            }

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
