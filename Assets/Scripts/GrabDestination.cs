using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabDestination : Destination
{
    [SerializeField] private GenericItem _item;
    [SerializeField] private Transform _referenceItem;
    private Transform _itemLength;
    [SerializeField] private bool _autoDisable = true;
    [SerializeField] private bool _makeItemUngrabbable = true;
    [SerializeField] private bool _makeKinematic = false;
    [SerializeField] private float _timeStillForStop = 2;
    private bool _itemInRange;
    private float _timeStop = 0;
    private Vector3 _lastPos;
    private Quaternion _lastRot;
    [SerializeField]
    private bool _debugDifference = false;

    private void Awake()
    {
        _itemLength = _referenceItem.GetChild(0);
    }

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
        var maxdist = Vector3.Distance(_referenceItem.position, _itemLength.position);
        if (dist >= maxdist)
            return 0;
        else
            return (1 - (dist / maxdist)) * 100;
    }

    public float GetRotDiff()
    {
        /*var dir1 = _item.transform.up;
        var dir2 = _referenceItem.up;
        dir1 = new Vector3(dir1.x, 0, dir1.z).normalized;
        dir2 = new Vector3(dir2.x, 0, dir2.z).normalized;
        var angle = Vector3.Angle(dir1, dir2);*/

        var angle = Quaternion.Angle(_item.transform.rotation, _referenceItem.transform.rotation);
        angle = angle % 360;
        if (angle > 180)
            angle -= 360;
        if (angle < -180)
            angle += 360;

        angle = Mathf.Abs((Mathf.Abs(angle) - 90));

        return (1 - (Mathf.Abs(angle) / 90)) * 100;
    }
    private void OnTriggerStay(Collider other)
    {
        if(_item.transform.position != _lastPos || _item.transform.rotation != _lastRot)
        {
            _lastPos = _item.transform.position;
            _lastRot = _item.transform.rotation;
            _timeStop = Time.time + _timeStillForStop;
        }
        else if (Time.time >= _timeStop)
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

    public override void Update()
    {
        if(_debugDifference && _itemInRange)
        {
            /*var dir1 = _item.transform.up;
            var dir2 = _referenceItem.up;
            dir1 = new Vector3(dir1.x, 0, dir1.z).normalized;
            dir2 = new Vector3(dir2.x, 0, dir2.z).normalized;
            var angle = Vector3.Angle(dir1, dir2);*/
            
            var angle = Quaternion.Angle(_item.transform.rotation, _referenceItem.transform.rotation);
            var angle2 = angle % 360;
            if (angle2 > 180)
                angle2 -= 360;
            if (angle2 < -180)
                angle2 += 360;

            angle2 = Mathf.Abs((Mathf.Abs(angle2) - 90));
            var perc = (Mathf.Abs(angle2) / 90) * 100;

            Debug.Log("Angle before: " + angle + " Angle next: " + angle2 + " Precision: " + perc);
        }
    }
}
