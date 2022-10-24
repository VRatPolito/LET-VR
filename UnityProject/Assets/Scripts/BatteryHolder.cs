using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrattiToolkit;
using UnityEngine.Events;

public class BatteryHolder : MonoBehaviour
{
    public bool NeedBattery
    {
        get { return _needBattery; }
        set { _needBattery = value; }
    }

    public GameObject Battery
    {
        get { return _battery; }
        protected set { _battery = value; }
    }

    public UnityEvent BatteryPlugged, BatteryUnplugged;

    [SerializeField] protected bool _needBattery = false;
    MeshRenderer _batteryHolder;
    [SerializeField] protected GameObject _batteryTarget;
    [SerializeField] protected GameObject _battery;
    ColliderEventsListener _batteryTrigger;
    [SerializeField] protected bool _pulseWhenUnplugged = false;

    private void Awake()
    {
        _batteryHolder = GetComponent<MeshRenderer>();
        _batteryTrigger = GetComponent<ColliderEventsListener>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_needBattery && _pulseWhenUnplugged)
            StartPulse();
        _batteryTrigger.OnTriggerEnterAction += c =>
        {
            if (!NeedBattery || c.tag != "Item") return;

            if (c.gameObject == Battery)
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
            if (!NeedBattery || c.tag != "Item") return;
            (LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>()).OnDrop -= ItemDropped;
            if (_pulseWhenUnplugged)
                StartPulse();
        };
    }

    private void ItemDropped(GenericItem gi)
    {
        if (NeedBattery)
        {
            var g = Battery.GetComponent<GenericItem>();
            g.GetComponent<Rigidbody>().isKinematic = true;
            g.CanInteract(false, LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>());
            (LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>()).OnDrop -= ItemDropped;
            Battery.transform.parent = _batteryTarget.transform;
            Battery.transform.position = _batteryTarget.transform.position;
            Battery.transform.rotation = _batteryTarget.transform.rotation;
            NeedBattery = false;
            BatteryPlugged.Invoke();
        }
    }

    public void UnplugBattery()
    {
        var g = Battery.GetComponent<GenericItem>();
        g.GetComponent<Rigidbody>().isKinematic = false;
        g.CanInteract(true, LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>());
        (LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>()).OnDrop += ItemDropped;
        Battery.transform.parent = null;
        if (_pulseWhenUnplugged)
            StartPulse();
        NeedBattery = true;
        BatteryUnplugged.Invoke();
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