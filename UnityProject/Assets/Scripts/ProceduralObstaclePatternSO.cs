
/*
 * Custom template by Gabriele P.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct POPelement
{
    public Color Color;
    [Range(0, 1)] public float Xs, Xe, Ys, Ye;
    public float Length, FrontLine;

    public Vector3 NormalizedSize
    {
        get
        {
            return new Vector3(Xe - Xs, Ye - Ys, 1);
        }
    }

    public Vector3 NormalizedCentre
    {
        get
        {
            return new Vector3((Xe + Xs) / 2, (Ye + Ys) / 2, 1);
        }
    }
}

[CreateAssetMenu(fileName = "ProceduralObstaclePattern", menuName = "ProceduralObstaclePattern/CreatePattern", order = 1)]
public class ProceduralObstaclePatternSO : ScriptableObject
{

    #region Editor Visible
    [Range(0, 10)] [SerializeField] private float _speed = 0.1f;
    [Range(0, 5)] [SerializeField] private float _endPadding = 0.2f;

    [SerializeField] private Material _defaultMaterial;

    [SerializeField] private List<POPelement> _elements = new List<POPelement>();

    #endregion

    #region Private Members and Constants

    #endregion

    #region Properties

    public List<POPelement> Elements
    {
        get { return _elements; }
        set { _elements = value; }
    }

    public Material DefaultMaterial
    {
        get { return _defaultMaterial; }
        set { _defaultMaterial = value; }
    }

    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    public float EndPadding
    {
        get { return _endPadding; }
        set { _endPadding = value; }
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
