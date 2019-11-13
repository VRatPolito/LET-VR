using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickMovement : MonoBehaviour
{
    enum InputMode { Touch, Click };
    [SerializeField]
    private InputMode _mode = InputMode.Click;
    [SerializeField]
    private float _walkSpeed = 5;
    [SerializeField]
    private float _runSpeed = 8;
    public bool Blocked
    {
        get { return _blocked; }
        set {
            if(value && !_blocked)
            {
                _bigArrow.gameObject.SetActive(false);
                _lastSpeed = 0;
                _factor = 0;
                _rightPadDown = false;
                _leftPadDown = false;
            }

            _blocked = value;
            }
    }
    bool _blocked = false;
    SteamVR_TrackedController _leftController, _rightController;
    [SerializeField]
    Transform _bigArrow;
    CharacterControllerVR _target;
    InputManagement _input;
    bool _leftPadDown = false;
    bool _rightPadDown = false;
    float _factor = 0;
    float _smoothStep = 0.05f;
    float _lastSpeed = 0;
    
    // Start is called before the first frame update
    void Awake()
    {
        _input = GetComponent<InputManagement>();
        _target = GetComponent<CharacterControllerVR>();
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
        _leftController = _input.LeftController.GetComponent<SteamVR_TrackedController>();
        _rightController = _input.RightController.GetComponent<SteamVR_TrackedController>();
        _bigArrow.gameObject.SetActive(false);
    }

    private void RightPadUp(object sender, ClickedEventArgs e)
    {
        if (!Blocked)
        {
            _rightPadDown = false;
            switch (_mode)
            {
                case InputMode.Click:
                    if (_input.IsLeftPadPressed)
                        _leftPadDown = true;
                    else
                        _bigArrow.gameObject.SetActive(false);
                    break;
                case InputMode.Touch:
                    if (_input.IsLeftPadTouched)
                        _leftPadDown = true;
                    else
                        _bigArrow.gameObject.SetActive(false);
                    break;
            }
        }
    }

    private void RightPadDown(object sender, ClickedEventArgs e)
    {
        if (!Blocked && !_leftPadDown)
        {
            _rightPadDown = true;
            _bigArrow.gameObject.SetActive(true);
        }
    }

    private void LeftPadUp(object sender, ClickedEventArgs e)
    {
        if (!Blocked)
        {
            _leftPadDown = false;
            switch (_mode)
            {
                case InputMode.Click:
                    if (_input.IsRightPadPressed)
                        _rightPadDown = true;
                    else
                        _bigArrow.gameObject.SetActive(false);
                    break;
                case InputMode.Touch:
                    if (_input.IsRightPadTouched)
                        _rightPadDown = true;
                    else
                        _bigArrow.gameObject.SetActive(false);
                    break;
            }
        }
    }

    private void LeftPadDown(object sender, ClickedEventArgs e)
    {
        if (!Blocked && !_rightPadDown)
        {
            _leftPadDown = true;
            _bigArrow.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Blocked)
        {
            if (_leftPadDown)
                _bigArrow.localEulerAngles = new Vector3(0, _leftController.transform.localEulerAngles.y, 0);
            else if (_rightPadDown)
                _bigArrow.localEulerAngles = new Vector3(0, _rightController.transform.localEulerAngles.y, 0);
            
            _target.Move(CalculateMotion());
        }
    }

    private Vector3 CalculateMotion()
    {
        Vector3 movement = Vector3.zero;

        if (_leftPadDown)
        {
            if (_input.IsLeftGripped)
                _lastSpeed = _runSpeed;
            else
            {
                Vector2 tp = _leftController.touchPos;
                float t = (tp.y + 1) / 2;
                _lastSpeed = Mathf.Lerp(0, _walkSpeed, t);
            }

            movement = getForwardXZ(Time.deltaTime * _factor * _lastSpeed, _leftController.transform.rotation);
            _factor += _smoothStep;
            if (_factor > 1)
                _factor = 1;
        }
        else if (_rightPadDown)
        {

            if (_input.IsRightGripped)
                _lastSpeed = _runSpeed;
            else
            {
                Vector2 tp = _rightController.touchPos;
                float t = (tp.y + 1) / 2;
                _lastSpeed = Mathf.Lerp(0, _walkSpeed, t);
            }

            movement = getForwardXZ(Time.deltaTime * _factor * _lastSpeed, _rightController.transform.rotation);
            _factor += _smoothStep;
            if (_factor > 1)
                _factor = 1;
        }
        else if (_factor > 0)
        {
            movement = getForwardXZ(Time.deltaTime * _factor * _lastSpeed, _rightController.transform.rotation);
            _factor -= _smoothStep;
            if (_factor < 0)
                _factor = 0;
        }
        else if (_lastSpeed > 0)
            _lastSpeed = 0;

        return movement;
    }

    // Returns a forward vector given the distance and direction
    public static Vector3 getForwardXZ(float forwardDistance, Quaternion direction)
    {
        Vector3 forwardMovement = direction * Vector3.forward * forwardDistance;
        return vector3XZOnly(forwardMovement);
    }

    // Returns a Vector3 with only the X and Z components (Y is 0'd)
    public static Vector3 vector3XZOnly(Vector3 vec)
    {
        return new Vector3(vec.x, 0f, vec.z);
    }
}
