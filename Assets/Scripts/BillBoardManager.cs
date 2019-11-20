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
        {
            CheckOrder(_button0.transform);
            _button0.GetComponent<AudioSource>().Play();
            var seq = DOTween.Sequence();
            seq.Append(_button0.transform.Find("Button").DOLocalMoveY(-0.2f, .2f));
            seq.Append(_button0.transform.Find("Button").DOLocalMoveY(-0.1f, .2f));
            seq.Play();
        }
        else if (id == 1)
        {
            CheckOrder(_button1.transform);
            _button1.GetComponent<AudioSource>().Play();
            var seq = DOTween.Sequence();
            seq.Append(_button1.transform.Find("Button").DOLocalMoveY(-0.2f, .2f));
            seq.Append(_button1.transform.Find("Button").DOLocalMoveY(-0.1f, .2f));
            seq.Play();
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
            Debug.Log(item.name + " inserted correctly!");
            /*if(index == _interactOrder.Length)
                signalvictory*/
            return true;
        }

        Debug.Log(item.name + " inserted incorrectly!");
        //signalerror 
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
