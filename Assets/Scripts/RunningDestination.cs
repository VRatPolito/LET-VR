
/*
 * Custom template by Gabriele P.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningDestination : WalkingDestination
{
	#region Events
		
	#endregion
	
	#region Editor Visible
		
	#endregion
	
	#region Private Members and Constants
		
	#endregion
	
    #region Properties
		
    #endregion
 
    #region MonoBehaviour
	
    #endregion
 
	#region Public Methods
		
	#endregion
 
    #region Helper Methods
		
	#endregion
	
	#region Events Callbacks
		
	#endregion

    public override void OnDisable()
    {
        Level1Manager.Instance.StatisticsLogger.StartLogSprinting();
        base.OnDisable();
    }

    #region Coroutines
		
	#endregion
	
}
