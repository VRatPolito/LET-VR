using PrattiToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoardManager : MonoBehaviour
{
    [SerializeField]
    LeverManager _levaManager;
    [SerializeField]
    GenericItem _button0;
    [SerializeField]
    GenericItem _button1;
    [SerializeField]
    BatteryHolder _batteryHolder;

    [SerializeField]
    Transform[] _interactOrder;
    int index = 0;

    public void ButtonPressed(int id)
    {
        if (id == 0)
            CheckOrder(_button0.transform);
        else if (id == 1)
            CheckOrder(_button1.transform);
    }

    public void LeverPushed()
    {
        CheckOrder(_levaManager.transform);
    }

    public void OnBatteryInserted()
    {
        CheckOrder(_batteryHolder.transform);
    }

    private void CheckOrder(Transform item)
    {
        if(_interactOrder[index]== item)
        {
            index++;
            /*if(index == _interactOrder.Length)
                signalvictory*/
        }
        /*else
           signalerror*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
