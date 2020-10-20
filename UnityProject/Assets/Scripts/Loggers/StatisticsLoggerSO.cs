
/*
 * Custom template by Gabriele P.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelN LoggerData", menuName = "StatisticsLoggerData/LevelN", order = 1)]
public class StatisticsLoggerSO : ScriptableObject
{
    #region Editor Visible

    [SerializeField] private string _scenarioName="undefined";
    [SerializeField] private List<LogFile> _logFiles = new List<LogFile>(new LogFile[] { new LogFile("Master", StatisticsLoggerBase.MasterHeader()) });
    [SerializeField] [Range(0,90)]protected float _samplingRate = 60.0f;
    public const string CSV_SEPARATOR = ";";

    #endregion

    #region Properties

    public String ScenarioName
    {
        get => _scenarioName;
        protected set => _scenarioName = value;
    }

    public float SamplingRate
    {
        get => _samplingRate;
        protected set => _samplingRate = value;
    }

    public List<LogFile> LogFiles
    {
        get => _logFiles;
        set => _logFiles = value;
    }

    #endregion

}
