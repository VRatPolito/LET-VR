
/*
 * Custom template by Gabriele P.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;


public class RunTimer : MonoBehaviour
{
	#region Events
		
	#endregion
	
	#region Editor Visible
		
	#endregion
	
	#region Private Members and Constants

    private TextMeshPro _text;
    private float _runTime = 0;

    #endregion

    #region Properties

    public bool Running { set; get; } = false;

    #endregion

    #region MonoBehaviour

    private void Start()
    {
        _text = GetComponentInChildren<TextMeshPro>();
        Assert.IsNotNull(_text);
    }

    void Update()
    {
        if (!Running) return;

        _runTime += Time.deltaTime;
        var ts = TimeSpan.FromSeconds(_runTime);
        _text.text = $"Run Time\n{ts.ToString("mm\\:ss\\.ff")}";
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
