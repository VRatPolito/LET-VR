using System;
using System.Collections;
using PrattiToolkit;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.XR.OpenXR.NativeTypes;
using CommonUsages = UnityEngine.XR.CommonUsages;

public class LimitTracking : MonoBehaviour
{
    public enum FadeState
    {
        FadeIn,
        FadeOut,
        Clear,
        Black
    };

    public enum Position
    {
        Standing,
        Crouched,
        Proned
    };

    /***** CLASS VARIABLES *****/

    /** Inspector Variables **/
    [SerializeField] private InputActionReference _resetCentering;
    
    // General Settings
    [Header("General Settings")]
    [Tooltip("Enables the fade to black when the user's head is blocked by the script:\n\n (Default: false)")]
    public bool FadeAtBlock = false;

    [Tooltip(
        "If enabled, the vertical position of the user's head is not considered by the limit checking logic.\n If disabled, the area available to head's motion is centered in Vector3.zero (local).\n\n (Default: false)")]
    public bool IgnoreY;

    [Tooltip(
        "If enabled, the available space on XZ is displayed through a circular indicator at foot level.\n\n (Default: true)")]
    public bool ShowLimit = true;

    [Tooltip(
        "Enables a vibration warning on the user's hand controllers when the head approaching to the limit:\n\n (Default: false)")]
    public bool WarningVibrationEnabled = false;

    [Tooltip(
        "Set at what percentage of available space the warning vibration shpuld be issued to the user:\n\n (Default: 80.0f)")]
    [Range(0.0f, 100.0f)]
    public float VibrationThreshold = 80.0f;

    [Tooltip(
        "If enabled, the script takes care of resizing the CharacterController's and NavMeshObstacle's Collider to match it with the head position.\n\n (Default: false)")]
    public bool ManageCharacterController = false;

    [Tooltip("If enabled, the Limit is kept at the global zero y coordinate.\n\n (Default: false)")]
    public bool KeepLimitAtGlobalZero = false;

    [SerializeField] private bool _debug = false;

    //External components
    [Header("External components")] [Tooltip("The SteamVR CameraRig object's Transform")]
    public Transform CameraRig;

    [Tooltip("The SteamVR CameraEye object's Transform")]
    public Transform CameraEye;

    [SerializeField] [Tooltip("The VibrationController script attached to the left SteamVR Controller")]
    VibrationController LeftController;

    [SerializeField] [Tooltip("The VibrationController script attached to the right SteamVR Controller")]
    VibrationController RightController;

    [SerializeField] [Tooltip("The limit object's Transform")]
    Transform Limit;

    [SerializeField]
    [Tooltip(
        "The material to be applied to the Limit's MeshRenderer when enabled during the initialization of the scrit (if ShowLimit is true).")]
    Material LimitMaterial;

    [SerializeField]
    [Tooltip("The CharacterController component attached to the Player, in order to manage the Collider height")]
    CharacterController PlayerCollider;

    [SerializeField]
    [Tooltip("The NavMeshObstacle component attached to the Player, in order to manage the Collider height")]
    NavMeshObstacle PlayerNavMeshObstacle;

    [SerializeField]
    [Tooltip("The Capsule Collider component attached to the Player, in order to manage the Collider height")]
    CapsuleCollider PlayerCapsule;

    //Initial position of the user's head at the script initialization. It will be applied as negative displacement to the CameraRig Transform to keep the user aligned with the CharacterController attached to the playerController in local Vector3.zero.
    [HideInInspector] public Vector3 StartCenterEyeLocalPos = Vector3.zero;

    //Previous CameraRig localPosition
    Vector3 PrevCameraRigLocalPos;

    //Previous VRNode.CenterEye localPosition
    Vector3 PrevCenterEyeLocalPos;

    //True if CameraEye localPosition has been already processed once
    bool PrevCameraEyeLocalPosSet = false;

    //Flag to signal when fade to black should be applied
    bool fading = false;

    //To store a running coroutine
    Coroutine routine;

    //Flag to signal when the vibration warning should be issued
    bool vibrationrequested = false;

    //Store the direction of the fade (to Black or to Clear)
    FadeState FadeDir = FadeState.Clear;

    //Previous head vertical position for the CharacterController and NavigationMesh size management
    float PrevHeadY = 0.0f;

    //Current user position
    Position Posizione = Position.Standing;

    //Flag to signal if the script is already initialized
    bool initialized = false;
    private float _playerheight;
    Vector3 LimitStartPos;

    private void Awake()
    {
        Assert.IsNotNull(_resetCentering);
        _resetCentering.action.performed += ctx => Reset();
        if (Limit != null)
            LimitStartPos = Limit.localPosition;
        if (_debug && !ExampleUtil.isPresent())
        {
            var CameraHead = CameraEye.parent;
            var c = CameraHead.childCount;
            for (int i = 0; i < c; i++)
                CameraHead.GetChild(0).parent = CameraRig;
            CameraHead.parent = CameraEye;
            CameraHead.gameObject.SetActive(false);
        }
    }

    public void Reset()
    {
        initialized = false;
        PrevCameraEyeLocalPosSet = false;
        Posizione = Position.Standing;
    }

    public void Initialize()
    {
        if (!initialized)
        {
            _playerheight = LocomotionManager.Instance.CalibrationData.HeadHeight;
            //Save the initial VRNode.CenterEye localPosition
            if (!ExampleUtil.isPresent())
                return; //StartCenterEyeLocalPos = CameraEye.localPosition;
            if (!InputDevices.GetDeviceAtXRNode(XRNode.CenterEye)
                    .TryGetFeatureValue(CommonUsages.devicePosition, out StartCenterEyeLocalPos) || StartCenterEyeLocalPos.magnitude < 0.1f)
                return;
            //Move the CameraRig in order to keep the the player centered
            CameraRig.localPosition = new Vector3(-StartCenterEyeLocalPos.x, CameraRig.localPosition.y,
                -StartCenterEyeLocalPos.z);

            if (ShowLimit && Limit != null)
            {
                if (LimitMaterial == null)
                {
                    var m = Limit.Find("teleport_marker_mesh").GetComponent<MeshRenderer>();
                    var mat = new Material[1];
                    mat[0] = LimitMaterial;
                    m.materials = mat;
                }

                var pos = transform.TransformPoint(0, 0, 0);
                pos.y = Limit.transform.position.y;
                Limit.transform.position = pos;
                Limit.gameObject.SetActive(true);
            }

            if (ManageCharacterController)
            {
                var heady = CameraEye.localPosition.y;

                if (heady >= _playerheight)
                {
                    PlayerCollider.height = _playerheight;
                    var c = PlayerCollider.center;
                    PlayerCollider.center = new Vector3(0, _playerheight / 2, 0);

                    if (PlayerNavMeshObstacle != null)
                    {
                        PlayerNavMeshObstacle.height = _playerheight;
                        PlayerNavMeshObstacle.center = new Vector3(0, _playerheight / 2, 0);
                    }

                    if (PlayerCapsule != null)
                    {
                        PlayerCapsule.height = _playerheight;
                        PlayerCapsule.center = new Vector3(0, _playerheight / 2, 0);
                    }

                    if (Posizione != Position.Standing)
                        Posizione = Position.Standing;
                }
                else if (heady >= _playerheight / 2 && heady < _playerheight)
                {
                    PlayerCollider.height = CameraEye.localPosition.y;
                    var c = PlayerCollider.center;
                    PlayerCollider.center = new Vector3(0, heady / 2, 0);

                    if (PlayerNavMeshObstacle != null)
                    {
                        PlayerNavMeshObstacle.height = CameraEye.localPosition.y;
                        PlayerNavMeshObstacle.center = new Vector3(0, heady / 2, 0);
                    }

                    if (PlayerCapsule != null)
                    {
                        PlayerCapsule.height = CameraEye.localPosition.y;
                        PlayerCapsule.center = new Vector3(0, heady / 2, 0);
                    }

                    if (Posizione != Position.Standing)
                        Posizione = Position.Standing;
                }
                else if (heady < _playerheight / 2)
                {
                    PlayerCollider.height = _playerheight / 2;
                    var c = PlayerCollider.center;
                    PlayerCollider.center = new Vector3(0, _playerheight / 4, 0);

                    if (PlayerNavMeshObstacle != null)
                    {
                        PlayerNavMeshObstacle.height = _playerheight / 2;
                        PlayerNavMeshObstacle.center = new Vector3(0, _playerheight / 4, 0);
                    }

                    if (PlayerCapsule != null)
                    {
                        PlayerCapsule.height = _playerheight / 2;
                        PlayerCapsule.center = new Vector3(0, _playerheight / 4, 0);
                    }

                    if (Posizione != Position.Crouched)
                        Posizione = Position.Crouched;
                }
            }

            initialized = true;
        }
    }

    private Vector3 GetMeanVector(Vector3 first, Vector3 second)
    {
        float x = 0f;
        float y = 0f;
        float z = 0f;

        x += first.x;
        y += first.y;
        z += first.z;

        x += second.x;
        y += second.y;
        z += second.z;

        return new Vector3(x / 2, y / 2, z / 2);
    }

    private void Update()
    {
        Initialize();
        if (!initialized) return;
        
    }

    void LateUpdate()
    {
        if (!initialized)
            Initialize();

        //Get the actual VRNode.CenterEye localPosition
        Vector3 NewCenterEyeLocalPos = Vector3.zero;

        if (!ExampleUtil.isPresent())
            return; //StartCenterEyeLocalPos = CameraEye.localPosition;
        if (!InputDevices.GetDeviceAtXRNode(XRNode.CenterEye)
                .TryGetFeatureValue(CommonUsages.devicePosition, out NewCenterEyeLocalPos))
            return;

        //If I already set the CameraEye localPosition once and it was at the same localPosition
        if (PrevCameraEyeLocalPosSet && PrevCenterEyeLocalPos == NewCenterEyeLocalPos)
        {
            //then use the previously calculated CameraRig localPosition
            CameraRig.localPosition = PrevCameraRigLocalPos;
        }
        else
        {
            //else check if the user is above or beyond the limits
            var outofbounds = false;
            var vibrate = false;
            float x, y, z;

            CheckBoundaries(NewCenterEyeLocalPos, out outofbounds, out vibrate, out x, out y, out z);

            /*if (DebugPosition)
            {
                if (outofbounds)
                    Debug.Log("Out of Bounds!");
                Debug.Log(new Vector3(x, y, z));
            }*/

            if (FadeAtBlock)
            {
                if (outofbounds && FadeDir != FadeState.FadeOut && FadeDir != FadeState.Black)
                    StartFadeOut();
                else if (!outofbounds && FadeDir != FadeState.FadeIn && FadeDir != FadeState.Clear)
                    StartFadeIn();
            }

            if (ExampleUtil.isPresent())
            {
                if (vibrate && !vibrationrequested && WarningVibrationEnabled)
                {
                    // LeftController.StartVibration(0.1f, 0.5f, 0.08f, this);
                    // RightController.StartVibration(0.1f, 0.5f, 0.08f, this);
                    vibrationrequested = true;
                }
                else if (!vibrate && vibrationrequested)
                {
                    // LeftController.StopVibration(0.1f, 0.5f, 0.08f, this);
                    // RightController.StopVibration(0.1f, 0.5f, 0.08f, this);
                    vibrationrequested = false;
                }
            }

            PrevCenterEyeLocalPos = NewCenterEyeLocalPos;

            CameraRig.localPosition = new Vector3(x, y, z);

            if (Limit != null && ShowLimit)
            {
                if (KeepLimitAtGlobalZero)
                    Limit.position = new Vector3(Limit.position.x, 0, Limit.position.z);
                else
                    Limit.localPosition = new Vector3(Limit.localPosition.x, LimitStartPos.y, Limit.localPosition.z);
            }

            PrevCameraRigLocalPos = CameraRig.localPosition;
            PrevCameraEyeLocalPosSet = true;
        }

        if (ManageCharacterController)
        {
            var HeadY = CameraEye.localPosition.y;

            if (HeadY != PrevHeadY)
            {
                if (HeadY >= _playerheight)
                {
                    PlayerCollider.height = _playerheight;
                    var c = PlayerCollider.center;
                    PlayerCollider.center = new Vector3(0, _playerheight / 2, 0);

                    if (PlayerNavMeshObstacle != null)
                    {
                        PlayerNavMeshObstacle.height = _playerheight;
                        PlayerNavMeshObstacle.center = new Vector3(0, _playerheight / 2, 0);
                    }

                    if (PlayerCapsule != null)
                    {
                        PlayerCapsule.height = _playerheight;
                        PlayerCapsule.center = new Vector3(0, _playerheight / 2, 0);
                    }

                    if (Posizione != Position.Standing)
                        Posizione = Position.Standing;
                }
                else if (HeadY >= _playerheight / 2 && HeadY < _playerheight)
                {
                    PlayerCollider.height = HeadY;
                    var c = PlayerCollider.center;
                    PlayerCollider.center = new Vector3(0, HeadY / 2, 0);

                    if (PlayerNavMeshObstacle != null)
                    {
                        PlayerNavMeshObstacle.height = HeadY;
                        PlayerNavMeshObstacle.center = new Vector3(0, HeadY / 2, 0);
                    }

                    if (PlayerCapsule != null)
                    {
                        PlayerCapsule.height = HeadY;
                        PlayerCapsule.center = new Vector3(0, HeadY / 2, 0);
                    }

                    if (Posizione != Position.Standing)
                        Posizione = Position.Standing;
                }
                else if (HeadY < _playerheight / 2)
                {
                    PlayerCollider.height = _playerheight / 2;
                    var c = PlayerCollider.center;
                    PlayerCollider.center = new Vector3(0, _playerheight / 4, 0);

                    if (PlayerNavMeshObstacle != null)
                    {
                        PlayerNavMeshObstacle.height = _playerheight / 2;
                        PlayerNavMeshObstacle.center = new Vector3(0, _playerheight / 4, 0);
                    }

                    if (PlayerCapsule != null)
                    {
                        PlayerCapsule.height = _playerheight / 2;
                        PlayerCapsule.center = new Vector3(0, _playerheight / 4, 0);
                    }

                    if (Posizione != Position.Crouched)
                        Posizione = Position.Crouched;
                }
            }

            PrevHeadY = HeadY;
        }
    }

    public virtual void CheckBoundaries(Vector3 NewCenterEyeLocalPos, out bool outofbounds, out bool vibrate,
        out float x, out float y, out float z)
    {
        throw new NotImplementedException();
    }

    private void StartFadeOut()
    {
        if (FadeDir != FadeState.FadeOut && FadeDir != FadeState.Black)
        {
            if (routine != null)
                routine = StartCoroutine(FadeRoutine(routine, false));
            else
                routine = StartCoroutine(FadeRoutine(null, false));
            FadeDir = FadeState.FadeOut;
        }
    }

    private void StartFadeIn()
    {
        if (FadeDir != FadeState.FadeIn && FadeDir != FadeState.Clear)
        {
            if (routine != null)
                routine = StartCoroutine(FadeRoutine(routine, true));
            else
                routine = StartCoroutine(FadeRoutine(null, true));
            FadeDir = FadeState.FadeIn;
        }
    }

    private IEnumerator FadeRoutine(Coroutine prevroutine, bool In)
    {
        if (prevroutine != null && fading)
            yield return new WaitUntil(() => !fading);
        fading = true;

        if (In)
            Debug.Log("Fade not updated yet");
        //SteamVRFade.fadeIn(.35f);
        else
            Debug.Log("Fade not updated yet");
        //SteamVRFade.fadeOut(.35f);

        yield return new WaitForSeconds(0.35f);
        if (In)
            FadeDir = FadeState.Clear;
        else
            FadeDir = FadeState.Black;
        fading = false;
    }

    private void OnDestroy()
    {
        if (vibrationrequested)
        {
            // LeftController.StopVibration(0.1f, 0.5f, 0.08f, this);
            // RightController.StopVibration(0.1f, 0.5f, 0.08f, this);
            vibrationrequested = false;
        }
    }

    private void OnDisable()
    {
        if (vibrationrequested)
        {
            // LeftController.StopVibration(0.1f, 0.5f, 0.08f, this);
            // RightController.StopVibration(0.1f, 0.5f, 0.08f, this);
            vibrationrequested = false;
        }
    }


    public void EnableCollider()
    {
        if (PlayerCollider != null)
            PlayerCollider.enabled = true;
        if (PlayerNavMeshObstacle != null)
            PlayerNavMeshObstacle.enabled = true;
        if (PlayerCapsule != null)
            PlayerCapsule.enabled = true;
    }

    public void DisableCollider()
    {
        if (PlayerCollider != null)
            PlayerCollider.enabled = false;
        if (PlayerNavMeshObstacle != null)
            PlayerNavMeshObstacle.enabled = false;
        if (PlayerCapsule != null)
            PlayerCapsule.enabled = false;
    }

    internal virtual void CopyConstraints(LimitTracking l)
    {
        throw new NotImplementedException();
    }
}