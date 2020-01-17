
/*
 * Custom template by Gabriele P.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceBasedSelectionToggle : BaseSelectionRadial
{
    #region Events

    #endregion

    #region Editor Visible

    [SerializeField] private Transform _src;
    [SerializeField] private Transform _dst;

    [SerializeField] private float _loadingTime = 1;

    #endregion

    #region Private Members and Constants

    private Transform _movingTarget;

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

    #endregion

    #region Coroutines

    protected override IEnumerator FillSelectionRadial()
    {
        yield return null;
    }

    #endregion
	
}
