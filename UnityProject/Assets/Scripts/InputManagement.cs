using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
public struct PadEventArgs
{
    public float padX, padY;
}

public class InputManagement : MonoBehaviour
{
    public GameObject LeftController;
    public GameObject RightController;
    //private SteamVR_TrackedController LeftVRController, RightVRController;
    private ActionBasedController LeftVRController, RightVRController;

    // sostituire questi eventi con action reference ? 
    //[SerializeField] private InputActionReference runActionRefernce;

    //public event Action<object, ClickedEventArgs> OnMouseClicked, OnMouseUnclicked, OnRightMouseClicked, OnRightMouseUnclicked, OnLeftMouseClicked, OnLeftMouseUnclicked;
    //public event Action<object, ClickedEventArgs> OnRightDropPressed, OnLeftDropPressed, OnRightDropUnpressed, OnLeftDropUnpressed;
    //public event Action<object, ClickedEventArgs> OnLeftTriggerClicked, OnLeftTriggerUnclicked, OnLeftPadUnpressed, OnRightPadUnpressed, OnLeftPadUntouched, OnRightPadUntouched, OnLeftGripped, OnLeftUngripped, OnRightTriggerClicked, OnRightTriggerUnclicked, OnRightGripped, OnRightUngripped;
    //public event Action<object, ClickedEventArgs> OnLeftPadPressed, OnRightPadPressed, OnLeftPadTouched, OnRightPadTouched;
    /*public event Action<object, InputAction.CallbackContext> OnMouseClicked, OnMouseUnclicked, OnRightMouseClicked, OnRightMouseUnclicked, OnLeftMouseClicked, OnLeftMouseUnclicked;
    public event Action<object, InputAction.CallbackContext> OnRightDropPressed, OnLeftDropPressed, OnRightDropUnpressed, OnLeftDropUnpressed;
    public event Action<object, InputAction.CallbackContext> OnLeftTriggerClicked, OnLeftTriggerUnclicked, OnLeftPadUnpressed, OnRightPadUnpressed, OnLeftPadUntouched, OnRightPadUntouched, OnLeftGripped, OnLeftUngripped, OnRightTriggerClicked, OnRightTriggerUnclicked, OnRightGripped, OnRightUngripped;
    public event Action<object, InputAction.CallbackContext> OnLeftPadPressed, OnRightPadPressed, OnLeftPadTouched, OnRightPadTouched;*/



    /*#reegion SteamVR Actions
    private SteamVR_Action_Boolean _grabPinchAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch");
    private SteamVR_Action_Boolean _grabGripAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");
    private SteamVR_Action_Boolean _teleportAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Teleport");
    private SteamVR_Action_Boolean _touchPadAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("TouchPad");
    private SteamVR_Action_Vector2 _touchPosAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("TouchPos");
    #endregion*/

    #region OpenXR Actions
    [SerializeField]
    private InputActionReference _leftGrabPinchAction;
    [SerializeField]
    private InputActionReference _leftGrabGripAction;
    [SerializeField]
    private InputActionReference _leftTeleportAction;
    [SerializeField]
    private InputActionReference _leftTouchPadAction;
    [SerializeField]
    private InputActionReference _leftTouchPosAction;
    [SerializeField]
    private InputActionReference _rightGrabPinchAction;
    [SerializeField]
    private InputActionReference _rightGrabGripAction;
    [SerializeField]
    private InputActionReference _rightTeleportAction;
    [SerializeField]
    private InputActionReference _rightTouchPadAction;
    [SerializeField]
    private InputActionReference _rightTouchPosAction;
    #endregion

    /*[Space]
    public InputActionReference LeftTrigger;
    public InputActionReference LeftPadPress;
    public InputActionReference LeftPadTouch;
    public InputActionReference LeftGrip;


    [Space]
    public InputActionReference RightTrigger;
    public InputActionReference RightPadPress;
    public InputActionReference RightPadTouch;
    public InputActionReference RightGrip;*/

    #region PadAxes
    private Vector2 _rightAxisTarget;
    private Vector2 _leftAxisTarget;
    private Vector2 _rightAxis;
    private Vector2 _leftAxis;
    #endregion

    #region Events
    public event Action<object> OnLeftTriggerClicked, OnLeftTriggerUnclicked, OnLeftPadUnpressed, OnRightPadUnpressed, OnLeftPadUntouched, OnRightPadUntouched, OnLeftGripped, OnLeftUngripped,
                               OnRightTriggerClicked, OnRightTriggerUnclicked, OnRightGripped, OnRightUngripped;
    public event Action<object, PadEventArgs> OnLeftPadPressed, OnRightPadPressed, OnLeftPadTouched, OnRightPadTouched;
    #endregion

    #region Bools
    private bool _leftTriggerClicked, _rightTriggerClicked, _leftPadPressed, _rightPadPressed, _leftGripped, _rightGripped, _leftPadTouched, _rightPadTouched;
    private bool _leftTriggerClickedFrame, _rightTriggerClickedFrame, _leftPadPressedFrame, _rightPadPressedFrame, _leftGrippedFrame, _rightGrippedFrame, _leftPadTouchedFrame, _rightPadTouchedFrame;
    private bool _leftTriggerUnclickedFrame, _rightTriggerUnclickedFrame, _leftPadUnpressedFrame, _rightPadUnpressedFrame, _leftUngrippedFrame, _rightUngrippedFrame, _leftPadUntouchedFrame, _rightPadUntouchedFrame;
    private PadEventArgs _leftPadEventArgs, _rightPadEventArgs;
    #endregion

    //    runActionRefernce.action.performed += OnContinuousRun;
    //    runActionRefernce.action.canceled += OnContinuousRun;

    //private bool _leftMouseClicked, _rightMouseClicked, _leftDropPressed, _rightDropPressed, _leftTriggerClicked, _rightTriggerClicked, _leftPadPressed, _rightPadPressed, _leftGripped, _rightGripped, _leftPadTouched, _rightPadTouched;

    //private ClickedEventArgs _leftTriggerClickedEventArgs, _rightTriggerClickedEventArgs, _leftGripClickedEventArgs, _rightGripClickedEventArgs, _leftPadClickedEventArgs, _rightPadClickedEventArgs;
    //private InputAction.CallbackContext _leftTriggerClickedEventArgs, _rightTriggerClickedEventArgs, _leftGripClickedEventArgs, _rightGripClickedEventArgs, _leftPadClickedEventArgs, _rightPadClickedEventArgs;


    /* public bool IsLeftMouseClicked
     {
         private set
         {
             if (value != _leftMouseClicked)
             {
                 if (value)
                 {
                     if (OnLeftMouseClicked != null)
                         OnLeftMouseClicked.Invoke(this, new InputAction.CallbackContext());
                     if (OnMouseClicked != null)
                         OnMouseClicked.Invoke(this, new InputAction.CallbackContext());
                 }
                 else if (!value && OnLeftMouseUnclicked != null)
                 {
                     if (OnLeftMouseUnclicked != null)
                         OnLeftMouseUnclicked.Invoke(this, new InputAction.CallbackContext());
                     if (OnMouseUnclicked != null)
                         OnMouseUnclicked.Invoke(this, new InputAction.CallbackContext());
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
                         OnRightMouseClicked.Invoke(this, new InputAction.CallbackContext());
                     if (OnMouseClicked != null)
                         OnMouseClicked.Invoke(this, new InputAction.CallbackContext());
                 }
                 else if (!value && OnRightMouseUnclicked != null)
                 {
                     if (OnRightMouseUnclicked != null)
                         OnRightMouseUnclicked.Invoke(this, new InputAction.CallbackContext());
                     if (OnMouseUnclicked != null)
                         OnMouseUnclicked.Invoke(this, new InputAction.CallbackContext());
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
                     OnLeftDropPressed.Invoke(this, new InputAction.CallbackContext());
                 else if (!value && OnLeftDropUnpressed != null)
                     OnLeftDropUnpressed.Invoke(this, new InputAction.CallbackContext());
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
                     OnRightDropPressed.Invoke(this, new InputAction.CallbackContext());
                 else if (!value && OnRightDropUnpressed != null)
                     OnRightDropUnpressed.Invoke(this, new InputAction.CallbackContext());
             }
             _rightDropPressed = value;
         }
         get { return _rightDropPressed; }
     }

     //occhio che quelli sotto non hanno InputAction.CallbackContext()
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
     }*/


    #region Properties
    public bool IsLeftTriggerClickedDown
    {
        get { return _leftTriggerClickedFrame; }
    }
    public bool IsRightTriggerClickedDown
    {
        get { return _rightTriggerClickedFrame; }
    }
    public bool IsLeftPadPressedDown
    {
        get { return _leftPadPressedFrame; }
    }
    public bool IsRightPadPressedDown
    {
        get { return _rightPadPressedFrame; }
    }
    public bool IsLeftPadTouchedDown
    {
        get { return _leftPadTouchedFrame; }
    }
    public bool IsRightPadTouchedDown
    {
        get { return _rightPadTouchedFrame; }
    }
    public bool IsLeftGrippedDown
    {
        get { return _leftGrippedFrame; }
    }
    public bool IsRightGrippedDown
    {
        get { return _rightGrippedFrame; }
    }
    public bool IsLeftTriggerClickedUp
    {
        get { return _leftTriggerUnclickedFrame; }
    }
    public bool IsRightTriggerClickedUp
    {
        get { return _rightTriggerUnclickedFrame; }
    }
    public bool IsLeftPadPressedUp
    {
        get { return _leftPadUnpressedFrame; }
    }
    public bool IsRightPadPressedUp
    {
        get { return _rightPadUnpressedFrame; }
    }
    public bool IsLeftPadTouchedUp
    {
        get { return _leftPadUntouchedFrame; }
    }
    public bool IsRightPadTouchedUp
    {
        get { return _rightPadUntouchedFrame; }
    }
    public bool IsLeftGrippedUp
    {
        get { return _leftUngrippedFrame; }
    }
    public bool IsRightGrippedUp
    {
        get { return _rightUngrippedFrame; }
    }
    public Vector2 RightAxis
    {
        get { return _rightAxis; }
    }
    public Vector2 LeftAxis
    {
        get { return _leftAxis; }
    }
    #endregion

    #region Polling Methods
    public bool IsLeftTriggerClicked
    {
        private set
        {
            if (value != _leftTriggerClicked)
            {
                if (value && OnLeftTriggerClicked != null)
                    OnLeftTriggerClicked.Invoke(this);
                else if (!value && OnLeftTriggerUnclicked != null)
                    OnLeftTriggerUnclicked.Invoke(this);
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
                    OnRightTriggerClicked.Invoke(this);
                else if (!value && OnRightTriggerUnclicked != null)
                    OnRightTriggerUnclicked.Invoke(this);
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
                    OnLeftPadPressed.Invoke(this, LeftPadAxis);
                else if (!value && OnLeftPadUnpressed != null)
                    OnLeftPadUnpressed.Invoke(this);
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
                    OnRightPadPressed.Invoke(this, RightPadAxis);
                else if (!value && OnRightPadUnpressed != null)
                    OnRightPadUnpressed.Invoke(this);
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
                    OnLeftPadTouched.Invoke(this, LeftPadAxis);
                else if (!value && OnLeftPadUntouched != null)
                    OnLeftPadUntouched.Invoke(this);
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
                    OnRightPadTouched.Invoke(this, RightPadAxis);
                else if (!value && OnRightPadUntouched != null)
                    OnRightPadUntouched.Invoke(this);
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
                    OnLeftGripped.Invoke(this);
                else if (!value && OnLeftUngripped != null)
                    OnLeftUngripped.Invoke(this);
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
                    OnRightGripped.Invoke(this);
                else if (!value && OnRightUngripped != null)
                    OnRightUngripped.Invoke(this);
            }
            _rightGripped = value;
        }
        get { return _rightGripped; }
    }
    public PadEventArgs LeftPadAxis
    {
        private set
        {
            _leftPadEventArgs = value;
        }
        get
        {
            return _leftPadEventArgs;
        }
    }
    public PadEventArgs RightPadAxis
    {
        private set
        {
            _rightPadEventArgs = value;
        }
        get
        {
            return _rightPadEventArgs;
        }
    }
    #endregion

    private void Start()
    {
       /* if (LeftController != null)
        {
            LeftVRController = LeftController.GetComponent<ActionBasedController>();
            if (LeftVRController != null)
            {
                LeftTrigger.action.performed += LeftTriggerClicked;
                //LeftVRController.TriggerClicked += LeftTriggerClicked;

                LeftTrigger.action.canceled += LeftTriggerUnClicked;
                //LeftVRController.TriggerUnclicked += LeftTriggerUnClicked;

                LeftPadPress.action.performed += LeftPadPressed;
                //LeftVRController.PadClicked += LeftPadPressed;

                LeftPadPress.action.canceled += LeftPadUnPressed;
                //LeftVRController.PadUnclicked += LeftPadUnPressed;

                LeftPadTouch.action.performed += LeftPadTouched;
                //LeftVRController.PadTouched += LeftPadTouched;

                LeftPadTouch.action.canceled += LeftPadUntouched;
                //LeftVRController.PadUntouched += LeftPadUntouched;

                LeftGrip.action.performed += LeftGripped;
                //LeftVRController.Gripped += LeftGripped;

                LeftGrip.action.canceled += LeftUngripped;
                //LeftVRController.Ungripped += LeftUngripped;

            }
        }
        if (RightController != null)
        {
            RightVRController = RightController.GetComponent<ActionBasedController>();
            if (RightVRController != null)
            {
                RightTrigger.action.performed += RightTriggerClicked;
                //RightVRController.TriggerClicked += RightTriggerClicked;

                RightTrigger.action.canceled += RightTriggerUnClicked;
                //RightVRController.TriggerUnclicked += RightTriggerUnClicked;

                RightPadPress.action.performed += RightPadPressed;
                //RightVRController.PadClicked += RightPadPressed;

                RightPadPress.action.canceled += RightPadUnPressed;
                //RightVRController.PadUnclicked += RightPadUnPressed;

                RightPadTouch.action.performed += RightPadTouched;
                //RightVRController.PadTouched += RightPadTouched;

                RightPadTouch.action.canceled += RightPadUntouched;
                //RightVRController.PadUntouched += RightPadUntouched;

                RightGrip.action.performed += RightGripped;
                //RightVRController.Gripped += RightGripped;

                RightGrip.action.canceled += RightUngripped;
                //RightVRController.Ungripped += RightUngripped;
            }
        }*/
    }

    // Update is called once per frame
    void Update()
    {
#if MOUSEANDKEYBOARD
    Debug.LogError("keyboard not implemented");
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
#endif
        ResetFrameBooleans();

        if (ExampleUtil.isPresent())
        {
            bool axisUpdated = false;
            if (ReadValueAsButton(_leftGrabPinchAction))
            {
                if (!IsLeftTriggerClicked)
                    _leftTriggerClickedFrame = true;
                else
                    _leftTriggerClickedFrame = false;

                IsLeftTriggerClicked = true;
            }
            else if (!ReadValueAsButton(_leftGrabPinchAction))
            {
                if (IsLeftTriggerClicked)
                    _leftTriggerUnclickedFrame = true;
                else
                    _leftTriggerUnclickedFrame = false;

                IsLeftTriggerClicked = false;
            }

            if (ReadValueAsButton(_leftTeleportAction))
            {
                LeftPadAxis = PadEventArgs(_leftTouchPosAction.action.ReadValue<Vector2>());

                if (!IsLeftPadPressed)
                    _leftPadPressedFrame = true;
                else
                    _leftPadPressedFrame = false;

                IsLeftPadPressed = true;
                axisUpdated = true;
            }
            else
            {
                LeftPadAxis = default;

                if (IsLeftPadPressed)
                    _leftPadUnpressedFrame = true;
                else
                    _leftPadUnpressedFrame = false;

                IsLeftPadPressed = false;
                axisUpdated = true;
            }

            if (ReadValueAsButton(_leftTouchPadAction))
            {
                LeftPadAxis = PadEventArgs(_leftTouchPosAction.action.ReadValue<Vector2>());

                if (!IsLeftPadTouched)
                    _leftPadTouchedFrame = true;
                else
                    _leftPadTouchedFrame = false;

                IsLeftPadTouched = true;
                axisUpdated = true;
            }
            else
            {
                LeftPadAxis = default;

                if (IsLeftPadTouched)
                    _leftPadUntouchedFrame = true;
                else
                    _leftPadUntouchedFrame = false;

                IsLeftPadTouched = false;
                axisUpdated = true;
            }

            if (!axisUpdated)
            {
                if (IsLeftPadTouched || IsLeftPadPressed)
                    LeftPadAxis = PadEventArgs(_leftTouchPosAction.action.ReadValue<Vector2>());
                else
                    LeftPadAxis = PadEventArgs(0, 0);
            }
            else
                axisUpdated = false;

            if (ReadValueAsButton(_leftGrabGripAction))
            {
                if (!IsLeftGripped)
                    _leftGrippedFrame = true;
                else
                    _leftGrippedFrame = false;

                IsLeftGripped = true;
            }
            else
            {
                if (IsLeftGripped)
                    _leftUngrippedFrame = true;
                else
                    _leftUngrippedFrame = false;

                IsLeftGripped = false;
            }

            if (ReadValueAsButton(_rightGrabPinchAction))
            {
                if (!IsRightTriggerClicked)
                    _rightTriggerClickedFrame = true;
                else
                    _rightTriggerClickedFrame = false;

                IsRightTriggerClicked = true;
            }
            else if (!ReadValueAsButton(_rightGrabPinchAction))
            {
                if (IsRightTriggerClicked)
                    _rightTriggerUnclickedFrame = true;
                else
                    _rightTriggerUnclickedFrame = false;

                IsRightTriggerClicked = false;
            }

            if (ReadValueAsButton(_rightTeleportAction))
            {
                RightPadAxis = PadEventArgs(_rightTouchPosAction.action.ReadValue<Vector2>());

                if (!IsRightPadPressed)
                    _rightPadPressedFrame = true;
                else
                    _rightPadPressedFrame = false;

                IsRightPadPressed = true;
                axisUpdated = true;
            }
            else
            {
                RightPadAxis = default;

                if (IsRightPadPressed)
                    _rightPadUnpressedFrame = true;
                else
                    _rightPadUnpressedFrame = false;

                IsRightPadPressed = false;
                axisUpdated = true;
            }

            if (ReadValueAsButton(_rightTouchPadAction))
            {
                RightPadAxis = PadEventArgs(_rightTouchPosAction.action.ReadValue<Vector2>());

                if (!IsRightPadTouched)
                    _rightPadTouchedFrame = true;
                else
                    _rightPadTouchedFrame = false;

                IsRightPadTouched = true;
                axisUpdated = true;
            }
            else
            {
                RightPadAxis = default;

                if (IsRightPadTouched)
                    _rightPadUntouchedFrame = true;
                else
                    _rightPadUntouchedFrame = false;

                IsRightPadTouched = false;
                axisUpdated = true;
            }

            if (!axisUpdated)
            {
                if (IsRightPadTouched || IsRightPadPressed)
                    RightPadAxis = PadEventArgs(_rightTouchPosAction.action.ReadValue<Vector2>());
                else
                    RightPadAxis = PadEventArgs(0, 0);
            }

            if (ReadValueAsButton(_rightGrabGripAction))
            {
                if (!IsRightGripped)
                    _rightGrippedFrame = true;
                else
                    _rightGrippedFrame = false;

                IsRightGripped = true;
            }
            else
            {
                if (IsRightGripped)
                    _rightUngrippedFrame = true;
                else
                    _rightUngrippedFrame = false;

                IsRightGripped = false;
            }

            float x = 0, y = 0;
            if (IsRightPadPressed)
            {
                var a = RightPadAxis;
                if (a.padX > 0)
                    x = 1;
                else if (a.padX < 0)
                    x = -1;
                if (a.padY > 0)
                    y = 1;
                else if (a.padY < 0)
                    y = -1;
            }
            _rightAxisTarget = new Vector2(x, y);

            x = 0;
            y = 0;
            if (IsLeftPadPressed)
            {
                var a = LeftPadAxis;
                if (a.padX > 0)
                    x = 1;
                else if (a.padX < 0)
                    x = -1;
                if (a.padY > 0)
                    y = 1;
                else if (a.padY < 0)
                    y = -1;
            }
            _leftAxisTarget = new Vector2(x, y);
        }
    }

    public bool ReadValueAsButton(InputAction a)
    {
        bool value = false;
        try
        {
            value = a.ReadValue<bool>();
        }
        catch(InvalidOperationException)
        {
            value = Convert.ToBoolean(a.ReadValue<float>());
        }
        return value;
    }

   /* private void LeftTriggerClicked(object sender, ClickedEventArgs e)
    {
        _leftTriggerClickedEventArgs = e;
        IsLeftTriggerClicked = true;
    }*/
    /*private void LeftTriggerClicked(InputAction.CallbackContext e)
    {
        _leftTriggerClickedEventArgs = e;
        IsLeftTriggerClicked = true;
    }

    private void LeftTriggerUnClicked( InputAction.CallbackContext e)
    {
        _leftTriggerClickedEventArgs = e;
        IsLeftTriggerClicked = false;
    }

    private void RightTriggerClicked( InputAction.CallbackContext e)
    {
        _rightTriggerClickedEventArgs = e;
        IsRightTriggerClicked = true;
    }

    private void RightTriggerUnClicked( InputAction.CallbackContext e)
    {
        _rightTriggerClickedEventArgs = e;
        IsRightTriggerClicked = false;
    }

    private void LeftPadPressed( InputAction.CallbackContext e)
    {
        _leftPadClickedEventArgs = e;
        IsLeftPadPressed = true;
    }

    private void LeftPadUnPressed( InputAction.CallbackContext e)
    {
        _leftPadClickedEventArgs = e;
        IsLeftPadPressed = false;
    }

    private void LeftPadTouched( InputAction.CallbackContext e)
    {
        _leftPadClickedEventArgs = e;
        IsLeftPadTouched = true;
    }

    private void LeftPadUntouched( InputAction.CallbackContext e)
    {
        _leftPadClickedEventArgs = e;
        IsLeftPadTouched = false;
    }

    private void RightPadTouched( InputAction.CallbackContext e)
    {
        _rightPadClickedEventArgs = e;
        IsRightPadTouched = true;
    }

    private void RightPadUntouched( InputAction.CallbackContext e)
    {
        _rightPadClickedEventArgs = e;
        IsRightPadTouched = false;
    }

    private void RightPadPressed( InputAction.CallbackContext e)
    {
        _rightPadClickedEventArgs = e;
        IsRightPadPressed = true;
    }

    private void RightPadUnPressed( InputAction.CallbackContext e)
    {
        _rightPadClickedEventArgs = e;
        IsRightPadPressed = false;
    }

    private void LeftGripped( InputAction.CallbackContext e)
    {
        _leftGripClickedEventArgs = e;
        IsLeftGripped = true;
    }

    private void LeftUngripped( InputAction.CallbackContext e)
    {
        _leftGripClickedEventArgs = e;
        IsLeftGripped = false;
    }

    private void RightGripped( InputAction.CallbackContext e)
    {
        _rightGripClickedEventArgs = e;
        IsRightGripped = true;
    }

    private void RightUngripped( InputAction.CallbackContext e)
    {
        _rightGripClickedEventArgs = e;
        IsRightGripped = false;
    }*/

    void FixedUpdate()
    {
        float x = _rightAxis.x;
        float y = _rightAxis.y;
        if (_rightAxis.x > _rightAxisTarget.x)
        {
            x = _rightAxis.x - 0.05f;
            if (x < _rightAxisTarget.x)
                x = _rightAxisTarget.x;
        }
        else if (_rightAxis.x < _rightAxisTarget.x)
        {
            x = _rightAxis.x + 0.05f;
            if (x > _rightAxisTarget.x)
                x = _rightAxisTarget.x;
        }
        if (_rightAxis.y > _rightAxisTarget.y)
        {
            y = _rightAxis.y - 0.05f;
            if (y < _rightAxisTarget.y)
                y = _rightAxisTarget.y;
        }
        else if (_rightAxis.y < _rightAxisTarget.y)
        {
            y = _rightAxis.y + 0.05f;
            if (y > _rightAxisTarget.y)
                y = _rightAxisTarget.y;
        }
        _rightAxis = new Vector2(x, y);

        x = _leftAxis.x;
        y = _leftAxis.y;
        if (_leftAxis.x > _leftAxisTarget.x)
        {
            x = _leftAxis.x - 0.05f;
            if (x < _leftAxisTarget.x)
                x = _leftAxisTarget.x;
        }
        else if (_leftAxis.x < _leftAxisTarget.x)
        {
            x = _leftAxis.x + 0.05f;
            if (x > _leftAxisTarget.x)
                x = _leftAxisTarget.x;
        }
        if (_leftAxis.y > _leftAxisTarget.y)
        {
            y = _leftAxis.y - 0.05f;
            if (y < _leftAxisTarget.y)
                y = _leftAxisTarget.y;
        }
        else if (_leftAxis.y < _leftAxisTarget.y)
        {
            y = _leftAxis.y + 0.05f;
            if (y > _leftAxisTarget.y)
                y = _leftAxisTarget.y;
        }
        _leftAxis = new Vector2(x, y);
    }

    #region Helper Methods
    private void ResetFrameBooleans()
    {
        _leftTriggerClickedFrame = false;
        _rightTriggerClickedFrame = false;
        _leftPadPressedFrame = false;
        _rightPadPressedFrame = false;
        _leftGrippedFrame = false;
        _rightGrippedFrame = false;
        _leftPadTouchedFrame = false;
        _rightPadTouchedFrame = false;
        _leftTriggerUnclickedFrame = false;
        _rightTriggerUnclickedFrame = false;
        _leftPadUnpressedFrame = false;
        _rightPadUnpressedFrame = false;
        _leftUngrippedFrame = false;
        _rightUngrippedFrame = false;
        _leftPadUntouchedFrame = false;
        _rightPadUntouchedFrame = false;
    }
    public static PadEventArgs PadEventArgs()
    {
        PadEventArgs p = new PadEventArgs();
        p.padX = 0;
        p.padY = 0;
        return p;
    }
    public static PadEventArgs PadEventArgs(float X, float Y)
    {
        PadEventArgs p = new PadEventArgs();
        p.padX = X;
        p.padY = Y;
        return p;
    }
    public static PadEventArgs PadEventArgs(Vector2 A)
    {
        PadEventArgs p = new PadEventArgs();
        p.padX = A.x;
        p.padY = A.y;
        return p;
    }
    public static bool SamePadEventArgs(PadEventArgs e1, PadEventArgs e2)
    {
        return ((e1.padX == e2.padX) && (e1.padY == e2.padY));
    }
    public static Vector2 PadEventArgs(PadEventArgs p)
    {
        return new Vector2(p.padX, p.padY);
    }
    #endregion
}

