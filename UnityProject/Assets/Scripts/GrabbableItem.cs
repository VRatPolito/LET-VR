using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//public enum ItemHand { Left, Right, Any };
public abstract class GrabbableItem : MonoBehaviour
{
    public ItemCodes Code;
    public AudioClip Drop, Grab;
    public float DropVolume = 1, GrabVolume = 1;
    public bool HideController = true;
    public bool ItemEnabled = false;
    public /*ItemHand*/ControllerHand Hand;
    public GrabbableItemSlave Slave;
    [HideInInspector]
    public bool setupdone = false;
    [HideInInspector]
    public bool initialized = false;
    //Transform Son;
    public ItemController Player;
    [HideInInspector]
    public Transform controller;
    public GenericItem Item;
    /*[HideInInspector]
    public GrabbableItem OtherHand;*/

    public virtual void SetUp(ItemController i, ControllerHand hand)
    {
        if (i != null)
        {
            Hand = hand;
            Player = i;
            if (!setupdone)
            {
                if (Hand == ControllerHand.LeftHand)
                    controller = Player.LeftController;
                else if (Hand == ControllerHand.RightHand)
                    controller = Player.RightController;
                if (Slave != null)
                {
                    Slave.transform.position = controller.position + Slave.transform.position;
                    Slave.transform.rotation = Quaternion.Euler(controller.eulerAngles + Slave.transform.rotation.eulerAngles);
                    Slave.transform.parent = controller;
                }
                else
                {
                    transform.position = controller.position + transform.position;
                    transform.rotation = Quaternion.Euler(controller.eulerAngles + transform.rotation.eulerAngles);
                    transform.parent = controller;
                }
                setupdone = true;
            }
        }
    }

    public virtual void Initialize()
    {
        if (setupdone && !initialized)
            initialized = true;
    }

    /*public void SetHand(GrabbableItem g, ControllerHand hand)
    {
        if (g.TwoHands)
            Hand = ItemHand.Any;
        else
            Hand = ControllerToItemHand(hand);
    }*/

    public virtual bool CanDrop()
    {
        return true;
    }

    /*public static ItemHand ControllerToItemHand(ControllerHand hand)
    {
        if (hand == ControllerHand.LeftHand)
            return ItemHand.Left;
        else if (hand == ControllerHand.RightHand)
            return ItemHand.Right;
        else
            return ItemHand.Any;
    }*/

    public virtual void LoadState(GenericItem i)
    {
        Item = i;
        Slave.gameObject.SetActive(true);
    }

    public virtual void LoadState()
    {
        Slave.gameObject.SetActive(true);
    }

    public virtual void SaveState()
    {
        Item = null;
        Slave.DisableOutline(Player);
        Slave.gameObject.SetActive(false);
    }

    public virtual void Update()
    {
        if (!initialized)
            Initialize();
    }

    // Use this for initialization
    public virtual void Start()
    {
        //Son = transform.GetChild(0);
        Initialize();
    }

    public abstract void ClickButton(object sender, ClickedEventArgs e);

    public abstract void UnClickButton(object sender, ClickedEventArgs e);

    internal void EnableItem()
    {
        ItemEnabled = true;
    }
    internal void DisableItem()
    {
        ItemEnabled = false;
    }
}
