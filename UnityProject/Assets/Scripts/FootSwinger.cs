using System.Collections.Generic;
using PrattiToolkit;
using UnityEngine;
using UnityEngine.InputSystem.XR;
//using Valve.VR;
using UnityEngine.XR.Interaction.Toolkit;

public class FootSwinger : MonoBehaviour {

    /***** CLASS VARIABLES *****/

    /** Inspector Variables **/

    // General Settings
    [Header("General Settings")]
    [Tooltip("General - Scale World Units To Camera Rig Scale\n\nBy default, several unit- and speed-based settings are in absolute world units regardless of CameraRig scale.  If this setting is true, all of those settings will be automatically scaled to match the X scale of this CameraRig.  If you use a non-default CameraRig scale, enabling this setting will allow you to specify all settings in meters-per-second in relation to the CameraRig rather than in world units.\n\n(Default: false)")]
    public bool generalScaleWorldUnitsToCameraRigScale = false;
    [Tooltip("General - Auto Adjust Fixed Timestep\n\nIn order for FootSwinger to handle movement and wall collisions correctly, Time.fixedDeltaTime must be 0.0111 (90 per second) or less.  If this feature is enabled, the setting will be adjusted automatically if it is higher than 0.0111.  If disabled, an error will be generated but the value will not be changed.\n\n(Default: true)")]
    public bool generalAutoAdjustFixedTimestep = true;

    // Foot Swing Settings
    [Range(0.0f, 50.0f)]
    [Tooltip("Movement Speed Multiplier\n\n. A multiplier useful to regulate the movement speed without having to deal with curves or min/max values.  \n\n(Default: 1)")]
    public float movementSpeedMultiplier = 1.0f;
    [SerializeField]
    [Tooltip("Foot Swing Tracking Device\n\nThe device used to track the legs of the player.\n\n(Default: Controller)")]
    private LegTrackingDevice _footSwingLegTrackingDevice = LegTrackingDevice.Controller;
    [SerializeField]
    [Tooltip("Foot Swing - Button\nOnly if Foot Swing Navigation is enabled, FootS wing Mode isn't No Buttons and leg controllers are available\n\nDefines which controller button is used to activate FootSwinger.  The button is the same on both controllers.\n\n(Default: Grip button)")]
    private ControllerButton _footSwingButton = ControllerButton.Grip;
    [SerializeField]
    [Tooltip("Foot Swing - Direction Mode\nDetermines what direction is used to calculate the movement.\n\nLeg Direction Rear Controllers - uses the average rotation of the leg controllers, supports must be placed behind the calfs.\n\nLeg Direction Side Controllers - uses the average rotation of the leg controllers, supports must be placed on the external side of the legs.\n\nArm Direction - uses the rotation of the arm controllers.\n\nHead Direction - uses the direction of the headset.\n\n(Default: Leg Direction Rear Controllers)")]
    private FootSwingDirectionMode _footSwingDirectionMode = FootSwingDirectionMode.LegDirectionRearControllers;
    [SerializeField]
    [Tooltip("Foot Swing - Controller To Movement Curve\nOnly if Foot Swing Navigation is enabled.\n\nCurve that determines how much a given controller change translates into camera rig movement.  The far left of the curve is no controller movement and no virtual movement.  The far right is Controller Speed For Max Speed (controller movement) and Max Speed (virtual momvement).\n\n(Default: Linear)")]
    public AnimationCurve FootSwingControllerToMovementCurve = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
    [SerializeField]
    [Tooltip("Foot Swing - Controller Speed For Max Speed\nOnly if Foot Swing Navigation is enabled\n\nThe number of CameraRig local units per second a controller needs to be moving to be considered going max speed.\n\n(Default:3)")]
    private float _FootSwingControllerSpeedForMaxSpeed = 3f;
    [SerializeField]
    [Tooltip("Foot Swing - Max Speed\nOnly if Foot Swing Navigation is enabled\n\nThe fastest base speed (in world units) a player can travel when moving controllers at Controller Movement Per Second For Max Speed.  The actual max speed of the player will depend on the both/single controller coefficients you configure.\n\n(Default: 8)")]
    private float _FootSwingMaxSpeed = 8;
    [SerializeField]
    [Tooltip("Foot Swing - Both Controllers Coefficient\nOnly if Foot Swing Navigation is enabled and Swing Activation Mode allows both controllers to be used for Foot swinging.\n\nUsed to boost or nerf the player's speed when using boths controllers for Foot swinging.  A value of 1.0 will not modify the curve / max speed calculation.\n\n(Default: 1.0)")]
    [Range(0f, 10f)]
    private float _FootSwingBothControllersCoefficient = 1.0f;
    [SerializeField]
    [Tooltip("Foot Swing - Only Vertical Movement\nDiscard X and Z when calculating the space travelled by the leg tracking devices.\n(Default: false)")]
    public bool FootSwingOnlyVerticalMovement = false;
    [SerializeField]
    [Tooltip("Left Leg Device Color\nOnly if _footSwingLegTrackingDevice is Tracker and FootSwingDirectionMode is LegDirectionSideControllers\n\nColor of the left leg devices, in order to distinguish it.\n(Default: red)")]
    public Color LeftLegDeviceColor = Color.red;
    [SerializeField]
    [Tooltip("Right Leg Device Color\nOnly if _footSwingLegTrackingDevice is Tracker and FootSwingDirectionMode is LegDirectionSideControllers\n\nColor of the right leg devices, in order to distinguish it.\n(Default: green)")]
    public Color RightLegDeviceColor = Color.green;
    [SerializeField]
    [Tooltip("Right Leg Device Color\nOnly if the Directional Tracker is present\n\nColor of the directional device, in order to distinguish it.\n(Default: white)")]
    public Color DirectionalDeviceColor = Color.white;
    [SerializeField]
    [Tooltip("Custom Texture for the HTC Vive Trackers")]
    public Texture TrackerTexture;

    // Controller Smoothing Settings
    [Header("Controller Smoothing Settings")]
    [Tooltip("Controller Smoothing\n\nUses controller movement sampling to help eliminate jerks and unpleasant movement when controllers suddenly change position due to tracking inaccuracies. It is highly recommended to turn this setting on if using movingInertia\n\n(Default: true)")]
    public bool controllerSmoothing = true;
    [Tooltip("Controller Smoothing - Mode\nOnly if Controller Smoothing is enabled\n\nDetermines how controller smoothing calculates the smoothed movement value used by Foot swinging.\n\nLowest\nUse the lowest value in the cache.  Should only be used with small cache sizes.\n\nAverage\nUse the average of all values in the cache.\n\nAverage Minus Highest\nUse the average of all values in the cache, but disregard the highest value.  When a controller jitters, the value change in that frame is almost always higher than normal values and will be discarded.\n\n(Default: Average Minus Highest)")]
    public ControllerSmoothingMode controllerSmoothingMode = ControllerSmoothingMode.AverageMinusHighest;
    [Range(2, 90)]
    [Tooltip("Controller Smoothing - Cache Size\nOnly if Controller Smoothing is enabled\n\nSets the number of calculated controller movements to keep in the cache.  Setting this number too low may allow a jittering controller to cause jerky movements for the player.  Setting this number too high increases lag time from controller movement to camera rig movement.\n\n(Default: 3)")]
    public int controllerSmoothingCacheSize = 3;

    // Inertia Settings
    [Header("Inertia Settings")]
    [SerializeField]
    [Tooltip("Moving Inertia\n\nSimulates inertia while swinging.  If the controllers change position slower than the moving inertia calculation, the inertia calculation will be used to determine forward movement.\n\n(Default: true)")]
    private bool _movingInertia = true;
    [Tooltip("Moving Inertia - Time To Stop At Max Speed\nOnly if Moving Inertia is enabled\n\nThe time it will take to go from footSwingMaxSpeed to 0 if foot swinging is engaged and the player does not move the controllers.  Speeds lower than armSwingMaxSpeed will scale their stopping time linearly.\n\n(Default: .5)")]
    public float movingInertiaTimeToStopAtMaxSpeed = .5f;
    [SerializeField]
    [Tooltip("Stopping Inertia\n\nSimulates inertia when foot swinging stops.\n\n(Default: true)")]
    private bool _stoppingInertia = true;
    [Tooltip("Stopping Inertia - Time To Stop At Max Speed\nOnly if Stopping Inertia is enabled\n\nThe time it will take to go from armSwingMaxSpeed to 0 when foot swinging is disengaged.  Speeds lower than footSwingMaxSpeed will scale their stopping time linearly.\n\n(Default:.35)")]
    public float stoppingInertiaTimeToStopAtMaxSpeed = .35f;

    // Informational public bools
    [HideInInspector]
    public bool FootSwinging = false;
    [HideInInspector]
    bool initialized = false;

    //External components
    [Header("External Components")]
    [SerializeField]
    [Tooltip("CharactedController component attached to the parent of CameraRig, necessary to apply the movement generated by swinging.")]
    public CharacterController Target;
    [SerializeField]
    [Tooltip("Additiona tracker useful to track the decouple the movement direction from the limb average rotation.\n\n(Default: null)")]
    public TrackedPoseDriver DirectionalTracker;

    // Pause Variables
    [Header("Pause Variables")]
    [SerializeField]
    [Tooltip("Foot Swinging Paused\n\nPrevents the player from Foot swinging while true.\n\n(Default: false)")]
    private bool _FootSwingingPaused = false;

    // Enums
    
    public enum FootSwingDirectionMode
    {
        LegDirectionRearControllers,
        LegDirectionsSideControllers,
        ArmDirection,
        HeadDirection
    };

        public enum LegTrackingDevice
    {
        Controller,
        Tracker
    };

    public enum ControllerSmoothingMode { Lowest, Average, AverageMinusHighest };

    // Controller positions
    private Vector3 leftlegDeviceLocalPosition;
    private Vector3 rightlegDeviceLocalPosition;
    private Vector3 leftlegDevicePreviousLocalPosition;
    private Vector3 rightlegDevicePreviousLocalPosition;

    // Controller Movement Result History
    private LinkedList<float> controllerMovementResultHistory = new LinkedList<float>(); // The controller movement results after the Swing Mode calculations but before inertia and 1/2-hand coefficients

    // Saved angles

    // Saved movement
    private float latestArtificialMovement;
    private Quaternion latestArtificialRotation;
    private float previousTimeDeltaTime = 0f;

    // Inertia curves
    // WARNING: must be linear for now
    private AnimationCurve movingInertiaCurve = new AnimationCurve(new Keyframe(0, 1, -1, -1), new Keyframe(1, 0, -1, -1));
    private AnimationCurve stoppingInertiaCurve = new AnimationCurve(new Keyframe(0, 1, -1, -1), new Keyframe(1, 0, -1, -1));

    // Used for test scene only
    private bool _useNonLinearMovementCurve = true;
    private AnimationCurve inspectorCurve;

    //// Controller buttons ////
    [SerializeField]
    private InputManagement _input;
    private bool leftButtonPressed = false;
    private bool rightButtonPressed = false;

    //// Controllers ////
    //private SteamVR_ControllerManager controllerManager;
    [SerializeField]
    private GameObject leftlegDeviceGameObject;
    [SerializeField]
    private GameObject rightlegDeviceGameObject;
    private TrackedPoseDriver leftlegDeviceTrackedObj;
    private TrackedPoseDriver rightlegDeviceTrackedObj;
    /*private SteamVR_Controller.Device leftlegDevice;
    private SteamVR_Controller.Device rightlegDevice;
    /*private int leftlegDeviceIndex;
    private int rightlegDeviceIndex;*/
    [SerializeField]
    private GameObject leftControllerGameObject;
    [SerializeField]
    private GameObject rightControllerGameObject;
    private ActionBasedController leftController;
    private ActionBasedController rightController;
    /*private int leftControllerIndex;
    private int rightControllerIndex;*/

    // GameObjects
    [SerializeField]
    private GameObject headsetGameObject;

    // Camera Rig scaling
    private float cameraRigScaleModifier = 1.0f;
    
    private bool setleftcolorlater;
    private bool setrightcolorlater;
    private bool setdirectionalcolorlater;

    private void Awake()
    {
        //Debug.LogError("FootSwinger.cs hasn't been updated to XRInteractionToolkit yet");

    }

    /****** INITIALIZATION ******/ 
    void Initialize()
    {
        leftController = leftControllerGameObject.GetComponent<ActionBasedController>();
        rightController = rightControllerGameObject.GetComponent<ActionBasedController>();
        leftlegDeviceTrackedObj = leftlegDeviceGameObject.GetComponent<TrackedPoseDriver>();
        rightlegDeviceTrackedObj = rightlegDeviceGameObject.GetComponent<TrackedPoseDriver>();

        if (leftlegDeviceGameObject != null)
        {
            //var controller = leftlegDeviceGameObject.transform.Find("Model");
            // if (_footSwingLegTrackingDevice == LegTrackingDevice.Controller)
            // {
            //     var sc = leftlegDeviceGameObject.GetComponent<SphereCollider>();
            //     if (sc != null)
            //         sc.enabled = false;
            //     
            //     if (_footSwingDirectionMode == FootSwingDirectionMode.LegDirectionsSideControllers)
            //     {
            //             if (controller != null)
            //             {
            //                 if (controller.childCount == 0)
            //                     setleftcolorlater = true;
            //                 else
            //                 {
            //                     for (int i = 0; i < controller.childCount; i++)
            //                     {
            //                         Transform t = controller.GetChild(i);
            //                         var mr = t.GetComponent<MeshRenderer>();
            //                         if (mr != null)
            //                         {
            //                             var m = mr.materials[0];
            //                             Material m1 = new Material(m);
            //                             m1.EnableKeyword("_EMISSION");
            //                             m1.SetColor("_EmissionColor", LeftLegDeviceColor);
            //                             m1.SetTexture("_EmissionMap", m1.GetTexture("_MainTex"));
            //                             var mats = new Material[1];
            //                             mats[0] = m1;
            //                             mr.materials = mats;
            //                         }
            //                     }
            //                 }
            //             }
            //     }
            // }
            // else
            // {
            //     var mr = controller.GetComponent<MeshRenderer>();
            //     if (mr != null)
            //     {
            //         var m = mr.materials[0];
            //         Material m1 = new Material(m);
            //         if (_footSwingDirectionMode == FootSwingDirectionMode.LegDirectionsSideControllers)
            //         {
            //             m1.EnableKeyword("_EMISSION");
            //             m1.SetColor("_EmissionColor", LeftLegDeviceColor);
            //             if (TrackerTexture != null)
            //             {
            //                 m1.SetTexture("_EmissionMap", TrackerTexture);
            //                 m1.SetTexture("_MainTex", TrackerTexture);
            //             }
            //             else
            //                 m1.SetTexture("_EmissionMap", m1.GetTexture("_MainTex"));
            //         }
            //         else
            //         {
            //             if (TrackerTexture != null)
            //                 m1.SetTexture("_MainTex", TrackerTexture);
            //         }
            //         var mats = new Material[1];
            //         mats[0] = m1;
            //         mr.materials = mats;
            //     }
            //     else
            //         setleftcolorlater = true;
            // }
            leftlegDeviceTrackedObj = leftlegDeviceGameObject.GetComponent<TrackedPoseDriver>();
            var model = GetComponentInParent<ControllersModelsLoaderOpenXR>().RetrieveModel("HTC Vive Tracker");
            model.transform.parent = leftlegDeviceGameObject.transform;
            model.transform.localPosition = Vector3.zero;
            model.transform.rotation = Quaternion.Euler(0, 180, 0);;
        }
        
            /*if (controllerManager.objects.Length > 2)
                rightlegDeviceGameObject = controllerManager.objects[3];*/

            if (rightlegDeviceGameObject != null)
            {
                // var controller = rightlegDeviceGameObject.transform.Find("Model");
                //
                // if (_footSwingLegTrackingDevice == LegTrackingDevice.Controller)
                // {
                //     var sc = rightlegDeviceGameObject.GetComponent<SphereCollider>();
                //     if (sc != null)
                //         sc.enabled = false;
                //
                //
                //     if (_footSwingDirectionMode == FootSwingDirectionMode.LegDirectionsSideControllers)
                //     {
                //         if (controller != null)
                //         {
                //             if (controller.childCount == 0)
                //                 setrightcolorlater = true;
                //             else
                //             {
                //                 for (int i = 0; i < controller.childCount; i++)
                //                 {
                //                     Transform t = controller.GetChild(i);
                //                     var mr = t.GetComponent<MeshRenderer>();
                //                     if (mr != null)
                //                     {
                //                         var m = mr.materials[0];
                //                         Material m1 = new Material(m);
                //                         m1.EnableKeyword("_EMISSION");
                //                         m1.SetColor("_EmissionColor", RightLegDeviceColor);
                //                         m1.SetTexture("_EmissionMap", m1.GetTexture("_MainTex"));
                //                         var mats = new Material[1];
                //                         mats[0] = m1;
                //                         mr.materials = mats;
                //                     }
                //                 }
                //             }
                //         }
                //     }
                // }
                // else
                // {
                //     var mr = controller.GetComponent<MeshRenderer>();
                //     if (mr != null)
                //     {
                //         var m = mr.materials[0];
                //         Material m1 = new Material(m);
                //         if (_footSwingDirectionMode == FootSwingDirectionMode.LegDirectionsSideControllers)
                //         {
                //             m1.EnableKeyword("_EMISSION");
                //             m1.SetColor("_EmissionColor", RightLegDeviceColor);
                //             if (TrackerTexture != null)
                //             {
                //                 m1.SetTexture("_EmissionMap", TrackerTexture);
                //                 m1.SetTexture("_MainTex", TrackerTexture);
                //             }
                //             else
                //                 m1.SetTexture("_EmissionMap", m1.GetTexture("_MainTex"));
                //         }
                //         else
                //         {
                //             if (TrackerTexture != null)
                //                 m1.SetTexture("_MainTex", TrackerTexture);
                //         }
                //
                //         var mats = new Material[1];
                //         mats[0] = m1;
                //         mr.materials = mats;
                //     }
                //     else
                //         setrightcolorlater = true;
                // }

                rightlegDeviceTrackedObj = rightlegDeviceGameObject.GetComponent<TrackedPoseDriver>();
                var model = GetComponentInParent<ControllersModelsLoaderOpenXR>().RetrieveModel("HTC Vive Tracker");
                model.transform.parent = rightlegDeviceTrackedObj.transform;
                model.transform.localPosition = Vector3.zero;
                model.transform.rotation = Quaternion.Euler(0, 180, 0);;
            }
       
        if (DirectionalTracker != null)
        {
            var model = GetComponentInParent<ControllersModelsLoaderOpenXR>().RetrieveModel("HTC Vive Tracker");
            model.transform.parent = DirectionalTracker.transform;
            model.transform.localPosition = Vector3.zero;
            model.transform.rotation = Quaternion.Euler(0, 180, 0);;
            // var mr = DirectionalTracker.GetComponent<MeshRenderer>();
            // if (mr != null)
            // {
            //     var m = mr.materials[0];
            //     Material m1 = new Material(m);
            //     m1.EnableKeyword("_EMISSION");
            //     m1.SetColor("_EmissionColor", DirectionalDeviceColor);
            //     if (TrackerTexture != null)
            //     {
            //         m1.SetTexture("_EmissionMap", TrackerTexture);
            //         m1.SetTexture("_MainTex", TrackerTexture);
            //     }
            //     else
            //         m1.SetTexture("_EmissionMap", m1.GetTexture("_MainTex"));
            //     
            //     var mats = new Material[1];
            //     mats[0] = m1;
            //     mr.materials = mats;
            // }
            // else
            //     setdirectionalcolorlater = true;
        }        


        // Save the initial movement curve, in case it's switched off
        inspectorCurve = FootSwingControllerToMovementCurve;

        // Seed the initial previousLocalPositions
        if(leftlegDeviceGameObject != null)
            leftlegDevicePreviousLocalPosition = leftlegDeviceGameObject.transform.localPosition;
        if (rightlegDeviceGameObject != null)
            rightlegDevicePreviousLocalPosition = rightlegDeviceGameObject.transform.localPosition;

        // Verify and fix settings
        verifySettings();
        initialized = true;
    }

    /***** FIXED UPDATE *****/
   
    void FixedUpdate()
    {
        if (!initialized)
            Initialize();
        
        
        if(setleftcolorlater && leftlegDeviceGameObject != null)
            {
            setleftcolorlater = false;
            var controller = leftlegDeviceGameObject.transform.Find("Model");

            if (_footSwingLegTrackingDevice == LegTrackingDevice.Controller)
            {
                    if (_footSwingDirectionMode == FootSwingDirectionMode.LegDirectionsSideControllers)
                    {
                        if (controller.childCount == 0)
                            setleftcolorlater = true;
                        else
                        {
                            for (int i = 0; i < controller.childCount; i++)
                            {
                                Transform t = controller.GetChild(i);
                                var mr = t.GetComponent<MeshRenderer>();
                                if (mr != null)
                                {
                                    var m = mr.materials[0];
                                    Material m1 = new Material(m);
                                    m1.EnableKeyword("_EMISSION");
                                    m1.SetColor("_EmissionColor", LeftLegDeviceColor);
                                    m1.SetTexture("_EmissionMap", m1.GetTexture("_MainTex"));
                                    var mats = new Material[1];
                                    mats[0] = m1;
                                    mr.materials = mats;
                                }
                            }
                        }
                    }
            }
            else
            {
                var mr = controller.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    var m = mr.materials[0];
                    Material m1 = new Material(m);
                        if (_footSwingDirectionMode == FootSwingDirectionMode.LegDirectionsSideControllers)
                        {
                            m1.EnableKeyword("_EMISSION");
                            m1.SetColor("_EmissionColor", LeftLegDeviceColor);
                            if (TrackerTexture != null)
                            {
                                m1.SetTexture("_EmissionMap", TrackerTexture);
                                m1.SetTexture("_MainTex", TrackerTexture);
                            }
                            else
                                m1.SetTexture("_EmissionMap", m1.GetTexture("_MainTex"));
                        }
                    else
                        {
                            if (TrackerTexture != null)
                                m1.SetTexture("_MainTex", TrackerTexture);
                        }
                    var mats = new Material[1];
                    mats[0] = m1;
                    mr.materials = mats;
                }
                else
                    setleftcolorlater = true;
            }
        }


        if (setrightcolorlater && rightlegDeviceGameObject != null)
            {
                setrightcolorlater = false;
                var controller = rightlegDeviceGameObject.transform.Find("Model");

                if (_footSwingLegTrackingDevice == LegTrackingDevice.Controller)
                {
                    if (controller.childCount == 0)
                        setleftcolorlater = true;
                    else
                    {
                        for (int i = 0; i < controller.childCount; i++)
                        {
                            Transform t = controller.GetChild(i);
                            var mr = t.GetComponent<MeshRenderer>();
                            if (mr != null)
                            {
                                var m = mr.materials[0];
                                Material m1 = new Material(m);
                                m1.EnableKeyword("_EMISSION");
                                m1.SetColor("_EmissionColor", RightLegDeviceColor);
                                m1.SetTexture("_EmissionMap", m1.GetTexture("_MainTex"));
                                var mats = new Material[1];
                                mats[0] = m1;
                                mr.materials = mats;
                            }
                        }
                    }
                }
                else
                {
                    var mr = controller.GetComponent<MeshRenderer>();
                    if (mr != null)
                    {
                        var m = mr.materials[0];
                        Material m1 = new Material(m);
                        m1.EnableKeyword("_EMISSION");
                        m1.SetColor("_EmissionColor", RightLegDeviceColor);
                        if (TrackerTexture != null)
                        {
                            m1.SetTexture("_EmissionMap", TrackerTexture);
                            m1.SetTexture("_MainTex", TrackerTexture);
                        }
                        else
                            m1.SetTexture("_EmissionMap", m1.GetTexture("_MainTex"));
                        var mats = new Material[1];
                        mats[0] = m1;
                        mr.materials = mats;
                    }
                    else
                        setrightcolorlater = true;
                }
            }

            if (setdirectionalcolorlater)
            {
                setdirectionalcolorlater = false;
                var mr = DirectionalTracker.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    var m = mr.materials[0];
                    Material m1 = new Material(m);
                    m1.EnableKeyword("_EMISSION");
                    m1.SetColor("_EmissionColor", DirectionalDeviceColor);
                    if (TrackerTexture != null)
                    {
                        m1.SetTexture("_EmissionMap", TrackerTexture);
                        m1.SetTexture("_MainTex", TrackerTexture);
                    }
                    else
                        m1.SetTexture("_EmissionMap", m1.GetTexture("_MainTex"));

                    var mats = new Material[1];
                    mats[0] = m1;
                    mr.materials = mats;
                }
                else
                    setdirectionalcolorlater = true;
            }

            // Set scale as necessary (defaults to 1.0)
            // Doing this in Update() allows the Camera Rig to be scaled during runtime but keep the same FootSwinger feel
            if (generalScaleWorldUnitsToCameraRigScale)
            {
                cameraRigScaleModifier = this.transform.localScale.x;
            }

            // Save the current controller positions for our use
            if (leftlegDeviceGameObject != null)
                leftlegDeviceLocalPosition = leftlegDeviceGameObject.transform.localPosition;
            if (rightlegDeviceGameObject != null)
                rightlegDeviceLocalPosition = rightlegDeviceGameObject.transform.localPosition;

            // Variable motion based on controller movement
            if (!footSwingingPaused)
            {
                if (this.movementSpeedMultiplier != 0f)
                    {
                    Vector3 moveDirection = new Vector3();
                    if (Target.isGrounded)
                    {
                        moveDirection = variableFootSwingMotion();
                        //moveDirection *= movementSpeedMultiplier;
                    }
                    moveDirection.y -= 9.81f * Time.deltaTime;
                    Target.Move(moveDirection);
                    }
                else
                    Target.Move(Vector3.zero);
            }

            // Save the current controller positions for next frame
            if(leftlegDeviceGameObject != null)
                leftlegDevicePreviousLocalPosition = leftlegDeviceGameObject.transform.localPosition;
            if (rightlegDeviceGameObject != null)
                rightlegDevicePreviousLocalPosition = rightlegDeviceGameObject.transform.localPosition;                  

        // Save this Time.deltaTime for next frame (inertia simulation)
        previousTimeDeltaTime = Time.deltaTime;
    }

    /***** VERIFY SETTINGS *****/
    void verifySettings()
    {

        // Camera Rig checking
        if (!this.GetComponent<UnityEngine.XR.Interaction.Toolkit.Inputs.InputActionManager>())
        {
            Debug.LogError("FootSwinger.verifySettings():: FootSwinger is applied on a GameObject that is not an XR Origin, or is an XR Origin without an InputActionManager.  Please review the FootSwinger instructions.  FootSwinger will fail.");
        }

        // Check fixed time setting
        if (Time.fixedDeltaTime > 1f / 90f)
        {
            if (generalAutoAdjustFixedTimestep)
            {
                Debug.LogWarning("FootSwinger.verifySettings():: Fixed Timestep is set to " + Time.fixedDeltaTime + ".  Since you have generalAutoAdjustFixedTimestep set to true, FootSwinger will auto adjust this value to " + 1f / 90f + " (90 steps per second) for you.");
                Time.fixedDeltaTime = 1f / 90f;
            }
            else
            {
                Debug.LogError("FootSwinger.verifySettings():: Fixed Timestep is set to " + Time.fixedDeltaTime + ".  This will cause stuttering movement when Foot swinging.  Consider changing your Fixed Timestep to " + 1f / 90f + " (90 steps per second) by going to Edit -> Project Settings -> Time -> Fixed Timestep.");
            }
        }
    }

    /***** CORE FUNCTIONS *****/
    // Variable Foot Swing locomotion
    Vector3 variableFootSwingMotion()
    {

        // Initialize movement variables
        float movementAmount = 0f;
        Quaternion movementRotation = Quaternion.identity;
        bool movedThisFrame = swing(ref movementAmount, ref movementRotation);              

        if (movedThisFrame)
        {

            FootSwinging = true;
            
            latestArtificialMovement = movementAmount;
            latestArtificialRotation = movementRotation;

            // Move forward in the X and Z axis only (no flying!)
            Vector3 movement = new Vector3();

            movement = getForwardXZ(movementAmount * movementSpeedMultiplier, movementRotation);
            //Debug.Log(movement);
            return movement;

        }
        else
        {
            FootSwinging = false;

            return Vector3.zero;
        }
    }

    // Foot Swing when footSwingMode is NoButtons
    bool swing(ref float movement, ref Quaternion rotation)
    {
        if (DirectionalTracker != null)
        {
            //If the directional tracker is present, use its rotation
            rotation = DirectionalTracker.transform.rotation * Quaternion.Euler(0, 180, 0);
        }
        else
        {
            // Otherwise the rotation is the average of the two controllers
            rotation = determineAverageLegControllerRotation();
        }

        // Find the change in controller position since last Update()
        float leftControllerChange = 0;
        float rightControllerChange = 0;

        float leftMovement = 0;
        float rightMovement = 0;
      
        if (FootSwingOnlyVerticalMovement)
            leftControllerChange = Mathf.Abs(leftlegDevicePreviousLocalPosition.y - leftlegDeviceLocalPosition.y);
        else
            leftControllerChange = Vector3.Distance(leftlegDevicePreviousLocalPosition, leftlegDeviceLocalPosition);
        if (FootSwingOnlyVerticalMovement)
            rightControllerChange = Mathf.Abs(rightlegDevicePreviousLocalPosition.y - rightlegDeviceLocalPosition.y);
        else
            rightControllerChange = Vector3.Distance(rightlegDevicePreviousLocalPosition, rightlegDeviceLocalPosition);

        // Calculate what camera rig movement the change should be converted to
        leftMovement = calculateMovement(FootSwingControllerToMovementCurve, leftControllerChange, FootSwingControllerSpeedForMaxSpeed, footSwingMaxSpeed);
        rightMovement = calculateMovement(FootSwingControllerToMovementCurve, rightControllerChange, FootSwingControllerSpeedForMaxSpeed, footSwingMaxSpeed);

        // Both controllers are in use, so controller movement is the average of the two controllers' change times the both controller coefficient
        float controllerMovement = (leftMovement + rightMovement) / 2 * FootSwingBothControllersCoefficient;

        if (movingInertia)
        {
            controllerMovement = movingInertiaOrControllerMovement(controllerMovement);
        }

        if (controllerMovement == 0)
        {
            if (stoppingInertia && latestArtificialMovement != 0)
            {

                // The rotation is the cached one
                rotation = latestArtificialRotation;
                // The stopping movement is calculated using a curve, the latest movement, last frame's deltaTime, max speed, and the stop time for max speed
                movement = inertiaMovementChange(stoppingInertiaCurve, latestArtificialMovement, previousTimeDeltaTime, footSwingMaxSpeed, stoppingInertiaTimeToStopAtMaxSpeed);

                return true;
            }
            else
                return false;
        }
        else
            movement = controllerMovement;

        return true;
    }

    float movingInertiaOrControllerMovement(float movement)
    {

        if (controllerSmoothing)
        {
            // Save the movement amount for moving inertia calculations
            saveFloatToLinkedList(controllerMovementResultHistory, movement, controllerSmoothingCacheSize);

            movement = smoothedControllerMovement(controllerMovementResultHistory);

        }

        float inertiaMovement = inertiaMovementChange(movingInertiaCurve, latestArtificialMovement, previousTimeDeltaTime, footSwingMaxSpeed, movingInertiaTimeToStopAtMaxSpeed);

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

    /***** HELPER FUNCTIONS *****/

    // Returns the average of two Quaternions
    Quaternion averageRotation(Quaternion rot1, Quaternion rot2)
    {
        return Quaternion.Slerp(rot1, rot2, 0.5f);
    }

    // Returns a Vector3 with only the X and Z components (Y is 0'd)
    static Vector3 vector3XZOnly(Vector3 vec)
    {
        return new Vector3(vec.x, 0f, vec.z);
    }

    // Returns a forward vector given the distance and direction
    static Vector3 getForwardXZ(float forwardDistance, Quaternion direction)
    {
        Vector3 forwardMovement = direction * Vector3.forward * forwardDistance;
        return vector3XZOnly(forwardMovement);
    }

    Quaternion determineAverageLegControllerRotationPublic(out Quaternion left, out Quaternion right)
    {
        left = leftlegDeviceGameObject.transform.rotation;
        right = rightlegDeviceGameObject.transform.rotation;
        return determineAverageLegControllerRotation();
    }


    // Returns the average rotation of the two controllers
    Quaternion determineAverageArmControllerRotation()
    {
        // Build the average rotation of the controller(s)
        Quaternion newRotation;

        // Both controllers are present
        if (leftController != null && rightController != null)
        {
            newRotation = averageRotation(leftControllerGameObject.transform.rotation, rightControllerGameObject.transform.rotation);
        }
        // Left controller only
        else if (leftController != null && rightController == null)
        {
            newRotation = leftControllerGameObject.transform.rotation;
        }
        // Right controller only
        else if (rightController != null && leftController == null)
        {
            newRotation = rightControllerGameObject.transform.rotation;
        }
        // No controllers!
        else
        {
            newRotation = Quaternion.identity;
        }

        return newRotation;
    }


    Quaternion determineAverageArmControllerRotationPublic(out Quaternion left, out Quaternion right)
    {
        left = leftControllerGameObject.transform.rotation;
        right = rightControllerGameObject.transform.rotation;
        return determineAverageArmControllerRotation();
    }

    Quaternion determineAverageLegControllerRotation()
    {
        // Build the average rotation of the controller(s)
        Quaternion newRotation = Quaternion.identity;

        var ArmRoot = determineAverageArmControllerRotation();
        
            switch (footSwingDirectionMode)
            {
                case FootSwingDirectionMode.ArmDirection:
                    return ArmRoot;

                case FootSwingDirectionMode.HeadDirection:
                    return headsetGameObject.transform.rotation;

                case FootSwingDirectionMode.LegDirectionRearControllers:

                    // Both devices are present
                    if (leftlegDeviceTrackedObj != null && rightlegDeviceTrackedObj != null)
                    {
                        var leftrot = leftlegDeviceGameObject.transform.rotation;

                        var rightrot = rightlegDeviceGameObject.transform.rotation;

                        if (footSwingLegTrackingDevice == LegTrackingDevice.Controller)
                            newRotation = averageRotation(leftrot * Quaternion.Euler(90, 180, 0), rightrot * Quaternion.Euler(90, 180, 0));
                        else
                            newRotation = averageRotation(leftrot * Quaternion.Euler(0, 180, 0), rightrot * Quaternion.Euler(0, 180, 0));
                    }

                    // Left device only
                    else if (leftlegDeviceTrackedObj != null && rightlegDeviceTrackedObj == null)
                    {
                        var leftrot = leftlegDeviceGameObject.transform.rotation;
                        if (footSwingLegTrackingDevice == LegTrackingDevice.Controller)
                            newRotation = leftrot * Quaternion.Euler(90, 180, 0);
                        else
                            newRotation = leftrot * Quaternion.Euler(0, 180, 0);
                    }

                    // Right device only
                    else if (rightlegDeviceTrackedObj != null && leftlegDeviceTrackedObj == null)
                    {
                        var rightrot = rightlegDeviceGameObject.transform.rotation;
                        if (footSwingLegTrackingDevice == LegTrackingDevice.Controller)
                            newRotation = rightrot * Quaternion.Euler(90, 180, 0);
                        else
                            newRotation = rightrot * Quaternion.Euler(0, 180, 0);
                    }

                    // No devices!
                    else
                    {
                        newRotation = Quaternion.identity;
                    }

                    return newRotation;

                case FootSwingDirectionMode.LegDirectionsSideControllers:

                    // Both devices are present
                    if (leftlegDeviceTrackedObj != null && rightlegDeviceTrackedObj != null)
                    {
                        var leftrot = leftlegDeviceGameObject.transform.rotation;
                        var rightrot = rightlegDeviceGameObject.transform.rotation;
                        if(footSwingLegTrackingDevice == LegTrackingDevice.Controller)
                            newRotation = averageRotation(leftrot * Quaternion.Euler(90, 90, 0), rightrot * Quaternion.Euler(90, -90, 0));
                        else
                            newRotation = averageRotation(leftrot * Quaternion.Euler(0, 90, 0), rightrot * Quaternion.Euler(0, -90, 0));
                    }

                    // Left device only
                    else if (leftlegDeviceTrackedObj != null && rightlegDeviceTrackedObj == null)
                    {
                        var leftrot = leftlegDeviceGameObject.transform.rotation;


                        if (footSwingLegTrackingDevice == LegTrackingDevice.Controller)
                        {
                            leftrot *= Quaternion.Euler(90, 90, 0);
                            newRotation = leftrot;
                        }
                        else
                        {
                            leftrot *= Quaternion.Euler(0, 90, 0);
                            newRotation = leftrot;                            
                        }
                    }
                    // Right device only
                    else if (rightlegDeviceTrackedObj != null && leftlegDeviceTrackedObj == null)
                    {
                        var rightrot = rightlegDeviceGameObject.transform.rotation;


                        if (footSwingLegTrackingDevice == LegTrackingDevice.Controller)
                        {
                            rightrot *= Quaternion.Euler(90, -90, 0);
                            newRotation = rightrot;
                        }
                        else
                        {
                            rightrot *= Quaternion.Euler(0, -90, 0);
                            newRotation = rightrot;
                        }
                    }

                    // No devices!
                    else
                    {
                        newRotation = Quaternion.identity;
                    }

                    return newRotation;
            }

        return Quaternion.identity;
    }    

    float smoothedControllerMovement(LinkedList<float> controllerMovementHistory)
    {

        // Chose the lowest value in the cache
        if (controllerSmoothingMode == ControllerSmoothingMode.Lowest)
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
        else if (controllerSmoothingMode == ControllerSmoothingMode.Average)
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
        else if (controllerSmoothingMode == ControllerSmoothingMode.AverageMinusHighest)
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
            Debug.LogError("FootSwinger.smoothedControllerMovement():: Invalid value for controllerSmoothingMode!");
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
    public float FootSwingBothControllersCoefficient
    {
        get
        {
            return _FootSwingBothControllersCoefficient;
        }

        set
        {
            float min = 0f;
            float max = 10f;

            if (value >= min && value <= max)
            {
                _FootSwingBothControllersCoefficient = value;
            }
            else
            {
                Debug.LogWarning("FootSwinger:FootSwingBothControllersCoefficient:: Requested new value " + value + " is out of range (" + min + ".." + max + ")");
            }
        }

    }
    
    public bool useNonLineFootovementCurve
    {
        get
        {
            return _useNonLinearMovementCurve;
        }
        set
        {
            if (value)
            {
                FootSwingControllerToMovementCurve = inspectorCurve;
            }
            else
            {
                FootSwingControllerToMovementCurve = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));
            }

            _useNonLinearMovementCurve = value;
        }
    }

    public bool footSwingingPaused
    {
        get
        {
            return _FootSwingingPaused;
        }
        set
        {
            _FootSwingingPaused = value;
        }
    }

    public float FootSwingControllerSpeedForMaxSpeed
    {
        get
        {
            return _FootSwingControllerSpeedForMaxSpeed;
        }
        set
        {
            _FootSwingControllerSpeedForMaxSpeed = value;
        }
    }

    public float footSwingMaxSpeed
    {
        get
        {
            return _FootSwingMaxSpeed * cameraRigScaleModifier;
        }
        set
        {
            _FootSwingMaxSpeed = value;
        }
    }

    public FootSwingDirectionMode footSwingDirectionMode
    {
        get
        {
            return _footSwingDirectionMode;
        }
        set
        {
            _footSwingDirectionMode = value;
        }
    }


    public ControllerButton footSwingButton
    {
        get
        {
            return _footSwingButton;
        }
        set
        {
           // steamVRFootSwingButton = convertControllerButtonToSteamVRButton(value);
            _footSwingButton = value;
        }
    }
    
    public LegTrackingDevice footSwingLegTrackingDevice
    {
        get
        {
            return _footSwingLegTrackingDevice;
        }
        set
        {
            _footSwingLegTrackingDevice = value;
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
            latestArtificialMovement = 0f;
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
            latestArtificialMovement = 0f;
            _stoppingInertia = value;
        }
    }
}
