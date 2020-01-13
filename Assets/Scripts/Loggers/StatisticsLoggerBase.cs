/*
 * Custom template by Gabriele P.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PrattiToolkit;
using UnityEngine;


public enum PathDevAxis
{
    X,
    Y,
    Z
};

public interface IStatisticsLogger
{
    void Ticker();
}

public abstract class StatisticsLoggerBase : MonoBehaviour, IStatisticsLogger
{
    #region Events

    public event Action<int> OnLogFinalized;

    #endregion

    #region Editor Visible

    [Expandable] public StatisticsLoggerSO StatisticsLoggerData;

    #endregion

    #region Private Members and Constants

    private StringBuilder _sb = new StringBuilder();
    private List<String> _logFilePaths = new List<string>();
    protected bool _masterlog = false;
    protected string _masterlogtype = "";
    protected Vector3 _prevpos;
    protected float _lastsample = float.MinValue;
    protected float _pathDev = 0.0f;

    protected List<Vector3> _positions,
        _rotations,
        _headpositions,
        _headrotations,
        _lefthandpositions,
        _lefthandrotations,
        _righthandpositions,
        _righthandrotations,
        _dirtrackpositions,
        _dirtrackrotations,
        _leftlegpositions,
        _leftlegrotations,
        _rightlegpositions,
        _rightlegrotations,
        _targetpositions;

    protected List<float> _gazewalkangles;
    protected List<object> _locks;
    private List<List<string>> _allValues;

    #endregion

    #region Properties

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        Initialize();
    }

    void Update()
    {
        Ticker();
    }

    #endregion

    #region Public Methods

    public void Ticker()
    {
        if (_lastsample < (Time.time - (1 / StatisticsLoggerData.SamplingRate)))
        {
            ComputeStatisticsStep();
            _lastsample = Time.time;
        }
    }

    #endregion

    #region Helper Methods

    protected virtual void Initialize()
    {
        var u = UnityMainThreadDispatcher.Instance;
        string fileDirectory = Path.Combine(Application.dataPath, "StatisticsData",
            ConfigurationLookUp.Instance.GetString("ParticipantId", "undefined"),
            StatisticsLoggerData.ScenarioName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmssff"));
        if (!Directory.Exists(fileDirectory))
            Directory.CreateDirectory(fileDirectory);

        _locks = new List<object>();

        for (int i = 0; i < StatisticsLoggerData.LogFileNames.Count; i++)
        {
            string fileName = $"{i}_" + StatisticsLoggerData.LogFileNames[i] + "_" +
                              DateTime.Now.ToString("yyyyMMdd_HHmmssff") + ".csv";
            _logFilePaths.Add(Path.Combine(fileDirectory, fileName));
            _locks.Add(new object());
        }

        _positions = new List<Vector3>();
        _rotations = new List<Vector3>();
        _headpositions = new List<Vector3>();
        _headrotations = new List<Vector3>();
        _lefthandpositions = new List<Vector3>();
        _lefthandrotations = new List<Vector3>();
        _righthandpositions = new List<Vector3>();
        _righthandrotations = new List<Vector3>();
        _dirtrackpositions = new List<Vector3>();
        _dirtrackrotations = new List<Vector3>();
        _leftlegpositions = new List<Vector3>();
        _leftlegrotations = new List<Vector3>();
        _rightlegpositions = new List<Vector3>();
        _rightlegrotations = new List<Vector3>();
        _targetpositions = new List<Vector3>();
        _gazewalkangles = new List<float>();

        Debug.Log($"Logger {this.name} Initialized");
    }

    protected async void WriteToCSV(string type, List<string> values, int fileindex)
    {
        var csvSeparator = StatisticsLoggerSO.CSV_SEPARATOR;
        await Task.Run(() =>
        {
            lock (_locks[fileindex])
            {
                _sb.Length = 0;
                _sb.Append(type).Append(csvSeparator);
                _sb.Append(DateTime.Now.ToString("yyyy/MM/dd_HH:mm:ss.fff")).Append(csvSeparator);
                foreach (var s in values)
                {
                    _sb.Append(s).Append(csvSeparator);
                }

                _sb.Append("\n");

                if (!File.Exists(_logFilePaths[fileindex]))
                    File.WriteAllText(_logFilePaths[fileindex], _sb.ToString());
                else
                    File.AppendAllText(_logFilePaths[fileindex], _sb.ToString());
            }

            UnityMainThreadDispatcher.Instance.Enqueue(() => OnLogFinalized.RaiseEvent(fileindex));
        });
        Debug.Log($"Writing {_logFilePaths[fileindex]}");
    }

    protected async void WriteToCSVMulipleLines(string type, List<List<string>> lines, int fileindex)
    {
        var csvSeparator = StatisticsLoggerSO.CSV_SEPARATOR;
        var values = new List<List<string>>(lines);
        await Task.Run(() =>
        {
            lock (_locks[fileindex])
            {
                _sb.Length = 0;
                foreach (var line in values)
                {
                    _sb.Append(type).Append(csvSeparator);
                    _sb.Append(DateTime.Now.ToString("yyyy/MM/dd_HH:mm:ss.fff")).Append(csvSeparator);
                    foreach (var s in line)
                    {
                        _sb.Append(s).Append(csvSeparator);
                    }

                    _sb.Append("\n");
                }

                if (!File.Exists(_logFilePaths[fileindex]))
                {
                    File.WriteAllText(_logFilePaths[fileindex], _sb.ToString());
                }
                else
                    File.AppendAllText(_logFilePaths[fileindex], _sb.ToString());
            }

            UnityMainThreadDispatcher.Instance.Enqueue(() => OnLogFinalized.RaiseEvent(fileindex));
        });
        Debug.Log($"Writing {_logFilePaths[fileindex]}");
    }

    protected void StartMasterLog(string type)
    {
        _masterlog = true;
        _masterlogtype = type;
    }

    protected float GetAverageFloat(ref List<float> list)
    {
        float v = 0.0f;
        foreach (var s in list)
        {
            v += s;
        }

        return v / list.Count;
    }

    protected void StopMasterLog()
    {
        _masterlog = false;
        _allValues = new List<List<string>>();
        for (int i = 0; i < _positions.Count; i++)
        {
            var values = new List<string>
            {
                "" + _positions[i].x,
                "" + _positions[i].y,
                "" + _positions[i].z,
                "" + _rotations[i].x,
                "" + _rotations[i].y,
                "" + _rotations[i].z,
                "" + _headpositions[i].x,
                "" + _headpositions[i].y,
                "" + _headpositions[i].z,
                "" + _headrotations[i].x,
                "" + _headrotations[i].y,
                "" + _headrotations[i].z,
                "" + _lefthandpositions[i].x,
                "" + _lefthandpositions[i].y,
                "" + _lefthandpositions[i].z,
                "" + _lefthandrotations[i].x,
                "" + _lefthandrotations[i].y,
                "" + _lefthandrotations[i].z,
                "" + _righthandpositions[i].x,
                "" + _righthandpositions[i].y,
                "" + _righthandpositions[i].z,
                "" + _righthandrotations[i].x,
                "" + _righthandrotations[i].y,
                "" + _righthandrotations[i].z
            };

            if (LocomotionManager.Instance.Locomotion == ControllerType.FootSwing)
            {
                values.Add("" + _dirtrackpositions[i].x);
                values.Add("" + _dirtrackpositions[i].y);
                values.Add("" + _dirtrackpositions[i].z);
                values.Add("" + _dirtrackrotations[i].x);
                values.Add("" + _dirtrackrotations[i].y);
                values.Add("" + _dirtrackrotations[i].z);

                values.Add("" + _leftlegpositions[i].x);
                values.Add("" + _leftlegpositions[i].y);
                values.Add("" + _leftlegpositions[i].z);
                values.Add("" + _leftlegrotations[i].x);
                values.Add("" + _leftlegrotations[i].y);
                values.Add("" + _leftlegrotations[i].z);

                values.Add("" + _rightlegpositions[i].x);
                values.Add("" + _rightlegpositions[i].y);
                values.Add("" + _rightlegpositions[i].z);
                values.Add("" + _rightlegrotations[i].x);
                values.Add("" + _rightlegrotations[i].y);
                values.Add("" + _rightlegrotations[i].z);
            }

            if (_masterlogtype == "C")
            {
                values.Add("" + _targetpositions[i].x);
                values.Add("" + _targetpositions[i].y);
                values.Add("" + _targetpositions[i].z);
            }
            else if (_masterlogtype == "DG")
            {
                values.Add("" + _gazewalkangles[i]);
            }

            _allValues.Add(values);
        }

        WriteToCSVMulipleLines(_masterlogtype, _allValues, 0);

        _allValues.Clear();
        _targetpositions.Clear();
        _positions.Clear();
        _rotations.Clear();
        _headpositions.Clear();
        _headrotations.Clear();
        _lefthandpositions.Clear();
        _lefthandrotations.Clear();
        _righthandpositions.Clear();
        _righthandrotations.Clear();
        _dirtrackpositions.Clear();
        _dirtrackrotations.Clear();
        _leftlegpositions.Clear();
        _leftlegrotations.Clear();
        _rightlegpositions.Clear();
        _rightlegrotations.Clear();
        _gazewalkangles.Clear();
    }

    protected virtual void ComputeStatisticsStep()
    {
        if (_masterlog)
        {
            _positions.Add(LocomotionManager.Instance.CurrentPlayerController.position);
            _rotations.Add(LocomotionManager.Instance.CurrentPlayerController.eulerAngles);
            _headpositions.Add(LocomotionManager.Instance.CameraEye.localPosition);
            _headrotations.Add(LocomotionManager.Instance.CameraEye.localEulerAngles);
            _lefthandpositions.Add(LocomotionManager.Instance.LeftController.localPosition);
            _lefthandrotations.Add(LocomotionManager.Instance.LeftController.localEulerAngles);
            _righthandpositions.Add(LocomotionManager.Instance.RightController.localPosition);
            _righthandrotations.Add(LocomotionManager.Instance.RightController.localEulerAngles);
            if (LocomotionManager.Instance.Locomotion == ControllerType.FootSwing)
            {
                _dirtrackpositions.Add(LocomotionManager.Instance.DirectionalTracker.localPosition);
                _dirtrackrotations.Add(LocomotionManager.Instance.DirectionalTracker.localEulerAngles);
                _leftlegpositions.Add(LocomotionManager.Instance.LeftTracker.localPosition);
                _leftlegrotations.Add(LocomotionManager.Instance.LeftTracker.localEulerAngles);
                _rightlegpositions.Add(LocomotionManager.Instance.RightTracker.localPosition);
                _rightlegrotations.Add(LocomotionManager.Instance.RightTracker.localEulerAngles);
            }
        }
    }

    public virtual void LogCollisions(HitType type)
    {
    }

    public static float GetPathDev(Transform reference, PathDevAxis axis)
    {
        switch (axis)
        {
            case PathDevAxis.X:
                return Math.Abs(LocomotionManager.Instance.CurrentPlayerController.position.x - reference.position.x);
            case PathDevAxis.Y:
                return Math.Abs(LocomotionManager.Instance.CurrentPlayerController.position.y - reference.position.y);
            case PathDevAxis.Z:
                return Math.Abs(LocomotionManager.Instance.CurrentPlayerController.position.z - reference.position.z);
        }

        return -1;
    }

    #endregion

    #region Coroutines

    #endregion

    #region Events Callbacks

    #endregion
}