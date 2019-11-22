
/*
 * Custom template by Gabriele P.
 */
using System.Collections;
using System.Collections.Generic;
using PrattiToolkit;
using UnityEngine;

[RequireComponent(typeof(Level4Manager))]
public class StatisticsLoggerL4 : StatisticsLoggerBase
{
    #region Events

    #endregion

    #region Editor Visible

    #endregion

    #region Private Members and Constants
    private float _timeStart, _timeStop;
    private bool _agility = false, _headShooter = false, _bodyShooter = false;
    private Vector3 _stopPos = Vector3.negativeInfinity;
    #endregion

    #region Properties

    #endregion

    #region MonoBehaviour

    public void StartLogAgility()
    {
        StartMasterLog("AG");
        _timeStart = Time.time;
        _agility = true;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
        _speeds.Clear();
    }
    public void StopLogAgility()
    {
        _agility = false;
        _timeStop = Time.time - _timeStart;
        var values = new List<string>
        {
            "" + _timeStop,
            "" + GetAverageSpeed(),
            "" + _errors,
            "" + Collisions
        };
        WriteToCSV("AG", values, 1);
        StopMasterLog();        
    }

    public void StartLogHeadShooter()
    {
        StartMasterLog("HS");
        _headShooter = true;
    }

    public void StopLogHeadShooter()
    {
        _headShooter = false;
        var values = new List<string>
        { 
            "" + Level4Manager.Instance.HeadShooterHits
        };
        WriteToCSV("HS", values, 2);
        StopMasterLog();
    }

    public void StartLogBodyShooter()
    {
        StartMasterLog("BS");
        _bodyShooter = true;
    }

    public void StopLogBodyShooter()
    {
        _bodyShooter = false;
        var values = new List<string>
        {
            "" + Level4Manager.Instance.BodyShooterHits
        };
        WriteToCSV("BS", values, 4);
        StopMasterLog();
    }

    public override void LogCollisions(HitType type)
    {
        Collisions++;
        LocomotionManager.Instance.LeftController.GetComponent<VibrationController>().ShortVibration(.7f);
        LocomotionManager.Instance.RightController.GetComponent<VibrationController>().ShortVibration(.7f);
    }

    #endregion

    #region Public Methods

    #endregion

    #region Helper Methods

    protected override void ComputeStatisticsStep()
    {
        if (_agility)
        {
            // compute speed
            var t = (Time.time - _lastsample);
            var d = Mathf.Abs(Vector3.Distance(LocomotionManager.Instance.CurrentPlayerController.position, _prevpos));
            var v = d / t;
            _speeds.Add(v);

            var currpos = LocomotionManager.Instance.CurrentPlayerController.position;
            if (currpos == _prevpos)
            {
                if (_timeStop == float.MinValue)
                    _timeStop = Time.time;
                else if (Time.time >= _timeStop + Level4Manager.Instance.TimeToStop)
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

            _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
            
        }

        base.ComputeStatisticsStep();

    }

    #endregion

    #region Events Callbacks

    #endregion

    #region Coroutines

    #endregion
}
