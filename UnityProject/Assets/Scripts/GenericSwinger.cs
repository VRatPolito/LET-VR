using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
//using Valve.VR;

public class GenericSwinger : MonoBehaviour
{

    /***** CLASS VARIABLES *****/

    /** Inspector Variables **/

    // General Settings
    [Header("General Settings")]
    [SerializeField]
    [Tooltip("General - Scale World Units To Camera Rig Scale\n\nBy default, several unit- and speed-based settings are in absolute world units regardless of CameraRig scale.  If this setting is true, all of those settings will be automatically scaled to match the X scale of this CameraRig.  If you use a non-default CameraRig scale, enabling this setting will allow you to specify all settings in meters-per-second in relation to the CameraRig rather than in world units.\n\n(Default: false)")]
    private bool generalScaleWorldUnitsToCameraRigScale = false;
    [SerializeField]
    [Tooltip("General - Auto Adjust Fixed Timestep\n\nIn order for GenericSwinger to handle movement and wall collisions correctly, Time.fixedDeltaTime must be 0.0111 (90 per second) or less.  If this feature is enabled, the setting will be adjusted automatically if it is higher than 0.0111.  If disabled, an error will be generated but the value will not be changed.\n\n(Default: true)")]
    private bool generalAutoAdjustFixedTimestep = true;


    internal void SetTarget(CharacterControllerVR characterController)
    {
        _target = characterController;
    }

    public bool SwingEnabled
    {
        get { return _swingEnabled; }
        set { _swingEnabled = value;
        }
    }

        // Swing Settings
    [Header("Swing Settings")]
    [Tooltip("Swing - Navigation\n\nEnables variable locomotion using the controllers to determine speed and direction.  Activated according to the selected Mode. \n\n(Default: true)")]
    private bool _swingEnabled = true;
    [SerializeField]
    [Range(0.0f, 50.0f)]
    [Tooltip("Movement Speed Multiplier\n\n. A multiplier useful to regulate the movement speed without having to deal with curves or min/max values.  \n\n(Default: 1)")]
    private float _movementSpeedMultiplier = 1.0f;
    [Tooltip("Swing - Button\nOnly if Navigation is enabled and SwingMode is not NoButtons\n\nDefines which controller button is used to activate GenericSwinger. The button is the same on both controllers.\n\n(Default: Grip button)")]
    public ControllerButton _swingButton = ControllerButton.Grip;
    [SerializeField]
    [Tooltip("Swing - Mode\nOnly if Swing Navigation is enabled\n\nDetermines what is necessary to activate Swing locomotion.\n\nBoth Buttons - Activate by pushing both buttons on both controllers.\n\nLeft Button - Activate by pushing the left controller button.\n\nRight Button - Activate by pushing the right controller button.\n\nOne Button - Activate by pushing either controller's button.\n\n(Default: One Button Same Controller)")]
    private SwingMode _swingMode = SwingMode.BothButtons;
    [SerializeField]
    [Tooltip("Swing - Direction Mode\nDetermines what direction is used to calculate the movement.\n\nLeg Direction Rear Controllers - uses the average rotation of the leg controllers, supports must be placed behind the calfs.\n\nLeg Direction Side Controllers - uses the average rotation of the leg controllers, supports must be placed on the external side of the legs.\n\nArm Direction - uses the rotation of the arm controllers.\n\nHead Direction - uses the direction of the headset.\n\n(Default: Leg Direction Rear Controllers)")]
    private SwingDirectionMode _swingDirectionMode = SwingDirectionMode.AvgDeviceDirection;
    [SerializeField]
    [Tooltip("Swing - Device To Movement Curve\nOnly if Swing Navigation is enabled.\n\nCurve that determines how much a given device change translates into camera rig movement.  The far left of the curve is no controller movement and no virtual movement.  The far right is Controller Speed For Max Speed (controller movement) and Max Speed (virtual momvement).\n\n(Default: Linear)")]
    private AnimationCurve _deviceToMovementCurve = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
    [SerializeField]
    [Tooltip("Swing - Device Speed For Max Speed\nOnly if Swing Navigation is enabled\n\nThe number of CameraRig local units per second a controller needs to be moving to be considered going max speed.\n\n(Default:3)")]
    private float _swingDeviceSpeedForMaxSpeed = 3f;
    [SerializeField]
    [Tooltip("Swing - Max Speed\nOnly if Swing Navigation is enabled\n\nThe fastest base speed (in world units) a player can travel when moving controllers at Controller Movement Per Second For Max Speed.  The actual max speed of the player will depend on the both/single controller coefficients you configure.\n\n(Default: 8)")]
    private float _swingMaxSpeed = 8;
    [SerializeField]
    [Tooltip("Swing - Coefficient\nOnly if Swing Navigation is enabled and Swing Activation Mode allows both controllers to be used for swinging.\n\nUsed to boost or nerf the player's speed when using boths controllers for swinging.  A value of 1.0 will not modify the curve / max speed calculation.\n\n(Default: 1.0)")]
    [Range(0f, 10f)]
    private float _swingCoefficient = 1.0f;
    [SerializeField]
    [Tooltip("Swing - Only Vertical Movement\nDiscard X and Z when calculating the space travelled by the leg tracking devices.\n(Default: false)")]
    private bool _swingOnlyVerticalMovement = false;

    // Smoothing Settings
    [Header("Smoothing Settings")]
    [SerializeField]
    [Tooltip("Smoothing\n\nUses controller movement sampling to help eliminate jerks and unpleasant movement when controllers suddenly change position due to tracking inaccuracies. It is highly recommended to turn this setting on if using movingInertia\n\n(Default: true)")]
    private bool _smoothing = true;
    [SerializeField]
    [Tooltip("Smoothing - Mode\nOnly if Smoothing is enabled\n\nDetermines how controller smoothing calculates the smoothed movement value used by swinging.\n\nLowest\nUse the lowest value in the cache.  Should only be used with small cache sizes.\n\nAverage\nUse the average of all values in the cache.\n\nAverage Minus Highest\nUse the average of all values in the cache, but disregard the highest value.  When a controller jitters, the value change in that frame is almost always higher than normal values and will be discarded.\n\n(Default: Average Minus Highest)")]
    private SmoothingMode _smoothingMode = SmoothingMode.AverageMinusHighest;
    [SerializeField]
    [Range(2, 90)]
    [Tooltip("Smoothing - Cache Size\nOnly if Smoothing is enabled\n\nSets the number of calculated controller movements to keep in the cache.  Setting this number too low may allow a jittering controller to cause jerky movements for the player.  Setting this number too high increases lag time from controller movement to camera rig movement.\n\n(Default: 3)")]
    private int _deviceSmoothingCacheSize = 3;

    // Inertia Settings
    [Header("Inertia Settings")]
    [SerializeField]
    [Tooltip("Moving Inertia\n\nSimulates inertia while swinging.  If the controllers change position slower than the moving inertia calculation, the inertia calculation will be used to determine forward movement.\n\n(Default: true)")]
    private bool _movingInertia = true;
    [Tooltip("Moving Inertia - Time To Stop At Max Speed\nOnly if Moving Inertia is enabled\n\nThe time it will take to go from SwingMaxSpeed to 0 if swinging is engaged and the player does not move the controllers.  Speeds lower than armSwingMaxSpeed will scale their stopping time linearly.\n\n(Default: .5)")]
    public float movingInertiaTimeToStopAtMaxSpeed = .5f;
    [SerializeField]
    [Tooltip("Stopping Inertia\n\nSimulates inertia when swinging stops.\n\n(Default: true)")]
    private bool _stoppingInertia = true;
    [Tooltip("Stopping Inertia - Time To Stop At Max Speed\nOnly if Stopping Inertia is enabled\n\nThe time it will take to go from armSwingMaxSpeed to 0 when swinging is disengaged.  Speeds lower than SwingMaxSpeed will scale their stopping time linearly.\n\n(Default:.35)")]
    private float stoppingInertiaTimeToStopAtMaxSpeed = .35f;

    // Informational public bools
    public bool Swinging = false;

    //External components
    [SerializeField]
    [Tooltip("CharactedController component attached to the parent of CameraRig, necessary to apply the movement generated by swinging.")]
    private CharacterControllerVR _target;
    [SerializeField]
    private GameObject _headsetGameObject;
    [SerializeField]
    private Vector3 _headsetAngleOffset = Vector3.zero;
    [SerializeField]
    private GameObject _leftDeviceGameObject;
    [SerializeField]
    private Vector3 _leftDeviceAngleOffset = Vector3.zero;
    [SerializeField]
    private GameObject _rightDeviceGameObject;
    [SerializeField]
    private Vector3 _rightDeviceAngleOffset = Vector3.zero;
    [SerializeField]
    private GameObject _directionalDeviceGameObject;
    [SerializeField]
    private Vector3 _directionalDeviceAngleOfffset = new Vector3(0, 180, 0);

    // Enums
    public enum SwingMode
    {
        NoButtons,
        BothButtons,
        LeftButton,
        RightButton,
        OneButton
    };

    public enum ControllerButton
    {
        Menu,
        Grip,
        TouchPad,
        Trigger
    };

    public enum SwingDirectionMode
    {
        AvgDeviceDirection,
        HeadDirection,
        DirectionalDevice
    };

    public enum SmoothingMode { Lowest, Average, AverageMinusHighest };

    // Controller positions
    private Vector3 _leftDeviceLocalPosition;
    private Vector3 _rightDeviceLocalPosition;
    private Vector3 _previousLeftDeviceLocalPosition;
    private Vector3 _previousRightDeviceLocalPosition;

    // Controller Movement Result History
    private LinkedList<float> _deviceMovementResultHistory = new LinkedList<float>(); // The controller movement results after the Swing Mode calculations but before inertia and 1/2-hand coefficients

    // Saved angles

    // Saved movement
    private float _latestArtificialMovement;
    private Quaternion _latestArtificialRotation;
    private float _previousTimeDeltaTime = 0f;

    // Inertia curves
    // WARNING: must be linear for now
    private AnimationCurve _movingInertiaCurve = new AnimationCurve(new Keyframe(0, 1, -1, -1), new Keyframe(1, 0, -1, -1));
    private AnimationCurve _stoppingInertiaCurve = new AnimationCurve(new Keyframe(0, 1, -1, -1), new Keyframe(1, 0, -1, -1));

    // Used for test scene only
    private bool _useNonLinearMovementCurve = true;
    private AnimationCurve _inspectorCurve;

    //// Controller buttons ////
    private bool _leftButtonPressed = false;
    private bool _rightButtonPressed = false;

    // Camera Rig scaling
    private float _cameraRigScaleModifier = 1.0f;

    private bool _initialized = false;

    /****** INITIALIZATION ******/

    private void Start()
    {
        Initialize();
    }
    void Initialize()
    {
        if (_headsetGameObject == null)
            return;

        // Seed the initial previousLocalPositions
        if (_leftDeviceGameObject != null)
            _leftDeviceLocalPosition = _leftDeviceGameObject.transform.localPosition;
        else
            return;
        if (_rightDeviceGameObject != null)
            _rightDeviceLocalPosition = _rightDeviceGameObject.transform.localPosition;
        else
            return;

        if (_directionalDeviceGameObject == null && DirectionMode == SwingDirectionMode.DirectionalDevice)
            return;

        if (!EventSubscription())
            return;

        // Save the initial movement curve, in case it's switched off
        _inspectorCurve = _deviceToMovementCurve;

        // Verify and fix settings
        verifySettings();
        _initialized = true;
    }

    public virtual bool EventSubscription()
    {
        throw new NotImplementedException();
    }

    /***** FIXED UPDATE *****/
    void FixedUpdate()
    {
        EventGeneration();
        if (_initialized)
        {
            // Set scale as necessary (defaults to 1.0)
            // Doing this in Update() allows the Camera Rig to be scaled during runtime but keep the same Swinger feel
            if (generalScaleWorldUnitsToCameraRigScale)
            {
                _cameraRigScaleModifier = this.transform.localScale.x;
            }

            // Save the current controller positions for our use
            _leftDeviceLocalPosition = _leftDeviceGameObject.transform.localPosition;
            _rightDeviceLocalPosition = _rightDeviceGameObject.transform.localPosition;

            // Variable motion based on controller movement
            if (_swingEnabled)
            {
                if (this._movementSpeedMultiplier != 0f)
                {
                    Vector3 moveDirection = new Vector3();
                    /*if (Target.isGrounded)
                    {*/
                        moveDirection = variableSwingMotion();
                    /*}
                    moveDirection.y -= 9.81f * Time.deltaTime;*/
                    _target.Move(moveDirection);
                }
                /*else
                    Target.Move(Vector3.zero);*/
            }

            // Save the current controller positions for next frame
            _previousLeftDeviceLocalPosition = _leftDeviceLocalPosition;
            _previousRightDeviceLocalPosition = _rightDeviceLocalPosition;

            // Save this Time.deltaTime for next frame (inertia simulation)
            _previousTimeDeltaTime = Time.deltaTime;
        }
    }

    public virtual void EventGeneration()
    {
        throw new NotImplementedException();
    }

    /***** VERIFY SETTINGS *****/
    void verifySettings()
    {
        // Check fixed time setting
        if (Time.fixedDeltaTime > 1f / 90f)
        {
            if (generalAutoAdjustFixedTimestep)
            {
                Debug.LogWarning("GenericSwinger.verifySettings():: Fixed Timestep is set to " + Time.fixedDeltaTime + ".  Since you have generalAutoAdjustFixedTimestep set to true, Swinger will auto adjust this value to " + 1f / 90f + " (90 steps per second) for you.");
                Time.fixedDeltaTime = 1f / 90f;
            }
            else
            {
                Debug.LogError("GenericSwinger.verifySettings():: Fixed Timestep is set to " + Time.fixedDeltaTime + ".  This will cause stuttering movement when swinging.  Consider changing your Fixed Timestep to " + 1f / 90f + " (90 steps per second) by going to Edit -> Project Settings -> Time -> Fixed Timestep.");
            }
        }
    }

    /***** CORE FUNCTIONS *****/
    // Variable Swing locomotion
    Vector3 variableSwingMotion()
    {

        // Initialize movement variables
        float movementAmount = 0f;
        Quaternion movementRotation = Quaternion.identity;
        bool movedThisFrame = false;

        switch (swingMode)
        {
            case SwingMode.NoButtons:
                movedThisFrame = swingNoButtons(ref movementAmount, ref movementRotation);
                break;
            case SwingMode.BothButtons:
                movedThisFrame = swingBothButtons(ref movementAmount, ref movementRotation);
                break;
            case SwingMode.RightButton:
                movedThisFrame = swingLeftRightButtons(ref movementAmount, ref movementRotation);
                break;
            case SwingMode.LeftButton:
                movedThisFrame = swingLeftRightButtons(ref movementAmount, ref movementRotation);
                break;
            case SwingMode.OneButton:
                movedThisFrame = swingOneButton(ref movementAmount, ref movementRotation);
                break;
        }

        if (movedThisFrame)
        {
            Swinging = true;

            _latestArtificialMovement = movementAmount;
            _latestArtificialRotation = movementRotation;

            // Move forward in the X and Z axis only (no flying!)
            Vector3 movement = new Vector3();

            movement = getForwardXZ(movementAmount * _movementSpeedMultiplier, movementRotation);
            return movement;

        }
        else
        {
            Swinging = false;

            return Vector3.zero;
        }
    }

    // Swing when SwingMode is NoButtons
    bool swingNoButtons(ref float movement, ref Quaternion rotation)
    {
        rotation = determineRotation();

        // Find the change in controller position since last Update()
        float leftDeviceChange = 0;
        float rightDeviceChange = 0;

        float leftMovement = 0;
        float rightMovement = 0;


        if (_swingOnlyVerticalMovement)
            leftDeviceChange = Mathf.Abs(_previousLeftDeviceLocalPosition.y - _leftDeviceLocalPosition.y);
        else
            leftDeviceChange = Vector3.Distance(_previousLeftDeviceLocalPosition, _leftDeviceLocalPosition);
        if (_swingOnlyVerticalMovement)
            rightDeviceChange = Mathf.Abs(_previousRightDeviceLocalPosition.y - _rightDeviceLocalPosition.y);
        else
            rightDeviceChange = Vector3.Distance(_previousRightDeviceLocalPosition, _rightDeviceLocalPosition);

        // Calculate what camera rig movement the change should be converted to
        leftMovement = calculateMovement(_deviceToMovementCurve, leftDeviceChange, SwingDeviceSpeedForMaxSpeed, SwingMaxSpeed);
        rightMovement = calculateMovement(_deviceToMovementCurve, rightDeviceChange, SwingDeviceSpeedForMaxSpeed, SwingMaxSpeed);

        // Both controllers are in use, so controller movement is the average of the two controllers' change times the controller coefficient
        float controllerMovement = (leftMovement + rightMovement) / 2 * SwingCoefficient;

        if (movingInertia)
        {
            controllerMovement = movingInertiaOrDeviceMovement(controllerMovement);
        }

        if (controllerMovement == 0)
        {
            if (stoppingInertia && _latestArtificialMovement != 0)
            {

                // The rotation is the cached one
                rotation = _latestArtificialRotation;
                // The stopping movement is calculated using a curve, the latest movement, last frame's deltaTime, max speed, and the stop time for max speed
                movement = inertiaMovementChange(_stoppingInertiaCurve, _latestArtificialMovement, _previousTimeDeltaTime, SwingMaxSpeed, stoppingInertiaTimeToStopAtMaxSpeed);

                return true;
            }
            else
                return false;
        }
        else
            movement = controllerMovement;

        return true;
    }

    // Swing when SwingMode is BothButtons
    bool swingBothButtons(ref float movement, ref Quaternion rotation)
    {
        if (_leftButtonPressed && _rightButtonPressed)
        {
            rotation = determineRotation();

            // Find the change in controller position since last Update()
            float leftDeviceChange = 0;
            float rightDeviceChange = 0;

            float leftMovement = 0;
            float rightMovement = 0;

            if (_swingOnlyVerticalMovement)
                leftDeviceChange = Mathf.Abs(_previousLeftDeviceLocalPosition.y - _leftDeviceLocalPosition.y);
            else
                leftDeviceChange = Vector3.Distance(_previousLeftDeviceLocalPosition, _leftDeviceLocalPosition);
            if (_swingOnlyVerticalMovement)
                rightDeviceChange = Mathf.Abs(_previousRightDeviceLocalPosition.y - _rightDeviceLocalPosition.y);
            else
                rightDeviceChange = Vector3.Distance(_previousRightDeviceLocalPosition, _rightDeviceLocalPosition);

            // Calculate what camera rig movement the change should be converted to
            leftMovement = calculateMovement(_deviceToMovementCurve, leftDeviceChange, SwingDeviceSpeedForMaxSpeed, SwingMaxSpeed);
            rightMovement = calculateMovement(_deviceToMovementCurve, rightDeviceChange, SwingDeviceSpeedForMaxSpeed, SwingMaxSpeed);

            // Both controllers are in use, so controller movement is the average of the two controllers' change times the both controller coefficient
            float controllerMovement = (leftMovement + rightMovement) / 2 * SwingCoefficient;

            // If movingInertia is enabled, the higher of inertia or controller movement is used
            if (movingInertia)
            {
                movement = movingInertiaOrDeviceMovement(controllerMovement);
            }
            else
            {
                movement = controllerMovement;
            }

            return true;
        }
        // If stopping inertia is enabled
        else if (stoppingInertia && _latestArtificialMovement != 0)
        {

            // The rotation is the cached one
            rotation = _latestArtificialRotation;
            // The stopping movement is calculated using a curve, the latest movement, last frame's deltaTime, max speed, and the stop time for max speed
            movement = inertiaMovementChange(_stoppingInertiaCurve, _latestArtificialMovement, _previousTimeDeltaTime, SwingMaxSpeed, stoppingInertiaTimeToStopAtMaxSpeed);

            return true;
        }
        else
        {
            return false;
        }
    }

    // Swing when SwingMode is LeftButton or RightButton
    bool swingLeftRightButtons(ref float movement, ref Quaternion rotation)
    {
        if (swingMode == SwingMode.LeftButton && _leftButtonPressed ||
               swingMode == SwingMode.RightButton && _rightButtonPressed)
        {
            rotation = determineRotation();

            // Find the change in controller position since last Update()
            float leftDeviceChange = 0;
            float rightDeviceChange = 0;

            float leftMovement = 0;
            float rightMovement = 0;

            if (_swingOnlyVerticalMovement)
                leftDeviceChange = Mathf.Abs(_previousLeftDeviceLocalPosition.y - _leftDeviceLocalPosition.y);
            else
                leftDeviceChange = Vector3.Distance(_previousLeftDeviceLocalPosition, _leftDeviceLocalPosition);
            if (_swingOnlyVerticalMovement)
                rightDeviceChange = Mathf.Abs(_previousRightDeviceLocalPosition.y - _rightDeviceLocalPosition.y);
            else
                rightDeviceChange = Vector3.Distance(_previousRightDeviceLocalPosition, _rightDeviceLocalPosition);

            // Calculate what camera rig movement the change should be converted to
            leftMovement = calculateMovement(_deviceToMovementCurve, leftDeviceChange, SwingDeviceSpeedForMaxSpeed, SwingMaxSpeed);
            rightMovement = calculateMovement(_deviceToMovementCurve, rightDeviceChange, SwingDeviceSpeedForMaxSpeed, SwingMaxSpeed);

            // Both controllers are in use, so controller movement is the average of the two controllers' change times the both controller coefficient
            float controllerMovement = (leftMovement + rightMovement) / 2 * SwingCoefficient;

            // If movingInertia is enabled, the higher of inertia or controller movement is used
            if (movingInertia)
            {
                movement = movingInertiaOrDeviceMovement(controllerMovement);
            }
            else
            {
                movement = controllerMovement;
            }

            return true;
        }
        // If stopping inertia is enabled
        else if (stoppingInertia && _latestArtificialMovement != 0)
        {

            // The rotation is the cached one
            rotation = _latestArtificialRotation;
            // The stopping movement is calculated using a curve, the latest movement, last frame's deltaTime, max speed, and the stop time for max speed
            movement = inertiaMovementChange(_stoppingInertiaCurve, _latestArtificialMovement, _previousTimeDeltaTime, SwingMaxSpeed, stoppingInertiaTimeToStopAtMaxSpeed);

            return true;
        }
        else
        {
            return false;
        }
    }

    // Swing when SwingMode is OneButton
    bool swingOneButton(ref float movement, ref Quaternion rotation)
    {
        if (_leftButtonPressed || _rightButtonPressed)
        {
            rotation = determineRotation();

            // Find the change in controller position since last Update()
            float leftDeviceChange = 0;
            float rightDeviceChange = 0;

            float leftMovement = 0;
            float rightMovement = 0;

            if (_swingOnlyVerticalMovement)
                leftDeviceChange = Mathf.Abs(_previousLeftDeviceLocalPosition.y - _leftDeviceLocalPosition.y);
            else
                leftDeviceChange = Vector3.Distance(_previousLeftDeviceLocalPosition, _leftDeviceLocalPosition);
            if (_swingOnlyVerticalMovement)
                rightDeviceChange = Mathf.Abs(_previousRightDeviceLocalPosition.y - _rightDeviceLocalPosition.y);
            else
                rightDeviceChange = Vector3.Distance(_previousRightDeviceLocalPosition, _rightDeviceLocalPosition);

            // Calculate what camera rig movement the change should be converted to
            leftMovement = calculateMovement(_deviceToMovementCurve, leftDeviceChange, SwingDeviceSpeedForMaxSpeed, SwingMaxSpeed);
            rightMovement = calculateMovement(_deviceToMovementCurve, rightDeviceChange, SwingDeviceSpeedForMaxSpeed, SwingMaxSpeed);

            // Both controllers are in use, so controller movement is the average of the two controllers' change times the both controller coefficient
            float controllerMovement = (leftMovement + rightMovement) / 2 * SwingCoefficient;


            if (movingInertia)
            {
                movement = movingInertiaOrDeviceMovement(controllerMovement);
            }
            else
            {
                movement = controllerMovement;
            }

            return true;
        }
        // If stopping inertia is enabled
        else if (stoppingInertia && _latestArtificialMovement != 0)
        {

            // The rotation is the cached one
            rotation = _latestArtificialRotation;
            // The stopping movement is calculated using a curve, the latest movement, last frame's deltaTime, max speed, and the stop time for max speed
            movement = inertiaMovementChange(_stoppingInertiaCurve, _latestArtificialMovement, _previousTimeDeltaTime, SwingMaxSpeed, stoppingInertiaTimeToStopAtMaxSpeed);

            return true;
        }
        else
        {
            return false;
        }
    }

    float movingInertiaOrDeviceMovement(float movement)
    {

        if (_smoothing)
        {
            // Save the movement amount for moving inertia calculations
            saveFloatToLinkedList(_deviceMovementResultHistory, movement, _deviceSmoothingCacheSize);

            movement = smoothedControllerMovement(_deviceMovementResultHistory);

        }

        float inertiaMovement = inertiaMovementChange(_movingInertiaCurve, _latestArtificialMovement, _previousTimeDeltaTime, SwingMaxSpeed, movingInertiaTimeToStopAtMaxSpeed);

        if (inertiaMovement >= movement)
        {
            return inertiaMovement;
        }
        else
        {
            return movement;
        }
    }

    // Using a linear curve, the movement last frame, the max speed we can go, and the time it should take to stop at that max speed,
    // compute the amount of movement that should happen THIS frame.  Note that timeToStopAtMaxSpeed is only if the player is going armSwingMaxSpeed.  If the
    // player is going LESS than armSwingMaxSpeed, the time to stop will be a percentage of timeToStopAtMaxSpeed.
    //
    // I tried implemeting this with custom curves, but ran into an issue where I couldn't determine where in the curve to start given the player's 
    // current speed.  For now, I'm making it linear-only, which works fine.  Would be amazing to make it work with arbitrary curves in the future.
    //
    // Also, I guarantee this can be done better.  I should have paid more attention in math.  Sorry, Mrs. Powell.

    // TODO: Make work with custom curves instead of linear only
    static float inertiaMovementChange(AnimationCurve curve, float latestMovement, float latestTimeDeltaTime, float maxSpeed, float timeToStopAtMaxSpeed)
    {

        // Max speed in Movement Per Frame
        float maxSpeedInMPF = maxSpeed * Time.deltaTime;

        // Frames per second
        float fps = 1 / Time.deltaTime;

        // The percentage through the curve the last movement was (based on the previous frame's Time.deltaTime)
        float percentThroughCurve = 1 - (latestMovement / (maxSpeed * latestTimeDeltaTime));

        // The percentage change in speed that needs to happen each frame based on the current frame rate
        float percentChangeEachFrame = 1 / (timeToStopAtMaxSpeed * fps);

        // Calculate the new percentage through the curve by adding the percent change each frame to the last percent
        float newPercentThroughCurve = percentThroughCurve + percentChangeEachFrame;

        // Evaluate the curve at the new percentage to determine how what percentage of armSwingMaxSpeed we should be going this frame
        float curveEval = curve.Evaluate(newPercentThroughCurve);

        // Set the movement value based on the evaluated curve, multiplied by the max Speed in Movement Per Frame
        float inertiaMovement = curveEval * maxSpeedInMPF;

        if (inertiaMovement <= 0f)
        {
            inertiaMovement = 0f;
        }

        return inertiaMovement;

    }

    void savePosition(LinkedList<Vector3> positionList, Vector3 position, int maxListSize)
    {
        // Store the position
        positionList.AddLast(position);

        while (positionList.Count > maxListSize)
        {
            positionList.RemoveFirst();
        }
    }

    void saveFloatToLinkedList(LinkedList<float> linkedList, float value, int maxListSize)
    {
        // Store the position
        linkedList.AddLast(value);

        while (linkedList.Count > maxListSize)
        {
            linkedList.RemoveFirst();
        }

    }
    public void SetHMD(GameObject HMD)
    {
        if (!_initialized)
        {
            _headsetGameObject = HMD;
            Initialize();
        }
    }
    /***** HELPER FUNCTIONS *****/
    public void SetDevices(GameObject HMD, GameObject LeftHand, GameObject RightHand, GameObject DirectionalTracker)
    {
        if (!_initialized)
        {
            _headsetGameObject = HMD;
            _leftDeviceGameObject = LeftHand;
            _rightDeviceGameObject = RightHand;
            _directionalDeviceGameObject = DirectionalTracker;
            Initialize();
        }
    }

    //done replace with action from xrtoolkit 

    public void LeftButtonPressed(InputAction.CallbackContext e)
    {
        _leftButtonPressed = true;
    }
    public void RightButtonPressed(InputAction.CallbackContext e)
    {
        _rightButtonPressed = true;
    }
    public void LeftButtonUnpressed(InputAction.CallbackContext e)
    {
        _leftButtonPressed = false;
    }
    public void RightButtonUnpressed(InputAction.CallbackContext e)
    {
        _rightButtonPressed = false;
    }

    // Returns the average of two Quaternions
    Quaternion averageRotation(Quaternion rot1, Quaternion rot2)
    {
        return Quaternion.Slerp(rot1, rot2, 0.5f);
    }


    // Returns a Vector3 with only the X and Z components (Y is 0'd)
    public static Vector3 vector3XZOnly(Vector3 vec)
    {
        return new Vector3(vec.x, 0f, vec.z);
    }

    // Returns a forward vector given the distance and direction
    public static Vector3 getForwardXZ(float forwardDistance, Quaternion direction)
    {
        Vector3 forwardMovement = direction * Vector3.forward * forwardDistance;
        return vector3XZOnly(forwardMovement);
    }

    public Quaternion determineAverageDeviceRotationPublic(out Quaternion left, out Quaternion right)
    {
        left = _leftDeviceGameObject.transform.rotation;
        right = _rightDeviceGameObject.transform.rotation;
        return determineRotation();
    }

    // Returns the average rotation of the two controllers
    Quaternion determineAverageDeviceRotation()
    {
        // Build the average rotation of the controller(s)
        Quaternion newRotation;

        // Both controllers are present
        if (_leftDeviceGameObject != null && _rightDeviceGameObject != null)
        {
            newRotation = averageRotation(_leftDeviceGameObject.transform.rotation * Quaternion.Euler(_leftDeviceAngleOffset), _rightDeviceGameObject.transform.rotation * Quaternion.Euler(_rightDeviceAngleOffset));
        }
        // Left controller only
        else if (_leftDeviceGameObject != null && _rightDeviceGameObject == null)
        {
            newRotation = _leftDeviceGameObject.transform.rotation * Quaternion.Euler(_leftDeviceAngleOffset);
        }
        // Right controller only
        else if (_rightDeviceGameObject != null && _leftDeviceGameObject == null)
        {
            newRotation = _rightDeviceGameObject.transform.rotation * Quaternion.Euler(_rightDeviceAngleOffset);
        }
        // No controllers!
        else
        {
            newRotation = Quaternion.identity;
        }

        return newRotation;
    }

    Quaternion determineRotation()
    {
        // Build the average rotation of the controller(s)
        Quaternion Rot = Quaternion.identity;

        switch (DirectionMode)
        {
            case SwingDirectionMode.AvgDeviceDirection:
                Rot = determineAverageDeviceRotation();
                break;
            case SwingDirectionMode.HeadDirection:
                Rot = _headsetGameObject.transform.rotation * Quaternion.Euler(_headsetAngleOffset);
                break;
            case SwingDirectionMode.DirectionalDevice:
                Rot = _directionalDeviceGameObject.transform.rotation * Quaternion.Euler(_directionalDeviceAngleOfffset);
                break;
        }

        return Rot;
    }

    float smoothedControllerMovement(LinkedList<float> controllerMovementHistory)
    {

        // Chose the lowest value in the cache
        if (_smoothingMode == SmoothingMode.Lowest)
        {
            float low = controllerMovementHistory.First.Value;

            foreach (float val in controllerMovementHistory)
            {
                if (val < low)
                {
                    low = val;
                }
            }

            return low;
        }
        // Compute the average of all values in the cache
        else if (_smoothingMode == SmoothingMode.Average)
        {
            float total = 0;

            foreach (float val in controllerMovementHistory)
            {
                total += val;
            }

            return (total / controllerMovementHistory.Count);
        }
        // Compute the average of all values in the cache, but throw out the highest one
        // Functions the same as "Lowest" if the cache size is 2
        else if (_smoothingMode == SmoothingMode.AverageMinusHighest)
        {
            // If the controllerMovementHistory has a length of 1, just return that value
            if (controllerMovementHistory.Count == 1)
            {
                return controllerMovementHistory.First.Value;
            }

            float high = controllerMovementHistory.First.Value;
            float total = 0;

            foreach (float val in controllerMovementHistory)
            {
                total += val;
                if (val > high)
                {
                    high = val;
                }
            }

            return ((total - high) / (controllerMovementHistory.Count - 1));
        }
        else
        {
            Debug.LogError("GenericSwinger.smoothedControllerMovement():: Invalid value for controllerSmoothingMode!");
            return 0;
        }
    }

    float linkedListAverage(LinkedList<float> linkedList)
    {
        float sum = 0;
        foreach (float val in linkedList)
        {
            sum += val;
        }

        return sum / linkedList.Count;

    }

    static float calculateMovement(AnimationCurve curve, float change, float maxInput, float maxSpeed)
    {
        float changeInWUPS = change / Time.deltaTime;
        float movement = Mathf.Lerp(0, maxSpeed, curve.Evaluate(changeInWUPS / maxInput)) * Time.deltaTime;

        return movement;
    }

    /***** GET SET *****/
    public float SwingCoefficient
    {
        get
        {
            return _swingCoefficient;
        }

        set
        {
            float min = 0f;
            float max = 10f;

            if (value >= min && value <= max)
            {
                _swingCoefficient = value;
            }
            else
            {
                Debug.LogWarning("GenericSwinger:SwingBothControllersCoefficient:: Requested new value " + value + " is out of range (" + min + ".." + max + ")");
            }
        }

    }

    public bool useNonLineMovementCurve
    {
        get
        {
            return _useNonLinearMovementCurve;
        }
        set
        {
            if (value)
            {
                _deviceToMovementCurve = _inspectorCurve;
            }
            else
            {
                _deviceToMovementCurve = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
            }

            _useNonLinearMovementCurve = value;
        }
    }

    public float SwingDeviceSpeedForMaxSpeed
    {
        get
        {
            return _swingDeviceSpeedForMaxSpeed;
        }
        set
        {
            _swingDeviceSpeedForMaxSpeed = value;
        }
    }

    public float SwingMaxSpeed
    {
        get
        {
            return _swingMaxSpeed * _cameraRigScaleModifier;
        }
        set
        {
            _swingMaxSpeed = value;
        }
    }

    public SwingMode swingMode
    {
        get
        {
            return _swingMode;
        }
        set
        {
            _deviceMovementResultHistory.Clear();
            _swingMode = value;
        }
    }


    public SwingDirectionMode DirectionMode
    {
        get
        {
            return _swingDirectionMode;
        }
        set
        {
            _swingDirectionMode = value;
        }
    }

    public bool movingInertia
    {
        get
        {
            return _movingInertia;
        }
        set
        {
            _latestArtificialMovement = 0f;
            _movingInertia = value;
        }
    }

    public bool stoppingInertia
    {
        get
        {
            return _stoppingInertia;
        }
        set
        {
            _latestArtificialMovement = 0f;
            _stoppingInertia = value;
        }
    }
}
