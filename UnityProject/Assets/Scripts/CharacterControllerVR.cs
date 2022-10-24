using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerVR : MonoBehaviour {
    
    bool _initialized = false;
    public bool Blocked = false;
    Vector3 _startCenterEyeLocalPos = Vector3.zero;
    Vector3 _prevCenterEyeLocalPos;
    [Tooltip("The Steam VR CameraRig object's Transform")]
    public Transform CameraRig;
    [Tooltip("The Steam VR CameraEye object's Transform")]
    public Transform CameraEye;
    [SerializeField]
    [Tooltip("The CharacterController component attached to the Player")]
    private CharacterController _playerCollider;
    public float _gravity = -9.81f;
    Queue<Vector3> _externalMotion = new Queue<Vector3>();

    private void OnEnable()
    {
        Initialize();
    }

    private void OnDisable()
    {
        _initialized = false;
    }

    public void Initialize()
    {
        if (!_initialized)
        {
            //Save the initial VRNode.CenterEye localPosition
            _startCenterEyeLocalPos = UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.CenterEye);
            //Move the CameraRig in order to keep the the player centered
            CameraRig.localPosition = new Vector3(-_startCenterEyeLocalPos.x, CameraRig.localPosition.y, -_startCenterEyeLocalPos.z);
            
            _initialized = true;
        }
    }

    void LateUpdate()
    {
        if (!_initialized)
            Initialize();

        if (!Blocked)
        {
            //Get the actual VRNode.CenterEye localPosition
            var NewCenterEyeLocalPos = UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.CenterEye);

            //If I already set the CameraEye localPosition once and it was at the same localPosition

            float x, y, z;

            CheckBoundaries(NewCenterEyeLocalPos, out x, out y, out z);

            CameraRig.localPosition = new Vector3(x, y, z);

            Vector3 moveDirection = new Vector3();
            if (_playerCollider.isGrounded)
            {
                moveDirection = transform.TransformDirection(NewCenterEyeLocalPos - _prevCenterEyeLocalPos);
                while (_externalMotion.Count > 0)
                    moveDirection += _externalMotion.Dequeue();
            }
            else
                _externalMotion.Clear();

            moveDirection.y += _gravity * Time.deltaTime;
            _playerCollider.Move(moveDirection);

            _prevCenterEyeLocalPos = NewCenterEyeLocalPos;
        }
    }
    void CheckBoundaries(Vector3 NewCenterEyeLocalPos,out float x, out float y, out float z)
    {
        x = y = z = 0;
        //Generate the Vector2(x, z) versions of StartCenterEyeLocalPos and NewCenterEyeLocalPos for the circular limit check
        Vector2 StartCenterEyeLocalPosXZ = new Vector2(_startCenterEyeLocalPos.x, _startCenterEyeLocalPos.z);
        var NewCenterEyeLocalPosXZ = new Vector2(NewCenterEyeLocalPos.x, NewCenterEyeLocalPos.z);


        //Calculate the offsets between the actual and the initial VRNode.CenterEye localPositions
        //Calculate the vector useful to generate the max radial position in case of outofbounds
        var newPosXZ = (NewCenterEyeLocalPosXZ - StartCenterEyeLocalPosXZ).normalized;
        var offsx = NewCenterEyeLocalPos.x - _startCenterEyeLocalPos.x;
        var offsz = NewCenterEyeLocalPos.z - _startCenterEyeLocalPos.z;
        var offsy = NewCenterEyeLocalPos.y - _startCenterEyeLocalPos.y;
        
        newPosXZ *= 0;
        
        x = -_startCenterEyeLocalPos.x - offsx + newPosXZ.x;
        y = CameraRig.localPosition.y;
        z = -_startCenterEyeLocalPos.z - offsz + newPosXZ.y;        
    }

    internal void Move(Vector3 moveDirection)
    {
        if(moveDirection != Vector3.zero && gameObject.activeSelf)
            _externalMotion.Enqueue(moveDirection);
    }
}
