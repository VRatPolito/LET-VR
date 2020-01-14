
/*
 * Custom template by Gabriele P.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRunningDestination : WalkingDestination
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
	
   public override void OnDisable()
    {
        Scenario1Manager.Instance.StatisticsLogger.StartLogSprinting();
        Scenario1Manager.Instance.OpenAllDoors();
        base.OnDisable();
    }

    #endregion
 
	#region Public Methods
		
	#endregion
 
    #region Helper Methods
		
	#endregion
	
	#region Events Callbacks
		
	#endregion

    

    #region Coroutines
		
	#endregion
	
}
