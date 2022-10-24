using System;
using System.Collections;
using System.Collections.Generic;
using PrattiToolkit;
using UnityEngine;
using UnityEngine.Assertions;

public class MultiConditionedDoorController : DoorController
{
    public bool NeedBatteries
    {
        get { return _needBatteries; }
        set { _needBatteries = value; }
    }

    protected override bool PlayerInRange
    {
        get { return _playerInRange; }
        set
        {
            if (_playerInRange == value) return;
            _playerInRange = value;
            if (!NeedBatteries || !_playerInRange)
                OpenGate(_playerInRange);
        }
    }


    public event Action<GameObject> BatteryInserted;

    [SerializeField] protected List<MeshRenderer> _batteryHolders;
    [SerializeField] protected List<GameObject> _batteryTargets;
    [SerializeField] protected List<GameObject> _batteries;
    [SerializeField] protected List<ColliderEventsListener> _batteryTriggers;
    [SerializeField] protected bool _needBatteries = false;
    private bool[] _batteryInserted;
    
    protected override void Awake()
    {
        Assert.IsNotNull(_batteryHolders);
        Assert.IsNotNull(_batteries);
        Assert.IsNotNull(_batteryTargets);
        Assert.IsNotNull(_batteryTriggers);
        _batteryInserted = new bool[_batteryTargets.Count];
        base.Awake();
        if (NeedBatteries)
            StartPulse();
        foreach (var b in _batteryTriggers)
        {
            b.OnTriggerEnterAction += c =>
            {
                if (!NeedBatteries || !PlayerInRange || c.tag != "Item") return;

                var i = GetBatteryIndex(b);
                if (c.gameObject == _batteries[i])
                {
                    var g = c.GetComponent<GenericItem>();

                    if (g._hand == ControllerHand.Invalid)
                        return;
                    if (g._hand == ControllerHand.LeftHand)
                        g.Player.LeftController.GetComponent<VibrationController>().ShortVibration();
                    else if (g._hand == ControllerHand.RightHand)
                        g.Player.RightController.GetComponent<VibrationController>().ShortVibration();

                    ((VRItemController)g.Player).OnDrop += ItemDropped;
                    
                    _batteryHolders[i].GetComponent<QuickOutline>().enabled = true;
                    _batteryHolders[i].GetComponent<QuickOutline>().Blink = false;
                }
            };
        }

        foreach (var b in _batteryTriggers)
        {
            b.OnTriggerExitAction += c =>
            {
                if (!NeedBatteries || !PlayerInRange || c.tag != "Item") return;

                var i = GetBatteryIndex(b);
                if (c.gameObject == _batteries[i])
                {
                    (LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>()).OnDrop -=
                        ItemDropped;
                    StartPulse(i);
                }
            };
        }
    }

    private void ItemDropped(GenericItem gi)
    {
        if (NeedBatteries)
        {
            var i = GetBatteryIndex(gi.gameObject);
            _batteryInserted[i] = true;
            if (AllBatteriesInserted())
            {
                OpenGate(PlayerInRange);
                NeedBatteries = false;
                StopPulse();
            }

            var g = _batteries[i].GetComponent<GenericItem>();
            g.GetComponent<Rigidbody>().isKinematic = true;
            g.CanInteract(false, LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>());
            (LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>()).OnDrop -= ItemDropped;
            _batteries[i].transform.parent = _batteryTargets[i].transform;
            _batteries[i].transform.position = _batteryTargets[i].transform.position;
            _batteries[i].transform.rotation = _batteryTargets[i].transform.rotation;
            BatteryInserted.RaiseEvent(gi.gameObject);
        }
    }

    private void StopPulse()
    {
        for (int i = 0; i < _batteries.Count; i++)
            StopPulse(i);
    }

    private void StartPulse()
    {
        for (int i = 0; i < _batteries.Count; i++)
            StartPulse(i);
    }

    private int GetBatteryIndex(ColliderEventsListener b)
    {
        for (int i = 0; i < _batteryTriggers.Count; i++)
        {
            if (_batteryTriggers[i] == b)
            {
                return i;
            }
        }

        return -1;
    }

    private int GetBatteryIndex(GameObject b)
    {
        for (int i = 0; i < _batteries.Count; i++)
        {
            if (_batteries[i] == b)
            {
                return i;
            }
        }

        return -1;
    }


    private bool AllBatteriesInserted()
    {
        foreach (var b in _batteryInserted)
        {
            if (!b)
                return false;
        }

        return true;
    }

    public void StartPulse(int i)
    {
        _batteryHolders[i].GetComponent<QuickOutline>().enabled = true;
        _batteryHolders[i].GetComponent<QuickOutline>().Blink = true;
    }

    public void StopPulse(int i)
    {
        _batteryHolders[i].GetComponent<QuickOutline>().enabled = false;
        _batteryHolders[i].GetComponent<QuickOutline>().Blink = false;
    }
}