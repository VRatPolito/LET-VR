using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VibrationController))]
public class ControllerManager : MonoBehaviour
{
    
    public ControllerHand Hand;
    public VRItemController Controller;
    [HideInInspector]
    public VibrationController vibrationController;
    // Use this for initialization
    void Start()
    {
        vibrationController = GetComponent<VibrationController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartPulsePublic(ControllerButtonByFunc buttonToInteract)
    {
        Controller.StartPulsePublic(buttonToInteract, Hand);
    }

    public void StopPulsePublic()
    {
        Controller.StopPulsePublic(Hand);
    }

    private void OnTriggerEnter(Collider other)
    {
        Controller.ManageTriggerEnter(other, Hand);
    }

    private void OnTriggerStay(Collider other)
    {
        Controller.ManageTriggerEnter(other, Hand);
    }

    private void OnTriggerExit(Collider other)
    {
        Controller.ManageTriggerExit(other, Hand);
    }

    public void StartPulsePublic(ControllerButton cbutton)
    {
        Controller.StartPulsePublic(cbutton, Hand);
    }

    public void DropItem(Transform t)
    {
        Controller.DropItem(t, false);
    }
}
