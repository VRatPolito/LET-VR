using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableItemSlave : MonoBehaviour {

    // Use this for initialization
    public GrabbableItem Master;
    [HideInInspector]
    public bool outlined = false;
    public List<Transform> Pieces;

    void Start () {
		
	}
    public ItemCodes Code()
    {
        return Master.Code;
    }
    
    public void SetUp(ItemController i, ControllerHand hand)
    {
        if (Master != null)
            Master.SetUp(i, hand);
    }
 
    public bool CanDrop()
    {
        if (Master != null)
            return Master.CanDrop();
        else
            return false;
    }
    // Update is called once per frame
    void Update () {
		
	}

    public virtual void EnableOutline(ItemController c)
    {
        Material[] mts = null;
       
        var m = GetComponent<MeshRenderer>();
        if (m != null)
            mts = m.materials;

        Color col = Color.yellow;
        col.a = 255;
        if (mts != null)
        {
            foreach (Material ms in mts)
            {
                ms.SetColor("_OutlineColor", col);
            }

        }
        foreach (Transform t in Pieces)
        {
            var mat = t.GetComponent<MeshRenderer>();
            if (mat != null)
                mts = mat.materials;
            foreach (Material ms in mts)
            {
                ms.SetColor("_OutlineColor", col);
            }
        }
        outlined = true;
    }

    public virtual void DisableOutline(ItemController c)
    {
        Material[] mts = null;
       
        var m = GetComponent<MeshRenderer>();
        if (m != null)
            mts = m.materials;

        Color col = Color.yellow;
        col.a = 0;
        if (mts != null)
        {
            foreach (Material ms in mts)
            {
                ms.SetColor("_OutlineColor", col);
            }
        }
        foreach (Transform t in Pieces)
        {
            var mat = t.GetComponent<MeshRenderer>();
            if (mat != null)
                mts = mat.materials;
            foreach (Material ms in mts)
            {
                ms.SetColor("_OutlineColor", col);
            }
        }
        outlined = false;
    }

    internal void Unselect()
    {
        throw new NotImplementedException();
    }


}
