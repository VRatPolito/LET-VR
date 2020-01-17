using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour {

    public Transform From, To;
    public enum Direction {Front, Back};
    Direction dir = Direction.Front;
    Animator animator;
    public bool PowerOn = true;
    bool Idle = false;
    public float TimeBeforePowerOn = 3;

    private void Awake()
    {
    if(animator == null)
        animator = GetComponent<Animator>();
    }
    

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(PowerOn && Idle)
        {
            if(dir == Direction.Front)
            {
                if (transform.localPosition.z <= To.localPosition.z * 0.85f)
                    dir = Direction.Back;
                else
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, Mathf.Lerp(transform.localPosition.z, To.localPosition.z, Time.deltaTime));
            }
            else
            {
                if (transform.localPosition.z >= From.localPosition.z * 0.85f)
                    dir = Direction.Front;
                else
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, Mathf.Lerp(transform.localPosition.z, From.localPosition.z, Time.deltaTime));

            }
        }
    }
    public void TurnOff()
    {
        if(PowerOn)
        {
            animator.SetBool("PowerOn", false);
            PowerOn = false;
            Idle = false;
        }
    }
    public void TurnOn()
    {
        if (!PowerOn)
        {
            animator.SetBool("PowerOn", true);
            PowerOn = true;
            Idle = false;
        }
    }

    private void OnEnable()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        var a = animator.GetBehaviours<AdvancedStateMachineBehaviour>();
        a[0].StateEnter = IdleOn;
    }

    public void IdleOn(AdvancedStateMachineBehaviour a)
    {
        Idle = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            var o = other.GetComponent<VRItemController>();
            if (o != null)
            {
                TurnOff();
                //GameMaster.Instance.TakeCollision();
                Invoke("TurnOn", TimeBeforePowerOn);
            }
        }
    }
}
