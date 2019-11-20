using PrattiToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LeverManager : MonoBehaviour
{
    [SerializeField]
    ColliderEventsListener _collider;
    // Start is called before the first frame update
    [SerializeField]
    GenericItem _leverTarget;
    Vector3 _leverTargetPos;
    Quaternion _leverTargetRot;
    ControllerHand _targetGrabbedByHand = ControllerHand.Invalid;
    [SerializeField]
    Transform _arm;
    [SerializeField]
    Vector3 _armAxis = Vector3.right;
    [SerializeField]
    float _restAngle = -160;
    [SerializeField]
    float _pushAngle = -40;
    Collider _leftHandInArea, _rightHandInArea;
    bool _pushed;
    public UnityEvent OnLeverPushed;

    // Start is called before the first frame update
    void Awake()
    {
        _leverTargetPos = _leverTarget.transform.localPosition;
        _leverTargetRot = _leverTarget.transform.localRotation;
        _collider.OnTriggerEnterAction += LevaTriggerEnter;
        _collider.OnTriggerExitAction += LevaTriggerExit;
        //_arm.transform.localEulerAngles = _restAngle * _armAxis;
        _arm.transform.localRotation = Quaternion.AngleAxis(_restAngle, _armAxis);
    }

    private void LevaTriggerExit(Collider c)
    {
        if (_leftHandInArea == c)
            _leftHandInArea = null;
        else if (_rightHandInArea == c)
            _rightHandInArea = null;
    }

    public void OnTargetGrabbed(ItemController i, ControllerHand hand)
    {
        _targetGrabbedByHand = hand;
    }
    public void OnTargetDropped(ItemController i, ControllerHand hand)
    {
        _targetGrabbedByHand = ControllerHand.Invalid;
    }
    private void LevaTriggerEnter(Collider c)
    {
        if (c.tag == "Controller")
        {
            var m = c.GetComponent<ControllerManager>();
            if (m.Hand == ControllerHand.LeftHand)
                _leftHandInArea = c;
            if (m.Hand == ControllerHand.RightHand)
                _rightHandInArea = c;
        }
    }

    private void Update()
    {
        if (!_pushed)
        {
            if ((_targetGrabbedByHand == ControllerHand.LeftHand && !_leftHandInArea)
            || (_targetGrabbedByHand == ControllerHand.RightHand && !_rightHandInArea))
            {
                LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>().DropItem(_leverTarget.transform, true);

                _leverTarget.transform.localPosition = _leverTargetPos;
                _leverTarget.transform.localRotation = _leverTargetRot;
            }

            var dirToTarget = _arm.position - _leverTarget.transform.position;

            var rot = Quaternion.FromToRotation(_arm.forward, dirToTarget);
            var targetRot = rot * _arm.localRotation;
            Vector3 axis;
            float angle;
            targetRot.ToAngleAxis(out angle, out axis);
            angle = CheckLimits(angle);
            targetRot = Quaternion.AngleAxis(angle, axis);

            _arm.localRotation = Quaternion.Slerp(_arm.localRotation, targetRot, .5f);

            _arm.localRotation.ToAngleAxis(out angle, out axis);
            if (angle == _pushAngle)
            {
                _pushed = true;
                OnLeverPushed?.Invoke();
            }
        }
    }

    private float CheckLimits(float angle)
    {
        float newangle = angle;
        if(_restAngle <= _pushAngle)
        {
            if (angle < _restAngle)
                newangle = _restAngle;
            else if (angle > _pushAngle)
                newangle = _pushAngle;
        }
        else
        {
            if (angle > _restAngle)
                newangle = _restAngle;
            else if (angle < _pushAngle)
                newangle = _pushAngle;
        }
        return newangle;
    }
}
