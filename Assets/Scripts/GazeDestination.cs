
/*
 * Custom template by Gabriele P.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeDestination : WalkingDestination
{
    #region Events

    #endregion

    #region Editor Visible

    [SerializeField] private LockableMuseumItem _item;
    #endregion

    #region Private Members and Constants

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

    public override void OnTriggerEnter(Collider other)
    {
        Level2Manager.Instance.StatisticsLogger.StopLogMultiStrLineWalking(this);
        if (other.CompareTag("Player"))
        {
            _item.OnLocked += StopLog;
            _item.IsLockable = true;
        }
    }

    private void StopLog()
    {
        gameObject.SetActive(false);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        Level2Manager.Instance.StatisticsLogger.StartLogMultiStrLineWalking(this);
    }

    public override void OnDisable()
    {
        _item.OnLocked -= StopLog;
        Level2Manager.Instance.Grow();
        base.OnDisable();
    }

    #endregion

    #region Coroutines

    #endregion

}
