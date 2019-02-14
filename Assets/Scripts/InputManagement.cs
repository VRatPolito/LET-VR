using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManagement : MonoBehaviour
{
    public GameObject LeftController;
    public GameObject RightController;
    private SteamVR_TrackedController LeftVRController, RightVRController;

    public event Action<object, ClickedEventArgs> OnMouseClicked, OnMouseUnclicked, OnRightMouseClicked, OnRightMouseUnclicked, OnLeftMouseClicked, OnLeftMouseUnclicked;
    public event Action<object, ClickedEventArgs> OnRightDropPressed, OnLeftDropPressed, OnRightDropUnpressed, OnLeftDropUnpressed;
    public event Action<object, ClickedEventArgs> OnLeftTriggerClicked, OnLeftTriggerUnclicked, OnLeftPadUnpressed, OnRightPadUnpressed, OnLeftPadUntouched, OnRightPadUntouched, OnLeftGripped, OnLeftUngripped,
                               OnRightTriggerClicked, OnRightTriggerUnclicked, OnRightGripped, OnRightUngripped;
    public event Action<object, ClickedEventArgs> OnLeftPadPressed, OnRightPadPressed, OnLeftPadTouched, OnRightPadTouched;

    private bool _leftMouseClicked, _rightMouseClicked, _leftDropPressed, _rightDropPressed, _leftTriggerClicked, _rightTriggerClicked, _leftPadPressed, _rightPadPressed, _leftGripped, _rightGripped, _leftPadTouched, _rightPadTouched;
    private ClickedEventArgs _leftTriggerClickedEventArgs, _rightTriggerClickedEventArgs, _leftGripClickedEventArgs, _rightGripClickedEventArgs, _leftPadClickedEventArgs, _rightPadClickedEventArgs;

    public bool IsLeftMouseClicked
    {
        private set
        {
            if (value != _leftMouseClicked)
            {
                if (value)
                {
                    if (OnLeftMouseClicked != null)
                        OnLeftMouseClicked.Invoke(this, new ClickedEventArgs());
                    if (OnMouseClicked != null)
                        OnMouseClicked.Invoke(this, new ClickedEventArgs());
                }
                else if (!value && OnLeftMouseUnclicked != null)
                {
                    if (OnLeftMouseUnclicked != null)
                        OnLeftMouseUnclicked.Invoke(this, new ClickedEventArgs());
                    if (OnMouseUnclicked != null)
                        OnMouseUnclicked.Invoke(this, new ClickedEventArgs());
                }
            }
            _leftMouseClicked = value;
        }
        get { return _leftMouseClicked; }
    }

    public bool IsRightMouseClicked
    {
        private set
        {
            if (value != _rightMouseClicked)
            {
                if (value && OnRightMouseClicked != null)
                {
                    if (OnRightMouseClicked != null)
                        OnRightMouseClicked.Invoke(this, new ClickedEventArgs());
                    if (OnMouseClicked != null)
                        OnMouseClicked.Invoke(this, new ClickedEventArgs());
                }
                else if (!value && OnRightMouseUnclicked != null)
                {
                    if (OnRightMouseUnclicked != null)
                        OnRightMouseUnclicked.Invoke(this, new ClickedEventArgs());
                    if (OnMouseUnclicked != null)
                        OnMouseUnclicked.Invoke(this, new ClickedEventArgs());
                }
            }
            _rightMouseClicked = value;
        }
        get { return _rightMouseClicked; }
    }

    public bool IsLeftDropPressed
    {
        private set
        {
            if (value != _leftDropPressed)
            {
                if (value && OnLeftDropPressed != null)
                    OnLeftDropPressed.Invoke(this, new ClickedEventArgs());
                else if (!value && OnLeftDropUnpressed != null)
                    OnLeftDropUnpressed.Invoke(this, new ClickedEventArgs());
            }
            _leftDropPressed = value;
        }
        get { return _leftDropPressed; }
    }

    public bool IsRightDropPressed
    {
        private set
        {
            if (value != _rightDropPressed)
            {
                if (value && OnRightDropPressed != null)
                    OnRightDropPressed.Invoke(this, new ClickedEventArgs());
                else if (!value && OnRightDropUnpressed != null)
                    OnRightDropUnpressed.Invoke(this, new ClickedEventArgs());
            }
            _rightDropPressed = value;
        }
        get { return _rightDropPressed; }
    }

    public bool IsLeftTriggerClicked
    {
        private set
        {
            if (value != _leftTriggerClicked)
            {
                if (value && OnLeftTriggerClicked != null)
                    OnLeftTriggerClicked.Invoke(this, _leftTriggerClickedEventArgs);
                else if (!value && OnLeftTriggerUnclicked != null)
                    OnLeftTriggerUnclicked.Invoke(this, _leftTriggerClickedEventArgs);
            }
            _leftTriggerClicked = value;
        }
        get { return _leftTriggerClicked; }
    }

    public bool IsRightTriggerClicked
    {
        private set
        {
            if (value != _rightTriggerClicked)
            {
                if (value && OnRightTriggerClicked != null)
                    OnRightTriggerClicked.Invoke(this, _rightTriggerClickedEventArgs);
                else if (!value && OnRightTriggerUnclicked != null)
                    OnRightTriggerUnclicked.Invoke(this, _rightTriggerClickedEventArgs);
            }
            _rightTriggerClicked = value;
        }
        get { return _rightTriggerClicked; }
    }

    public bool IsLeftPadPressed
    {
        private set
        {
            if (value != _leftPadPressed)
            {
                if (value && OnLeftPadPressed != null)
                    OnLeftPadPressed.Invoke(this, _leftPadClickedEventArgs);
                else if (!value && OnLeftPadUnpressed != null)
                    OnLeftPadUnpressed.Invoke(this, _leftPadClickedEventArgs);
            }
            _leftPadPressed = value;
        }
        get { return _leftPadPressed; }
    }

    public bool IsRightPadPressed
    {
        private set
        {
            if (value != _rightPadPressed)
            {
                if (value && OnRightPadPressed != null)
                    OnRightPadPressed.Invoke(this, _rightPadClickedEventArgs);
                else if (!value && OnRightPadUnpressed != null)
                    OnRightPadUnpressed.Invoke(this, _rightPadClickedEventArgs);
            }
            _rightPadPressed = value;
        }
        get { return _rightPadPressed; }
    }

    public bool IsLeftPadTouched
    {
        private set
        {
            if (value != _leftPadTouched)
            {
                if (value && OnLeftPadTouched != null)
                    OnLeftPadTouched.Invoke(this, _leftPadClickedEventArgs);
                else if (!value && OnLeftPadUntouched != null)
                    OnLeftPadUntouched.Invoke(this, _leftPadClickedEventArgs);
            }
            _leftPadTouched = value;
        }
        get { return _leftPadTouched; }
    }

    public bool IsRightPadTouched
    {
        private set
        {
            if (value != _rightPadTouched)
            {
                if (value && OnRightPadTouched != null)
                    OnRightPadTouched.Invoke(this, _rightPadClickedEventArgs);
                else if (!value && OnRightPadUntouched != null)
                    OnRightPadUntouched.Invoke(this, _rightPadClickedEventArgs);
            }
            _rightPadTouched = value;
        }
        get { return _rightPadTouched; }
    }

    public bool IsLeftGripped
    {
        private set
        {
            if (value != _leftGripped)
            {
                if (value && OnLeftGripped != null)
                    OnLeftGripped.Invoke(this, _leftGripClickedEventArgs);
                else if (!value && OnLeftUngripped != null)
                    OnLeftUngripped.Invoke(this, _leftGripClickedEventArgs);
            }
            _leftGripped = value;
        }
        get { return _leftGripped; }
    }

    public bool IsRightGripped
    {
        private set
        {
            if (value != _rightGripped)
            {
                if (value && OnRightGripped != null)
                    OnRightGripped.Invoke(this, _rightGripClickedEventArgs);
                else if (!value && OnRightUngripped != null)
                    OnRightUngripped.Invoke(this, _rightGripClickedEventArgs);
            }
            _rightGripped = value;
        }
        get { return _rightGripped; }
    }


    private void Start()
    {
        if (LeftController != null)
        {
            LeftVRController = LeftController.GetComponent<SteamVR_TrackedController>();
            if (LeftVRController != null)
            {
                LeftVRController.TriggerClicked += LeftTriggerClicked;

                LeftVRController.TriggerUnclicked += LeftTriggerUnClicked;

                LeftVRController.PadClicked += LeftPadPressed;

                LeftVRController.PadUnclicked += LeftPadUnPressed;

                LeftVRController.PadTouched += LeftPadTouched;

                LeftVRController.PadUntouched += LeftPadUntouched;

                LeftVRController.Gripped += LeftGripped;

                LeftVRController.Ungripped += LeftUngripped;

            }
        }
        if (RightController != null)
        {
            RightVRController = RightController.GetComponent<SteamVR_TrackedController>();
            if (RightVRController != null)
            {
                RightVRController.TriggerClicked += RightTriggerClicked;

                RightVRController.TriggerUnclicked += RightTriggerUnClicked;

                RightVRController.PadClicked += RightPadPressed;

                RightVRController.PadUnclicked += RightPadUnPressed;

                RightVRController.PadTouched += RightPadTouched;

                RightVRController.PadUntouched += RightPadUntouched;

                RightVRController.Gripped += RightGripped;

                RightVRController.Ungripped += RightUnGripped;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
            if (Input.GetButtonDown("Fire Left Mouse"))
                IsLeftMouseClicked = true;
            else if (Input.GetButtonUp("Fire Left Mouse"))
                IsLeftMouseClicked = false;
            if (Input.GetButtonDown("Fire Right Mouse"))
                IsRightMouseClicked = true;
            else if (Input.GetButtonUp("Fire Right Mouse"))
                IsRightMouseClicked = false;
            if (Input.GetButtonDown("Drop Item Left"))
                IsLeftDropPressed = true;
            else if (Input.GetButtonUp("Drop Item Left"))
                IsLeftDropPressed = false;
            if (Input.GetButtonDown("Drop Item Right"))
                IsRightDropPressed = true;
            else if (Input.GetButtonUp("Drop Item Right"))
                IsRightDropPressed = false;      
    }

    private void LeftTriggerClicked(object sender, ClickedEventArgs e)
    {
        _leftTriggerClickedEventArgs = e;
        IsLeftTriggerClicked = true;
    }

    private void LeftTriggerUnClicked(object sender, ClickedEventArgs e)
    {
        _leftTriggerClickedEventArgs = e;
        IsLeftTriggerClicked = false;
    }

    private void RightTriggerClicked(object sender, ClickedEventArgs e)
    {
        _rightTriggerClickedEventArgs = e;
        IsRightTriggerClicked = true;
    }

    private void RightTriggerUnClicked(object sender, ClickedEventArgs e)
    {
        _rightTriggerClickedEventArgs = e;
        IsRightTriggerClicked = false;
    }

    private void LeftPadPressed(object sender, ClickedEventArgs e)
    {
        _leftPadClickedEventArgs = e;
        IsLeftPadPressed = true;
    }

    private void LeftPadUnPressed(object sender, ClickedEventArgs e)
    {
        _leftPadClickedEventArgs = e;
        IsLeftPadPressed = false;
    }

    private void LeftPadTouched(object sender, ClickedEventArgs e)
    {
        _leftPadClickedEventArgs = e;
        IsLeftPadTouched = true;
    }

    private void LeftPadUntouched(object sender, ClickedEventArgs e)
    {
        _leftPadClickedEventArgs = e;
        IsLeftPadTouched = false;
    }

    private void RightPadTouched(object sender, ClickedEventArgs e)
    {
        _rightPadClickedEventArgs = e;
        IsRightPadTouched = true;
    }

    private void RightPadUntouched(object sender, ClickedEventArgs e)
    {
        _rightPadClickedEventArgs = e;
        IsRightPadTouched = false;
    }

    private void RightPadPressed(object sender, ClickedEventArgs e)
    {
        _rightPadClickedEventArgs = e;
        IsRightPadPressed = true;
    }

    private void RightPadUnPressed(object sender, ClickedEventArgs e)
    {
        _rightPadClickedEventArgs = e;
        IsRightPadPressed = false;
    }

    private void LeftGripped(object sender, ClickedEventArgs e)
    {
        _leftGripClickedEventArgs = e;
        IsLeftGripped = true;
    }

    private void LeftUngripped(object sender, ClickedEventArgs e)
    {
        _leftGripClickedEventArgs = e;
        IsLeftGripped = false;
    }
    private void RightGripped(object sender, ClickedEventArgs e)
    {
        _rightGripClickedEventArgs = e;
        IsRightGripped = true;
    }

    private void RightUnGripped(object sender, ClickedEventArgs e)
    {
        _rightGripClickedEventArgs = e;
        IsRightGripped = false;
    }


}

