
/*
 * Custom template by Gabriele P.
 */
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using PrattiToolkit;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

public enum LocomotionTechniqueType { ArmSwing, WalkInPlace, CVirtualizer, KatWalk, RealWalk, Joystick };


public class LocomotionManager : UnitySingleton<LocomotionManager>
{
    protected LocomotionManager() { }

    #region Events

    #endregion

    #region Editor Visible

    [SerializeField] private bool _getLocomotionFromConfigFile = true;
    [SerializeField] private LocomotionTechniqueType _locomotion;
    [SerializeField] private bool _getAutoFreezableFromConfigFile = true;
    [SerializeField] private bool _autoFreezable = true;
    [SerializeField] private List<Transform> _playerControllers;

    [SerializeField] private KeyCode _freezePalyerKeyCode = KeyCode.F;
    //[Expandable]
    [SerializeField] private LocomotionCalibrationData _calibrationData;

    #endregion

    #region Private Members and Constants

    private Vector3 _lastPlayerPosition;
    private float _startingKATmultiply, _startingKATmultiplyback;
    private bool _isPlayerFreezed;
    private float _initialMult, _initialBackMultKat;

    #endregion

    #region Properties
    public bool IsAutoFreezable { get { return _autoFreezable; } private set { _autoFreezable = value; } }
    public float CurrentPlayerSpeed { get; private set; }
    public Transform CurrentPlayerController { get; private set; }
    public Transform LeftController { get; private set; }
    public Transform RightController { get; private set; }
    public Transform LeftTracker { get; private set; }
    public Transform RightTracker { get; private set; }
    public Transform DirectionalTracker { get; private set; }
    public Transform CameraEye { get; private set; }
    public I_UI_HUDController CurrentUIController { get; set; }

    public LocomotionTechniqueType Locomotion
    {
        get
        {
            return _locomotion;
        }

        private set
        {
            _locomotion = value;
        }
    }

    public bool IsPlayerFreezed
    {
        get { return _isPlayerFreezed; }
        set
        {
            _isPlayerFreezed = value;
            if (_isPlayerFreezed)
            {
                StopLocomotion();
                if (CurrentUIController != null) CurrentUIController.ShowFreezeIcon();
            }
            else
            {
                StartLocomotion();
                if (CurrentUIController != null) CurrentUIController.HideFreezeIcon();
            }
        }
    }

    public LocomotionCalibrationData CalibrationData
    {
        get { return _calibrationData; }
        protected set { _calibrationData = value; }
    }



    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        if (_getLocomotionFromConfigFile)
            Locomotion = ConfigurationLookUp.Instance.GetEnum("LocomotionTechnique", Locomotion);
        if (_getAutoFreezableFromConfigFile)
            _autoFreezable = ConfigurationLookUp.Instance.GetBool("AutoFreeze", _autoFreezable);

        _calibrationData = GetOrCreateCalibrationData();
        Assert.IsNotNull(_calibrationData);

        _startingKATmultiply = _playerControllers[(int)LocomotionTechniqueType.KatWalk].GetComponentInChildren<KATDevice>().multiply;
        _startingKATmultiplyback = _playerControllers[(int)LocomotionTechniqueType.KatWalk].GetComponentInChildren<KATDevice>().multiplyBack;
        IsPlayerFreezed = false;
        CurrentPlayerController = _playerControllers[(int)Locomotion];
    }

    private void Start()
    {
        InitializeTechnique();

        var c = FindObjectOfType<UI_HUDController>();
        Assert.IsNotNull(c);
        var cc = c.GetComponent<Canvas>();
        Assert.IsNotNull(cc);
        cc.renderMode = RenderMode.ScreenSpaceCamera;
        cc.worldCamera = CameraEye.transform.GetChildRecursive("UI").GetComponent<Camera>();
        CurrentUIController = c;
        _lastPlayerPosition = CurrentPlayerController.position;

        AutoFreeze();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(_freezePalyerKeyCode) || (Locomotion == LocomotionTechniqueType.RealWalk && CurrentPlayerController != null && (CurrentPlayerController.GetComponent<InputManagement>().IsLeftGripped && CurrentPlayerController.GetComponent<InputManagement>().IsRightGripped)))
            IsPlayerFreezed = !IsPlayerFreezed;
        
        CurrentPlayerSpeed = Vector3.Distance(_lastPlayerPosition, CurrentPlayerController.position) / Time.deltaTime;
        _lastPlayerPosition = CurrentPlayerController.position;
    }

    protected override void OnApplicationQuit()
    {
        CalibrationData.SavePersistent();
        base.OnApplicationQuit();
    }
    
    #endregion

    #region Public Methods

    public void AutoFreeze()
    {
        if(_autoFreezable && !Application.isEditor)
            IsPlayerFreezed = true;
    }

    public void StartLocomotionPublic()
    {
        if (!IsPlayerFreezed)
            StartLocomotion();
    }

    public void StopLocomotionPublic()
    {
        StopLocomotion();
    }
    
    #endregion

    #region Helper Methods

    private void InitializeTechnique()
    {
        CurrentPlayerController.gameObject.SetActive((true));
        var ic = CurrentPlayerController.GetComponent<VRItemController>();
        LeftController = ic.LeftController;
        RightController = ic.RightController;
        switch (Locomotion)
        {
            case LocomotionTechniqueType.ArmSwing:
                var cr = CurrentPlayerController.Find("[CameraRig]");
                DirectionalTracker = cr.Find("Tracker (hip)");
                var l = CurrentPlayerController.GetComponent<CircularLimitTracking>();
                CameraEye = l.CameraEye;
                break;
            case LocomotionTechniqueType.WalkInPlace:
                l = CurrentPlayerController.GetComponent<CircularLimitTracking>();
                CameraEye = l.CameraEye;
                cr = CurrentPlayerController.Find("[CameraRig]");
                DirectionalTracker = cr.Find("Tracker (hip)");
                LeftTracker = cr.Find("Tracker (left)");
                RightTracker = cr.Find("Tracker (right)");
                break;
            case LocomotionTechniqueType.CVirtualizer:
                var pcm = CurrentPlayerController.GetComponent<PlayerColliderManager>();
                CameraEye = pcm.Head;
                _initialMult = CurrentPlayerController.GetComponent<CVirtPlayerController>().movementSpeedMultiplier;
                break;
            case LocomotionTechniqueType.KatWalk:
                pcm = CurrentPlayerController.GetComponent<PlayerColliderManager>();
                CameraEye = pcm.Head;
                _initialMult = CurrentPlayerController.GetComponentInChildren<KATDevice>().multiply;
                _initialBackMultKat = CurrentPlayerController.GetComponentInChildren<KATDevice>().multiplyBack;
                break;
            case LocomotionTechniqueType.RealWalk:
                var cvr = CurrentPlayerController.GetComponent<CharacterControllerVR>();
                CameraEye = cvr.CameraEye;
                break;
            case LocomotionTechniqueType.Joystick:
                l = CurrentPlayerController.GetComponent<CircularLimitTracking>();
                CameraEye = l.CameraEye;
                break;
        }
    }

    private void StartLocomotion()
    {
        if (CurrentPlayerController == null) return;
        switch (Locomotion)
        {
            case LocomotionTechniqueType.ArmSwing:
                CurrentPlayerController.GetComponentInChildren<ArmSwinger>().armSwingingPaused = false;
                break;
            case LocomotionTechniqueType.WalkInPlace:
                CurrentPlayerController.GetComponentInChildren<FootSwinger>().footSwingingPaused = false;
                break;
            case LocomotionTechniqueType.CVirtualizer:
                CurrentPlayerController.GetComponent<CVirtPlayerController>().movementSpeedMultiplier = _initialMult;
                break;
            case LocomotionTechniqueType.KatWalk:
                CurrentPlayerController.GetComponentInChildren<KATDevice>().multiply = _initialMult;
                CurrentPlayerController.GetComponentInChildren<KATDevice>().multiplyBack = _initialBackMultKat;
                break;
            case LocomotionTechniqueType.RealWalk:
                CurrentPlayerController.GetComponent<CharacterControllerVR>().Blocked = false;
                break;
            case LocomotionTechniqueType.Joystick:
                CurrentPlayerController.GetComponent<JoystickMovement>().Blocked = false;
                break;
        }
    }

    private void StopLocomotion()
    {
        if (CurrentPlayerController == null) return;
        switch (Locomotion)
        {
            case LocomotionTechniqueType.ArmSwing:
                CurrentPlayerController.GetComponentInChildren<ArmSwinger>().armSwingingPaused = true;
                break;
            case LocomotionTechniqueType.WalkInPlace:
                CurrentPlayerController.GetComponentInChildren<FootSwinger>().footSwingingPaused = true;
                break;
            case LocomotionTechniqueType.CVirtualizer:
                CurrentPlayerController.GetComponent<CVirtPlayerController>().movementSpeedMultiplier = 0.0f;
                break;
            case LocomotionTechniqueType.KatWalk:
                CurrentPlayerController.GetComponentInChildren<KATDevice>().multiply = 0;
                CurrentPlayerController.GetComponentInChildren<KATDevice>().multiplyBack = 0;
                break;
            case LocomotionTechniqueType.RealWalk:
                CurrentPlayerController.GetComponent<CharacterControllerVR>().Blocked = true;
                break;
            case LocomotionTechniqueType.Joystick:
                CurrentPlayerController.GetComponent<JoystickMovement>().Blocked = true;
                break;
        }
    }

    private LocomotionCalibrationData GetOrCreateCalibrationData()
    {
        var dataPath = PersistentSaveLoad.GetDefaultDataPath("LET_VR", "calibrationData.pgd");
        var calib = PersistentSaveLoad.Load<LocomotionCalibrationData>(dataPath,
            PersistentSaveLoad.SerializationType.Json);

        if (calib == null)
            calib = new LocomotionCalibrationData();
        return calib;
    }

    #endregion

    #region Events Callbacks

    #endregion

    #region Coroutines

    #endregion

}
