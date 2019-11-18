
/*
 * Custom template by F. Gabriele Pratticò {filippogabriele.prattico@polito.it}
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DroneWithPanelController : MonoBehaviour
{
	#region Events
		
	#endregion
	
	#region Editor Visible
		
	#endregion
	
	#region Private Members and Constants

    private MIPanelController _panelController;

    #endregion

    #region Properties

    #endregion

    #region MonoBehaviour

    void Awake()
    {

        _panelController = GetComponentInChildren<MIPanelController>();
        Assert.IsNotNull(_panelController);
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
