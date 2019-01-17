
/*
 * Custom template by Gabriele P.
 */
using System.Collections;
using System.Collections.Generic;
using BansheeGz.BGSpline.Curve;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(BGCurve))]
public class ProgressionLinePoint : MonoBehaviour
{
    #region Events

    #endregion

    #region Editor Visible

    #endregion

    #region Private Members and Constants

    private BGCurvePoint[] _curveFinalPoints;
    private int _addIdx = 0;

    #endregion

    #region Properties

    public BGCurve BgCurve { get; private set; }

    #endregion

    #region MonoBehaviour

    void Start()
    {
        BgCurve = GetComponent<BGCurve>();
        Assert.IsNotNull(BgCurve);
        _curveFinalPoints = BgCurve.Points as BGCurvePoint[];
        BgCurve.Clear();
        /*GrowHead();
        GrowHead();*/
    }

    void Update()
    {
#if DEBUG
        if (Input.GetKeyDown(KeyCode.X))
            GrowHead();
        else if(Input.GetKeyDown(KeyCode.Z))
            ShrinkHead();
#endif

    }


    private void OnApplicationQuit()
    {
        BgCurve.Clear();
        BgCurve.AddPoints(_curveFinalPoints);
    }

    #endregion

    #region Public Methods

    #endregion

    #region Helper Methods

    public void Hide()
    {
        BgCurve.gameObject.SetActive(false);
    }

    public void Show()
    {
        BgCurve.gameObject.SetActive(true);
    }

    public void GrowHead()
    {
        if(_addIdx==_curveFinalPoints.Length) return;
        BgCurve.AddPoint(_curveFinalPoints[_addIdx++]);
    }

    public void ShrinkHead()
    {
        if(_addIdx==0) return;
        BgCurve.Delete(_curveFinalPoints[--_addIdx]);
    }

    public void ShrinkTail()
    {
        if(BgCurve.PointsCount==0) return;
        BgCurve.Delete(0);
    }

    #endregion

    #region Events Callbacks

    #endregion

    #region Coroutines

    #endregion
}
