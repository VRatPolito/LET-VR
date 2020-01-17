
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
    [SerializeField] private List<String> _logFileNames = new List<string>();
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

    public List<string> LogFileNames
    {
        get => _logFileNames;
        set => _logFileNames = value;
    }

    #endregion

}
