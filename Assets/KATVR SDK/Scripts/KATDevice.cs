using KATVR;
using UnityEngine;

public class KATDevice : MonoBehaviour
{

    #region Enums 
    public enum DeviceTypeList { KAT_WALK, ComingSoon };
    public enum State { Standing, Forward, Backward };
    public enum MovementStyleList { Translate, Velocity, CharacterController, Auto }
    #endregion

    #region Shared Variables

    public DeviceTypeList device;
    public Transform targetMoveObject, targetRotateObject, vrCameraRig, vrHandset;

    #endregion

    public float multiply, multiplyBack;
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

    private void Awake()
    {
        SetupDevice(device);
    }

    private void Start()
    {
        ActiveDevice(device);
    }

    private void FixedUpdate()
    { //FixedUpdate for v1.4
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
                if (KATDevice_Walk.Instance)
                {
                    ; //workaround for their bad singleton implementation
                }

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
                KATDevice_Walk.Instance.Initialize(1);
                KATDevice_Walk.Instance.LaunchDevice();
                if (MovementStyle == MovementStyleList.Auto)
                {
                    if (target_Rig == null)
                    {
                        if (targetMoveObject.GetComponent<Rigidbody>())
                        {
                            target_Rig = targetMoveObject.GetComponent<Rigidbody>();
                        }
                        else
                        {
                            if (target_Controller == null)
                            {
                                if (targetMoveObject.GetComponent<CharacterController>())
                                {
                                    target_Controller = targetMoveObject.GetComponent<CharacterController>();
                                }
                                else
                                {
                                    MovementStyle = MovementStyleList.Translate;
                                }
                            }
                        }
                    }
                }
                else if (MovementStyle == MovementStyleList.CharacterController)
                {
                    if (target_Controller == null)
                    {
                        if (targetMoveObject.GetComponent<CharacterController>())
                        {
                            target_Controller = targetMoveObject.GetComponent<CharacterController>();
                        }
                        else
                        {
                            MovementStyle = MovementStyleList.Translate;
                            Debug.LogWarning("Can not find CharacterController component in Movement Object, the Movement Style will be changed to Translate.");
                        }
                    }
                }
                else if (MovementStyle == MovementStyleList.Velocity)
                {
                    if (target_Rig == null)
                    {
                        if (targetMoveObject.GetComponent<Rigidbody>())
                        {
                            target_Rig = targetMoveObject.GetComponent<Rigidbody>();
                        }
                        else
                        {
                            MovementStyle = MovementStyleList.Translate;
                            Debug.LogWarning("Can not find Rigidbody component in Movement Object, the Movement Style will be changed to Translate.");
                        }
                    }
                }
                else
                {
                    MovementStyle = MovementStyleList.Translate;
                }

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
                KATDevice_Walk.Instance.UpdateData();
                TargetTransform(MovementStyle);
                if (Input.GetKeyDown(ResetCameraKey))
                {
                    KATDevice_Walk.Instance.ResetCamera(vrHandset);
                }

                break;
            case DeviceTypeList.ComingSoon:
                break;
            default:
                break;
        }

        #region Update Status commodity //the following part in not included in the new v1.4

        CurrentSpeed = KATDevice_Walk.Instance.Data.displayedSpeed;
        CurrentRotation = KATDevice_Walk.Instance.Data.bodyYaw;
        if (KATDevice_Walk.Instance.Data.isMoving == 1)
        {
            if (KATDevice_Walk.Instance.Data.moveDirection > 0)
            {
                CurrentState = State.Forward;
            }
            else if (KATDevice_Walk.Instance.Data.moveDirection < 0)
            {
                CurrentState = State.Backward;
            }
        }
        else
        {
            CurrentState = State.Standing;
        }

        #endregion 

    }
    #endregion

    #region Function For KAT WALK

    private void TargetTransform(MovementStyleList Type)
    {
        //vrCameraRig.position = targetRotateObject.position; ///Commented in v1.4
        if (KATDevice_Walk.Instance.Data.moveDirection > 0)
        {
            KATDevice_Walk.Instance.Data.moveSpeed *= multiply;
        }
        else if (KATDevice_Walk.Instance.Data.moveDirection < 0)
        {
            KATDevice_Walk.Instance.Data.moveSpeed *= multiplyBack;
        }

        switch (Type)
        {
            #region Translate
            case MovementStyleList.Translate:
                //targetMoveObject.Translate(targetRotateObject.forward / 100 * KATDevice_Walk.Instance.Data.moveSpeed * KATDevice_Walk.Instance.Data.moveDirection); //replaced by the next in v1.4 
                targetMoveObject.position += (targetRotateObject.forward / 100 * KATDevice_Walk.Instance.Data.moveSpeed * KATDevice_Walk.Instance.Data.moveDirection);
                targetRotateObject.localEulerAngles = new Vector3(targetRotateObject.localEulerAngles.x, KATDevice_Walk.Instance.Data.bodyYaw, targetRotateObject.localEulerAngles.z);
                break;
            #endregion
            #region Velocity
            case MovementStyleList.Velocity:
                target_Rig.velocity = targetRotateObject.forward * KATDevice_Walk.Instance.Data.moveSpeed * KATDevice_Walk.Instance.Data.moveDirection;
                targetRotateObject.localEulerAngles = new Vector3(targetRotateObject.localEulerAngles.x, KATDevice_Walk.Instance.Data.bodyYaw, targetRotateObject.localEulerAngles.z);
                break;
            #endregion
            #region CharacterController
            case MovementStyleList.CharacterController:
                /*Vector3 moveDirection = Vector3.zero;
                if (target_Controller.isGrounded)
                    moveDirection = targetRotateObject.forward / 100 * KWalk.Data.moveSpeed * KWalk.Data.moveDirection;
                moveDirection.y -= 9.81f * Time.deltaTime;
                target_Controller.Move(moveDirection);*/            //DA TESTARE
                if (KATDevice_Walk.Instance.Data.moveSpeed != 0f)
                {
                    target_Controller.SimpleMove(targetRotateObject.forward / 100 * KATDevice_Walk.Instance.Data.moveSpeed * KATDevice_Walk.Instance.Data.moveDirection);
                }
                else
                {
                    target_Controller.SimpleMove(Vector3.zero);
                }

                targetRotateObject.localEulerAngles = new Vector3(targetRotateObject.localEulerAngles.x, KATDevice_Walk.Instance.Data.bodyYaw, targetRotateObject.localEulerAngles.z);
                break;
            #endregion
            default:
                break;
        }
    }

    #endregion
}
