using System;
using System.Collections;
using System.Collections.Generic;
using PrattiToolkit;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;

public class JoystickMovement : MonoBehaviour
{
    enum InputMode
    {
        Touch,
        Click
    };

    //private const float DETECTION_TH = 0.2f;
    [SerializeField] private InputMode _mode = InputMode.Click;
    [SerializeField] private float _walkSpeed = 5;
    [SerializeField] private float _runSpeed = 8;

    public bool Blocked
    {
        get { return _blocked; }
        set
        {
            if (value && !_blocked)
            {
                _bigArrow.gameObject.SetActive(false);
                _lastSpeed = 0;
                _lastDir = Quaternion.identity;
                _factor = 0;
                _rightPadDown = false;
                _leftPadDown = false;
            }

            _blocked = value;
        }
    }

    public float WalkSpeed => _walkSpeed;

    public float RunSpeed => _runSpeed;

    bool _blocked = false;

    [Space] [SerializeField] Transform _bigArrow;
    CharacterController _target;
    InputManagement _input;
    bool _leftPadDown = false;
    bool _rightPadDown = false;
    float _factor = 0;
    float _smoothStep = 0.05f;
    float _lastSpeed = 0;
    Quaternion _lastDir = Quaternion.identity;
    PadEventArgs tp;
    float t;
    Vector3 moveDirection;

    void Awake()
    {
        _input = GetComponent<InputManagement>();
        _target = GetComponent<CharacterController>();
        switch (_mode)
        {
            case InputMode.Click:
                _input.OnLeftPadPressed += LeftPadDown;
                _input.OnLeftPadUnpressed += LeftPadUp;
                _input.OnRightPadPressed += RightPadDown;
                _input.OnRightPadUnpressed += RightPadUp;

                break;

            case InputMode.Touch:
                _input.OnLeftPadTouched += LeftPadDown;
                _input.OnLeftPadUntouched += LeftPadUp;
                _input.OnRightPadTouched += RightPadDown;
                _input.OnRightPadUntouched += RightPadUp;
                break;
        }

        _bigArrow.gameObject.SetActive(false);
    }

    private void RightPadUp(object sender)
    {
        if (!Blocked)
        {
            _rightPadDown = false;
            switch (_mode)
            {
                case InputMode.Click:
                    if (_input.IsLeftPadPressed)
                        _leftPadDown = true;
                    else if (_bigArrow != null)
                        _bigArrow.gameObject.SetActive(false);
                    break;
                case InputMode.Touch:
                    if (_input.IsLeftPadTouched)
                        _leftPadDown = true;
                    else if (_bigArrow != null)
                        _bigArrow.gameObject.SetActive(false);
                    break;
            }
        }
    }

    private void LeftPadUp(object sender)
    {
        if (!Blocked)
        {
            _leftPadDown = false;
            switch (_mode)
            {
                case InputMode.Click:
                    if (_input.IsRightPadPressed)
                        _rightPadDown = true;
                    else if (_bigArrow != null)
                        _bigArrow.gameObject.SetActive(false);
                    break;
                case InputMode.Touch:
                    if (_input.IsRightPadTouched)
                        _rightPadDown = true;
                    else if (_bigArrow != null)
                        _bigArrow.gameObject.SetActive(false);
                    break;
            }
        }
    }

    private void RightPadDown(object sender, PadEventArgs e)
    {
        if (!Blocked && !_leftPadDown)
        {
            _rightPadDown = true;
            if (_bigArrow != null)
                _bigArrow.gameObject.SetActive(true);
        }
    }

    private void LeftPadDown(object sender, PadEventArgs e)
    {
        if (!Blocked && !_rightPadDown)
        {
            _leftPadDown = true;
            if (_bigArrow != null)
                _bigArrow.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (!Blocked)
        {
            if (_bigArrow != null)
            {
                if (_leftPadDown)
                    _bigArrow.localEulerAngles = new Vector3(0, _input.LeftController.transform.localEulerAngles.y, 0);
                else if (_rightPadDown)
                    _bigArrow.localEulerAngles = new Vector3(0, _input.RightController.transform.localEulerAngles.y, 0);
            }

            moveDirection = Vector3.zero;
            if (_target.isGrounded)
                moveDirection = CalculateMotion();
            moveDirection = ApplyGravity(moveDirection);
            _target.Move(moveDirection);
        }
    }

    private Vector3 ApplyGravity(Vector3 moveDirection)
    {
        moveDirection.y -= 9.81f * Time.deltaTime;
        return moveDirection;
    }

    private Vector3 CalculateMotion()
    {
        Vector3 movement = Vector3.zero;
        if (_leftPadDown)
        {
            _lastDir = _input.LeftController.transform.rotation;
            if (_input.IsLeftGripped)
                _lastSpeed = RunSpeed;
            else
            {
                tp = _input.LeftPadAxis;
                t = (tp.padY + 1) / 2;
                _lastSpeed = Mathf.Lerp(0, _walkSpeed, t);
            }

            movement = _lastDir.getForwardXZ(Time.deltaTime * _factor * _lastSpeed);
            _factor += _smoothStep;
            if (_factor > 1)
                _factor = 1;
        }
        else if (_rightPadDown)
        {
            _lastDir = _input.RightController.transform.rotation;
            if (_input.IsRightGripped)
                _lastSpeed = RunSpeed;
            else
            {
                tp = _input.RightPadAxis;
                t = (tp.padY + 1) / 2; //TODO as r247
                _lastSpeed = Mathf.Lerp(0, _walkSpeed, t);
            }

            movement = _lastDir.getForwardXZ(Time.deltaTime * _factor * _lastSpeed);
            _factor += _smoothStep;
            if (_factor > 1)
                _factor = 1;
        }
        else if (_factor > 0)
        {
            movement = _lastDir.getForwardXZ(Time.deltaTime * _factor * _lastSpeed);
            _factor -= _smoothStep;
            if (_factor < 0)
                _factor = 0;
        }
        else if (_lastSpeed > 0)
        {
            _lastSpeed = 0;
            _lastDir = Quaternion.identity;
        }

        return movement;
    }
}