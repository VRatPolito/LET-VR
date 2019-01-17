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

    private bool _pulseEnabled = false;
    private float _t;
    private bool _up = false;

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
                    _batteryInserted[i] = true;
                    if (AllBatteriesInserted())
                    {
                        OpenGate(PlayerInRange);
                        NeedBatteries = false;
                        StopPulse();
                    }
					var battery = c.GetComponent<GenericItem>();
                    battery.IsKinematic = true;
					var p = battery.Player;
                    if (p != null)
                        p.DropItem(c.transform, true);
                    else
                        battery.GetComponent<Rigidbody>().isKinematic = true;
                    battery.CanInteract(false, LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>());
                    c.transform.parent = _batteryTargets[i].transform;
                    c.transform.position = _batteryTargets[i].transform.position;
                    c.transform.rotation = _batteryTargets[i].transform.rotation;
                    BatteryInserted.RaiseEvent(c.gameObject);
                }
            };
        }

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
    

    private bool AllBatteriesInserted()
    {
        foreach (var b in _batteryInserted)
        {
            if (!b)
                return false;
        }

        return true;
    }

    void Update()
    {
        Pulse();
    }

    public void StartPulse()
    {
        if (_pulseEnabled)
            StopPulse();

        _pulseEnabled = true;
    }
    public void StopPulse()
    {
        foreach (var b in _batteryHolders)
        {
            var m1 = b.materials[0];
            var c = m1.GetColor("_OutlineColor");
            m1.SetColor("_OutlineColor", new Color(c.r, c.g, c.b, 0));
            var m = new Material[1];
            m[0] = m1;
            b.materials = m;
            _pulseEnabled = false;
        }
    }

    void Pulse()
    {
        if (_pulseEnabled)
        {

            for (int i = 0; i < _batteryHolders.Count; i++)
            {
                if (_batteryInserted[i])
                {
                    var m1 = _batteryHolders[i].materials[0];
                    var c = m1.GetColor("_OutlineColor");
                    m1.SetColor("_OutlineColor", new Color(c.r, c.g, c.b, 0));
                    var m = new Material[1];
                    m[0] = m1;
                    _batteryHolders[i].materials = m;
                    _pulseEnabled = false;
                }
                else if (_up)
                {
                    _t += Time.deltaTime / 0.5f;
                    var m1 = _batteryHolders[i].materials[0];
                    var c = m1.GetColor("_OutlineColor");
                    m1.SetColor("_OutlineColor", new Color(c.r, c.g, c.b, Mathf.Lerp(0, 1, _t)));
                    var m = new Material[1];
                    m[0] = m1;
                    _batteryHolders[i].materials = m;
                    if (_t >= 1)
                    {
                        _up = false;
                        _t = 0;
                    }
                }
                else
                {
                    _t += Time.deltaTime / 0.5f;
                    var m1 = _batteryHolders[i].materials[0];
                    var c = m1.GetColor("_OutlineColor");
                    m1.SetColor("_OutlineColor", new Color(c.r, c.g, c.b, Mathf.Lerp(1, 0, _t)));
                    var m = new Material[1];
                    m[0] = m1;
                    _batteryHolders[i].materials = m;
                    if (_t >= 1)
                    {
                        _up = true;
                        _t = 0;
                    }
                }
            }
        }
    }


}
