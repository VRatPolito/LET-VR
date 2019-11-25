
/*
 * Custom template by Gabriele P.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using PrattiToolkit;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Serialization;

[RequireComponent(typeof(StatisticsLoggerL2))]
[RequireComponent(typeof(LocomotionManager))]
public class Level2Manager : UnitySingleton<Level2Manager>
{

    protected Level2Manager() { }

    #region Events

    #endregion

    #region Editor Visible

    public float TimeToStop = .5f;
    [SerializeField] private ProgressionLinePoint _pathController;
    public BackwardDestination StartBack;
    public DoorController CurvedDoor;
    public WalkingDestination CurvedStart, CurvedEnd, BackEnd;
    [SerializeField] private GameObject _headlight;
    public LockableMuseumItem BackwardItem;
    public Transform Edge;
    public DisableOnContactDestination StartDest, StartStairs, EndStairs, StarSlope, EndSlope, StartHalfStairs, StartHalfSlope, EndStairsSlope, FearStart, FearEnd, LevelEnd;
    [SerializeField] private Transform _fearDoor;
    #endregion

    #region Private Members and Constants

    private List<LockableMuseumItem> _lockableMuseumItems;
    private int _lmi_Idx = 0;
    private Coroutine _closeDoorCoroutineRef;


    #endregion

    #region Properties

    public StatisticsLoggerL2 StatisticsLogger { get; private set; }

    private Transform _player => LocomotionManager.Instance.CurrentPlayerController;

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        StatisticsLogger = GetComponent<StatisticsLoggerL2>();
        Assert.IsNotNull(StatisticsLogger);
        Assert.IsNotNull(_pathController);


        StartDest.OnDisabled += StartGrow;
        BackEnd.OnDisabled += StatisticsLogger.StopLogBackWalking;
        CurvedStart.OnDisabled += StatisticsLogger.StartLogCurvedWalking;
        CurvedStart.OnDisabled += AttachLight;
        CurvedEnd.OnDisabled += StatisticsLogger.StopLogCurvedWalking;
        CurvedEnd.OnDisabled += DetachLight;
        StartStairs.OnDisabled += StatisticsLogger.StartLogStair;
        EndStairs.OnDisabled += StatisticsLogger.StopLogStair;
        StarSlope.OnDisabled += StatisticsLogger.StartLogSlope;
        EndSlope.OnDisabled += StatisticsLogger.StopLogSlope;
        StartHalfStairs.OnDisabled += StatisticsLogger.StartLogHalfStairs;
        StartHalfSlope.OnDisabled += StatisticsLogger.StartLogHalfSlope;
        EndStairsSlope.OnDisabled += StatisticsLogger.StopLogStairsSlope;
        FearStart.OnDisabled += StatisticsLogger.StartLogFear;
        FearEnd.OnDisabled += OpenFearDoor;
        FearEnd.OnDisabled += StatisticsLogger.StopLogFear;
        LevelEnd.OnDisabled += EndGame;
    }

    private void EndGame(Destination d)
    {
        Application.Quit();
    }

    private void OpenFearDoor(Destination d)
    {
        var seq = DOTween.Sequence();
        seq.Append(_fearDoor.DOMoveX(15.02f, 1.5f));
        seq.Play();
    }

    private void DetachLight(Destination d)
    {
        _headlight.transform.parent = null;
        _headlight.GetComponent<Light>().enabled = false;
    }

    private void AttachLight(Destination d)
    {
        _headlight.transform.parent = LocomotionManager.Instance.CameraEye;
        _headlight.transform.localPosition = Vector3.zero;
        _headlight.GetComponent<Light>().enabled = true;
    }

    void Start()
    {
        _lockableMuseumItems = FindObjectsOfType<LockableMuseumItem>().ToList();
        _lockableMuseumItems.Sort((a, b) => string.Compare(a.name, b.name));
        //_lockableMuseumItems.Sort(l => l.gameObject.name);
        _lockableMuseumItems.ForEach(l => l.IsLockable = false);

        //var lmOne = _lockableMuseumItems[_lmi_Idx];
        // lmOne.OnLocked += OnLockedItem;
        // lmOne.IsLockable = true;

    }


    #endregion

    #region Public Methods

    public void StartGrow(Destination d)
    {
        _pathController.GrowHead();
        _pathController.GrowHead();
    }

    public void Grow()
    {
        _pathController.GrowHead();
        _pathController.ShrinkTail();
    }

    public void StartBackwardWalkingBehaviour()
    {
        Instance.StatisticsLogger.StartLogBackWalking();
        Instance.BackwardItem.IsLockable = true;
        Instance.BackwardItem.OnLocked += () => { Instance.CurvedDoor.SensorEnabled = true; CurvedDoor.ForceCloseDoor();
            _pathController.Hide();
        };

        Instance.BackwardItem.InteractiveItem.OnOut += () =>
        {
            if (_closeDoorCoroutineRef != null) StopCoroutine(_closeDoorCoroutineRef);
            _closeDoorCoroutineRef = StartCoroutine(CloseDoorCoroutine());
        };
        Instance.BackwardItem.InteractiveItem.OnOver += () =>
        {
            if (_closeDoorCoroutineRef != null) StopCoroutine(_closeDoorCoroutineRef);
            var dc = Instance.CurvedDoor;
            if (!dc.SensorEnabled && !Instance.BackwardItem.IsLocked)
                dc.ForceOpenDoor();
        };
    }

    #endregion

    #region Helper Methods

    #endregion

    #region Events Callbacks

    private void OnLockedItem()
    {
        var lm = _lockableMuseumItems[_lmi_Idx++];
        lm.OnLocked -= OnLockedItem;
        if (_lmi_Idx == _lockableMuseumItems.Count) return;
        lm = _lockableMuseumItems[_lmi_Idx];
        lm.OnLocked += OnLockedItem;
        lm.IsLockable = true;
        _pathController.GrowHead();

    }

    public void OnPlayerFallen()
    {
        StatisticsLogger.PlayerFallen();
    }

    #endregion

    #region Coroutines

    IEnumerator CloseDoorCoroutine()
    {
        yield return new WaitForSeconds(1);
        var dc = Level2Manager.Instance.CurvedDoor;
        if (!dc.SensorEnabled)
            dc.ForceCloseDoor();
        _closeDoorCoroutineRef = null;
    }

    #endregion
}
