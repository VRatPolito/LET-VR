using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class StartSeekMe : MonoBehaviour {

    public Transform Headset;
    Vector3 initialpos;
    // Use this for initialization
    void Start () {
        initialpos = transform.parent.worldToLocalMatrix.MultiplyPoint3x4(Headset.position);
        transform.localPosition = initialpos;
    }

}
