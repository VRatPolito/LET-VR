using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ItemCodes { Generic };

public class GenericItem : MonoBehaviour {

    
    public virtual void Interact(GenericItemSlave slave, ItemController c)
    {
        throw new NotImplementedException();
    }

    public ItemCodes ItemCode = ItemCodes.Generic;
    public bool Grabbable = false;

    public virtual void DisableOutline(GenericItemSlave slave, ItemController c)
    {
        DisableOutline(c);
    }

    public virtual void EnableOutline(GenericItemSlave slave, ItemController c)
    {
        EnableOutline(c);
    }

    public virtual void ClickButton(object sender)
    {
        throw new NotImplementedException();
    }

    public virtual void UnClickButton(object sender)
    {
        throw new NotImplementedException();
    }

    public bool CanInteractFromStart = true;
    bool _CanInteract;
    [HideInInspector]
    public bool outlined = false;
    public bool IsKinematic = false;
    [HideInInspector]
    public Transform InitialParent;
    [HideInInspector]
    public Vector3 prevpos;
    [HideInInspector]
    public Vector3 currvelocity;
    [HideInInspector]
    public Vector3 direction;
    Rigidbody rbody;
    public List<Transform> Pieces;
    [HideInInspector]
    public bool ItemActive = false;
    public enum RotationAxis { X, Y, Z };
    [HideInInspector]
    public Transform SeekTarget;
    public GenericItemSlave Slave;
    internal ItemController Player;
    internal ControllerHand _hand = ControllerHand.Invalid;
    [SerializeField]
    internal bool _grabSound = true;

    [HideInInspector]
    public Vector3 startParentPosition;
    [HideInInspector]
    public Quaternion startParentRotationQ;
    [HideInInspector]
    public Vector3 startChildPosition;
    [HideInInspector]
    public Quaternion startChildRotationQ;

    [Serializable]
    public class InteractEvent : UnityEvent<ItemController, ControllerHand> { }

    public InteractEvent OnInteract, OnGrab, OnDrop;

    // Use this for initialization
    public virtual void Start()
    {
        if(Slave != null)
        {
            Slave.Master = this;
            prevpos = Slave.transform.position;
            InitialParent = Slave.transform.parent;
            rbody = Slave.GetComponent<Rigidbody>();
        }
        else
        {
            prevpos = transform.position;
            InitialParent = transform.parent;
            rbody = GetComponent<Rigidbody>();
        }
        if (CanInteractFromStart)
            CanInteract(true, null);
        else
            CanInteract(false, null);
    }

    // Update is called once per frame
    public virtual void Update () {
	}

    public virtual void EnablePhysics()
    {
        if (!IsKinematic && rbody != null)
        {
            rbody.isKinematic = false;
            rbody.velocity = currvelocity;
        }
    }


    public virtual void DisablePhysics()
    {
        if (!IsKinematic && rbody != null)
            rbody.isKinematic = true;
    }

    public virtual bool CanInteract(ItemController c)
    {
        return _CanInteract;
    }

    public virtual void CanInteract(bool can, ItemController c)
    {
        _CanInteract = can;
        if (!can && outlined)
            DisableOutline(c);
    }

    public virtual void EnableOutline(ItemController c)
    {
        Material[] mts = null;
        if (Slave == null)
        {
            var m = GetComponent<MeshRenderer>();
            if (m != null)
                mts = m.materials;
        }

        else
        {
            var m = Slave.GetComponent<MeshRenderer>();
            if(m != null)
                mts = m.materials;
        }

        Color col;
        if (mts != null)
        {
            foreach (Material m in mts)
            {
                if (Grabbable)
                    col = Color.yellow;
                else
                    col = Color.white;
                col.a = 255;
                m.SetColor("_OutlineColor", col);
            }
        }

        foreach (Transform t in Pieces)
        {
            var mat = t.GetComponent<MeshRenderer>();
            if (mat != null)
                mts = mat.materials;
            foreach (Material m in mts)
            {
                if (Grabbable)
                    col = Color.yellow;
                else
                    col = Color.white;
                col.a = 255;
                m.SetColor("_OutlineColor", col);
            }
        }
        outlined = true;
    }

    public virtual void DisableOutline(ItemController c)
    {
        Material[] mts = null;
        if (Slave == null)
        {
            var m = GetComponent<MeshRenderer>();
            if (m != null)
                mts = m.materials;
        }

        else
        {
            var m = Slave.GetComponent<MeshRenderer>();
            if (m != null)
                mts = m.materials;
        }

        Color col;
        if (mts != null)
        {
            foreach (Material m in mts)
            {
                if (Grabbable)
                    col = Color.yellow;
                else
                    col = Color.white;
                col.a = 0;
                m.SetColor("_OutlineColor", col);
            }
        }
        foreach (Transform t in Pieces)
        {
            var mat = t.GetComponent<MeshRenderer>();
            if (mat != null)
                mts = mat.materials;
            foreach (Material m in mts)
            {
                if (Grabbable)
                    col = Color.yellow;
                else
                    col = Color.white;
                col.a = 0;
                m.SetColor("_OutlineColor", col);
            }
        }
        outlined = false;
    }

    public virtual void ClickButton(object sender, ClickedEventArgs e) { }

    public virtual void UnClickButton(object sender, ClickedEventArgs e) { }

    public virtual void Interact(ItemController c) { OnInteract?.Invoke(c, ControllerHand.Invalid); }

    public void DisableItem(Transform controller)
    {
        if (Slave != null)
        {
            if (Slave.gameObject.activeSelf)
            {
                Slave.gameObject.SetActive(false);
                var tcontr = controller.GetComponent<SteamVR_TrackedController>();
                if (tcontr != null)
                {
                    tcontr.PadClicked += ClickButton;
                    tcontr.GetComponent<SteamVR_TrackedController>().PadUnclicked += UnClickButton;
                }
                //SeekTarget = controller;
                //transform.parent = controller;
                return;
            }

        }
        else
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                var tcontr = controller.GetComponent<SteamVR_TrackedController>();
                if (tcontr != null)
                {
                    tcontr.PadClicked += ClickButton;
                    tcontr.GetComponent<SteamVR_TrackedController>().PadUnclicked += UnClickButton;
                }
                //SeekTarget = controller;
                //transform.parent = controller;
                return;
            }
        }
    }

    public void EnableItem(Transform controller)
    {
        if (Slave != null)
        {
            if (!Slave.gameObject.activeSelf)
            {
                Slave.gameObject.SetActive(true);
                var tcontr = controller.GetComponent<SteamVR_TrackedController>();
                if (tcontr != null)
                {
                    tcontr.GetComponent<SteamVR_TrackedController>().PadClicked -= ClickButton;
                    tcontr.GetComponent<SteamVR_TrackedController>().PadUnclicked -= UnClickButton;
                }
                DropParent();
                return;
            }
        }
        else
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                var tcontr = controller.GetComponent<SteamVR_TrackedController>();
                if (tcontr != null)
                {
                    tcontr.GetComponent<SteamVR_TrackedController>().PadClicked -= ClickButton;
                    tcontr.GetComponent<SteamVR_TrackedController>().PadUnclicked -= UnClickButton;
                }
                DropParent();
                return;
            }
        }
    }

    public virtual void ForceParent(Transform p, bool KeepOrientation)
    {
        SeekTarget = p;
        //if (type != ItemControllerType.FPS)
        if (KeepOrientation)
        {
            startParentPosition = p.position;
            startParentRotationQ = p.rotation;

            startChildPosition = transform.position;
            startChildRotationQ = transform.rotation;

            startChildPosition = DivideVectors(Quaternion.Inverse(p.rotation) * (startChildPosition - startParentPosition), p.lossyScale);
        }
        else
        {
            startParentPosition = Vector3.zero;
            startParentRotationQ = Quaternion.identity;

            startChildPosition = Vector3.zero;
            startChildRotationQ = Quaternion.identity;
        }
        //transform.parent = p;
    }
    Vector3 DivideVectors(Vector3 num, Vector3 den)
    {

        return new Vector3(num.x / den.x, num.y / den.y, num.z / den.z);

    }
    public virtual void DropParent()
    {
        SeekTarget = null;
        startParentPosition = Vector3.zero;
        startParentRotationQ = Quaternion.identity;

        startChildPosition = Vector3.zero;
        startChildRotationQ = Quaternion.identity;
        //transform.parent = null;
    }

    public virtual void Reset()
    {
        if (CanInteractFromStart)
            CanInteract(true, null);
        else
            CanInteract(false, null);
        DisableOutline(null);
    }

    internal void Grab(ItemController player, ControllerHand hand)
    {
        if (Player != null)
            Player.DropItem(transform, true);
        Player = player;
        _hand = hand;
        DisablePhysics();
    }

    internal void SignalGrab()
    {
        OnGrab?.Invoke(Player, _hand);
    }

    internal void Drop()
    {
        Player = null;
        _hand = ControllerHand.Invalid;
    }

    internal void SignalDrop(ItemController player, ControllerHand hand)
    {
        OnDrop?.Invoke(player, hand);
    }
}
