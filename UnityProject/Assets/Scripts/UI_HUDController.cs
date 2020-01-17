using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public interface I_UI_HUDController
{
    void ShowCalibrationIcon(float autoHide = 5);
    void ShowFreezeIcon();
    void HideCalibrationIcon();
    void HideFreezeIcon();
}

public class UI_HUDController : MonoBehaviour, I_UI_HUDController
{

    #region Events

    #endregion

    #region Editor Visible

    [SerializeField] private Image _calibrationImage, _freezeImage;

    #endregion

    #region Private Members and Constants

    #endregion

    #region Properties

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        Assert.IsNotNull(_calibrationImage);
        Assert.IsNotNull(_freezeImage);
        HideFreezeIcon();
        HideCalibrationIcon();
        //LocomotionManager.Instance.CurrentUIController = this;
    }

    #endregion

    #region Public Methods

    public void ShowCalibrationIcon(float autoHide = 5)
    {
        _calibrationImage.enabled = true;
        Invoke("HideCalibrationIcon", autoHide);
    }

    public void ShowFreezeIcon()
    {
        _freezeImage.enabled = true;
    }

    public void HideCalibrationIcon()
    {
        _calibrationImage.enabled = false;
    }

    public void HideFreezeIcon()
    {
        _freezeImage.enabled = false;
    }


    #endregion

    #region Helper Methods

    #endregion

    #region Events Callbacks

    #endregion

    #region Coroutines

    #endregion
}
