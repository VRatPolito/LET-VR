using UnityEngine;
using System.Collections;
using KATVR;
public class KATDevice : MonoBehaviour {

    #region Common Variable
    public enum DeviceTypeList { KAT_WALK, ComingSoon };
    public DeviceTypeList device;
    public enum State { Standing, Forward, Backward };
    public Transform targetMoveObject, targetRotateObject, vrCameraRig, vrHandset;
    #endregion
    [HideInInspector]
    public KATDevice_Walk KWalk;

    public float multiply, multiplyBack;
    public enum MovementStyleList { Translate, Velocity, CharacterController, Auto }
    public MovementStyleList MovementStyle;
    [HideInInspector]
    public Rigidbody target_Rig;
    [HideInInspector]
    public CharacterController target_Controller;
    public KeyCode ResetCameraKey;

    [Header("Real-Time Values")]
    public State CurrentState;
    [Range(0, 1)]
    public float CurrentSpeed = 0.0f;
    [Range(0, 360)]
    public float CurrentRotation = 0.0f;

    void Awake()
    {
        SetupDevice(device);
    }

    void Start()
    {
        ActiveDevice(device);
    }

    void Update () {
        DeviceUpdate(device);
    }

    #region Common Function

    public void SetCameraController(Transform CameraRig)
    {

    }
    
    #endregion

    #region Start function

    public void SetupDevice(DeviceTypeList Type)
    {
        switch (Type)
        {
            case DeviceTypeList.KAT_WALK:
                KWalk = this.gameObject.AddComponent<KATDevice_Walk>();
                KATVR_Global.KDevice_Walk = KWalk;
                break;
            case DeviceTypeList.ComingSoon:
                break;
            default:
                break;
        }
    }

    public void ActiveDevice(DeviceTypeList Type)
    {
        switch (Type)
        {
            case DeviceTypeList.KAT_WALK:
                KWalk.Initialize(1);
                var lauched = KWalk.LaunchDevice();
                if (MovementStyle == MovementStyleList.Auto)
                {
                    if (target_Rig == null)
                        if (targetMoveObject.GetComponent<Rigidbody>())
                            target_Rig = targetMoveObject.GetComponent<Rigidbody>();
                        else
                        {
                            if (target_Controller == null)
                                if (targetMoveObject.GetComponent<CharacterController>())
                                    target_Controller = targetMoveObject.GetComponent<CharacterController>();
                                else
                                    MovementStyle = MovementStyleList.Translate;
                        }
                }
                else if (MovementStyle == MovementStyleList.CharacterController)
                {
                    if (target_Controller == null)
                        if (targetMoveObject.GetComponent<CharacterController>())
                            target_Controller = targetMoveObject.GetComponent<CharacterController>();
                        else
                        {
                            MovementStyle = MovementStyleList.Translate;
                            Debug.LogWarning("Can not find CharacterController component in Movement Object, the Movement Style will be changed to Translate.");
                        }
                }
                else if (MovementStyle == MovementStyleList.Velocity)
                {
                    if (target_Rig == null)
                        if (targetMoveObject.GetComponent<Rigidbody>())
                            target_Rig = targetMoveObject.GetComponent<Rigidbody>();
                        else
                            {
                                MovementStyle = MovementStyleList.Translate;
                                Debug.LogWarning("Can not find Rigidbody component in Movement Object, the Movement Style will be changed to Translate.");
                            }
                }
                else
                    MovementStyle = MovementStyleList.Translate;
                break;
            case DeviceTypeList.ComingSoon:
                break;
            default:
                break;
        }
    }
    public void DeviceUpdate(DeviceTypeList Type)
    {
        switch (Type)
        {
            case DeviceTypeList.KAT_WALK:
                KWalk.UpdateData();
                TargetTransform(MovementStyle);
                if (Input.GetKeyDown(ResetCameraKey))
                    KWalk.ResetCamera(vrHandset);
                break;
            case DeviceTypeList.ComingSoon:
                break;
            default:
                break;
        }

        if (KATVR_Global.KDevice_Walk != null)
        {
            CurrentSpeed = KATVR_Global.KDevice_Walk.Data.displayedSpeed;
            CurrentRotation = KATVR_Global.KDevice_Walk.Data.bodyYaw;
            if (KATVR_Global.KDevice_Walk.Data.isMoving == 1)
            {
                if (KATVR_Global.KDevice_Walk.Data.moveDirection > 0)
                    CurrentState = State.Forward;
                else if (KATVR_Global.KDevice_Walk.Data.moveDirection < 0)
                    CurrentState = State.Backward;
            }
            else
            {
                CurrentState = State.Standing;
            }
        }
    }
    #endregion

    #region Function For KAT WALK

    void TargetTransform(MovementStyleList Type)
    {
        //vrCameraRig.position = targetRotateObject.position;       //a che serve?
        if (KWalk.Data.moveDirection > 0) KWalk.Data.moveSpeed *= multiply;
        else if (KWalk.Data.moveDirection < 0) KWalk.Data.moveSpeed *= multiplyBack;
        switch (Type)
        {
            #region Translate
            case MovementStyleList.Translate:
                targetMoveObject.Translate(targetRotateObject.forward / 100 * KWalk.Data.moveSpeed * KWalk.Data.moveDirection);
                targetRotateObject.localEulerAngles = new Vector3(targetRotateObject.localEulerAngles.x, KWalk.Data.bodyYaw, targetRotateObject.localEulerAngles.z);
                break;
            #endregion
            #region Velocity
            case MovementStyleList.Velocity:
                target_Rig.velocity = targetRotateObject.forward * KWalk.Data.moveSpeed * KWalk.Data.moveDirection;
                targetRotateObject.localEulerAngles = new Vector3(targetRotateObject.localEulerAngles.x, KWalk.Data.bodyYaw, targetRotateObject.localEulerAngles.z);
                break;
            #endregion
            #region CharacterController
            case MovementStyleList.CharacterController:
                /*Vector3 moveDirection = Vector3.zero;
                if (target_Controller.isGrounded)
                    moveDirection = targetRotateObject.forward / 100 * KWalk.Data.moveSpeed * KWalk.Data.moveDirection;
                moveDirection.y -= 9.81f * Time.deltaTime;
                target_Controller.Move(moveDirection);*/            //DA TESTARE
                if (KWalk.Data.moveSpeed != 0f)
                    target_Controller.SimpleMove(targetRotateObject.forward / 100 * KWalk.Data.moveSpeed * KWalk.Data.moveDirection);
                else
                    target_Controller.SimpleMove(Vector3.zero);
                targetRotateObject.localEulerAngles = new Vector3(targetRotateObject.localEulerAngles.x, KWalk.Data.bodyYaw, targetRotateObject.localEulerAngles.z);
                break;
            #endregion
            default:
                break;
        }
    }

    #endregion
}
