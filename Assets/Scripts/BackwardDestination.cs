
/*
 * Custom template by Gabriele P.
 */
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BackwardDestination : DisableOnContactDestination
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
        Level2Manager.Instance.Grow();
        base.OnDisable();
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Level2Manager.Instance.StartBackwardWalkingBehaviour();
            base.OnTriggerEnter(other);
        }
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
