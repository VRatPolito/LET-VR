﻿using System;
using System.Collections;
using System.Collections.Generic;
using PrattiToolkit;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(ColliderEventsListener))]
public class CollisionDetect : MonoBehaviour
{
    public enum HitType : byte
    {
        Player,
        NotPlayer
    }

    public event Action<CollisionDetect, HitType> OnHit;

    [SerializeField] private string[] TagToHit = new[]{"Player"};
    [SerializeField] private bool _isBullet = false;

    private ColliderEventsListener _colliderEventsListener;

    private void Start()
    {
        _colliderEventsListener = gameObject.GetOrAddComponent<ColliderEventsListener>();
        Assert.IsNotNull(_colliderEventsListener);
        _colliderEventsListener.OnTriggerEnterAction += c =>
        {
            foreach (var tag in TagToHit)
            {
                if (c.CompareTag(tag))
                {
                    OnHit.RaiseEvent(this, HitType.Player);
                    FindObjectOfType<StatisticsLoggerBase>().LogCollisions();
                    break; //TODO just onces
                }
                else
                {
                    OnHit.RaiseEvent(this, HitType.NotPlayer);
                }
            }
        };
        if (_isBullet)
            _colliderEventsListener.OnColliderEnterAction += c => { GetComponent<Rigidbody>().useGravity = true; };
    }

    public void ResetHitEventListener()
    {
        OnHit = null;
    }
}