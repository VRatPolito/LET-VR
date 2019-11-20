using PrattiToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryHolder : MonoBehaviour
{
    ColliderEventsListener _collider;

    private void Awake()
    {
        _collider = GetComponent<ColliderEventsListener>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
