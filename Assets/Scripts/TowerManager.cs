using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [SerializeField]
    private Transform[] _pieces;
    [SerializeField]
    private Transform[] _refPieces;
    Transform[] _refLenghts;
    // Start is called before the first frame update
    private void Awake()
    {
        _refLenghts = new Transform[_pieces.Length];
        for(int i = 0; i < _pieces.Length; i++)
            _refLenghts[i] = _refPieces[i].GetChild(0);
    }

    public float GetPosDiff()
    {
        float diff = 0;
        for (int i = 0; i < _pieces.Length; i++)
            diff += GetPosDiff(i);
        return diff / _pieces.Length;
    }
    public float GetRotDiff()
    {
        float diff = 0;
        for (int i = 0; i < _pieces.Length; i++)
            diff += GetRotDiff(i);
        return diff / _pieces.Length;
    }

    private float GetPosDiff(int i)
    {
        var dist = Vector3.Distance(_pieces[i].transform.position, _refPieces[i].position);
        var maxdist = Vector3.Distance(_refPieces[i].position, _refLenghts[i].position);
        if (dist >= maxdist)
            return 0;
        else
            return (1 - (dist / maxdist)) * 100;
    }

    private float GetRotDiff(int i)
    {
        /*var dir1 = _pieces[i].up;
        var dir2 = _refPieces[i].up;
        dir1 = new Vector3(dir1.x, 0, dir1.z).normalized;
        dir2 = new Vector3(dir2.x, 0, dir2.z).normalized;
        var angle = Vector3.Angle(dir1, dir2);*/

        var angle = Quaternion.Angle(_pieces[i].rotation, _refPieces[i].rotation);
        angle = angle % 360;
        if (angle > 180)
            angle -= 360;
        if (angle < -180)
            angle += 360;

        angle = Mathf.Abs((Mathf.Abs(angle) - 90));

        return (Mathf.Abs(angle) / 90) * 100;
    }
}
