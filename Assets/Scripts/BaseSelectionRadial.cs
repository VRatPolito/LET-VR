
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

public abstract class BaseSelectionRadial : MonoBehaviour
{
    #region Events

    public event Action OnSelectionComplete;                                                // This event is triggered when the bar has filled.

    #endregion

    #region Editor Visible

    [SerializeField] protected bool m_HideOnStart = true;                                     // Whether or not the bar should be visible at the start.
    [SerializeField] protected Image m_Selection;

    #endregion

    #region Private Members and Constants

    protected Coroutine m_SelectionFillRoutine;                                               // Used to start and stop the filling coroutine based on input.
    protected bool m_RadialFilled;

    #endregion

    #region Properties

    public bool IsVisible { get; private set; }

    #endregion

    #region MonoBehaviour

    protected virtual void Start()
    {
        // Setup the radial to have no fill at the start and hide if necessary.
        m_Selection.fillAmount = 0f;

        if (m_HideOnStart)
            Hide();
    }

    #endregion

    #region Public Methods

    public void Show()
    {
        m_Selection.gameObject.SetActive(true);
        IsVisible = true;
    }


    public void Hide()
    {
        m_Selection.gameObject.SetActive(false);
        IsVisible = false;

        // This effectively resets the radial for when it's shown again.
        m_Selection.fillAmount = 0f;
    }

    public void StartFilling(bool autoShow = true)
    {
        if (autoShow) Show();
        // If the radial is active start filling it.
        if (IsVisible)
        {
            m_SelectionFillRoutine = StartCoroutine(FillSelectionRadial());
        }
    }


    public void StopFilling(bool autHide = true)
    {
        // If the radial is active stop filling it and reset it's amount.
        if (IsVisible)
        {
            if (m_SelectionFillRoutine != null)
                StopCoroutine(m_SelectionFillRoutine);

            m_Selection.fillAmount = 0f;
        }

        if (autHide) Hide();
    }

    #endregion

    #region Helper Methods

    protected void SelectionComlete()
    {
        OnSelectionComplete?.Invoke();
    }

    #endregion

    #region Events Callbacks



    #endregion

    #region Coroutines

    protected abstract IEnumerator FillSelectionRadial();


    public IEnumerator WaitForSelectionRadialToFill()
    {
        // Set the radial to not filled in order to wait for it.
        m_RadialFilled = false;

        // Make sure the radial is visible and usable.
        Show();

        // Check every frame if the radial is filled.
        while (!m_RadialFilled)
        {
            yield return null;
        }

        // Once it's been used make the radial invisible.
        Hide();
    }

    #endregion

}
