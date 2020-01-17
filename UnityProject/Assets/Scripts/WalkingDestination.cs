
/*
 * Custom template by Gabriele P.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingDestination : DisableOnContactDestination
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
