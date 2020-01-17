
/*
 * Custom template by Gabriele P.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType : byte { Head, Position }

[Serializable]
public struct Bullet
{
    [Tooltip("Aim to Player or Specified position relative to Transform")]
    public TargetType TargetType;
    public Vector3 TargetPosition;

    [Range(0, 20)] public float Speed;

    [Tooltip("Time before next bullet")]
    [Range(0, 10)] public float LoadingTime;
    [Range(0, 2)] public float AimingTime;
    [Range(0, 2)] public float CoolDownTime;
    public int Id;
    //public Color Color;

    /// <summary>
    /// Get Target based on the Setted TargetType
    /// </summary>
    /// <param name="_referenceTransform"></param>
    /// <returns></returns>
    public Vector3 GetTarget(Transform _referenceTransform)
    {
        var r = Vector3.zero;
        switch (TargetType)
        {
            case TargetType.Head:
                r = LocomotionManager.Instance.CameraEye.position;
                break;
            case TargetType.Position:
                r = _referenceTransform.position + TargetPosition;
                break;
        }

        return r;
    }
}

[CreateAssetMenu(fileName = "FPSPattern", menuName = "FPSPattern/CreatePattern", order = 1)]
public class FPSPatternSO : ScriptableObject
{
    #region Events

    #endregion

    #region Editor Visible

    #endregion

    #region Editor Visible

    [SerializeField] private Material _defaultMaterial;

    [SerializeField] private GameObject _bulletPrefab;

    [SerializeField] private List<Bullet> _bullets = new List<Bullet>();

    #endregion

    #region Private Members and Constants

    #endregion

    #region Properties

    public List<Bullet> Bullets
    {
        get { return _bullets; }
        set { _bullets = value; }
    }

    public Material DefaultMaterial
    {
        get { return _defaultMaterial; }
        private set { _defaultMaterial = value; }
    }

    public GameObject BulletPrefab
    {
        get { return _bulletPrefab; }
        private set { _bulletPrefab = value; }
    }

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

    #endregion

}
