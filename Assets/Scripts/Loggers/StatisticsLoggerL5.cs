
/*
 * Custom template by Gabriele P.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Level5Manager))]
public class StatisticsLoggerL5 : StatisticsLoggerBase
{
    #region Events

    #endregion

    #region Editor Visible

    #endregion

    #region Private Members and Constants

    private float _timeStart, _timeStop;
    private uint  _grabtask;
    private bool _grabbing, _manipulation;
    private int _itemCollisions;
    #endregion

    #region Properties

    #endregion

    #region MonoBehaviour

    protected override void Initialize()
    {
        base.Initialize();
    }

    public void StartLogGrabbing()
    {
        if (_grabtask == 0)
        {
            LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>().OnDrop += LogDrop;

            StartMasterLog("G");
        }

        _timeStart = Time.time;
        Collisions = 0;
        _itemCollisions = 0;
        switch (_grabtask)
        {
            case 0:
                _errors = -1;
                break;
            case 1:
            case 2:
                _errors = -2;
                break;
        }
        _speeds.Clear();
        _grabtask++;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
        _grabbing = true;
    }

    public void StartLogManipulation()
    {
        _timeStart = Time.time;

        StartMasterLog("M");
        _manipulation = true;
    }

    private void LogDrop()
    {
        if (_grabbing)
            _errors++;
    }
    public override void LogCollisions(HitType type)
    {
        if (_grabbing)
        {
            if (type == HitType.Player)
                Collisions++;
            else if (type == HitType.Item)
                _itemCollisions++;
        }
        LocomotionManager.Instance.LeftController.GetComponent<VibrationController>().ShortVibration(.5f);
        LocomotionManager.Instance.RightController.GetComponent<VibrationController>().ShortVibration(.5f);
    }

    public void StopLogGrabbing()
    {
        _timeStop = Time.time - _timeStart;
        var values = new List<string>
        {
            "" + _timeStop,
            "" + GetAverageSpeed(),
            "" + _errors,
            "" + Collisions,
            "" + _itemCollisions
        };
        WriteToCSV("G"+_grabtask, values, 1);
        if (_grabtask == 3)
        {
            LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>().OnDrop -= LogDrop;
            StopMasterLog();
        }

        _grabbing = false;
    }
    public void StopLogManipulation()
    {
        _timeStop = Time.time - _timeStart;
        var values = new List<string>
        {
            "" + _timeStop
        };
        WriteToCSV("M", values, 2);
        StopMasterLog();
        _manipulation = false;
    }

    #endregion

    #region Public Methods

    #endregion

    #region Helper Methods
    protected override void ComputeStatisticsStep()
    {
        if (_grabbing)
        {
            var t = (Time.time - _lastsample); // compute delta time
            var d = Mathf.Abs(Vector3.Distance(LocomotionManager.Instance.CurrentPlayerController.position, _prevpos)); // compute distance traveled
            var v = d / t; //compute speed

            _speeds.Add(v);
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
