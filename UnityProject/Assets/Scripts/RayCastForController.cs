using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SteamVR_LaserPointer))]
[RequireComponent(typeof(SteamVR_TrackedController))]
public class RayCastForController : MonoBehaviour
{

    public ControllerHand ControllerType;
    SteamVR_TrackedController controller;
    Transform currentelement = null;
    public RayCastForController OtherHand;
    public Color PadColor = Color.yellow;
    public Color TriggerColor = Color.magenta;
    public Color GripColor = Color.cyan;    
    AudioSource Source;
    public Shader Outline;
    float t;
    bool up = true;
    bool clickeffect = false;
    public float PulseTime = 0.5f;

    ControllerButton ButtonToPulse = ControllerButton.Nothing, ExternalButtonToPulse = ControllerButton.Nothing;
    public ControllerButtonInput ButtonToInteract = ControllerButtonInput.Trigger;
    public enum ControllerButtonInput { Trigger, RGrip, LGrip, Grip, Pad, All, Any };

    List<MeshRenderer> Grips = new List<MeshRenderer>();
    List<MeshRenderer> Pads = new List<MeshRenderer>();
    List<MeshRenderer> Triggers = new List<MeshRenderer>();
    Material oldmat;

    Material PadMaterial, GripMaterial, TriggerMaterial;

    // Use this for initialization
    private void Awake()
    {
        Source = GetComponent<AudioSource>();
    }
    void Start()
    {
        controller = GetComponent<SteamVR_TrackedController>();
    }

    private void OnDisable()
    {
        GetComponent<SteamVR_LaserPointer>().active = false;
        if(transform.Find("New Game Object"))
            transform.Find("New Game Object").gameObject.SetActive(false);
        if (clickeffect)
        {
            switch (ButtonToInteract)
            {
                case ControllerButtonInput.Pad:
                    controller.PadClicked -= ClickEffect;
                    break;
                case ControllerButtonInput.Grip:
                    controller.Gripped -= ClickEffect;
                    break;
                case ControllerButtonInput.Any:
                    controller.TriggerClicked -= ClickEffect;
                    controller.Gripped -= ClickEffect;
                    controller.PadClicked -= ClickEffect;
                    break;
                default:
                    controller.TriggerClicked -= ClickEffect;
                    break;
            }
            clickeffect = false;
        }
        StopPulse();
    }

    private void OnEnable()
    {
        GetComponent<SteamVR_LaserPointer>().active = true;
        if(transform.Find("New Game Object"))
            transform.Find("New Game Object").gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(controller.transform.position, controller.transform.rotation * Vector3.forward);
        RaycastHit hit;
        var mask = LayerMask.GetMask("UI");
        if (Physics.Raycast(ray, out hit, 100.0F, mask))
        {
            var uihit = hit.collider.transform.GetComponent<VRUIElement>();
            /*var buttonhit = hit.collider.transform.GetComponent<VRButtonController>();
            var togglehit = hit.collider.transform.GetComponent<VRToggleController>();
            var inputfieldhit = hit.collider.transform.GetComponent<VRInputFieldController>();*/
            if(uihit != null && uihit.IsElementActive() && currentelement != uihit.transform && !uihit.transition && (OtherHand == null || OtherHand.currentelement == null))
            {
                if (currentelement != null)
                {
                var b = currentelement.GetComponent<VRUIElement>();
                    b.UnSelected();
                }
                currentelement = uihit.transform;
                currentelement.GetComponent<VRUIElement>().Selected(controller);
                StartCoroutine(LongVibration(0.1f, 0.1f));
                if (!clickeffect)
                {
                    switch (ButtonToInteract)
                    {
                        case ControllerButtonInput.Pad:
                            controller.PadClicked += ClickEffect;
                            StartPulse(ControllerButton.Pad);
                            break;
                        case ControllerButtonInput.Grip:
                            controller.Gripped += ClickEffect;
                            StartPulse(ControllerButton.Grip);
                            break;
                        case ControllerButtonInput.Any:
                            controller.TriggerClicked += ClickEffect;
                            controller.Gripped += ClickEffect;
                            controller.PadClicked += ClickEffect;
                            StartPulse(ControllerButton.All);
                            break;
                        default:
                            controller.TriggerClicked += ClickEffect;
                            StartPulse(ControllerButton.Trigger);
                            break;
                    }
                    clickeffect = true;
                }
            }
            else if (uihit == null || !uihit.IsElementActive() || uihit.transition || (OtherHand != null && OtherHand.currentelement != null))
            {
                if (currentelement != null)
                {
                    var b = currentelement.GetComponent<VRUIElement>();
                    b.UnSelected();
                }
                currentelement = null;

                if (clickeffect)
                {
                    switch (ButtonToInteract)
                    {
                        case ControllerButtonInput.Pad:
                            controller.PadClicked -= ClickEffect;
                            break;
                        case ControllerButtonInput.Grip:
                            controller.Gripped -= ClickEffect;
                            break;
                        case ControllerButtonInput.Any:
                            controller.TriggerClicked -= ClickEffect;
                            controller.Gripped -= ClickEffect;
                            controller.PadClicked -= ClickEffect;
                            break;
                        default:
                            controller.TriggerClicked -= ClickEffect;
                            break;
                    }
                    clickeffect = false;
                }
                if (ExternalButtonToPulse != ControllerButton.Nothing && ButtonToPulse != ExternalButtonToPulse)
                    StartPulse(ExternalButtonToPulse);
                else if (ExternalButtonToPulse == ControllerButton.Nothing)
                    StopPulse();
            }
        }
        else
        {
            if (currentelement != null)
            {
                var b = currentelement.GetComponent<VRUIElement>();
                if (b != null)
                {
                    if (!b.transition)
                    {
                        b.UnSelected();
                        currentelement = null;

                        if (clickeffect)
                        {
                            switch (ButtonToInteract)
                            {
                                case ControllerButtonInput.Pad:
                                    controller.PadClicked -= ClickEffect;
                                    break;
                                case ControllerButtonInput.Grip:
                                    controller.Gripped -= ClickEffect;
                                    break;
                                case ControllerButtonInput.Any:
                                    controller.TriggerClicked -= ClickEffect;
                                    controller.Gripped -= ClickEffect;
                                    controller.PadClicked -= ClickEffect;
                                    break;
                                default:
                                    controller.TriggerClicked -= ClickEffect;
                                    break;
                            }
                            clickeffect = false;
                        }
                        if (ExternalButtonToPulse != ControllerButton.Nothing)
                            StartPulse(ExternalButtonToPulse);
                        else
                            StopPulse();
                    }

                    if (!b.IsElementActive() || !b.gameObject.activeSelf)
                    {
                        b.UnSelected();
                        currentelement = null;
                        if (ExternalButtonToPulse != ControllerButton.Nothing)
                            StartPulse(ExternalButtonToPulse);
                        else
                            StopPulse();
                    }
                    else if (currentelement == null && ExternalButtonToPulse != ControllerButton.Nothing && ButtonToPulse != ExternalButtonToPulse)
                    {
                        StartPulse(ExternalButtonToPulse);
                    }
                }
            }
        }
        PulseButton();
    }

    public void ClickEffect(object sender, ClickedEventArgs e)
    {
        if (!enabled)
        {
            GetComponent<SteamVR_LaserPointer>().active = false;
            if (transform.Find("New Game Object"))
                transform.Find("New Game Object").gameObject.SetActive(false);
            if (clickeffect)
            {
                switch (ButtonToInteract)
                {
                    case ControllerButtonInput.Pad:
                        controller.PadClicked -= ClickEffect;
                        break;
                    case ControllerButtonInput.Grip:
                        controller.Gripped -= ClickEffect;
                        break;
                    case ControllerButtonInput.Any:
                        controller.TriggerClicked -= ClickEffect;
                        controller.Gripped -= ClickEffect;
                        controller.PadClicked -= ClickEffect;
                        break;
                    default:
                        controller.TriggerClicked -= ClickEffect;
                        break;
                }
                clickeffect = false;
            }
            StopPulse();
        }
        else
        {
            if (Source.isPlaying)
                Source.Stop();
            Source.Play();
            StartCoroutine(LongVibration(0.1f, 0.1f));
        }
    }
    

    IEnumerator LongVibration(float length, float strength)
{
    for (float i = 0; i < length; i += Time.deltaTime)
    {
        SteamVR_Controller.Input((int)controller.controllerIndex).TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, strength));
        yield return null;
    }
}

    void StartPulse(ControllerButton button)
    {
            MeshRenderer r = null;
            if (ButtonToPulse != ControllerButton.Nothing)
                StopPulse();

            ButtonToPulse = button;

            var c = transform.Find("Model");
            Transform t = null;
            switch (button)
            {
                case ControllerButton.RGrip:
                    t = c.Find("rgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if(r != null)
							Grips.Add(r);
                    }
                    break;
                case ControllerButton.LGrip:
                    t = c.Find("lgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if(r != null)
							Grips.Add(r);
                    }
                    break;
                case ControllerButton.Grip:
                    t = c.Find("rgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if(r != null)
							Grips.Add(r);
                    }
                    t = c.Find("lgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if(r != null)
							Grips.Add(r);
                    } 
					t = c.Find("handgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if(r != null)
							Grips.Add(r);
                    }
                    break;
                case ControllerButton.Pad:
                    t = c.Find("trackpad");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();                        
						if(r != null)
							Pads.Add(r);
                    }
					else
					{
					t = c.Find("thumbstick");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
						if(r != null)
							Pads.Add(r);
                    }
					}
                    break;
                case ControllerButton.Trigger:
                    t = c.Find("trigger");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if(r != null)
							Triggers.Add(r);
                    }
                    break;
                case ControllerButton.All:
                    t = c.Find("trackpad");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if(r != null)
							Pads.Add(r);
                    }
					else
					{
					t = c.Find("thumbstick");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if(r != null)
							Pads.Add(r);
                    }
					}
                    t = c.Find("trigger");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if(r != null)
							Triggers.Add(r);
                    }
                    t = c.Find("rgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if(r != null)
							Grips.Add(r);
                    }
                    t = c.Find("lgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if(r != null)
							Grips.Add(r);
                    }
					t = c.Find("handgrip");
                    if (t != null)
                    {
                        r = t.GetComponent<MeshRenderer>();
                        if(r != null)
							Grips.Add(r);
                    }
                    break;
                default:
                    return;
            }
            if (oldmat == null)
            {
            if (Triggers.Count > 0)
                oldmat = Triggers[0].materials[0];
            else if (Pads.Count > 0)
                oldmat = Pads[0].materials[0];
            else if (Grips.Count > 0)
                oldmat = Grips[0].materials[0];
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
            foreach (MeshRenderer mr in Pads)
            {
                var m = new Material[1];
                m[0] = PadMaterial;
                mr.materials = m;
            }
            foreach (MeshRenderer mr in Triggers)
            {
                var m = new Material[1];
                m[0] = TriggerMaterial;
                mr.materials = m;
            }
            foreach (MeshRenderer mr in Grips)
            {
                var m = new Material[1];
                m[0] = GripMaterial;
                mr.materials = m;
            }
        }
    }

    void PulseButton()
    {
        if (ButtonToPulse != ControllerButton.Nothing)
        {
            if (ControllerType == ControllerHand.RightHand && OtherHand != null && OtherHand.ButtonToPulse != ControllerButton.Nothing)
                return;
            t += Time.deltaTime / PulseTime;
            if (ControllerType == ControllerHand.LeftHand && OtherHand != null && OtherHand.ButtonToPulse != ControllerButton.Nothing)
                {
                foreach (MeshRenderer mr in OtherHand.Grips)
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
                foreach (MeshRenderer mr in OtherHand.Triggers)
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
                foreach (MeshRenderer mr in OtherHand.Pads)
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

            foreach (MeshRenderer mr in Grips)
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
            foreach (MeshRenderer mr in Triggers)
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
            foreach (MeshRenderer mr in Pads)
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
            if (t >= 1)
            {
                up = false;
                t = 0;
            }
        }
    }

    public void StartPulsePublic(ControllerButton button)
    {
        ExternalButtonToPulse = button;
    }

    public void StopPulsePublic()
    {
        ExternalButtonToPulse = ControllerButton.Nothing;
    }

    void StopPulse()
    {
            /*var c = transform.FindChild("Model");
            var t = c.FindChild("trigger");*/
            foreach (MeshRenderer mr in Triggers)
            {
                var m = new Material[1];
                m[0] = oldmat;
                mr.materials = m;
            }
            foreach (MeshRenderer mr in Grips)
            {
                var m = new Material[1];
                m[0] = oldmat;
                mr.materials = m;
            }
            foreach (MeshRenderer mr in Pads)
            {
                var m = new Material[1];
                m[0] = oldmat;
                mr.materials = m;
            }
            Triggers.Clear();
            Pads.Clear();
            Grips.Clear();
            ButtonToPulse = ControllerButton.Nothing;
    }
}
