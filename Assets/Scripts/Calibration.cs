using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Calibration : MonoBehaviour
{

    #region Events

    #endregion

    #region Editor Visible

    [SerializeField] private KeyCode _startCalibrationKey = KeyCode.Space;

    #endregion

    #region Private Members and Constants

    private bool _calibrated = false;
    private bool _playerinside = false;

    private AudioSource _calibrationDoneAudioSource;

    #endregion

    #region Properties

    public bool Calibrated { get { return _calibrated; } }

    #endregion

    #region MonoBehaviour

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            _playerinside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            _playerinside = false;
    }

    private void Awake()
    {
        _calibrationDoneAudioSource = GetComponent<AudioSource>();
        Assert.IsNotNull(_calibrationDoneAudioSource);
        _calibrationDoneAudioSource.spatialize = false;
    }

    void Update()
    {
        if (_playerinside && (Input.GetKeyDown(_startCalibrationKey) || (LocomotionManager.Instance.Locomotion == ControllerType.RealWalk && LocomotionManager.Instance.CurrentPlayerController != null && (LocomotionManager.Instance.CurrentPlayerController.GetComponent<InputManagement>().IsLeftTriggerClicked && LocomotionManager.Instance.CurrentPlayerController.GetComponent<InputManagement>().IsRightTriggerClicked))))
            Calibrate();
    }

    #endregion

    #region Public Methods

    #endregion

    #region Helper Methods

    private void Calibrate()
    {
        LocomotionManager.Instance.CalibrationData.HeadHeight = LocomotionManager.Instance.CameraEye.localPosition.y;
        LocomotionManager.Instance.CalibrationData.ControllerDistance = Vector3.Distance(LocomotionManager.Instance.LeftController.position, LocomotionManager.Instance.RightController.position);
        LocomotionManager.Instance.CalibrationData.SavePersistent();
        _calibrated = true;

        LocomotionManager.Instance.CurrentUIController.ShowCalibrationIcon(1.5f);
        _calibrationDoneAudioSource.PlayOneShot(_calibrationDoneAudioSource.clip, 0.7f);
        Debug.Log($"Calibration Ended with Value: {LocomotionManager.Instance.CalibrationData}");
    }

    #endregion

    #region Events Callbacks

    #endregion

    #region Coroutines

    #endregion
}
