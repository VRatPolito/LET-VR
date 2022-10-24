
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
    [SerializeField] private bool _stopOnCollision = true;
    public float Angle = 0;

    #endregion

    #region Private Members and Constants

    private Rigidbody _rb;
    private ParticleSystem _ps;
    private float _flightHeight;
    private bool _startingOrEnding=true;
    private bool _collided=false;
    private Vector3 _p;

    #endregion
	
    #region Properties
		
    #endregion
 
    #region MonoBehaviour

    private void OnTriggerEnter(Collider other)
    {
        if (!_startingOrEnding && _stopOnCollision) _collided = true;
    }

    void Start()
    {
        Angle = Mathf.Rad2Deg * Mathf.Asin(_distanceFromPlayer / Mathf.Sqrt((Mathf.Pow(_distanceFromPlayer, 2) + Mathf.Pow(_aheadOfPlayer, 2))));
        _rb = GetComponent<Rigidbody>();
        _ps = GetComponent<ParticleSystem>();
        _ps.Stop();
        Invoke("Starting", 2);
    }
 
    void Update()
    {
        if(_startingOrEnding || _collided) return;
        CalcPosition();
        _rb.DOMove(_p,Time.deltaTime); //_rb.MovePosition(p);
    }

    #endregion

    #region Public Methods

    public void End()
    {
        _startingOrEnding = true;
        var shutdownSequence = DOTween.Sequence();
        shutdownSequence.Append(transform.DOShakePosition(4, Vector3.one*0.2f));
        shutdownSequence.Join(transform.DOScale(Vector3.zero, 4).SetEase(Ease.InCubic));
        shutdownSequence.OnComplete(() =>
        {
            _ps.Stop();
            Destroy(gameObject);
        });
        shutdownSequence.Play();

    }

    #endregion

    #region Helper Methods

    private Vector3 CalcPosition()
    {
        _p = LocomotionManager.Instance.CurrentPlayerController.position;
        _p.x += _aheadOfPlayer;
        _p.y = _rb.position.y;
        _p.z += _distanceFromPlayer;
        return _p;
    }

    private void Starting()
    {
        _flightHeight = LocomotionManager.Instance.CameraEye.transform.position.y;
        CalcPosition();
        _p.y = _flightHeight;
        _ps.Play();
        _rb.DOMove(_p, 2).OnComplete(() => _startingOrEnding = false);
    }

	#endregion
	
	#region Events Callbacks
		
	#endregion
	
	#region Coroutines
		
	#endregion
	
}
