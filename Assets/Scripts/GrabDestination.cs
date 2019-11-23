using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabDestination : Destination
{
    [SerializeField] private GenericItem _item;
    [SerializeField] private Transform _referenceItem;
    [SerializeField] private Transform _itemLength;
    [SerializeField] private Transform _colliderLength;
    [SerializeField] private bool _autoDisable = true;
    [SerializeField] private bool _makeItemUngrabbable = true;
    [SerializeField] private bool _makeKinematic = false;
    [SerializeField] private float _timeStillForStop = 2;
    private bool _itemInRange;
    private float _timeStop = 0;
    private Vector3 _lastPos;
    private Quaternion _lastRot;

    private void OnTriggerEnter(Collider other)
    {
		var item = other.GetComponent<GenericItem>();
        if (item == _item)
        {
            _itemInRange = true;
            _timeStop = Time.time + _timeStillForStop;
            _lastPos = _item.transform.position;
            _lastRot = _item.transform.rotation;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var item = other.GetComponent<GenericItem>();
        if (item == _item)
        {
            _itemInRange = false;
            _timeStop = 0;
        }
    }

    public float GetPosDiff()
    {
        var dist = Vector3.Distance(_item.transform.position, _referenceItem.position);
        var maxdist = Vector3.Distance(transform.position, _colliderLength.position) + Vector3.Distance(_referenceItem.position, _itemLength.position);
        return dist / maxdist * 100;
    }

    public float GetRotDiff()
    {
        var angle = Quaternion.Angle(_item.transform.rotation, _referenceItem.transform.rotation);
        //todo
        return angle;
    }
    private void OnTriggerStay(Collider other)
    {
        if(_item.transform.position != _lastPos || _item.transform.rotation != _lastRot)
            _timeStop = Time.time + _timeStillForStop;
        else if (Time.time == _timeStop)
        {
            var p = _item.Player;
            if (_makeKinematic)
                _item.IsKinematic = true;
            if (p != null)
                p.DropItem(_item.transform, true);
            if (_makeItemUngrabbable)
                _item.CanInteract(false, LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>());
            if (_autoDisable)
                gameObject.SetActive(false);
        }
    }
}
