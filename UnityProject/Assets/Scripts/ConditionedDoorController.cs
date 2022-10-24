using System;
using System.Collections;
using System.Collections.Generic;
using PrattiToolkit;
using UnityEngine;
using UnityEngine.Assertions;

public class ConditionedDoorController : DoorController
{
    public bool NeedBattery
    {
        get { return _needBattery; }
        set { _needBattery = value; }
    }

    protected override bool PlayerInRange
    {
        get { return _playerInRange; }
        set
        {
            if (_playerInRange == value) return;
            _playerInRange = value;
            if (!NeedBattery || !_playerInRange)
                OpenGate(_playerInRange);
        }
    }


    public event Action BatteryInserted;

    [SerializeField] protected MeshRenderer _batteryHolder;
    [SerializeField] protected GameObject _batteryTarget;
    [SerializeField] protected GameObject _battery;
    [SerializeField] protected ColliderEventsListener _batteryTrigger;
    [SerializeField] protected bool _needBattery = false;

    protected override void Awake()
    {
        Assert.IsNotNull(_batteryTarget);
        Assert.IsNotNull(_batteryTrigger);
        base.Awake();
        _batteryHolder.GetComponent<QuickOutline>().enabled = false;
        if (NeedBattery)
            StartPulse();
        _batteryTrigger.OnTriggerEnterAction += c =>
        {
            if (!NeedBattery || !PlayerInRange || c.tag != "Item") return;

            if (c.gameObject == _battery)
            {
                var g = c.GetComponent<GenericItem>();

                if (g._hand == ControllerHand.Invalid)
                    return;
                if (g._hand == ControllerHand.LeftHand)
                    g.Player.LeftController.GetComponent<VibrationController>().ShortVibration();
                else if (g._hand == ControllerHand.RightHand)
                    g.Player.RightController.GetComponent<VibrationController>().ShortVibration();

                ((VRItemController)g.Player).OnDrop += ItemDropped;


                _batteryHolder.GetComponent<QuickOutline>().enabled = true;
                _batteryHolder.GetComponent<QuickOutline>().Blink = false;
            }
        };

        _batteryTrigger.OnTriggerExitAction += c =>
        {
            if (!NeedBattery || !PlayerInRange || c.tag != "Item") return;
            (LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>()).OnDrop -= ItemDropped;
            StartPulse();
        };
    }

    private void ItemDropped(GenericItem gi)
    {
        if (NeedBattery)
        {
            OpenGate(PlayerInRange);
            var g = _battery.GetComponent<GenericItem>();
            g.GetComponent<Rigidbody>().isKinematic = true;
            g.CanInteract(false, LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>());
            (LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>()).OnDrop -= ItemDropped;
            _battery.transform.parent = _batteryTarget.transform;
            _battery.transform.position = _batteryTarget.transform.position;
            _battery.transform.rotation = _batteryTarget.transform.rotation;
            StopPulse();
            NeedBattery = false;
            BatteryInserted.RaiseEvent();
        }
    }

    
    
    public void StartPulse()
    {
        _batteryHolder.GetComponent<QuickOutline>().enabled = true;
        _batteryHolder.GetComponent<QuickOutline>().Blink = true;
    }

    public void StopPulse()
    {
        _batteryHolder.GetComponent<QuickOutline>().enabled = false;
        _batteryHolder.GetComponent<QuickOutline>().Blink = false;
    }
}