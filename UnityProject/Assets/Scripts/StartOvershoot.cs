
/*
 * Custom template by Gabriele P.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartOvershoot : MonoBehaviour
{
    #region Events

    #endregion

    #region Editor Visible

    public OvershootingDestination Dest;
    #endregion

    #region Private Members and Constants

    #endregion
    bool _started = false;
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
		
	#endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !_started)
        {
            Dest.gameObject.SetActive(true);;
            _started = true;
        }
    }

    #region Coroutines
		
	#endregion
	
}
