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

    public enum Position { Standing, Crouched, Proned };
    Position Posizione = Position.Standing;
    // Use this for initialization
    void Start()
    {
        if (height == 0.0f)
            height = LocomotionManager.Instance.CalibrationData.HeadHeight;

        GetComponent<CharacterController>().height = height;
        GetComponent<NavMeshObstacle>().height = height;

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

            GetComponent<CharacterController>().height = height;
            GetComponent<CharacterController>().center = new Vector3(offset.x, height / 2, offset.z);
            GetComponent<NavMeshObstacle>().height = height;
            GetComponent<NavMeshObstacle>().center = new Vector3(offset.x, height / 2, offset.z);

            if (Posizione != Position.Standing)
                Posizione = Position.Standing;
        }
        else if (Head.localPosition.y >= 1 && Head.localPosition.y < height)
        {

            GetComponent<CharacterController>().height = Head.localPosition.y;
            GetComponent<CharacterController>().center = new Vector3(offset.x, Head.localPosition.y / 2, offset.z);
            GetComponent<NavMeshObstacle>().height = Head.localPosition.y;
            GetComponent<NavMeshObstacle>().center = new Vector3(offset.x, Head.localPosition.y / 2, offset.z);

            if (Posizione != Position.Standing)
                Posizione = Position.Standing;
        }
        else if (Head.localPosition.y < 1)
        {

            GetComponent<CharacterController>().height = 1;
            GetComponent<CharacterController>().center = new Vector3(offset.x, 0.5f, offset.z);
            GetComponent<NavMeshObstacle>().height = 1;
            GetComponent<NavMeshObstacle>().center = new Vector3(offset.x, 0.5f, offset.z);

            if (Posizione != Position.Crouched)
                Posizione = Position.Crouched;
        }
    }
}
