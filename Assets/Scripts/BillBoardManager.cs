using PrattiToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BillBoardManager : MonoBehaviour
{
    [SerializeField]
    LeverManager _leverManager;
    [SerializeField]
    ButtonManager _button0;
    [SerializeField]
    ButtonManager _button1;
    [SerializeField]
    BatteryHolder _batteryHolder;
    [SerializeField]
    Transform[] _interactOrder;
    int index = 0;

    public void ButtonPressed(int id)
    {
        if (id == 0)
        {
            if (!CheckOrder(_button0.transform))
                _button0.ResetPush();
        }
        else if (id == 1)
        {
            if (!CheckOrder(_button1.transform))
                _button1.ResetPush();
        }
    }
    
    public void LeverPushed()
    {
        if (!CheckOrder(_leverManager.transform))
            _leverManager.ResetPush();
    }

    public void OnBatteryInserted()
    {
        if (!CheckOrder(_batteryHolder.transform))
            _batteryHolder.UnplugBattery();
    }

    private bool CheckOrder(Transform item)
    {
        if(_interactOrder[index]== item)
        {
            index++;
            Debug.Log(item.name + " interaction is correct!");
            /*if(index == _interactOrder.Length)
                signalvictory*/
            return true;
        }

        Debug.Log(item.name + " interaction is incorrect!");
        //signalerror 
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
