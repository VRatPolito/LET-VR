using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControllerButton { Nothing, Trigger, RGrip, LGrip, Grip, Pad, All };
public enum ControllerButtonInput { Trigger, RGrip, LGrip, Grip, Pad, All, Any };
public enum ControllerButtonByFunc { ButtonToInteract, ButtonToDrop, ButtonToUse, All };
public enum GrabMode { HoldButton, ClickButton };

public class VRItemController : ItemController
{
    public event Action OnGrab, OnDrop;
    [HideInInspector]
    public SteamVR_TrackedController LeftVRController, RightVRController;
    Material oldmat;
    public Shader Outline;
    float t;
    bool up = true;
    ControllerButton LeftButtonToPulse = ControllerButton.Nothing, RightButtonToPulse = ControllerButton.Nothing, ExternalLeftButtonToPulse = ControllerButton.Nothing, ExternalRightButtonToPulse = ControllerButton.Nothing;
    public ControllerButtonInput ButtonToInteract = ControllerButtonInput.Trigger;
    public ControllerButtonInput ButtonToDrop = ControllerButtonInput.Trigger;
    public ControllerButtonInput ButtonToUse = ControllerButtonInput.Pad;
    public GrabMode GrabbingMethod = GrabMode.ClickButton;
    List<MeshRenderer> LGrips = new List<MeshRenderer>();
    List<MeshRenderer> LPads = new List<MeshRenderer>();
    List<MeshRenderer> LTriggers = new List<MeshRenderer>();
    List<MeshRenderer> RGrips = new List<MeshRenderer>();
    List<MeshRenderer> RPads = new List<MeshRenderer>();
    List<MeshRenderer> RTriggers = new List<MeshRenderer>();
    Material PadMaterial, GripMaterial, TriggerMaterial;
    bool leftpulsing, rightpulsing;
    public float PulseTime = 0.5f;
    public Color PadColor = Color.yellow;
    public Color TriggerColor = Color.magenta;
    public Color GripColor = Color.cyan;
    [HideInInspector]
    public VibrationController LeftVibrationController, RightVibrationController;


    private void Awake()
    {
        Type = ItemControllerType.VR;
    }
    // Use this for initialization
    public override void Start()
    {
        base.Start();
        LeftVibrationController = LeftController.GetComponent<VibrationController>();
        RightVibrationController = RightController.GetComponent<VibrationController>();
        LeftVRController = LeftController.GetComponent<SteamVR_TrackedController>();
        RightVRController = RightController.GetComponent<SteamVR_TrackedController>();

     
        switch (ButtonToInteract)
        {
            case ControllerButtonInput.Pad:
                LeftVRController.PadClicked += LeftInteract;
                RightVRController.PadClicked += RightInteract;
                break;
            case ControllerButtonInput.Grip:
                LeftVRController.Gripped += LeftInteract;
                RightVRController.Gripped += RightInteract;
                break;
            case ControllerButtonInput.Any:
                LeftVRController.PadClicked += LeftInteract;
                LeftVRController.Gripped += LeftInteract;
                LeftVRController.TriggerClicked += LeftInteract;
                RightVRController.PadClicked += RightInteract;
                RightVRController.Gripped += RightInteract;
                RightVRController.TriggerClicked += RightInteract;
                break;
            default:
                LeftVRController.TriggerClicked += LeftInteract;
                RightVRController.TriggerClicked += RightInteract;
                break;
        }

        if (GrabbingMethod == GrabMode.HoldButton)
        {
            switch (ButtonToInteract)
            {
                case ControllerButtonInput.Pad:
                    LeftVRController.PadUnclicked += LeftDropPressed;
                    RightVRController.PadUnclicked += RightDropPressed;
                    break;
                case ControllerButtonInput.Grip:
                    LeftVRController.Ungripped += LeftDropPressed;
                    RightVRController.Ungripped += RightDropPressed;
                    break;
                default:
                    LeftVRController.TriggerUnclicked += LeftDropPressed;
                    RightVRController.TriggerUnclicked += RightDropPressed;
                    break;
            }
        }
    }

    private void OnEnable()
    {
        var c = GetComponents<ItemController>();
        foreach (ItemController i in c)
            if (i != this)
                i.enabled = false; 
    }
    // Update is called once per frame
    void Update()
    {
        PulseButton();
    }

    void StartPulse(ControllerButton button)
    {
        StartPulse(button, ControllerHand.LeftHand);
        StartPulse(button, ControllerHand.RightHand);
    }

    void StartPulse(ControllerButton button, ControllerHand hand)
    {
        if (hand == ControllerHand.RightHand)
        {
            if (button == ControllerButton.Nothing)
                return;
            if (!rightpulsing)
            {
                RightButtonToPulse = button;
                StartPulse(hand);
            }
            else if (button != RightButtonToPulse)
            {
                StopPulse(hand);
                RightButtonToPulse = button;
                StartPulse(hand);
            }
        }
        else if (hand == ControllerHand.LeftHand)
        {
            if (button == ControllerButton.Nothing)
                return;
            if (!leftpulsing)
            {
                LeftButtonToPulse = button;
                StartPulse(hand);
            }
            else if (button != LeftButtonToPulse)
            {
                StopPulse(hand);
                LeftButtonToPulse = button;
                StartPulse(hand);
            }
        }
    }

    void StartPulse()
    {
        StartPulse(ControllerHand.LeftHand);
        StartPulse(ControllerHand.RightHand);
    }

    void StartPulse(ControllerHand hand)
    {
        if (hand == ControllerHand.LeftHand)
        {
            if (leftpulsing || (LeftButtonToPulse == ControllerButton.Nothing && ExternalLeftButtonToPulse == ControllerButton.Nothing))
                return;

            MeshRenderer r = null;
            ControllerButton button = ControllerButton.Nothing;
            if (LeftButtonToPulse != ControllerButton.Nothing)
                button = LeftButtonToPulse;
            else
                button = ExternalLeftButtonToPulse;

            var c = LeftController.Find("Model");
            Transform t = null;
            switch (button)
            {
                case ControllerButton.RGrip:
                    t = c.Find("rgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            LGrips.Add(r);
                    }
                    break;
                case ControllerButton.LGrip:
                    t = c.Find("lgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            LGrips.Add(r);
                    }
                    break;
                case ControllerButton.Grip:
                    t = c.Find("rgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            LGrips.Add(r);
                    }
                    t = c.Find("lgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            LGrips.Add(r);
                    }
                    t = c.Find("handgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            LGrips.Add(r);
                    }
                    break;
                case ControllerButton.Pad:
                    t = c.Find("trackpad");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            LPads.Add(r);
                    }
                    else
                    {
                        t = c.Find("thumbstick");
                        if (t != null)
                        {
                            r = t.GetComponent<MeshRenderer>();
                            if (r != null)
                                LPads.Add(r);
                        }
                    }
                    break;
                case ControllerButton.Trigger:
                    t = c.Find("trigger");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            LTriggers.Add(r);
                    }
                    break;
                case ControllerButton.All:

                    t = c.Find("trackpad");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            LPads.Add(r);
                    }
                    else
                    {
                        t = c.Find("thumbstick");
                        if (t != null)
                        {
                            r = t.GetComponent<MeshRenderer>();
                            if (r != null)
                                LPads.Add(r);
                        }
                    }
                    t = c.Find("trigger");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            LTriggers.Add(r);
                    }
                    t = c.Find("rgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            LGrips.Add(r);
                    }
                    t = c.Find("lgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            LGrips.Add(r);
                    }
                    t = c.Find("handgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            LGrips.Add(r);
                    }
                    break;
                default:
                    return;
            }
            if (oldmat == null)
            {
                if (LTriggers.Count > 0)
                    oldmat = LTriggers[0].materials[0];
                else if (LPads.Count > 0)
                    oldmat = LPads[0].materials[0];
                else if (LGrips.Count > 0)
                    oldmat = LGrips[0].materials[0];
                else
                {
                    var model = transform.Find("Model");
                    if (model != null)
                    {
                        var mr = model.GetComponent<MeshRenderer>();
                        if (mr != null)
                            oldmat = mr.materials[0];
                    }
                }
            }
            if (oldmat != null)
            {
                if (PadMaterial == null)
                {
                    PadMaterial = new Material(Outline);
                    PadMaterial.SetTexture("_MainTex", oldmat.GetTexture("_MainTex"));
                    PadMaterial.SetColor("_OutlineColor", PadColor);
                    PadMaterial.SetFloat("_Outline", 0.03f);
                }
                if (GripMaterial == null)
                {
                    GripMaterial = new Material(Outline);
                    GripMaterial.SetTexture("_MainTex", oldmat.GetTexture("_MainTex"));
                    GripMaterial.SetColor("_OutlineColor", GripColor);
                    GripMaterial.SetFloat("_Outline", 0.03f);
                }
                if (TriggerMaterial == null)
                {
                    TriggerMaterial = new Material(Outline);
                    TriggerMaterial.SetTexture("_MainTex", oldmat.GetTexture("_MainTex"));
                    TriggerMaterial.SetColor("_OutlineColor", TriggerColor);
                    TriggerMaterial.SetFloat("_Outline", 0.03f);
                }
                foreach (MeshRenderer mr in LPads)
                {
                    var m = new Material[1];
                    m[0] = PadMaterial;
                    mr.materials = m;
                }
                foreach (MeshRenderer mr in LTriggers)
                {
                    var m = new Material[1];
                    m[0] = TriggerMaterial;
                    mr.materials = m;
                }
                foreach (MeshRenderer mr in LGrips)
                {
                    var m = new Material[1];
                    m[0] = GripMaterial;
                    mr.materials = m;
                }
            }
            leftpulsing = true;
        }
        else if (hand == ControllerHand.RightHand)
        {
            if (rightpulsing || (RightButtonToPulse == ControllerButton.Nothing && ExternalRightButtonToPulse == ControllerButton.Nothing))
                return;

            MeshRenderer r = null;
            ControllerButton button = ControllerButton.Nothing;
            if (RightButtonToPulse != ControllerButton.Nothing)
                button = RightButtonToPulse;
            else
                button = ExternalRightButtonToPulse;

            var c = RightController.Find("Model");
            Transform t = null;
            switch (button)
            {
                case ControllerButton.RGrip:
                    t = c.Find("rgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            RGrips.Add(r);
                    }
                    break;
                case ControllerButton.LGrip:
                    t = c.Find("lgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            RGrips.Add(r);
                    }
                    break;
                case ControllerButton.Grip:
                    t = c.Find("rgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            RGrips.Add(r);
                    }
                    t = c.Find("lgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            RGrips.Add(r);
                    }
                    t = c.Find("handgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            RGrips.Add(r);
                    }
                    break;
                case ControllerButton.Pad:
                    t = c.Find("trackpad");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            RPads.Add(r);
                    }
                    else
                    {
                        t = c.Find("thumbstick");
                        if (t != null)
                        {
                            r = t.GetComponent<MeshRenderer>();
                            if (r != null)
                                if (r != null)
                                    RPads.Add(r);
                        }
                    }
                    break;
                case ControllerButton.Trigger:
                    t = c.Find("trigger");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            RTriggers.Add(r);
                    }
                    break;
                case ControllerButton.All:

                    t = c.Find("trackpad");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            RPads.Add(r);
                    }
                    else
                    {
                        t = c.Find("thumbstick");
                        if (t != null)
                        {
                            r = t.GetComponent<MeshRenderer>();
                            if (r != null)
                                RPads.Add(r);
                        }
                    }
                    t = c.Find("trigger");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            RTriggers.Add(r);
                    }
                    t = c.Find("rgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            RGrips.Add(r);
                    }
                    t = c.Find("lgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            RGrips.Add(r);
                    }
                    t = c.Find("handgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if (r != null)
                            RGrips.Add(r);
                    }
                    break;
                default:
                    return;
            }
            if (oldmat == null)
            {
                if (RTriggers.Count > 0)
                    oldmat = RTriggers[0].materials[0];
                else if (RPads.Count > 0)
                    oldmat = RPads[0].materials[0];
                else if (RGrips.Count > 0)
                    oldmat = RGrips[0].materials[0];
                else
                {
                    var model = transform.Find("Model");
                    if (model != null)
                    {
                        var mr = model.GetComponent<MeshRenderer>();
                        if (mr != null)
                            oldmat = mr.materials[0];
                    }
                }
            }
            if (oldmat != null)
            {
                if (PadMaterial == null)
                {
                    PadMaterial = new Material(Outline);
                    PadMaterial.SetTexture("_MainTex", oldmat.GetTexture("_MainTex"));
                    PadMaterial.SetColor("_OutlineColor", PadColor);
                    PadMaterial.SetFloat("_Outline", 0.03f);
                }
                if (GripMaterial == null)
                {
                    GripMaterial = new Material(Outline);
                    GripMaterial.SetTexture("_MainTex", oldmat.GetTexture("_MainTex"));
                    GripMaterial.SetColor("_OutlineColor", GripColor);
                    GripMaterial.SetFloat("_Outline", 0.03f);
                }
                if (TriggerMaterial == null)
                {
                    TriggerMaterial = new Material(Outline);
                    TriggerMaterial.SetTexture("_MainTex", oldmat.GetTexture("_MainTex"));
                    TriggerMaterial.SetColor("_OutlineColor", TriggerColor);
                    TriggerMaterial.SetFloat("_Outline", 0.03f);
                }
                foreach (MeshRenderer mr in RPads)
                {
                    var m = new Material[1];
                    m[0] = PadMaterial;
                    mr.materials = m;
                }
                foreach (MeshRenderer mr in RTriggers)
                {
                    var m = new Material[1];
                    m[0] = TriggerMaterial;
                    mr.materials = m;
                }
                foreach (MeshRenderer mr in RGrips)
                {
                    var m = new Material[1];
                    m[0] = GripMaterial;
                    mr.materials = m;
                }
            }
            rightpulsing = true;
        }
    }

    void StopPulse()
    {
        StopPulse(ControllerHand.LeftHand);
        StopPulse(ControllerHand.RightHand);
    }

    void StopPulse(ControllerHand hand)
    {
        if (hand == ControllerHand.LeftHand)
        {
            if (!leftpulsing)
                return;
            if (ExternalLeftButtonToPulse != ControllerButton.Nothing && ExternalLeftButtonToPulse == LeftButtonToPulse)
            {
                LeftButtonToPulse = ControllerButton.Nothing;
                return;
            }

            foreach (MeshRenderer mr in LTriggers)
            {
                var m = new Material[1];
                m[0] = oldmat;
                mr.materials = m;
            }
            foreach (MeshRenderer mr in LGrips)
            {
                var m = new Material[1];
                m[0] = oldmat;
                mr.materials = m;
            }
            foreach (MeshRenderer mr in LPads)
            {
                var m = new Material[1];
                m[0] = oldmat;
                mr.materials = m;
            }
            LTriggers.Clear();
            LPads.Clear();
            LGrips.Clear();
            LeftButtonToPulse = ControllerButton.Nothing;
            leftpulsing = false;
            if (ExternalLeftButtonToPulse != ControllerButton.Nothing)
                StartPulse(ControllerHand.LeftHand);
        }
        else if (hand == ControllerHand.RightHand)
        {
            if (!rightpulsing)
                return;
            if (ExternalRightButtonToPulse != ControllerButton.Nothing && ExternalRightButtonToPulse == RightButtonToPulse)
            {
                RightButtonToPulse = ControllerButton.Nothing;
                return;
            }

            foreach (MeshRenderer mr in RTriggers)
            {
                var m = new Material[1];
                m[0] = oldmat;
                mr.materials = m;
            }
            foreach (MeshRenderer mr in RGrips)
            {
                var m = new Material[1];
                m[0] = oldmat;
                mr.materials = m;
            }
            foreach (MeshRenderer mr in RPads)
            {
                var m = new Material[1];
                m[0] = oldmat;
                mr.materials = m;
            }
            RTriggers.Clear();
            RPads.Clear();
            RGrips.Clear();
            RightButtonToPulse = ControllerButton.Nothing;
            rightpulsing = false;
            if (ExternalRightButtonToPulse != ControllerButton.Nothing)
                StartPulse(ControllerHand.RightHand);
        }
    }



    void PulseButton()
    {
        if (LeftButtonToPulse != ControllerButton.Nothing || RightButtonToPulse != ControllerButton.Nothing || ExternalLeftButtonToPulse != ControllerButton.Nothing || ExternalRightButtonToPulse != ControllerButton.Nothing)
        {
            t += Time.deltaTime / PulseTime;

            if (leftpulsing)
            {
                foreach (MeshRenderer mr in LGrips)
                {
                    if (up)
                    {
                        var m1 = mr.materials[0];
                        var c = m1.GetColor("_OutlineColor");
                        m1.SetColor("_OutlineColor", new Color(c.r, c.g, c.b, Mathf.Lerp(0, 1, t)));
                        var m = new Material[1];
                        m[0] = m1;
                        mr.materials = m;
                    }
                    else
                    {
                        var m1 = mr.materials[0];
                        var c = m1.GetColor("_OutlineColor");
                        m1.SetColor("_OutlineColor", new Color(c.r, c.g, c.b, Mathf.Lerp(1, 0, t)));
                        var m = new Material[1];
                        m[0] = m1;
                        mr.materials = m;
                    }
                }
                foreach (MeshRenderer mr in LTriggers)
                {
                    if (up)
                    {
                        var m1 = mr.materials[0];
                        var c = m1.GetColor("_OutlineColor");
                        m1.SetColor("_OutlineColor", new Color(c.r, c.g, c.b, Mathf.Lerp(0, 1, t)));
                        var m = new Material[1];
                        m[0] = m1;
                        mr.materials = m;
                    }
                    else
                    {
                        var m1 = mr.materials[0];
                        var c = m1.GetColor("_OutlineColor");
                        m1.SetColor("_OutlineColor", new Color(c.r, c.g, c.b, Mathf.Lerp(1, 0, t)));
                        var m = new Material[1];
                        m[0] = m1;
                        mr.materials = m;
                    }
                }
                foreach (MeshRenderer mr in LPads)
                {
                    if (up)
                    {
                        var m1 = mr.materials[0];
                        var c = m1.GetColor("_OutlineColor");
                        m1.SetColor("_OutlineColor", new Color(c.r, c.g, c.b, Mathf.Lerp(0, 1, t)));
                        var m = new Material[1];
                        m[0] = m1;
                        mr.materials = m;
                    }
                    else
                    {
                        var m1 = mr.materials[0];
                        var c = m1.GetColor("_OutlineColor");
                        m1.SetColor("_OutlineColor", new Color(c.r, c.g, c.b, Mathf.Lerp(1, 0, t)));
                        var m = new Material[1];
                        m[0] = m1;
                        mr.materials = m;
                    }
                }
            }
            if (rightpulsing)
            {
                foreach (MeshRenderer mr in RGrips)
                {
                    if (up)
                    {
                        var m1 = mr.materials[0];
                        var c = m1.GetColor("_OutlineColor");
                        m1.SetColor("_OutlineColor", new Color(c.r, c.g, c.b, Mathf.Lerp(0, 1, t)));
                        var m = new Material[1];
                        m[0] = m1;
                        mr.materials = m;
                    }
                    else
                    {
                        var m1 = mr.materials[0];
                        var c = m1.GetColor("_OutlineColor");
                        m1.SetColor("_OutlineColor", new Color(c.r, c.g, c.b, Mathf.Lerp(1, 0, t)));
                        var m = new Material[1];
                        m[0] = m1;
                        mr.materials = m;
                    }
                }
                foreach (MeshRenderer mr in RTriggers)
                {
                    if (up)
                    {
                        var m1 = mr.materials[0];
                        var c = m1.GetColor("_OutlineColor");
                        m1.SetColor("_OutlineColor", new Color(c.r, c.g, c.b, Mathf.Lerp(0, 1, t)));
                        var m = new Material[1];
                        m[0] = m1;
                        mr.materials = m;
                    }
                    else
                    {
                        var m1 = mr.materials[0];
                        var c = m1.GetColor("_OutlineColor");
                        m1.SetColor("_OutlineColor", new Color(c.r, c.g, c.b, Mathf.Lerp(1, 0, t)));
                        var m = new Material[1];
                        m[0] = m1;
                        mr.materials = m;
                    }
                }
                foreach (MeshRenderer mr in RPads)
                {
                    if (up)
                    {
                        var m1 = mr.materials[0];
                        var c = m1.GetColor("_OutlineColor");
                        m1.SetColor("_OutlineColor", new Color(c.r, c.g, c.b, Mathf.Lerp(0, 1, t)));
                        var m = new Material[1];
                        m[0] = m1;
                        mr.materials = m;
                    }
                    else
                    {
                        var m1 = mr.materials[0];
                        var c = m1.GetColor("_OutlineColor");
                        m1.SetColor("_OutlineColor", new Color(c.r, c.g, c.b, Mathf.Lerp(1, 0, t)));
                        var m = new Material[1];
                        m[0] = m1;
                        mr.materials = m;
                    }
                }
            }
            if (t >= 1)
            {
                up = true;
                t = 0;
            }
        }
    }

    public void ManageTriggerEnter(Collider other, ControllerHand hand)
    {
        if (other == null)
            return;
        var o = other.transform;
        if (hand == ControllerHand.LeftHand && ItemLeft == null)
        {
            if (o == toucheditemleft)
                return;
            if (o.tag == "Item" && toucheditemleft == null && o != toucheditemright)
            {
                var g = o.GetComponent<GenericItem>();
                if (g != null)
                    VRInteract(g, hand);
                else
                {
                    var gs = o.GetComponent<GenericItemSlave>();
                    if (gs != null)
                        VRInteract(gs, hand);
                    else
                    {
                        var gbs = GetGrabbableItemSlave(o);
                        if (gbs != null)
                            VRInteract(gbs, hand);
                        else
                            Debug.Log("No usable component on Item-tagged object: " + o.name);
                    }
                }
            }
        }
        else if (hand == ControllerHand.RightHand && ItemRight == null)
        {
            if (o.tag == "Item" && toucheditemright == null && o != toucheditemleft)
            {
                var g = o.GetComponent<GenericItem>();
                if (g != null)
                    VRInteract(g, hand);
                else
                {
                    var gs = o.GetComponent<GenericItemSlave>();
                    if (gs != null)
                        VRInteract(gs, hand);
                    else
                    {
                        var gbs = GetGrabbableItemSlave(o);
                        if (gbs != null)
                            VRInteract(gbs, hand);
                        else
                            Debug.Log("No usable component on Item-tagged object: " + o.name);
                    }
                }
            }
        }
    }

    private void VRInteract(GrabbableItemSlave gbs, ControllerHand hand)
    {
        if (gbs != null)
        {
            if (hand == ControllerHand.LeftHand && toucheditemleft == null)
            {
                toucheditemleft = gbs.transform.GetChild(0);
                gbs.EnableOutline(this);
                ShowCanInteractVR(hand);
            }
            else if (hand == ControllerHand.RightHand && toucheditemright == null)
            {
                toucheditemright = gbs.transform.GetChild(0);
                gbs.EnableOutline(this);
                ShowCanInteractVR(hand);
            }
        }
    }

    private void VRInteract(GenericItemSlave gs, ControllerHand hand)
    {
        if (gs != null && gs.Master.CanInteract(this))
        {
            if (hand == ControllerHand.LeftHand && toucheditemleft == null)
            {
                toucheditemleft = gs.transform;
                gs.EnableOutline(this);
                ShowCanInteractVR(hand);
            }
            else if (hand == ControllerHand.RightHand && toucheditemright == null)
            {
                toucheditemright = gs.transform;
                gs.EnableOutline(this);
                ShowCanInteractVR(hand);
            }
        }
    }

    private void VRInteract(GenericItem g, ControllerHand hand)
    {
        if (g != null && g.CanInteract(this))
        {
            if (hand == ControllerHand.LeftHand && toucheditemleft == null)
            {
                toucheditemleft = g.transform;
                g.EnableOutline(this);
                ShowCanInteractVR(hand);
            }
            else if (hand == ControllerHand.RightHand && toucheditemright == null)
            {
                toucheditemright = g.transform;
                g.EnableOutline(this);
                ShowCanInteractVR(hand);
            }
        }
    }

    private void ShowCanInteractVR(ControllerHand hand)
    {
        if (hand == ControllerHand.LeftHand)
        {
            switch (ButtonToInteract)
            {
                case ControllerButtonInput.Pad:
                    StartPulse(ControllerButton.Pad, hand);
                    break;
                case ControllerButtonInput.Grip:
                    StartPulse(ControllerButton.Grip, hand);
                    break;
                case ControllerButtonInput.Any:
                    StartPulse(ControllerButton.All, hand);
                    break;
                default:
                    StartPulse(ControllerButton.Trigger, hand);
                    break;
            }
            LeftVibrationController.ShortVibration();
        }
        else if (hand == ControllerHand.RightHand)
        {
            switch (ButtonToInteract)
            {
                case ControllerButtonInput.Pad:
                    StartPulse(ControllerButton.Pad, hand);
                    break;
                case ControllerButtonInput.Grip:
                    StartPulse(ControllerButton.Grip, hand);
                    break;
                case ControllerButtonInput.Any:
                    StartPulse(ControllerButton.All, hand);
                    break;
                default:
                    StartPulse(ControllerButton.Trigger, hand);
                    break;
            }
            RightVibrationController.ShortVibration();
        }
    }

    public void ManageTriggerExit(Collider other, ControllerHand hand)
    {
        if (other == null)
            return;
        var o = other.transform;
        if (hand == ControllerHand.LeftHand)
        {
            if (o.tag == "Item" && toucheditemleft == o)
            {
                GenericItem g = o.GetComponent<GenericItem>();
                if (g != null)
                    g.DisableOutline(this);
                else
                {
                    GenericItemSlave gs = o.GetComponent<GenericItemSlave>();
                    if (gs != null)
                        gs.Master.DisableOutline(this);
                    else
                    {
                        GrabbableItemSlave gbs = GetGrabbableItemSlave(o);
                        if (gbs != null)
                            gbs.DisableOutline(this);
                    }
                }
            }
            StopPulse(hand);
            toucheditemleft = null;
        }
        else if (hand == ControllerHand.RightHand)
        {
            if (o.tag == "Item" && toucheditemright == o)
            {
                GenericItem g = o.GetComponent<GenericItem>();
                if (g != null)
                    g.DisableOutline(this);
                else
                {
                    GenericItemSlave gs = o.GetComponent<GenericItemSlave>();
                    if (gs != null)
                        gs.Master.DisableOutline(this);
                    else
                    {
                        GrabbableItemSlave gbs = GetGrabbableItemSlave(o);
                        if (gbs != null)
                            gbs.DisableOutline(this);
                    }
                }
            }
            StopPulse(hand);
            toucheditemright = null;
        }
    }

    private void LeftInteract(object sender, ClickedEventArgs e)
    {
        if (!leftoperating)
        {
            leftoperating = true;
            if (GrabbingMethod == GrabMode.ClickButton && ItemLeft != null && !LeftInteracting)
            {
                LeftInteracting = true;
                leftoperating = false;
                DropItem(ControllerHand.LeftHand, false);
                return;
            }
            else if (toucheditemleft != null)
            {
                if (ItemLeft == null && !LeftInteracting)
                {
                    LeftInteracting = true;
                    var g = toucheditemleft.GetComponent<GenericItem>();
                    if (g != null)
                    {
                        if (g.Grabbable)
                            GrabItem(g, ControllerHand.LeftHand);
                        else
                        {
                            ClickItem(g, ControllerHand.LeftHand);
                            LeftInteracting = false;
                        }
                    }
                    else
                    {
                        var gs = toucheditemleft.GetComponent<GenericItemSlave>();
                        if (gs != null)
                        {
                            if (gs.Master.Grabbable)
                                GrabItem(gs.Master, ControllerHand.LeftHand);
                            else
                            {
                                ClickItem(gs, ControllerHand.LeftHand);
                                LeftInteracting = false;
                            }
                        }
                        else
                        {
                            GrabbableItemSlave gbs = GetGrabbableItemSlave(toucheditemleft);
                            if (gbs != null)
                                GrabItem(gbs.Master, ControllerHand.LeftHand);
                        }
                    }
                    LeftInteracting = false;
                }
            }
            leftoperating = false;
        }
    }

    private void RightInteract(object sender, ClickedEventArgs e)
    {
        if (!rightoperating)
        {
            rightoperating = true;
            if (GrabbingMethod == GrabMode.ClickButton && ItemRight != null && !RightInteracting)
            {
                RightInteracting = true;
                rightoperating = false;
                DropItem(ControllerHand.RightHand, false);
                return;
            }
            else if (toucheditemright != null)
            {
                if (ItemRight == null && !RightInteracting)
                {
                    RightInteracting = true;
                    var g = toucheditemright.GetComponent<GenericItem>();
                    if (g != null)
                    {
                        if (g.Grabbable)
                            GrabItem(g, ControllerHand.RightHand);
                        else
                        {
                            ClickItem(g, ControllerHand.RightHand);
                            RightInteracting = false;
                        }
                    }
                    else
                    {
                        var gs = toucheditemright.GetComponent<GenericItemSlave>();
                        if (gs != null)
                        {
                            if (gs.Master.Grabbable)
                                GrabItem(gs.Master, ControllerHand.RightHand);
                            else
                            {
                                ClickItem(gs, ControllerHand.RightHand);
                                RightInteracting = false;
                            }
                        }
                        else
                        {
                            GrabbableItemSlave gbs = GetGrabbableItemSlave(toucheditemright);
                            if (gbs != null)
                                GrabItem(gbs.Master, ControllerHand.RightHand);
                        }
                    }
                    RightInteracting = false;
                }
            }
            rightoperating = false;
        }
    }
    private void GrabItem(GrabbableItem i, ControllerHand hand)
    {
        StopPulse(hand);
        var g = i.Item;
        if (g.Slave != null)
            i.Player.DropItem(g.Slave.transform, true);
        else
            i.Player.DropItem(g.transform, true);
        if (hand == ControllerHand.LeftHand)
            toucheditemleft = g.transform;
        else if (hand == ControllerHand.RightHand)
            toucheditemright = g.transform;
        GrabItem(g, hand);
    }
    public override void GrabItem(GenericItem g, ControllerHand hand)
    {
        g.Interact(this);
        StopPulse(hand);
        if (g.ItemCode != ItemCodes.Generic)
        {
            g.Player = this;
            g.DisablePhysics();
            if (hand == ControllerHand.LeftHand)
            {
                g.DisableItem(LeftController);
                GrabbableItem gb = null;
                gb = EnableItem(g.ItemCode, g, hand);
                g.ForceParent(gb.Slave.transform.GetChild(0), false);
                LeftItemSource.clip = gb.Grab;
                LeftItemSource.volume = gb.GrabVolume;
                LeftItemSource.Play();
                toucheditemleft = null;
            }
            else if (hand == ControllerHand.RightHand)
            {
                g.DisableItem(RightController);
                GrabbableItem gb = null;
                gb = EnableItem(g.ItemCode, g, hand);
                g.ForceParent(gb.Slave.transform.GetChild(0), false);
                RightItemSource.clip = gb.Grab;
                RightItemSource.volume = gb.GrabVolume;
                RightItemSource.Play();
                toucheditemright = null;
            }
        }
        else
        {
            if (g.Player != null)
                g.Player.DropItem(g.transform, true);
            g.Player = this;
            g.DisablePhysics();
            g.DisableOutline(this);
            if (hand == ControllerHand.LeftHand)
            {
                g.ForceParent(LeftController, true);
                ItemLeft = g.transform;
                LeftItemSource.clip = DefaultGrabSound;
                LeftItemSource.volume = DefaultGrabSoundVolume;
                LeftItemSource.Play();
                toucheditemleft = null;
                LeftVibrationController.ShortVibration();
            }
            else if (hand == ControllerHand.RightHand)
            {
                g.ForceParent(RightController, true);
                ItemRight = g.transform;
                RightItemSource.clip = DefaultGrabSound;
                RightItemSource.volume = DefaultGrabSoundVolume;
                RightItemSource.Play();
                toucheditemright = null;
                RightVibrationController.ShortVibration();
            }
        }
        if(OnGrab != null)
            OnGrab.Invoke();
    }




    private void ClickItem(GenericItem i, ControllerHand hand)
    {
        i.Interact(this);
        if (hand == ControllerHand.LeftHand)
            LeftVibrationController.ShortVibration();
        else if (hand == ControllerHand.RightHand)
            RightVibrationController.ShortVibration();
    }

    /*private void ClickItem(NPCController i, ControllerHand hand)
    {
        i.Interact(this);
        if (hand == ControllerHand.LeftHand)
            LeftVibrationController.ShortVibration();
        else if (hand == ControllerHand.RightHand)
            RightVibrationController.ShortVibration();
    }*/


    private void ClickItem(GenericItemSlave i, ControllerHand hand)
    {
        i.Interact(this);
        if (hand == ControllerHand.LeftHand)
            LeftVibrationController.ShortVibration();
        else if (hand == ControllerHand.RightHand)
            RightVibrationController.ShortVibration();
    }

    public override void DropItem(ControllerHand hand, bool forced)
    {
        if (hand == ControllerHand.LeftHand)
            DropLeftItem(forced);
        else if (hand == ControllerHand.RightHand)
            DropRightItem(forced);
    }

    private void DropLeftItem(bool force)
    {
        if (!leftoperating)
        {
            leftoperating = true;
            if (ItemLeft != null)
            {
                var i = ItemLeft.GetComponent<GenericItem>();
                if (i != null)
                {
                    i.Player = null;
                    if (i.ItemCode == ItemCodes.Generic)
                    {
                        i.DisableOutline(this);
                        i.DropParent();
                        i.EnablePhysics();
                        ItemLeft = null;
                        LeftItemSource.clip = DefaultDropSound;
                        LeftItemSource.volume = DefaultDropSoundVolume;
                        LeftItemSource.Play();
                    }
                    else if (force || GetCurrentItem(ControllerHand.LeftHand).CanDrop())
                    {
                        i.DisableOutline(this);
                        i.EnableItem(transform);
                        i.EnablePhysics();
                        var g = DisableItem(ControllerHand.LeftHand);
                        LeftItemSource.clip = g.Drop;
                        LeftItemSource.volume = g.DropVolume;
                        LeftItemSource.Play();
                    }
                    else
                    {
                        LeftInteracting = false;
                        leftoperating = false;
                        return;
                    }
                }

                StopPulse(ControllerHand.LeftHand);
                ItemLeft = null;
                LeftVibrationController.ShortVibration();

                if (OnDrop != null)
                    OnDrop.Invoke();
            }
            LeftInteracting = false;
            leftoperating = false;
        }
    }

    private void DropRightItem(bool force)
    {
        if (!rightoperating)
        {
            rightoperating = true;
            if (ItemRight != null)
            {
                var i = ItemRight.GetComponent<GenericItem>();
                if (i != null)
                {
                    i.Player = null;
                    if (i.ItemCode == ItemCodes.Generic)
                    {
                        i.DisableOutline(this);
                        i.DropParent();
                        i.EnablePhysics();
                        ItemRight = null;
                        RightItemSource.clip = DefaultDropSound;
                        RightItemSource.volume = DefaultDropSoundVolume;
                        RightItemSource.Play();
                    }
                    else if (force || GetCurrentItem(ControllerHand.RightHand).CanDrop())
                    {
                        i.DisableOutline(this);
                        i.EnableItem(transform);
                        i.EnablePhysics();
                        var g = DisableItem(ControllerHand.RightHand);
                        RightItemSource.clip = g.Drop;
                        RightItemSource.volume = g.DropVolume;
                        RightItemSource.Play();
                    }
                    else
                    {
                        RightInteracting = false;
                        rightoperating = false;
                        return;
                    }
                }

                StopPulse(ControllerHand.RightHand);
                ItemRight = null;
                RightVibrationController.ShortVibration();
                if (OnDrop != null)
                    OnDrop.Invoke();
            }
            RightInteracting = false;
            rightoperating = false;
        }
    }

    void DropLeftItem(object sender, ClickedEventArgs e)
    {
        DropLeftItem(false);
    }

    void DropRightItem(object sender, ClickedEventArgs e)
    {
        DropRightItem(false);
    }

    public GrabbableItem EnableItem(ItemCodes code, GenericItem g, ControllerHand hand)
    {
        if (hand == ControllerHand.LeftHand)
        {
            for (int i = 0; i < ItemsLeft.Length; i++)
            {
                var item = ItemsLeft[i].GetComponent<GrabbableItem>();
                if (item != null && item.Code == code)
                {
                    if (code == ItemCodes.Brochure)
                        return EnableItem(i, null, hand);
                    else if (g.Slave != null)
                        return EnableItem(i, g.Slave.transform, hand);
                    else
                        return EnableItem(i, g.transform, hand);
                }
            }
        }
        else if (hand == ControllerHand.RightHand)
        {
            for (int i = 0; i < ItemsRight.Length; i++)
            {
                var item = ItemsRight[i].GetComponent<GrabbableItem>();
                if (item != null && item.Code == code)
                {
                    if (code == ItemCodes.Brochure)
                        return EnableItem(i, null, hand);
                    else if (g.Slave != null)
                        return EnableItem(i, g.Slave.transform, hand);
                    else
                        return EnableItem(i, g.transform, hand);
                }
            }
        }
        return null;
    }

    Vector3 CurrentLeftItemPos()
    {
        if (ItemLeftIndex != -1)
            return ItemsLeft[ItemLeftIndex].transform.position;
        else
            return new Vector3(0, 0, 0);
    }

    Vector3 CurrentRightItemPos()
    {
        if (ItemRightIndex != -1)
            return ItemsRight[ItemRightIndex].transform.position;
        else
            return new Vector3(0, 0, 0);
    }

    Quaternion CurrentLeftItemRot()
    {
        if (ItemLeftIndex != -1)
            return ItemsLeft[ItemLeftIndex].transform.rotation;
        else
            return new Quaternion(0, 0, 0, 0);
    }
    Quaternion CurrentRightItemRot()
    {
        if (ItemRightIndex != -1)
            return ItemsRight[ItemRightIndex].transform.rotation;
        else
            return new Quaternion(0, 0, 0, 0);
    }
    GrabbableItem DisableItem(ControllerHand hand)
    {
        if (hand == ControllerHand.LeftHand && ItemLeftIndex != -1)
            return DisableItem(hand, ItemLeftIndex);
        else if (hand == ControllerHand.RightHand && ItemRightIndex != -1)
            return DisableItem(hand, ItemRightIndex);
        return null;
    }

    GrabbableItem EnableItem(int i, Transform t, ControllerHand hand)
    {
        GrabbableItem g = null;
        if (hand == ControllerHand.LeftHand)
        {
            if (i < 0 || i > ItemsLeft.Length)
                return g;
            ItemLeft = t;
            g = ItemsLeft[i].GetComponent<GrabbableItem>();
            ItemLeftIndex = i;
            if (g.HideController)
            {
                if (hand == ControllerHand.LeftHand)
                    LeftController.Find("Model").gameObject.SetActive(false);
                else if (hand == ControllerHand.RightHand)
                    RightController.Find("Model").gameObject.SetActive(false);
            }
            if (t != null)
            {
                var gi = t.GetComponent<GenericItem>();
                var gis = t.GetComponent<GenericItemSlave>();

                if (gi != null)
                    g.LoadState(gi);
                else if (gis != null)
                    g.LoadState(gis.Master);
                else
                    g.LoadState();
            }
            else
                g.LoadState();
        }
        else if (hand == ControllerHand.RightHand)
        {
            if (i < 0 || i > ItemsRight.Length)
                return g;
            ItemRightIndex = i;
            ItemRight = t;
            g = ItemsRight[i].GetComponent<GrabbableItem>();
            if (g.HideController)
            {
                if (hand == ControllerHand.LeftHand)
                    LeftController.Find("Model").gameObject.SetActive(false);
                else if (hand == ControllerHand.RightHand)
                    RightController.Find("Model").gameObject.SetActive(false);
            }
            if (t != null)
            {
                var gi = t.GetComponent<GenericItem>();
                var gis = t.GetComponent<GenericItemSlave>();

                if (gi != null)
                    g.LoadState(gi);
                else if (gis != null)
                    g.LoadState(gis.Master);
                else
                    g.LoadState();
            }
            else
                g.LoadState();
        }

        return g;
    }

    GrabbableItem DisableItem(ControllerHand hand, int i)
    {
        GrabbableItem g = null;
        if (hand == ControllerHand.LeftHand)
        {
            if (i < 0 || i > ItemsLeft.Length || ItemLeftIndex == -1)
                return g;
            ItemLeftIndex = -1;
            g = ItemsLeft[i].GetComponent<GrabbableItem>();
            if (g.HideController)
            {
                if (hand == ControllerHand.LeftHand)
                    LeftController.Find("Model").gameObject.SetActive(true);
                else if (hand == ControllerHand.RightHand)
                    RightController.Find("Model").gameObject.SetActive(true);
            }
            g.SaveState();
        }
        else if (hand == ControllerHand.RightHand)
        {
            if (i < 0 || i > ItemsRight.Length || ItemRightIndex == -1)
                return g;
            ItemRightIndex = -1;
            g = ItemsRight[i].GetComponent<GrabbableItem>();
            if (g.HideController)
            {
                if (hand == ControllerHand.LeftHand)
                    LeftController.Find("Model").gameObject.SetActive(true);
                else if (hand == ControllerHand.RightHand)
                    RightController.Find("Model").gameObject.SetActive(true);
            }
            g.SaveState();
        }
        return g;
    }

    public void StartPulsePublic(ControllerButton button)
    {
        ExternalLeftButtonToPulse = button;
        ExternalRightButtonToPulse = button;
        if (LeftButtonToPulse == ControllerButton.Nothing)
            StartPulse(ControllerHand.LeftHand);
        if (RightButtonToPulse == ControllerButton.Nothing)
            StartPulse(ControllerHand.RightHand);
    }

    public void StartPulsePublic(ControllerButton button, ControllerHand hand)
    {
        if (hand == ControllerHand.LeftHand)
        {
            ExternalLeftButtonToPulse = button;
            if (LeftButtonToPulse == ControllerButton.Nothing)
                StartPulse(ControllerHand.LeftHand);
        }
        else if (hand == ControllerHand.RightHand)
        {
            ExternalRightButtonToPulse = button;
            if (RightButtonToPulse == ControllerButton.Nothing)
                StartPulse(ControllerHand.RightHand);

        }
    }

    public void StartPulsePublic(ControllerButtonByFunc button, ControllerHand hand)
    {
        if (hand == ControllerHand.LeftHand)
        {
            switch (button)
            {
                case ControllerButtonByFunc.ButtonToInteract:
                    ExternalLeftButtonToPulse = GetButton(ButtonToInteract);
                    break;
                case ControllerButtonByFunc.ButtonToDrop:
                    ExternalLeftButtonToPulse = GetButton(ButtonToDrop);
                    break;
                case ControllerButtonByFunc.ButtonToUse:
                    ExternalLeftButtonToPulse = GetButton(ButtonToUse);
                    break;
                case ControllerButtonByFunc.All:
                    ExternalLeftButtonToPulse = ControllerButton.All;
                    break;
                default:
                    ExternalLeftButtonToPulse = ControllerButton.Nothing;
                    break;
            }
            if (LeftButtonToPulse == ControllerButton.Nothing)
                StartPulse(ControllerHand.LeftHand);
        }
        else if (hand == ControllerHand.RightHand)
        {
            switch (button)
            {
                case ControllerButtonByFunc.ButtonToInteract:
                    ExternalRightButtonToPulse = GetButton(ButtonToInteract);
                    break;
                case ControllerButtonByFunc.ButtonToDrop:
                    ExternalRightButtonToPulse = GetButton(ButtonToDrop);
                    break;
                case ControllerButtonByFunc.ButtonToUse:
                    ExternalRightButtonToPulse = GetButton(ButtonToUse);
                    break;
                case ControllerButtonByFunc.All:
                    ExternalRightButtonToPulse = ControllerButton.All;
                    break;
                default:
                    ExternalRightButtonToPulse = ControllerButton.Nothing;
                    break;
            }
            if (RightButtonToPulse == ControllerButton.Nothing)
                StartPulse(ControllerHand.RightHand);
        }
    }

    public void StartPulsePublic(ControllerButtonByFunc button)
    {
        switch (button)
        {
            case ControllerButtonByFunc.ButtonToInteract:
                ExternalLeftButtonToPulse = GetButton(ButtonToInteract);
                ExternalRightButtonToPulse = GetButton(ButtonToInteract);
                break;
            case ControllerButtonByFunc.ButtonToDrop:
                ExternalLeftButtonToPulse = GetButton(ButtonToDrop);
                ExternalRightButtonToPulse = GetButton(ButtonToDrop);
                break;
            case ControllerButtonByFunc.ButtonToUse:
                ExternalLeftButtonToPulse = GetButton(ButtonToUse);
                ExternalRightButtonToPulse = GetButton(ButtonToUse);
                break;
            case ControllerButtonByFunc.All:
                ExternalLeftButtonToPulse = ControllerButton.All;
                ExternalRightButtonToPulse = ControllerButton.All;
                break;
            default:
                ExternalLeftButtonToPulse = ControllerButton.Nothing;
                ExternalRightButtonToPulse = ControllerButton.Nothing;
                break;
        }
        if (LeftButtonToPulse == ControllerButton.Nothing)
            StartPulse(ControllerHand.LeftHand);
        if (RightButtonToPulse == ControllerButton.Nothing)
            StartPulse(ControllerHand.RightHand);
    }

    private ControllerButton GetButton(ControllerButtonInput buttonToUse)
    {
        switch (buttonToUse)
        {
            case ControllerButtonInput.Trigger:
                return ControllerButton.Trigger;
            case ControllerButtonInput.RGrip:
                return ControllerButton.RGrip;
            case ControllerButtonInput.LGrip:
                return ControllerButton.LGrip;
            case ControllerButtonInput.Grip:
                return ControllerButton.Grip;
            case ControllerButtonInput.Pad:
                return ControllerButton.Pad;
            case ControllerButtonInput.All:
                return ControllerButton.All;
            default:
                return ControllerButton.Nothing;
        }
    }

    public void StopPulsePublic()
    {
        ExternalLeftButtonToPulse = ControllerButton.Nothing;
        ExternalRightButtonToPulse = ControllerButton.Nothing;
        ControllerButton button = ControllerButton.Nothing;
        if (LeftButtonToPulse != ControllerButton.Nothing)
        {
            button = LeftButtonToPulse;
            StopPulse(ControllerHand.LeftHand);
            StartPulse(button, ControllerHand.LeftHand);
        }
        else
            StopPulse(ControllerHand.LeftHand);
        if (RightButtonToPulse != ControllerButton.Nothing)
        {
            button = RightButtonToPulse;
            StopPulse(ControllerHand.RightHand);
            StartPulse(button, ControllerHand.RightHand);
        }
        else
            StopPulse();
    }

    public void StopPulsePublic(ControllerHand hand)
    {
        if (hand == ControllerHand.LeftHand)
        {
            ExternalLeftButtonToPulse = ControllerButton.Nothing;
            ControllerButton button = ControllerButton.Nothing;
            if (LeftButtonToPulse != ControllerButton.Nothing)
            {
                button = LeftButtonToPulse;
                StopPulse(ControllerHand.LeftHand);
                StartPulse(button, ControllerHand.LeftHand);
            }
            else
                StopPulse(ControllerHand.LeftHand);
        }
        else if (hand == ControllerHand.RightHand)
        {
            ExternalRightButtonToPulse = ControllerButton.Nothing;
            ControllerButton button = ControllerButton.Nothing;
            if (RightButtonToPulse != ControllerButton.Nothing)
            {
                button = RightButtonToPulse;
                StopPulse(ControllerHand.RightHand);
                StartPulse(button, ControllerHand.RightHand);
            }
            else
                StopPulse();
        }
    }
}
