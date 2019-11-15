using System;
using System.Collections;
using System.Collections.Generic;
using PrattiToolkit;
using UnityEngine;
using UnityEngine.Assertions;

public enum HitType : byte
{
    Player,
    Item,
    Other
}

[RequireComponent(typeof(ColliderEventsListener))]
public class CollisionDetect : MonoBehaviour
{
    public event Action<CollisionDetect, HitType> OnHit;

    [SerializeField] private string[] _playerTag = new[] {"Player"};
    [SerializeField] private string _itemTag = "Item";
    [SerializeField] private bool _isBullet = false;

    private ColliderEventsListener _colliderEventsListener;

    private void Start()
    {
        _colliderEventsListener = gameObject.GetOrAddComponent<ColliderEventsListener>();
        Assert.IsNotNull(_colliderEventsListener);
        _colliderEventsListener.OnTriggerEnterAction += c =>
        {
            HitType type = HitAnyThing(c.tag);
            if (type != HitType.Other)
                FindObjectOfType<StatisticsLoggerBase>().LogCollisions(type);
            OnHit.RaiseEvent(this, type);
        };
        if (_isBullet)
            _colliderEventsListener.OnColliderEnterAction += c => { GetComponent<Rigidbody>().useGravity = true; };
    }

    private HitType HitAnyThing(string tag)
    {
        foreach (var ptag in _playerTag)
            if (tag == ptag)
                return HitType.Player;


        return tag == _itemTag ? HitType.Item : HitType.Other;
    }

    public void ResetHitEventListener()
    {
        OnHit = null;
    }
}