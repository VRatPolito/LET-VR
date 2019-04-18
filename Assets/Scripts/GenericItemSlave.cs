using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericItemSlave : MonoBehaviour
{
    [HideInInspector]
    public GenericItem Master;

    public void Interact(ItemController c)
    {
        Master.Interact(this, c);
    }

    public void ClickButton(object sender, ClickedEventArgs e)
    {
        if (Master != null)
            Master.ClickButton(this);
    }

    public void UnClickButton(object sender, ClickedEventArgs e)
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
