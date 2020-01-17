using PrattiToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class BillBoardManager : MonoBehaviour
{
    public UnityEvent InteractionError, AllInteractionsDone;

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

    [SerializeField] private GameObject _projector;
    int index = 0;
    private Sequence _intro;

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
            if(index == _interactOrder.Length)
                AllInteractionsDone?.Invoke();
            return true;
        }

        Debug.Log(item.name + " interaction is incorrect!");
        InteractionError?.Invoke();
        return false;
    }

    void Start()
    {
        _batteryHolder.Battery.GetComponent<GenericItem>().OnGrab.AddListener((arg0, hand) =>
        {
            _intro.Kill();
            _projector.GetComponentInChildren<ParticleSystem>().Stop();
        });

        _intro = DOTween.Sequence();
        _intro.Append(_batteryHolder.Battery.transform.DOMoveY(_batteryHolder.Battery.transform.position.y + 0.15f, 2.0f).SetEase(Ease.InOutSine));
        _intro.SetLoops(-1, LoopType.Yoyo);
        _intro.Play();
    }
}
