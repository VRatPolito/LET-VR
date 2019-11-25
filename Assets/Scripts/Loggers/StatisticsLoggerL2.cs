
/*
 * Custom template by Gabriele P.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrattiToolkit;

[RequireComponent(typeof(Level2Manager))]
public class StatisticsLoggerL2 : StatisticsLoggerBase
{
    #region Events

    #endregion

    #region Editor Visible
    [SerializeField] private float _speedThreshold = 0;
    #endregion

    #region Private Members and Constants

    private float _timeStart, _timeStop, _avoidance;
    float _estpathlength, _angularerror, _recalltime, _timesover;
    private bool _backWalking = false, _curvedWalking = false, _fear = false, _dirWalking = false, _stairslopeWalking = false;
    private string _choice;
    // private OvershootingDestination _overshotingtarget = null; ???
    private Transform _prevTarget;
    protected List<float> _hdr;
    private uint _dirTargets = 0;
    private GazeDestination _currDest = null;
    private Vector3 _stopPos = Vector3.negativeInfinity;



    #endregion

    #region Properties

    #endregion

    #region MonoBehaviour
    protected override void Initialize()
    {
        _hdr = new List<float>();
        base.Initialize();
    }
    public void StartLogDirWalking(GazeDestination g)
    {
        if (_dirTargets == 0)
        {
            StartMasterLog("DW");
            _dirWalking = true;
        }
        _timeStart = Time.time;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
        _estpathlength = 0;
        _recalltime = -1;
        _angularerror = -1;
        _currDest = g;

    }
    public void StopLogDirWalking(GazeDestination g)
    {
        if (_currDest == g)
        {
            _dirTargets++;

            _timeStop = Time.time - _timeStart;
            var values = new List<string>
            {
                "" + _timeStop,
                "" + _angularerror,
                "" + _estpathlength,
                "" + _recalltime
            };
            WriteToCSV("DW" + _dirTargets, values, 1);
            if (_dirTargets == 6)
            {
                StopMasterLog();
                _dirWalking = false;
            }

            _prevTarget = _currDest.transform;
            _currDest = null;
        }
    }

    public void StartLogBackWalking()
    {
        StartMasterLog("B");
        _timeStart = Time.time;
        _backWalking = true;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
        _speeds.Clear();
        _errors = 0;
    }


    internal void StartLogSlope(Destination d)
    {
        _timeStop = float.MinValue;
        _timeStart = Time.time;
        _stopPos = Vector3.negativeInfinity;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
        _errors = 0;
        _stairslopeWalking = true;
    }

    internal void StopLogSlope(Destination d)
    {
        _timeStop = Time.time - _timeStart;
        _stairslopeWalking = false;
        var values = new List<string>
        {
            "" + _timeStop,
            "" + _errors
        };
        WriteToCSV("SL", values, 4);
        Level2Manager.Instance.StartHalfSlope.gameObject.SetActive(true);
        Level2Manager.Instance.StartHalfStairs.gameObject.SetActive(true);
    }

    internal void StartLogHalfStairs(Destination d)
    {
        _choice = "ST";
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
        if (_stairslopeWalking)
        {
            Level2Manager.Instance.StartHalfSlope.gameObject.SetActive(true);
        }
        else
        {
            _stairslopeWalking = true;
            Level2Manager.Instance.EndStairsSlope.gameObject.SetActive(true);
            _errors = 0;
        }
    }

    internal void StopLogStairsSlope(Destination d)
    {
        _timeStop = Time.time - _timeStart;
        _stairslopeWalking = false;

        var values = new List<string>
        {
            "" + _timeStop,
            "" + _choice,
            "" + _errors
        };
        WriteToCSV("STL", values, 4);
        if (Level2Manager.Instance.StartHalfSlope.gameObject.activeSelf)
        {
            Level2Manager.Instance.StartHalfSlope.OnDisabled -= StartLogHalfSlope;
            Level2Manager.Instance.StartHalfSlope.gameObject.SetActive(false);
        }
        else if (Level2Manager.Instance.StartHalfStairs.gameObject.activeSelf)
        {
            Level2Manager.Instance.StartHalfStairs.OnDisabled -= StartLogHalfStairs;
            Level2Manager.Instance.StartHalfStairs.gameObject.SetActive(false);
        }

        StopMasterLog();
    }

    internal void StartLogHalfSlope(Destination d)
    {
        _choice = "SL";
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
        if (_stairslopeWalking)
        {
            Level2Manager.Instance.StartHalfStairs.gameObject.SetActive(true);
        }
        else
        {
            _stairslopeWalking = true;
            Level2Manager.Instance.EndStairsSlope.gameObject.SetActive(true);
            _errors = 0;
        }
    }

    internal void StopLogStair(Destination d)
    {
        _timeStop = Time.time - _timeStart;
        var values = new List<string>
        {
            "" + _timeStop,
            "" + _errors
        };
        WriteToCSV("ST", values, 4);
        _stairslopeWalking = false;
    }

    internal void StartLogStair(Destination d)
    {
        StartMasterLog("S");
        _timeStart = Time.time;
        _timeStop = float.MinValue;
        _stopPos = Vector3.negativeInfinity;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
        _errors = 0;
        _stairslopeWalking = true;
    }

    internal void PlayerFallen()
    {
        if (_fear)
            _errors++;
    }

    public void StopLogBackWalking(Destination d)
    {
        _backWalking = false;
        _timeStop = Time.time - _timeStart;
        var values = new List<string>
        {
            "" + _timeStop,
            "" + GetAverageSpeed(),
            "" + _errors,
            "" + (100 - (_errors / _speeds.Count * 100))
        };
        WriteToCSV("B", values, 2);
        StopMasterLog();
    }

    public void StartLogCurvedWalking(Destination d)
    {
        StartMasterLog("CW");
        _timeStart = Time.time;
        _curvedWalking = true;
        _timeStop = float.MinValue;
        _stopPos = Vector3.negativeInfinity;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
        _speeds.Clear();
        _errors = 0; // # walking interruptions
    }
    public void StopLogCurvedWalking(Destination d)
    {
        _curvedWalking = false;
        _timeStop = Time.time - _timeStart;
        var values = new List<string>
        {
            "" + _timeStop,
            "" + GetAverageSpeed(),
            "" + _errors
        };
        WriteToCSV("CW", values, 3);
        StopMasterLog();
    }

    public void StartLogFear(Destination d)
    {
        StartMasterLog("F");
        _timeStart = Time.time;
        _fear = true;
        _errors = 0;
        _avoidance = 0;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
        _speeds.Clear();
    }
    public void StopLogFear(Destination d)
    {
        _fear = false;
        _timeStop = Time.time - _timeStart;
        var values = new List<string>
        {
            "" + _timeStop,
            "" + GetAverageSpeed(),
            "" + GetAvgHDR(),
            "" + _avoidance,
            "" + _errors
        };
        WriteToCSV("F", values, 5);
        StopMasterLog();
    }

    #endregion

    #region Public Methods

    #endregion

    #region Helper Methods    
    public float GetAvgHDR()
    {
        float v = 0.0f;
        foreach (var s in _hdr)
        {
            v += s;
        }
        return v / _hdr.Count;
    }
    protected override void ComputeStatisticsStep()
    {
        if (_backWalking)
        {
            var t = (Time.time - _lastsample); // compute delta time
            var d = Mathf.Abs(Vector3.Distance(LocomotionManager.Instance.CurrentPlayerController.position, _prevpos)); // compute distance traveled
            var v = d / t; //compute speed
            if (!Level2Manager.Instance.BackwardItem.InteractiveItem.IsOver)
            {
                _errors++;
            }
           
            _speeds.Add(v);
        }
        else if (_curvedWalking)
        {
            var t = (Time.time - _lastsample);
            var currpos = LocomotionManager.Instance.CurrentPlayerController.position;
            if (currpos == _prevpos)
            {
                if (_timeStop == float.MinValue)
                    _timeStop = Time.time;
                else if (Time.time >= _timeStop + Level2Manager.Instance.TimeToStop)
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

            var d = Mathf.Abs(Vector3.Distance(LocomotionManager.Instance.CurrentPlayerController.position, _prevpos));
            var v = d / t; //compute speed

            _speeds.Add(v);
        }
        else if (_fear)
        {
            var t = (Time.time - _lastsample); // compute delta time
            var d = Mathf.Abs(Vector3.Distance(LocomotionManager.Instance.CurrentPlayerController.position, _prevpos));
            var v = d / t; //compute speed

            _speeds.Add(v);

            if (LocomotionManager.Instance.CurrentPlayerController.position.x <= Level2Manager.Instance.Edge.position.x)
            {
                var diff = Math.Abs(LocomotionManager.Instance.CurrentPlayerController.position.x - Level2Manager.Instance.Edge.position.x);
                _avoidance += diff * (1 / StatisticsLoggerData.SamplingRate);
            }

            var x = LocomotionManager.Instance.CameraEye.eulerAngles.x;
            if (x > 0)
                _hdr.Add(x);
        }
        else if (_dirWalking)
        {
            var d = Mathf.Abs(Vector3.Distance(LocomotionManager.Instance.CurrentPlayerController.position, _prevpos));
            _estpathlength += d;
            if (d > 0 && _recalltime == -1)
                _recalltime = Time.time - _timeStart;
            if (_estpathlength >= 1 && _angularerror == -1)
            {
                var v1 = LocomotionManager.Instance.CurrentPlayerController.position - _prevpos;
                Vector3 v2 = Vector3.zero;
                if (_dirTargets == 0)
                    v2 = _currDest.transform.position - Level2Manager.Instance.StartDest.transform.position;
                else
                    v2 = _currDest.transform.position - _prevTarget.transform.position;
                _angularerror = Vector3.Angle(v1, v2);
            }

        }
        else if (_stairslopeWalking)
        {
            var t = (Time.time - _lastsample);
            var d = Mathf.Abs(Vector3.Distance(LocomotionManager.Instance.CurrentPlayerController.position, _prevpos));
            var v = d / t; //compute speed

            var currpos = LocomotionManager.Instance.CurrentPlayerController.position;
            if (currpos == _prevpos)
            {
                if (_timeStop == float.MinValue)
                    _timeStop = Time.time;
                else if (Time.time >= _timeStop + Level2Manager.Instance.TimeToStop)
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
