/*
 * Custom template by Gabriele P.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnContactDestination : Destination
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

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            gameObject.SetActive(false);
        }
    }

    #region Coroutines

    #endregion
}