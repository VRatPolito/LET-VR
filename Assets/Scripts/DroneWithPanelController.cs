/*
 * Custom template by F. Gabriele Pratticò {filippogabriele.prattico@polito.it}
 */

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PrattiToolkit;
using UnityEngine;
using UnityEngine.Assertions;
using Valve.VR;
using Random = UnityEngine.Random;

public class DroneWithPanelController : MonoBehaviour
{ 
    #region Events

    #endregion

    #region Editor Visible

    [SerializeField] [Range(0, 3)] private float _aheadOfPlayer = 1.5f;
    [SerializeField] [Range(0, 20)] private float _warningRadius = 5f;
    [SerializeField] private float _escapeSpeed = 3;
    [SerializeField] private float _rotateSpeed = 15;

    [SerializeField] private GameObject _dummyPlayer, _target;

    #endregion

    #region Private Members and Constants
    
    private CharacterController _controller;
    private Vector3 _lastDir;
    private float _currSpeed = 0;

    #endregion

    #region Properties

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
      
    }

    void FixedUpdate()
    {
        if (_dummyPlayer != null && _target != null)
        {
            var dir = (_target.transform.position - _dummyPlayer.transform.position);
            _dummyPlayer.GetComponent<CharacterController>().SimpleMove(dir);
        }

        if(PlayerInRange())
        {
            var pDir = (transform.position - actualPlayer.position).normalized;
            pDir = new Vector3(pDir.x, 0, pDir.z);
            Debug.DrawRay(transform.position, pDir, Color.red, 2f);
            _currSpeed = Mathf.Lerp(_currSpeed, _escapeSpeed, .5f);
            transform.Translate(pDir * _currSpeed * Time.fixedDeltaTime, Space.World);
            _lastDir = pDir;

            var rDir = (actualPlayer.position - transform.position).normalized;
            rDir = new Vector3(rDir.x, 0, rDir.z);
            Debug.DrawRay(transform.position, rDir, Color.green, 2f);
            var rot = Quaternion.LookRotation(pDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.fixedDeltaTime * _rotateSpeed);
        }
        else
        {
            _currSpeed = Mathf.Lerp(_currSpeed, 0, .01f);
            transform.Translate(_lastDir * _currSpeed * Time.fixedDeltaTime, Space.World);
        }
    }

    #endregion

    #region Public Methods

    public bool PlayerInRange()
    {
        return (actualPlayer.position - transform.position).magnitude <
               Mathf.Max(_aheadOfPlayer, _warningRadius); //warning
    }

    public Transform actualPlayer
    {
        get => LocomotionManager.Instance.CurrentPlayerController;
    }

    #endregion

    #region Helper Methods    
    
    #endregion

    #region Events Callbacks

    #endregion

    #region Coroutines

    #endregion
}