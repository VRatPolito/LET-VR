
/*
 * Custom template by Gabriele P.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TeleportPlayer : MonoBehaviour
{
    #region Events
    public UnityEvent OnPlayerFallenDown;
	#endregion
	
	#region Editor Visible
		
	#endregion
	
	#region Private Members and Constants
		
	#endregion
	
    #region Properties
		
    #endregion
 
    #region MonoBehaviour
	
    void Start()
    {
		
    }
 
    void Update()
    {
		
    }

    #endregion

    #region Public Methods

    #endregion

    #region Helper Methods

    #endregion

    #region Events Callbacks
    private void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Player"))
        {
            c.transform.position = new Vector3(transform.position.x- 1, transform.position.y, transform.position.z-1);
            OnPlayerFallenDown?.Invoke();
        }
    }
    #endregion

    #region Coroutines

    #endregion

}
