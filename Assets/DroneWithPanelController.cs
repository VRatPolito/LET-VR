
/*
 * Custom template by F. Gabriele Pratticò {filippogabriele.prattico@polito.it}
 */
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PrattiToolkit;
using UnityEngine;
using UnityEngine.Assertions;

public class DroneWithPanelController : MonoBehaviour
{

    public enum DroneStatus : byte
    {
        IDLE,
        WANDER,
        ESCAPE,
        RESET
    }

    #region Events

    #endregion

    #region Editor Visible

    [SerializeField] [Range(0, 1)] private float _robotMoveSmoothing = 0.3f;
    [SerializeField] [Range(0, 2)] private float _wanderingSpeed = 2f;
    [SerializeField] [Range(0, 360)] private float _maxWanderingAngleVariation = 30f;
    [SerializeField] private ColliderEventsListener ArenaTrigger;
    [SerializeField] [Range(0, 20)] private float _wanderingArea=5f;

    #endregion

    #region Private Members and Constants

    private float _calibratedControllerDistance = 2;
    private CharacterController _currentCharacterController;
    private MIPanelController _panelController;

    private Vector3 _dir;
    private DroneStatus _status = DroneStatus.IDLE;

    #endregion

    #region Properties

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _calibratedControllerDistance = LocomotionManager.Instance.CalibrationData.ControllerDistance;
        _panelController = GetComponentInChildren<MIPanelController>();
        Assert.IsNotNull(_panelController);

        _status = DroneStatus.WANDER;
        StartCoroutine(Wander());

    }

    void Start()
    {
        _currentCharacterController =
            LocomotionManager.Instance.CurrentPlayerController.GetComponent<CharacterController>();
        Assert.IsNotNull(_currentCharacterController);
    }


    void Update()
    {
        //MoveRobot();
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

    private IEnumerator Wander()
    {
        float heading = transform.rotation.y;
        float wanderSpeed = _wanderingSpeed;
        Vector3 targetRotation = transform.rotation.eulerAngles;
        float headingTimeout = Time.time - 1;
        float areaTimeout = Time.time - 1;

        while (_status == DroneStatus.WANDER)
        {
            if (Time.time > areaTimeout  && (this.transform.position-ArenaTrigger.transform.position).magnitude > _wanderingArea)
            {
                areaTimeout = Time.time + 2;
                headingTimeout = Time.time + Random.Range(2f, 4f);
                targetRotation = ArenaTrigger.transform.position;
                targetRotation.y = transform.position.y;
                transform.DOLookAt(targetRotation, 1);
            }

            else if (Time.time > headingTimeout)
            {
                var floor = Mathf.Clamp(heading - _maxWanderingAngleVariation, 0, 360);
                var ceil = Mathf.Clamp(heading + _maxWanderingAngleVariation, 0, 360);
                heading = Random.Range(floor, ceil);
                wanderSpeed = Random.Range(0.01f, wanderSpeed);
                targetRotation = new Vector3(0, heading, 0);

                headingTimeout = Time.time + Random.Range(0.5f, 4f);
                transform.DOBlendableRotateBy(targetRotation - transform.eulerAngles, Mathf.Max((targetRotation.y - transform.eulerAngles.y)/5, headingTimeout-Time.deltaTime)).SetEase(Ease.InOutQuad);
            }


            //transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation,
            //    Time.deltaTime * _robotMoveSmoothing);
            var forward = transform.TransformDirection(Vector3.forward);
            transform.DOBlendableMoveBy(forward * _wanderingSpeed * Time.fixedDeltaTime, _robotMoveSmoothing);
            yield return new WaitForFixedUpdate();

        }

        yield return null;
    }

    #endregion

}
