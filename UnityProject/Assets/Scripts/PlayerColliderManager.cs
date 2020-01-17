using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
public class PlayerColliderManager : MonoBehaviour
{
    public Transform Head;
    public float height = 0.0f;
    Vector3 prevpos = Vector3.zero;
    [SerializeField]
    private CapsuleCollider _capsule;
    private CharacterController _charc;
    private NavMeshObstacle _navObst;

    public enum Position { Standing, Crouched, Proned };
    Position Posizione = Position.Standing;
    // Use this for initialization
    private void Awake()
    {
        _charc = GetComponent<CharacterController>();
        _navObst = GetComponent<NavMeshObstacle>();
    }

    public void EnableCollider()
    {
        if (_charc != null)
            _charc.enabled = true;
        if (_navObst != null)
            _navObst.enabled = true;
        if (_capsule != null)
            _capsule.enabled = true;
    }

    public void DisableCollider()
    {

        if (_charc != null)
            _charc.enabled = false;
        if (_navObst != null)
            _navObst.enabled = false;
        if (_capsule != null)
            _capsule.enabled = false;
    }

    void Start()
    {
        if (height == 0.0f)
            height = LocomotionManager.Instance.CalibrationData.HeadHeight;

        if(_charc != null)
            _charc.height = height;
        if(_navObst != null)
            _navObst.height = height;
        if (_capsule != null)
            _capsule.height = height;

        var headpos = Head.position;
        ManageCollider(Vector3.zero);
        prevpos = headpos;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var headpos = Head.position;
        if (Head.position != prevpos)
        {
            var offset = Head.position - transform.position;
            ManageCollider(offset);
            prevpos = headpos;
        }
    }

    private void ManageCollider(Vector3 offset)
    {
        if (Head.localPosition.y >= height)
        {
            if (_charc != null)
            {
                _charc.height = height;
                _charc.center = new Vector3(offset.x, height / 2, offset.z);
            }
            if (_navObst != null)
            {
                _navObst.height = height;
                _navObst.center = new Vector3(offset.x, height / 2, offset.z);
            }
            if (_capsule != null)
            {
                _capsule.height = height;
                _capsule.center = new Vector3(offset.x, height / 2, offset.z);
            }

            if (Posizione != Position.Standing)
                Posizione = Position.Standing;
        }
        else if (Head.localPosition.y >= 1 && Head.localPosition.y < height)
        {
            if (_charc != null)
            {
                _charc.height = Head.localPosition.y; ;
                _charc.center = new Vector3(offset.x, Head.localPosition.y / 2, offset.z);
            }
            if (_navObst != null)
            {
                _navObst.height = Head.localPosition.y; ;
                _navObst.center = new Vector3(offset.x, Head.localPosition.y / 2, offset.z);
            }
            if (_capsule != null)
            {
                _capsule.height = Head.localPosition.y; ;
                _capsule.center = new Vector3(offset.x, Head.localPosition.y / 2, offset.z);
            }

            if (Posizione != Position.Standing)
                Posizione = Position.Standing;
        }
        else if (Head.localPosition.y < 1)
        {
            if (_charc != null)
            {
                _charc.height = 1;
                _charc.center = new Vector3(offset.x, 0.5f, offset.z);
            }
            if (_navObst != null)
            {
                _navObst.height = 1;
                _navObst.center = new Vector3(offset.x, 0.5f, offset.z);
            }
            if (_capsule != null)
            {
                _capsule.height = 1;
                _capsule.center = new Vector3(offset.x, 0.5f, offset.z);
            }

            if (Posizione != Position.Crouched)
                Posizione = Position.Crouched;
        }
    }
}
