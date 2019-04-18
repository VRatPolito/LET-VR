
/*
 * Custom template by Gabriele P.
 */
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using VRStandardAssets.Utils;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(VRInteractiveItem))]
public class LookAtMeCatalizer : MonoBehaviour
{
	#region Events
		
	#endregion
	
	#region Editor Visible

	[SerializeField] [Range(0,10)]private float _distanceFromPlayer = 2;
	[SerializeField] [Range(0,15)]private float _aheadOfPlayer = 1;

	#endregion
	
	#region Private Members and Constants

    private Rigidbody _rb;
    private ParticleSystem _ps;
    private float _flightHeight;
    private bool _starting=true;
    private bool _collided=false;

    #endregion
	
    #region Properties
		
    #endregion
 
    #region MonoBehaviour

    private void OnTriggerEnter(Collider other)
    {
        if (!_starting) _collided = true;
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _ps = GetComponent<ParticleSystem>();
        _ps.Stop();
        Invoke("Starting", 2);
    }
 
    void Update()
    {
        if(_starting || _collided) return;
        var p = CalcPosition();
        _rb.MovePosition(p);
    }

    #endregion

    #region Public Methods

    #endregion

    #region Helper Methods

    private Vector3 CalcPosition()
    {
        var p = LocomotionManager.Instance.CurrentPlayerController.position;
        p.x += _aheadOfPlayer;
        p.y = _rb.position.y;
        p.z += _distanceFromPlayer;
        return p;
    }

    private void Starting()
    {
        _flightHeight = LocomotionManager.Instance.CameraEye.transform.position.y;
        var p = CalcPosition();
        p.y = _flightHeight;
        _ps.Play();
        _rb.DOMove(p, 2).OnComplete(() => _starting = false);
    }

	#endregion
	
	#region Events Callbacks
		
	#endregion
	
	#region Coroutines
		
	#endregion
	
}
