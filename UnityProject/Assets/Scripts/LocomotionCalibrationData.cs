
/*
 * Custom template by Gabriele P.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using PrattiToolkit;
using UnityEngine;

//[CreateAssetMenu(fileName = "LocomotionCalibrationData", menuName = "LocomotionCalibrationData/Generic", order = 1)]
[Serializable]
public class LocomotionCalibrationData
{
    #region Events

    #endregion

    #region Editor Visible

    [SerializeField] private float _controllerDistance = 1.5f;
    [SerializeField] private float _headHeight = 1.7f;

    #endregion
	
	#region Private Members and Constants
		
	#endregion

    #region Properties

    public float HeadHeight
    {
        get { return _headHeight; }
        set { _headHeight = value; }
    }

    public float ControllerDistance
    {
        get { return _controllerDistance; }
        set { _controllerDistance = value; }
    }

    public string DefaultDataPath => PersistentSaveLoad.GetDefaultDataPath("LET_VR", "calibrationData.pgd");

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        SavePersistent();
    }

    #endregion

    #region Public Methods

    public void SavePersistent()
    {
       PersistentSaveLoad.Save<LocomotionCalibrationData>(this, DefaultDataPath, PersistentSaveLoad.SerializationType.Json);
    }

    public override string ToString()
    {
        return $"HeadHeight={HeadHeight:F1}\nControllerDistance={ControllerDistance:F1}";
    }

    #endregion

    #region Helper Methods

    #endregion

    #region Events Callbacks

    #endregion

    #region Coroutines

    #endregion
}
