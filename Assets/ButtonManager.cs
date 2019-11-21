using PrattiToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonManager : MonoBehaviour
{
    [SerializeField]
    ColliderEventsListener _collider;
    // Start is called before the first frame update
    [SerializeField]
    GenericItem _buttonTarget;
    Vector3 _buttonTargetPos;
    Quaternion _buttonTargetRot;
    ControllerHand _targetPressedByHand = ControllerHand.Invalid;
    [SerializeField]
    Transform _button;
    [SerializeField]
    Vector3 _buttonAxis = Vector3.forward;
    [SerializeField]
    float _restPos = -0.1f;
    [SerializeField]
    float _pushPos = -0.3f;
    Vector3 _startPos;
    Collider _leftHandInArea, _rightHandInArea;
    bool _pushed;
    public UnityEvent OnButtonPushed;
    AudioSource _source;
    // Start is called before the first frame update
    void Start()
    {
        _source = GetComponent<AudioSource>();
        _buttonTargetPos = _buttonTarget.transform.localPosition;
        _buttonTargetRot = _buttonTarget.transform.localRotation;
        _collider.OnTriggerEnterAction += ButtonTriggerEnter;
        _collider.OnTriggerExitAction += ButtonTriggerExit;
        //_arm.transform.localEulerAngles = _restAngle * _armAxis;
        _button.transform.localPosition = MoveButtonOnAxis(_button.transform.localPosition, _restPos, _buttonAxis);
        _startPos = _button.transform.localPosition;
    }

    private static Vector3 MoveButtonOnAxis(Vector3 pos, float target, Vector3 axis)
    {
        return (target * axis) + MultiplyVectors(pos, FlipVector(axis));
    }
    

    public static Vector3 FlipVector(Vector3 v)
    {
        return new Vector3(Flip(v.x), Flip(v.y), Flip(v.z));
    }

    public static float Flip(float x)
    {
        if (x == 0)
            return 1;
        return 0;
    }

    public static Vector3 MultiplyVectors(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static float VectorSum(Vector3 a)
    {
        return a.x + a.y + a.z;
    }


    private void ButtonTriggerExit(Collider c)
    {
        if (_leftHandInArea == c)
            _leftHandInArea = null;
        else if (_rightHandInArea == c)
            _rightHandInArea = null;
    }

    public void OnTargetGrabbed(ItemController i, ControllerHand hand)
    {
        _buttonTarget.transform.parent = null;
        _targetPressedByHand = hand;
    }
    public void OnTargetDropped(ItemController i, ControllerHand hand)
    {
        _targetPressedByHand = ControllerHand.Invalid;
        _buttonTarget.transform.parent = transform;
        _buttonTarget.transform.localPosition = _buttonTargetPos;
        _buttonTarget.transform.localRotation = _buttonTargetRot;
    }
    private void ButtonTriggerEnter(Collider c)
    {
        if (c.tag == "Controller")
        {
            var m = c.GetComponent<ControllerManager>();
            if (m.Hand == ControllerHand.LeftHand)
            {
                _leftHandInArea = c;
                if (LocomotionManager.Instance.CurrentPlayerController.GetComponent<InputManagement>().IsLeftTriggerClicked)
                    LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>().GrabItem(_buttonTarget, m.Hand, false);
            }
            else if (m.Hand == ControllerHand.RightHand)
            {
                _rightHandInArea = c;
                if (LocomotionManager.Instance.CurrentPlayerController.GetComponent<InputManagement>().IsRightTriggerClicked)
                    LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>().GrabItem(_buttonTarget, m.Hand, false);
            }
        }
    }

    public void ResetPush()
    {
        _pushed = false;
        _buttonTarget.CanInteract(true, LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>());
    }

    private void Update()
    {
        if (!_pushed)
        {
            if ((_targetPressedByHand == ControllerHand.LeftHand && !_leftHandInArea)
            || (_targetPressedByHand == ControllerHand.RightHand && !_rightHandInArea))
            {
                LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>().DropItem(_buttonTarget.transform, true, false);
            }
            Vector3 targetPos = _startPos;
            if (_targetPressedByHand != ControllerHand.Invalid)
            {
                targetPos = CheckLimits(_buttonTarget.transform.localPosition) ;
                /*if (_buttonTargetPos != _button.localPosition)
                    VibrateHand(_targetPressedByHand);*/
            }
            _button.localPosition = Vector3.Lerp(_button.localPosition, targetPos, .5f);
            targetPos = CheckLimits(_button.localPosition);

            if (GetSingleComponent(targetPos) == _pushPos)
            {
                _pushed = true;
                _source.Play();
                LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>().DropItem(_buttonTarget.transform, true, false);
                _buttonTarget.CanInteract(false, LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>());
                OnButtonPushed?.Invoke();
            }
        }
    }

    private void VibrateHand(ControllerHand hand)
    {
        var c = LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>();
        if (hand == ControllerHand.LeftHand)
            c.LeftController.GetComponent<VibrationController>().ShortVibration();
        else if (hand == ControllerHand.RightHand)
            c.RightController.GetComponent<VibrationController>().ShortVibration();
    }

    private float GetSingleComponent(Vector3 pos)
    {
       return VectorSum(MultiplyVectors(pos, _buttonAxis));
    }

    private Vector3 CheckLimits(Vector3 pos)
    {
        float newpos = GetSingleComponent(pos);
        if (newpos < _restPos)
            newpos = _restPos;
        else if (newpos > _pushPos)
            newpos = _pushPos;

        return (newpos * _buttonAxis) + MultiplyVectors(pos, FlipVector(_buttonAxis));
    }
}
