
/*
 * Custom template by Gabriele P.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceBasedSelectionRadial : BaseSelectionRadial
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
        _movingTarget = LocomotionManager.Instance.CurrentPlayerController.transform;

        float currentDist, finalDist;
        for (currentDist = Vector3.Distance(_src.position, _movingTarget.position), finalDist = Vector3.Distance(_dst.position, _src.position);
            currentDist < finalDist;
            currentDist = Vector3.Distance(_src.position, _movingTarget.position), finalDist = Vector3.Distance(_dst.position, _src.position))
        {
            m_Selection.fillAmount = m_Selection.fillAmount > 0.99f ? 0 : m_Selection.fillAmount + Time.deltaTime / _loadingTime;
            timer += Time.deltaTime;
            yield return null;
        }

        m_Selection.fillAmount = 1f;

        // Turn off the radial so it can only be used once.
        Hide();

        // The radial is now filled so the coroutine waiting for it can continue.
        m_RadialFilled = true;

        // If there is anything subscribed to OnSelectionComplete call it.
        SelectionComlete();
    }

    //protected override IEnumerator FillSelectionRadial()
    //{
    //    // At the start of the coroutine, the bar is not filled.
    //    m_RadialFilled = false;

    //    m_Selection.fillAmount = 0f;
    //    _movingTarget = LocomotionManager.Instance.CurrentPlayerController.transform;

    //    // This loop is executed once per frame until the timer exceeds the duration.
    //    float currentDist, finalDist;
    //    for (currentDist = Vector3.Distance(_src.position, _movingTarget.position), finalDist = Vector3.Distance(_dst.position, _src.position);
    //        currentDist < finalDist;
    //        currentDist = Vector3.Distance(_src.position, _movingTarget.position), finalDist = Vector3.Distance(_dst.position, _src.position))
    //    {
    //        // The image's fill amount requires a value from 0 to 1 so we normalise the time.
    //        m_Selection.fillAmount = currentDist / finalDist;

    //        yield return null;
    //    }

    //    // When the loop is finished set the fill amount to be full.
    //    m_Selection.fillAmount = 1f;

    //    // Turn off the radial so it can only be used once.
    //    Hide();

    //    // The radial is now filled so the coroutine waiting for it can continue.
    //    m_RadialFilled = true;

    //    // If there is anything subscribed to OnSelectionComplete call it.
    //    SelectionComlete();
    //}

    #endregion

}
