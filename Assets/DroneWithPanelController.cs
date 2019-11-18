
/*
 * Custom template by F. Gabriele Pratticò {filippogabriele.prattico@polito.it}
 */
using System.Collections;
using System.Collections.Generic;
using PrattiToolkit;
using UnityEngine;
using UnityEngine.Assertions;

public class DroneWithPanelController : MonoBehaviour
{
    #region Events

    #endregion

    #region Editor Visible

    [SerializeField] [Range(0, 1)] private float _robotMoveSmoothing = 0.3f;
    [SerializeField] private ColliderEventsListener ArenaTrigger;

    #endregion

    #region Private Members and Constants

    private float _calibratedControllerDistance = 2;
    private CharacterController _currentCharacterController;
    private MIPanelController _panelController;

    private Vector3 _dir;

    #endregion

    #region Properties

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _calibratedControllerDistance = LocomotionManager.Instance.CalibrationData.ControllerDistance;
        _panelController = GetComponentInChildren<MIPanelController>();
        _currentCharacterController =
            LocomotionManager.Instance.CurrentPlayerController.GetComponent<CharacterController>();
        Assert.IsNotNull(_panelController);
        Assert.IsNotNull(_currentCharacterController);
        
    }


    void Update()
    {
        MoveRobot();
    }

    #endregion

    #region Public Methods

    #endregion

    #region Helper Methods

    private void MoveRobot()
    {
        _dir = _currentCharacterController.velocity.normalized;
    }

    #endregion

    #region Events Callbacks

    #endregion

    #region Coroutines

    #endregion

}
