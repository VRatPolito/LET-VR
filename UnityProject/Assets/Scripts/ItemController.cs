﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
public enum ItemControllerType { VR, FPS };
public enum ControllerHand { LeftLeg, RightLeg, LeftHand, RightHand, Invalid };

public class ItemController : MonoBehaviour
{
    public ItemControllerType Type;
    public Transform LeftController, RightController;
    
    [HideInInspector]
    public Transform ItemLeft, ItemRight;
    [HideInInspector]
    public Transform[] ItemsLeft;
    [HideInInspector]
    public Transform[] ItemsRight;
    [HideInInspector]
    public int ItemRightIndex, ItemLeftIndex;
    public AudioSource BlipSource;
    [HideInInspector]
    public AudioSource LeftItemSource, RightItemSource;
    public AudioClip DefaultGrabSound, DefaultDropSound;
    public float DefaultGrabSoundVolume = 1, DefaultDropSoundVolume = 1;
    [HideInInspector]
    public Transform toucheditemleft, toucheditemright;
    [HideInInspector]
    public bool LeftInteracting, RightInteracting = false;
    [HideInInspector]
    public bool rightoperating, leftoperating;
    protected InputManagement _inputManager;

    public InputManagement InputManager => _inputManager;

    // Use this for initialization
    public virtual void Start()
    {
        _inputManager = GetComponent<InputManagement>();
        BlipSource = GetComponent<AudioSource>();
        RightItemSource = RightController.GetComponent<AudioSource>();
        LeftItemSource = LeftController.GetComponent<AudioSource>();
    }

    public bool LeftHandFree()
    {
        return ItemLeftIndex == -1 && ItemLeft == null;
    }
    public bool RightHandFree()
    {
        return ItemRightIndex == -1 && ItemRight == null;
    }

    public GrabbableItem GetCurrentItem(ControllerHand hand)
    {
        if (hand == ControllerHand.LeftHand)
        {
            if (ItemLeftIndex == -1)
                return null;
            else
                return ItemsLeft[ItemLeftIndex].GetComponent<GrabbableItem>();
        }
        else if (hand == ControllerHand.RightHand)
        {
            if (ItemRightIndex == -1)
                return null;
            else
                return ItemsRight[ItemRightIndex].GetComponent<GrabbableItem>();
        }
        else
            return null;
    }

    // Update is called once per frame
    void Update()
    {

    }



    private void LateUpdate()
    {
        Matrix4x4 parentMatrix;
        if (ItemLeft != null)
        {
            var g = ItemLeft.GetComponent<GenericItem>();
            if (g != null)
            {
                if (g.SeekTarget != null)
                {
                    parentMatrix = Matrix4x4.TRS(g.SeekTarget.position, g.SeekTarget.rotation, g.SeekTarget.lossyScale);
                    ItemLeft.transform.position = parentMatrix.MultiplyPoint3x4(g.startChildPosition);
                    ItemLeft.transform.rotation = (g.SeekTarget.rotation * Quaternion.Inverse(g.startParentRotationQ)) * g.startChildRotationQ;
                }

                if (!g.IsKinematic && (g.SeekTarget != null))
                {
                    g.currvelocity = (g.transform.position - g.prevpos) / Time.deltaTime;
                    g.prevpos = g.transform.position;
                }
            }
        }
        if (ItemRight != null)
        {
            var g = ItemRight.GetComponent<GenericItem>();
            if (g != null)
            {
                if (g.SeekTarget != null)
                {
                    parentMatrix = Matrix4x4.TRS(g.SeekTarget.position, g.SeekTarget.rotation, g.SeekTarget.lossyScale);
                    ItemRight.transform.position = parentMatrix.MultiplyPoint3x4(g.startChildPosition);
                    ItemRight.transform.rotation = (g.SeekTarget.rotation * Quaternion.Inverse(g.startParentRotationQ)) * g.startChildRotationQ;
                }

                if (!g.IsKinematic && (g.SeekTarget != null))
                {
                    g.currvelocity = (g.transform.position - g.prevpos) / Time.deltaTime;
                    g.prevpos = g.transform.position;
                }
            }
        }
    }

    public static GrabbableItemSlave GetGrabbableItemSlave(Transform item)
    {
        GrabbableItemSlave g = null;
        if (item != null && item.parent != null)
            g = item.parent.GetComponent<GrabbableItemSlave>();
        return g;
    }

    public virtual void GrabItem(GenericItem i, ControllerHand hand)
    {
        throw new NotImplementedException();
    }
    public virtual void DropItem(ControllerHand hand, bool forced)
    {
        throw new NotImplementedException();
    }
    public void DropItem(Transform item, bool forced)
    {
        if (ItemRight == item)
            DropItem(ControllerHand.RightHand, forced);
        else if (ItemLeft == item)
            DropItem(ControllerHand.LeftHand, forced);
    }


    public void ForceDrop()
    {
        DropItem(ControllerHand.LeftHand, true);
        DropItem(ControllerHand.RightHand, true);
    }
    public void RightDropPressed(object sender, PadEventArgs e)
    {
        RightDropPressed(sender);
    }
    public void RightDropPressed(object sender)
     {
         if (ItemRight != null && !RightInteracting)
         {
             RightInteracting = true;
             DropItem(ControllerHand.RightHand, false);
         }
    }
    public void LeftDropPressed(object sender, PadEventArgs e)
    {
        LeftDropPressed(sender);
    }
    public void LeftDropPressed(object sender)
      {
          if (ItemLeft != null && !LeftInteracting)
          {
              LeftInteracting = true;
              DropItem(ControllerHand.LeftHand, false);
          }
      }
    /*public void RightDropPressed(InputAction.CallbackContext obj)
    {
        if (ItemRight != null && !RightInteracting)
        {
            RightInteracting = true;
            DropItem(ControllerHand.RightHand, false);
        }
    }
    public void LeftDropPressed(InputAction.CallbackContext obj)
    {
        if (ItemLeft != null && !LeftInteracting)
        {
            LeftInteracting = true;
            DropItem(ControllerHand.LeftHand, false);
        }
    }*/
}
