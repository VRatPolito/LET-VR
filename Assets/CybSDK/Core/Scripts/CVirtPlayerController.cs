using UnityEngine;
using System.Collections;
using CybSDK;
using UnityEngine.VR;

[RequireComponent(typeof(CharacterController))]
public class CVirtPlayerController : MonoBehaviour
{
    private CVirtDeviceController deviceController;
    private CharacterController characterController;

    public Transform forwardDirection;

    public float movementSpeedMultiplier = 1.0f;

    public float maxSpeed = 7.0f;

    private Vector3 _input;
    private Vector3 _newForward;
    private Vector3 _f;
    private Vector3 _speed;

    // Use this for initialization
    void Start()
    {
        // Find the forward direction        
        if (this.forwardDirection == null)
        {
            this.forwardDirection = this.transform.Find("ForwardDirection");
        }

        //Check if this object has a CVirtDeviceController attached
        deviceController = GetComponent<CVirtDeviceController>();
        if (deviceController == null)
        {
            Debug.LogError("CVirtPlayerController gameobject does not have a CVirtDeviceController attached.");
        }

        this.characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        bool executeBaseUpdate = true;
        if (this.deviceController != null)
        {
            CVirtDevice device = this.deviceController.GetDevice();
            if (device != null)
            {
                if (device.IsOpen() == false)
                {
                    device.Open();
                }
                else
                {
#if !UNITY_EDITOR
                    executeBaseUpdate = false;
#endif

                    // MOVE
                    ///////////
                    _input = device.GetMovementDirection();
                    _speed = _input * maxSpeed; //Prepare the clamped speed ad maxSpeed
                    _input *= device.GetMovementSpeed();
                    // ROTATION
                    ///////////
                    _newForward = device.GetPlayerOrientation();
                    if (this.forwardDirection != null)
                        this.forwardDirection.transform.localRotation =
                            Quaternion.LookRotation(_newForward, Vector3.up);

                    // Get the forward direction
                    _f = forwardDirection.forward;
                    _f.y = 0f;

                    // If not dead, move
                    if (this.movementSpeedMultiplier != 0f)
                    {
                        if ((_input * this.movementSpeedMultiplier).magnitude < maxSpeed)
                            _speed = _input * this.movementSpeedMultiplier;
                        characterController.SimpleMove(
                            Quaternion.LookRotation(_f) * _speed + 0.1f * Vector3.down);
                    }
                    else
                        characterController.SimpleMove(Vector3.zero);
                }
            }
        }

        // When a Virtualizer isn't plugged and we want 
        // to test with keyboard/mouse for example
        ///////////////////////////////////////////////
        if (executeBaseUpdate == true && this.movementSpeedMultiplier != 0f
        ) //aggiunto check sullo speed multiplier per evitare rotazioni quando non voglio
        {
            _input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

            // Find the forward direction
            _f = forwardDirection.forward;
            _f.y = 0f;

            characterController.SimpleMove(Quaternion.LookRotation(_f) * Vector3.ClampMagnitude(_input, 1f) *
                                           this.movementSpeedMultiplier *
                                           (Input.GetKey(KeyCode.LeftShift) ? 3.0F : 1.0F)); //@Cyberith: Run

            if (Input.GetKeyDown(KeyCode.E)) transform.rotation = Quaternion.Euler(0f, 45f, 0f) * transform.rotation;
            else if (Input.GetKeyDown(KeyCode.Q))
                transform.rotation = Quaternion.Euler(0f, -45f, 0f) * transform.rotation;

            transform.rotation = Quaternion.Euler(0f, Input.GetAxis("Mouse X") * 2f, 0f) * transform.rotation;
        }
    }

    public void OnDestroy()
    {
        if (this.deviceController != null)
        {
            CVirtDevice device = this.deviceController.GetDevice();
            if (device != null)
            {
                if (device.IsOpen() == true)
                {
                    device.Close();
                }
            }
        }
    }
}