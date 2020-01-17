
/*
 * Custom template by Gabriele P.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using PrattiToolkit;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Assertions;
using VRStandardAssets.Utils;

[RequireComponent(typeof(VRInteractiveItem))]
[RequireComponent(typeof(BaseSelectionRadial))]
public class LockableMuseumItem : MonoBehaviour
{
    #region Events

    public event Action OnLocked;

    #endregion

    #region Editor Visible

    [SerializeField] [Range(0.1f, 2)] private float _playerDetectionRange;
    [SerializeField] private bool _checkDistance = true;

    #endregion

    #region Private Members and Constants


    private BaseSelectionRadial _selectionRadial;
    private bool _isLocked;
    private Light _spotlight;
    private GameObject _lockIcon;

    #endregion

    #region Properties

    public bool IsLockable { get; set; }

    public bool IsLocked
    {
        get
        {
            return _isLocked;
        }

        set
        {
            _isLocked = value;

            if (_isLocked)
            {
                OnLocked.RaiseEvent();
                IsLockable = false;
            }
            if (_spotlight != null) _spotlight.enabled = !_isLocked;
        }
    }

    public VRInteractiveItem InteractiveItem { get; protected set; }

    #endregion

    #region MonoBehaviour

    private void OnEnable()
    {
        if (InteractiveItem != null)
        {
            InteractiveItem.OnOver += LookAtMe;
            InteractiveItem.OnOut += LookAway;
        }
    }

    private void OnDisable()
    {
        if (InteractiveItem != null)
        {
            InteractiveItem.OnOver -= LookAtMe;
            InteractiveItem.OnOut -= LookAway;
        }
    }

    void Start()
    {
        InteractiveItem = GetComponent<VRInteractiveItem>();
        _selectionRadial = GetComponent<BaseSelectionRadial>();
        _spotlight = transform.GetChildRecursive("LockLight").GetComponent<Light>();
        _lockIcon = transform.GetChildRecursive("Lock").gameObject;
        Assert.IsNotNull(InteractiveItem);
        Assert.IsNotNull(_selectionRadial);
        Assert.IsNotNull(_spotlight);
        Assert.IsNotNull(_lockIcon);
        _lockIcon.SetActive(false);

        IsLocked = false;
        _selectionRadial.OnSelectionComplete += () =>
        {
            IsLocked = true;
            //_selectionRadial.Hide();
        };

        InteractiveItem.OnOut += () =>
        {
            if (!IsLocked && IsLockable)
                _selectionRadial.StopFilling();
        };
    }

    void Update()
    {
        if (!IsLockable || IsLocked) return;
        float playerDistance = Vector3.Distance(LocomotionManager.Instance.CurrentPlayerController.position, transform.position);
        //Debug.Log($"{gameObject.name} player Distance {playerDistance:F1} Locked= {IsLocked}");
        if (InteractiveItem.IsOver && !IsLocked && !_selectionRadial.IsVisible
                    && (!_checkDistance || _playerDetectionRange >= playerDistance))
            _selectionRadial.StartFilling();

    }

    #endregion

    #region Public Methods

    #endregion

    #region Helper Methods

    #endregion

    #region Events Callbacks

    private void LookAtMe()
    {
        //_selectionRadial.StartFilling();
    }

    private void LookAway()
    {
        _selectionRadial.StopFilling();
    }

    #endregion

    #region Coroutines

    #endregion

}
