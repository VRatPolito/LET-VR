
/*
 * Custom template by Gabriele P.
 */
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SolarPanelController : MonoBehaviour
{
    #region Events

    #endregion

    #region Editor Visible

    [SerializeField] [Range(0, 3)] private float _aheadOfPlayer = 1;
    [SerializeField] [Range(0, 2)] private float _heighFromFloor = 1.4f;
    [SerializeField] [Range(0, 10)] private float ScoreFactor = 3;

    #endregion

    #region Private Members and Constants

    private Vector3 _objectSize;
    private TextMeshPro _scoreVisualizerText;
    private float _score = 0;
    private float _calibratedControllerDistance = 2;

    #endregion

    #region Properties

    public float ControllerDistance { get; private set; }

    public float Score
    {
        get { return _score; }
        private set
        {
            _score = value;
            _scoreVisualizerText.text = $"Energy Harvested\n{_score:###0} cd";
        }
    }

    protected float NormalizedControllerDistance => Mathf.InverseLerp(0, _calibratedControllerDistance, ControllerDistance);

    #endregion

    #region MonoBehaviour

    void Start()
    {
        _calibratedControllerDistance = LocomotionManager.Instance.CalibrationData.ControllerDistance;
        _objectSize = transform.localScale;
        _scoreVisualizerText = GameObject.Find("ScoreVisualizer").GetComponent<TextMeshPro>();
    }

    void Update()
    {
        ControllerDistance = Vector3.Distance(LocomotionManager.Instance.LeftController.position,
            LocomotionManager.Instance.RightController.position);

        _objectSize.z = ControllerDistance;
        var p = LocomotionManager.Instance.CurrentPlayerController.transform.position;
        p += LocomotionManager.Instance.CurrentPlayerController.transform.forward * _aheadOfPlayer;
        p.y = _heighFromFloor;
        transform.position = p;
        transform.localScale = _objectSize;

        if (LocomotionManager.Instance.CurrentPlayerSpeed > 0.05f)
            Score += Time.deltaTime * Mathf.Pow(ScoreFactor, NormalizedControllerDistance * 20);
        //Debug.Log($"Score = {Score}");
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
