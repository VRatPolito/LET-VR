
/*
 * Custom template by Gabriele P.
 */
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using PrattiToolkit;
using UnityEngine;
using UnityEngine.Assertions;


public struct SceneObstacleRepresentation
{
    public GameObject GO;
    public int POPItemIndex;
}


public class ProceduralObstacleSpawnerSystem : MonoBehaviour
{
    #region Events

    #endregion

    #region Editor Visible

    //[Range(0,1)][SerializeField] private float _frontlineDetectionThreshold = 0.1f;
    [SerializeField] private MeshFilter _sourceSpawnQuad;
    [SerializeField] private Transform _farClippingTransform;
    [Expandable] [SerializeField] private ProceduralObstaclePatternSO _proceduralObstaclePattern;

    #endregion

    #region Private Members and Constants

    private Vector3 _movingDirection = Vector3.right;

    private float _currentFrontLine = 0;
    private int _currentPOPIdx = 0;


    private Dictionary<GameObject, SceneObstacleRepresentation> _sceneObstaclesList = new Dictionary<GameObject, SceneObstacleRepresentation>();
    private float _toTravel;
    private Bounds _sqBounds;

    #endregion

    #region Properties

    #endregion

    #region MonoBehaviour

    void Start()
    {
        _movingDirection = (_farClippingTransform.position - _sourceSpawnQuad.transform.position).normalized;
        Assert.IsNotNull(_proceduralObstaclePattern);
        _proceduralObstaclePattern.Elements.Sort((e1, e2) => e1.FrontLine.CompareTo(e2.FrontLine));
        _toTravel = Vector3.Distance(_farClippingTransform.position, _sourceSpawnQuad.transform.position);

        _sqBounds = _sourceSpawnQuad.gameObject.GetComponent<Renderer>().bounds;
    }

    void Update()
    {
        var dS = _proceduralObstaclePattern.Speed * _movingDirection * Time.deltaTime;
        _currentFrontLine += dS.magnitude;

        //Move forward already existing obstacle
        foreach (var ob in _sceneObstaclesList)
        {
            var p = ob.Value.GO.transform.position;
            p += dS;
            ob.Value.GO.transform.position = p;
        }

        //Splattener
        //foreach (var ob in _sceneObstaclesList)
        //{
        //    var p = ob.Value.GO.transform.position;
        //    var e = _proceduralObstaclePattern.Elements[ob.Value.POPItemIndex];
        //    var fp = Vector3.Distance(p, _sourceSpawnQuad.transform.position) + e.Length / 2 - _toTravel; //must be checked with real lenght not e.Lenght
        //    if (fp > 0)
        //    {
        //        ob.Value.GO.transform.localScale -= fp * Vector3.left; //left?
        //    }
        //}

        //Recycle
        var toRemove = new List<GameObject>();
        foreach (var ob in _sceneObstaclesList)
        {
            var p = ob.Value.GO.transform.position;
            var e = _proceduralObstaclePattern.Elements[ob.Value.POPItemIndex];
            if (Vector3.Distance(p, _sourceSpawnQuad.transform.position) + e.Length / 2 > _toTravel)
            {
                Destroy(ob.Key);
                toRemove.Add(ob.Key);
            }
        }
        foreach (var garbage in toRemove)
            _sceneObstaclesList.Remove(garbage);

        //Generate Entering Frontline
        bool endProcessing = false;
        while (endProcessing == false)
        {
            if (_currentPOPIdx == _proceduralObstaclePattern.Elements.Count)
            {
                _currentPOPIdx = 0;
                _currentFrontLine = 0 - dS.sqrMagnitude - _proceduralObstaclePattern.EndPadding;
                endProcessing = true;
            }

            var e = _proceduralObstaclePattern.Elements[_currentPOPIdx];
            if (e.FrontLine < _currentFrontLine)
                SpawnElement(_currentPOPIdx++);
            else
                endProcessing = true;
        }
    }

    #endregion

    #region Public Methods

    public void DestroyRenderedElements()
    {
        _currentFrontLine = _currentPOPIdx = 0;
        foreach (var ob in _sceneObstaclesList)
        {
            Destroy(ob.Key);
        }
        _sceneObstaclesList.Clear();
    }

    #endregion

    #region Helper Methods

    private void SpawnElement(int elementIdx)
    {
        if (Vector3.Dot(LocomotionManager.Instance.CurrentPlayerController.position - _sourceSpawnQuad.transform.position, -_movingDirection) > 0) return;

        var e = _proceduralObstaclePattern.Elements[elementIdx];
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.AddComponent<CollisionDetect>();
        var c = go.AddComponent<BoxCollider>();
        c.isTrigger = true;
        c.size *= 1.2f;
        //go.GetComponent<MeshFilter>().mesh.FillMesh();

        _sceneObstaclesList.Add(go, new SceneObstacleRepresentation
        {
            GO = go,
            POPItemIndex = elementIdx
        });
        go.name = elementIdx.ToString();
        //Debug.Log($"[{elementIdx}] Frontline = {_currentFrontLine}");

        //TODO GENERALIZE SIZE & POSITION FROM ORIENTATION *************************************************
        var p = _sqBounds.center - _movingDirection * e.Length / 2;

        p.z += Mathf.LerpUnclamped(-_sqBounds.extents.z, _sqBounds.extents.z, e.NormalizedCentre.x);
        p.y += Mathf.LerpUnclamped(-_sqBounds.extents.y, _sqBounds.extents.y, e.NormalizedCentre.y);

        var scale = Vector3.one;
        scale.z = e.NormalizedSize.x * _sqBounds.extents.z * 2;
        scale.y = e.NormalizedSize.y * _sqBounds.extents.y * 2;
        scale.x = e.Length;
        //***************************************************************************************************
        go.transform.position = p;
        go.transform.localScale = scale;

        var r = go.GetComponent<Renderer>();
        r.material = _proceduralObstaclePattern.DefaultMaterial;
        r.material.color = e.Color;
    }

    #endregion

    #region Events Callbacks

    #endregion

    #region Coroutines

    #endregion
}
