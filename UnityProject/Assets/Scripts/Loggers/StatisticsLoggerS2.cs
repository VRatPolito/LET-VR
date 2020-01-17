
/*
 * Custom template by Gabriele P.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrattiToolkit;

[RequireComponent(typeof(Scenario2Manager))]
public class StatisticsLoggerS2 : StatisticsLoggerBase
{
    #region Events

    #endregion

    #region Editor Visible
    [SerializeField] private float _speedThreshold = 0;
    [SerializeField] private float _dirWalkingDistThreshold = 0.5f;
    [SerializeField] private PathDevAxis _pathDevAxis = PathDevAxis.X;
    [SerializeField] private PathDevAxis _avoidanceAxis = PathDevAxis.X;
    #endregion

    #region Private Members and Constants

    private float _timeStart, _avoidance;
    float _estPathLen, _initAngErr, _recallTime, _timesover;
    private bool _errorCounted = false;
    private bool _backWalking = false, _curvedWalking = false, _fear = false, _multiStrLineWalking = false, _stairsRamp = false;
    private string _choice;
	private int _inCount = 0;
    private int _count = 0;
    private int _numLookOut = 0;
    private int _numInterr = 0;
    private int _numFalls = 0;
    private float _timeStop = float.MinValue;
    private Transform _prevTarget;
    private uint _dirTargets = 0;
    private GazeDestination _currDest = null;
    private Vector3 _stopPos = Vector3.negativeInfinity;

    #endregion

    #region Properties

    #endregion

    #region MonoBehaviour
    public void StartLogMultiStrLineWalking(GazeDestination g)
    {
        if (_dirTargets == 0)
        {
            StartMasterLog("MW");
            _multiStrLineWalking = true;
        }
        _timeStart = Time.time;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
        _estPathLen = 0;
        _recallTime = -1;
        _initAngErr = -1;
        _currDest = g;

    }
    public void StopLogMultiStrLineWalking(GazeDestination g)
    {
        if (_currDest == g)
        {
            _dirTargets++;

            var ComplTime = Time.time - _timeStart;
            var values = new List<string>
            {
                "" + ComplTime,
                "" + _initAngErr,
                "" + _estPathLen,
                "" + _recallTime
            };
            WriteToCSV("MW" + _dirTargets, values, 1);
            if (_dirTargets == 6)
            {
                StopMasterLog();
                _multiStrLineWalking = false;
            }

            _prevTarget = _currDest.transform;
            _currDest = null;
        }
    }

    public void StartLogBackWalking()
    {
        StartMasterLog("BW");
        _timeStart = Time.time;
        _backWalking = true;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
		_inCount = 0;
        _numLookOut = -1;
    }

    public void StopLogBackWalking(Destination d)
    {
        _backWalking = false;
        if (_numLookOut == -1)
            _numLookOut = 0;
        var ComplTime = Time.time - _timeStart;
        var TimeLookAt = (100 - ((float)_inCount / (float)_count * 100));
        var values = new List<string>
        {
            "" + ComplTime,
            "" + _numLookOut,
            "" + TimeLookAt,
            "" + _pathDev
        };
        WriteToCSV("BW", values, 2);
        StopMasterLog();
    }

    public void StartLogCurvedWalking(Destination d)
    {
        StartMasterLog("CW");
        _timeStart = Time.time;
        _curvedWalking = true;
        _stopPos = Vector3.negativeInfinity;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
        _numInterr = 0;
    }
    public void StopLogCurvedWalking(Destination d)
    {
        _curvedWalking = false;
        var ComplTime = Time.time - _timeStart;
        var values = new List<string>
        {
            "" + ComplTime,
            "" + _numInterr
        };
        WriteToCSV("CW", values, 3);
        StopMasterLog();
    }

    internal void StartLogStair(Destination d)
    {
        StartMasterLog("STL");
        _stairsRamp = true;
    }

    internal void StopLogStair(Destination d)
    {
        _stairsRamp = false;
    }

    internal void StartLogSlope(Destination d)
    {
        _timeStart = Time.time;
        _stairsRamp = true;
    }

    internal void StopLogSlope(Destination d)
    {
        _stairsRamp = false;
        Scenario2Manager.Instance.StartHalfSlope.gameObject.SetActive(true);
        Scenario2Manager.Instance.StartHalfStairs.gameObject.SetActive(true);
    }

    internal void StartLogHalfStairs(Destination d)
    {
        _choice = "ST";
        if (_stairsRamp)
        {
            Scenario2Manager.Instance.StartHalfSlope.gameObject.SetActive(true);
        }
        else
        {
            _stairsRamp = true;
            Scenario2Manager.Instance.EndStairsSlope.gameObject.SetActive(true);
        }
    }

    internal void StartLogHalfSlope(Destination d)
    {
        _choice = "SL";
        if (_stairsRamp)
        {
            Scenario2Manager.Instance.StartHalfStairs.gameObject.SetActive(true);
        }
        else
        {
            _stairsRamp = true;
            Scenario2Manager.Instance.EndStairsSlope.gameObject.SetActive(true);
        }
    }

    internal void StopLogStairsSlope(Destination d)
    {
        _stairsRamp = false;
        var values = new List<string>
        {
            "" + _choice
        };
        WriteToCSV("STL", values, 4);
        if (Scenario2Manager.Instance.StartHalfSlope.gameObject.activeSelf)
        {
            Scenario2Manager.Instance.StartHalfSlope.OnDisabled.RemoveListener(StartLogHalfSlope);
            Scenario2Manager.Instance.StartHalfSlope.gameObject.SetActive(false);
        }
        else if (Scenario2Manager.Instance.StartHalfStairs.gameObject.activeSelf)
        {
            Scenario2Manager.Instance.StartHalfStairs.OnDisabled.RemoveListener(StartLogHalfStairs);
            Scenario2Manager.Instance.StartHalfStairs.gameObject.SetActive(false);
        }

        StopMasterLog();
    }   

    public void StartLogFear(Destination d)
    {
        StartMasterLog("F");
        _timeStart = Time.time;
        _fear = true;
        _numFalls = 0;
        _avoidance = 0;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
    }

    public void StopLogFear(Destination d)
    {
        _fear = false;
        var ComplTime = Time.time - _timeStart;
        var values = new List<string>
        {
            "" + ComplTime,
            "" + _avoidance,
            "" + _numFalls
        };
        WriteToCSV("F", values, 5);
        StopMasterLog();
    }

    #endregion

    #region Public Methods

    #endregion

    #region Helper Methods    

    internal void PlayerFallen()
    {
        if (_fear)
            _numFalls++;
    }

    protected override void ComputeStatisticsStep()
    {
        if (_backWalking)
        {
            var diff = GetPathDev(Scenario2Manager.Instance._pathDevRef, _pathDevAxis);
            _pathDev += diff * (1 / StatisticsLoggerData.SamplingRate);
            
            if (!Scenario2Manager.Instance.BackwardItem.InteractiveItem.IsOver)
            {
				_inCount++;
				if(!_errorCounted)
					{
                    _numLookOut++;
					_errorCounted = true;
					}
            }
            else
            {
                _errorCounted = false;
            }
            _count++;
        }
        else if (_curvedWalking)
        {
            var t = (Time.time - _lastsample);
            var currpos = LocomotionManager.Instance.CurrentPlayerController.position;
            if (currpos == _prevpos)
            {
                if (_timeStop == float.MinValue)
                    _timeStop = Time.time;
                else if (Time.time >= _timeStop + Scenario2Manager.Instance.TimeToStop)
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
        else if (_fear)
        {
            if (LocomotionManager.Instance.CurrentPlayerController.position.x <= Scenario2Manager.Instance._avoidanceRef.position.x)
            {
                var diff = GetPathDev(Scenario2Manager.Instance._avoidanceRef, _avoidanceAxis);
                _avoidance += diff * (1 / StatisticsLoggerData.SamplingRate);
            }
        }
        else if (_multiStrLineWalking)
        {
            var d = Mathf.Abs(Vector3.Distance(LocomotionManager.Instance.CurrentPlayerController.position, _prevpos));
            _estPathLen += d;
            if (d > 0 && _recallTime == -1)
                _recallTime = Time.time - _timeStart;
            if (_estPathLen >= _dirWalkingDistThreshold && _initAngErr == -1)
            {
                var v1 = LocomotionManager.Instance.CurrentPlayerController.position - _prevpos;
                Vector3 v2 = Vector3.zero;
                if (_dirTargets == 0)
                    v2 = _currDest.transform.position - Scenario2Manager.Instance.StartDest.transform.position;
                else
                    v2 = _currDest.transform.position - _prevTarget.transform.position;
                _initAngErr = Vector3.Angle(v1, v2);
            }

        }
        else 
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;

        base.ComputeStatisticsStep();
    }

    #endregion

    #region Events Callbacks

    #endregion

    #region Coroutines

    #endregion

}
