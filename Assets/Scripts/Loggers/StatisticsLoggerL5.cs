
/*
 * Custom template by Gabriele P.
 */
using System;
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
    protected List<float> _setupPrecision = new List<float>();
    protected List<float> _towerPrecision = new List<float>();
    private float _timeStart, _timeStop;
    private uint  _grabtask;
    private bool _grabbing;
    private bool _movingInteraction;
	private bool _playerInside = false;
	private uint _counter = 0;
	private uint _inCounter = 0;
    private int _numItemFalls = 0;
    private int _numItemColl = 0;
    private int _numBodyColl = 0;
    private int _numIntErrors = 0;

    #endregion

    #region Properties

    #endregion

    #region MonoBehaviour

    protected override void Initialize()
    {
        base.Initialize();
    }

    public void StartLogGrabbing(Destination d)
    {
        if (_grabtask == 0)
        {
            StartMasterLog("G");
            LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>().OnDrop += LogDrop;
        }

        _timeStart = Time.time;
        _numBodyColl = 0;
        _numItemColl = 0;
        switch (_grabtask)
        {
            case 0:
                _numItemFalls = -1;
                break;
            case 1:
            case 2:
                _numItemFalls = -2;
                break;
        }
        _grabtask++;
        _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
        _grabbing = true;
    }

    public void StartLogManipulation(Destination d)
    {
        _timeStart = Time.time;
        StartMasterLog("M");
    }
    public void PlayerOutRange()
    {
		_playerInside = false;
    }  
	
	public void PlayerInRange()
    {
		_playerInside = true;
    }

    public void LogInteractionError()
    {
        _numIntErrors++;
    }
    private void LogDrop(GenericItem i)
    {
        if (_grabbing || _movingInteraction)
            _numItemFalls++;
    }
    public override void LogCollisions(HitType type)
    {
        if (_grabbing)
        {
            if (type == HitType.Player)
                _numBodyColl++;
            else if (type == HitType.Item)
                _numItemColl++;
        }
        LocomotionManager.Instance.LeftController.GetComponent<VibrationController>().ShortVibration(.5f);
        LocomotionManager.Instance.RightController.GetComponent<VibrationController>().ShortVibration(.5f);
    }

    public void StopLogGrabbing(Destination d)
    {
        _timeStop = Time.time - _timeStart;
        var values = new List<string>
        {
            "" + _timeStop,
            "" + _numItemFalls,
            "" + _numBodyColl,
            "" + _numItemColl
        };
        WriteToCSV("G"+_grabtask, values, 1);
        if (_grabtask == 3)
            StopMasterLog();

        _grabbing = false;
    }

    public void StopLogManipulation(Destination d)
    {
        _timeStop = Time.time - _timeStart;
        var SetupPrecision = GetAvgSetupPrecision();
        var TowerPrecision = GetAvgTowerPrecision();
        var values = new List<string>
        {
            "" + _timeStop,
            "" + SetupPrecision,
            "" + TowerPrecision
        };
        WriteToCSV("M", values, 2);
        StopMasterLog();
    }

    internal void LogSetupPrecision(float pos, float rot)
    {
        _setupPrecision.Add((pos + rot) / 2);
    }
    internal void LogTowerPrecision(float pos, float rot)
    {
        _towerPrecision.Add((pos + rot) / 2);
    }

    public void StartLogInteractionInMotion()
    {
        _movingInteraction = true;
        _timeStart = Time.time;
        _numItemFalls = 0;

        StartMasterLog("IM");
    }

    public void StopLogInteractionInMotion()
    {
        _movingInteraction = false;
        _timeStop = Time.time - _timeStart;
        var TimeClose = GetPercTimeClose();
        var NumErrors = _numItemFalls + _numIntErrors;
        var values = new List<string>
        {
            "" + _timeStop,
            "" + TimeClose,
            "" + NumErrors
        };
        WriteToCSV("IM", values, 3);
        StopMasterLog();
    }

    #endregion

    #region Public Methods

    #endregion

    #region Helper Methods     
    private float GetPercTimeClose()
    {
        if (_inCounter == 0) return 0;
        return ((float)_inCounter / (float)(_counter)) * 100;
    }	
    protected float GetAvgSetupPrecision()
    {
        float v = 0.0f;
        foreach (var s in _setupPrecision)
        {
            v += s;
        }
        return v / _setupPrecision.Count;
    }  
 
    protected float GetAvgTowerPrecision()
    {
        float v = 0.0f;
        foreach (var s in _towerPrecision)
        {
            v += s;
        }
        return v / _towerPrecision.Count;
    }
    protected override void ComputeStatisticsStep()
    {
		if (_movingInteraction)
        {
            _counter++;
            if (_playerInside)
                _inCounter++;
        }
        base.ComputeStatisticsStep();
    }

    #endregion

    #region Events Callbacks

    #endregion

    #region Coroutines

    #endregion

}
