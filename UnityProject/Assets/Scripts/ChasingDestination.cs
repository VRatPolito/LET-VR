using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.PlayerLoop;
using PrattiToolkit;
using UnityEngine.Assertions;

public class ChasingDestination : Destination {
    
    bool moving = false;
    [SerializeField]
    float traveltime = 10;
    [SerializeField]
    Transform targetpos;
    private ColliderEventsListener _celTargetPos;
    private bool _playerAtTargetPos = false;


    public bool PlayerInside
    {
        get { return _inside; }
    }
    private bool _inside = false;
    
    public AnimationCurve Curve;

    public override void Start()
    {
        base.Start();

        _celTargetPos = targetpos.GetOrAddComponent<ColliderEventsListener>();
        Assert.IsNotNull(_celTargetPos);
        _celTargetPos.OnTriggerEnterAction += c =>
        {
            if (c.tag == "Player")
                _playerAtTargetPos = true;
        };
    }

    public override void Update()
    {
        if (moving &&  _playerAtTargetPos && transform.position == targetpos.position)
            AutoDisable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
        if (!moving)
            {
            Scenario1Manager.Instance.RobotStopPointing();
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
            Scenario1Manager.Instance.StatisticsLogger.StartLogChasing();
        }
    }

    private void AutoDisable()
    {
        Scenario1Manager.Instance.StatisticsLogger.StopLogChasing();
        Scenario1Manager.Instance.CloseRobotDoor();
        gameObject.SetActive(false);
    }
    
}
