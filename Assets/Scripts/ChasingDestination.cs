using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Experimental.PlayerLoop;

public class ChasingDestination : Destination {
    
    bool moving = false;
    [SerializeField]
    float traveltime = 10;
    [SerializeField]
    Transform targetpos;

    public bool PlayerInside
    {
        get { return _inside; }
    }
    private bool _inside = false;
    
    public AnimationCurve Curve;
    // Update is called once per frame
    public override void Update()
    {
        if (moving && transform.position == targetpos.position)
            AutoDisable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
        if (!moving)
            {
            Level1Manager.Instance.RobotStopPointing();
            Invoke("StartMoving", 1);
            }
        _inside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            _inside = false;
        }
    }

    private void StartMoving()
    {
        if (!moving)
        {
            transform.parent.DOMove(targetpos.position, traveltime, false).SetEase(Curve);
            moving = true;
            Level1Manager.Instance.StatisticsLogger.StartLogChasing();
        }
    }

    private void AutoDisable()
    {
        Level1Manager.Instance.StatisticsLogger.StopLogChasing();
        Level1Manager.Instance.CloseRobotDoor();
        gameObject.SetActive(false);
    }
    
}
