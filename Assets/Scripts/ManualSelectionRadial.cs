
/*
 * Custom template by Gabriele P.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using VRStandardAssets.Utils;

public class ManualSelectionRadial : BaseSelectionRadial
{
    #region Events

    #endregion

    #region Editor Visible

    [SerializeField] private float m_SelectionDuration = 2f;                                // How long it takes for the bar to fill.


    #endregion

    #region Private Members and Constants


    #endregion

    #region Properties

    public float SelectionDuration { get { return m_SelectionDuration; } }

    #endregion

    #region MonoBehaviour

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
        // At the start of the coroutine, the bar is not filled.
        m_RadialFilled = false;

        // Create a timer and reset the fill amount.
        float timer = 0f;
        m_Selection.fillAmount = 0f;

        // This loop is executed once per frame until the timer exceeds the duration.
        while (timer < m_SelectionDuration)
        {
            // The image's fill amount requires a value from 0 to 1 so we normalise the time.
            m_Selection.fillAmount = timer / m_SelectionDuration;

            // Increase the timer by the time between frames and wait for the next frame.
            timer += Time.deltaTime;
            yield return null;
        }

        // When the loop is finished set the fill amount to be full.
        m_Selection.fillAmount = 1f;

        // Turn off the radial so it can only be used once.
        Hide();

        // The radial is now filled so the coroutine waiting for it can continue.
        m_RadialFilled = true;

        // If there is anything subscribed to OnSelectionComplete call it.
        SelectionComlete();
    }


    #endregion

}
