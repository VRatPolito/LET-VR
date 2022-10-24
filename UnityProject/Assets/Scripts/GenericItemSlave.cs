using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class GenericItemSlave : MonoBehaviour
{
    [HideInInspector]
    public GenericItem Master;
    
    public void Interact(ItemController c)
    {
        Master.Interact(this, c);
    }

    //done update to action 
    public void ClickButton(object arg1, PadEventArgs arg2)
    {
        if (Master != null)
            Master.ClickButton(this, arg2);
    }

    public void UnClickButton(object arg1)
    {
        if (Master != null)
            Master.UnClickButton(this);
    }
    public void DisableOutline(ItemController c)
    {
        Master.DisableOutline(this, c);
    }
    public void EnableOutline(ItemController c)
    {
        Master.EnableOutline(this, c);
    }
}
