using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakePlayerChild : MonoBehaviour
{
    internal bool _makePlayerChild = false;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
     if(_makePlayerChild)
        {
            LocomotionManager.Instance.CurrentPlayerController.parent = transform;
            enabled = false;
        }
    }
}
