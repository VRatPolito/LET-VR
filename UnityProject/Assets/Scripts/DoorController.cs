﻿using System;
using System.Collections;
using System.Collections.Generic;
using PrattiToolkit;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class DoorController : MonoBehaviour
{
    #region Events

    public UnityEvent OnOpenGate,
        OnOpenGateFirstTime,
        OnCloseGate,
        DoorClosedPermanently;

    #endregion

    #region Editor Visible

    [SerializeField] private Collider _triggerCollider;
    [SerializeField] protected bool AutoClose = true;
    [SerializeField] protected bool _sensorEnabled = false;
    [SerializeField] protected bool _allowOverride = false;
    [SerializeField] private KeyCode _toggleGateKeyCode = KeyCode.D;

    #endregion

    #region Private Variables and Constants

    protected Animator _animator;
    protected ColliderEventsListener _triggerEventsListener;
    protected bool _playerInRange = false;
    private bool _firstTime = true;
#if ENABLE_INPUT_SYSTEM
    private KeyControl _toggleGateKey;
#endif

    #endregion

    #region Properties

    protected virtual bool PlayerInRange
    {
        get { return _playerInRange; }
        set
        {
            if (_playerInRange == value) return;
            _playerInRange = value;
            if (PlayerInRange)
                OpenGate(_playerInRange);
            else if (AutoClose)
                OpenGate(!AutoClose);
        }
    }

    public virtual void ForceOpenDoor()
    {
        OpenGate(true);
    }

    public virtual void ForceCloseDoor()
    {
        OpenGate(false);
    }

    public virtual bool UseTrigger
    {
        get { return _triggerCollider != null; }
    }

    public bool SensorEnabled
    {
        get { return _sensorEnabled; }
        set { _sensorEnabled = value; }
    }

    #endregion

    #region Mono Behaviour

    protected virtual void Awake()
    {
#if ENABLE_INPUT_SYSTEM
        _toggleGateKey = Keyboard.current.FindKeyOnCurrentKeyboardLayout(_toggleGateKeyCode.ToString());
#endif
        _animator = GetComponent<Animator>();
        if (_triggerCollider != null)
        {
            _triggerCollider.isTrigger = true;
            _triggerEventsListener = _triggerCollider.gameObject.GetOrAddComponent<ColliderEventsListener>();
        }

        if (UseTrigger)
        {
            _triggerEventsListener.OnTriggerEnterAction += c =>
            {
                if (SensorEnabled && c.tag == "Player") PlayerInRange = true;
            };
            _triggerEventsListener.OnTriggerExitAction += c =>
            {
                if (SensorEnabled && c.tag == "Player") PlayerInRange = false;
            };
            _triggerEventsListener.OnTriggerStayAction += c =>
            {
                if (SensorEnabled && c.tag == "Player") PlayerInRange = true;
            };
        }
    }

    protected virtual void Start()
    {
        var b = _animator.GetBehaviours<AdvancedStateMachineBehaviour>();
        b[0].StatePlayed += z => { DoorClosedPermanently?.Invoke(); };
    }

    protected virtual void Update()
    {
#if ENABLE_INPUT_SYSTEM
        if (_allowOverride && _toggleGateKey.isPressed)
            PlayerInRange = !PlayerInRange;
#else
        if (_allowOverride && Input.GetKeyDown(_toggleGateKeyCode))
            PlayerInRange = !PlayerInRange;
#endif
    }

    #endregion

    #region Helper Methods

    protected virtual void OpenGate(bool openOrClose)
    {
        _animator.SetBool("character_nearby", openOrClose);
        if (openOrClose)
        {
            OnOpenGate?.Invoke();
            if (_firstTime)
            {
                OnOpenGateFirstTime?.Invoke();
                _firstTime = false;
            }
        }
        else
            OnCloseGate?.Invoke();
    }

    #endregion
}