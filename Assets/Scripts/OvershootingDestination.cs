
/*
 * Custom template by Gabriele P.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvershootingDestination : Destination
{
	#region Events
		
	#endregion
	
	#region Editor Visible
		
	#endregion
    
    #region Private Members and Constants

    #endregion
    private float _timeStop = float.MinValue;
    private Vector3 _prevpos = Vector3.negativeInfinity;
    #region Properties
		
    #endregion
 
    #region MonoBehaviour
	
    public override void Start()
    {
		
    }

    public override void Update()
    {

    }

    #endregion

    #region Public Methods

    #endregion

    #region Helper Methods

    #endregion

    #region Events Callbacks

    #endregion

    public override void OnDisable()
    {
        if (Next != null)
        {
            var d = Next.GetComponent<DoorController>();
            if (d != null)
                d.SensorEnabled = true;
            base.OnDisable();
        }
        else
            base.OnDisable();
    }

    public override void OnEnable()
    {
        Scenario1Manager.Instance.StatisticsLogger.StartLogOvershooting(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LocomotionManager.Instance.CurrentPlayerController == other.transform)
            _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
    }

    private void OnTriggerExit(Collider other)
    {
        if (LocomotionManager.Instance.CurrentPlayerController == other.transform)
        {
            Scenario1Manager.Instance.StatisticsLogger.LogOvershootError();
            _timeStop = float.MinValue;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (LocomotionManager.Instance.CurrentPlayerController == other.transform)
        {
            if (LocomotionManager.Instance.CurrentPlayerController.position == _prevpos)
            {
                if (_timeStop == float.MinValue)
                    _timeStop = Time.time;
                else if (Time.time >= _timeStop + Scenario1Manager.Instance.TimeToStop)
                {
                    Scenario1Manager.Instance.StatisticsLogger.StopLogOvershooting();
                        gameObject.SetActive(false);
                    }
            }
            else
            {
                _prevpos = LocomotionManager.Instance.CurrentPlayerController.position;
            }
        }
    }

    #region Coroutines

    #endregion
}
