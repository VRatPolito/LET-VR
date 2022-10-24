//#if(UNITY_EDITOR)
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class VRButtonController : VRUIElement
{
    public Texture UnselectedTexture, SelectedTexture;
    public delegate void MyDelegate(object c);
    [HideInInspector]
    public MyDelegate OnButtonSelected, OnButtonUnSelected, onClick;

    [SerializeField] private InputActionReference Trigger;

    Button button;
    //Text text;
    public bool SendCommandOnClick = true;

    public override void Awake()
    {
        button = GetComponent<Button>();
        var t = transform.Find("Text");
        /*if (t != null)
            text = t.GetComponent<Text>();*/
        base.Awake();        
    }
    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }
    
    public override void ElementSelected()
    {
        if (button != null)
            button.Select();
        else
        {
            EnableOutline();
            if (OnButtonSelected != null)
            {
                if (c != null)
                    OnButtonSelected.Invoke(c);
                /*else if (fp != null)
                    OnButtonSelected.Invoke(fp);
                else if (fpi != null)
                    OnButtonSelected.Invoke(fpi);*/
            }
        }
        if (!SendCommandOnClick)
        {
            button.onClick.Invoke();
            ElementUnSelected();
        }
        else
        {
           /* if (fp != null)
                fp.OnButtonClicked += Click;
            else */if(c!=null)
                //c.TriggerClicked += Click;
            Trigger.action.performed += Click;
            /*else if (fpi != null)
                fpi.OnButtonClicked += Click;*/
        }

    }

    public override void Click(InputAction.CallbackContext e)   // todo: controllare se vr button controller needed, in tal caso correggere click _LV - sender => debugmsg
    {
        if (button != null)
            button.onClick.Invoke();
        if (onClick != null)
        {
            DisableOutline();
            //onClick.Invoke(sender);
            Debug.Log("onclick.invoke(sender) not updated yet");
        }
        base.Click(e);
    }

    public override void ElementUnSelected()
    {
        if (SendCommandOnClick)
        {
            if (c != null)
                //c.TriggerClicked -= Click;
                Trigger.action.performed -= Click;
            /*else if (fp != null)
                fp.OnButtonClicked -= Click;
            else if (fpi != null)
                fpi.OnButtonClicked -= Click;*/
        }
        if (button != null)
            base.ElementUnSelected();
        else
        {
            DisableOutline();
            if (OnButtonUnSelected != null)
            {
                if (OnButtonUnSelected != null)
                {
                    if (c != null)
                        OnButtonUnSelected.Invoke(c);
                    /*else if (fp != null)
                        OnButtonUnSelected.Invoke(fp);
                    else if (fpi != null)
                        OnButtonUnSelected.Invoke(fpi);*/
                }
            }
        }
    }

    public override void ElementNotActive()
    {
        if (Active)
        {
            if (c!=null)
                UnSelected();
            Active = false;
            if (button != null)
                button.interactable = false;
            else
                {
                var mr = GetComponent<MeshRenderer>();
                    if (mr != null && UnselectedTexture != null)
                    {
                        var m = mr.materials[0];
                        m.SetTexture("_MainTex", UnselectedTexture);
                        m.SetTexture("_EmitMap", UnselectedTexture);
                        m.SetFloat("_EmitStrength", 0);
                    }
                }
        }
    }

    public override void ElementActive()
    {
        if (!Active)
        {
            Active = true;
            if (button != null)
                button.interactable = true;
            else
            {
                var mr = GetComponent<MeshRenderer>();
                if (mr != null && UnselectedTexture != null)
                {
                    var m = mr.materials[0];
                    m.SetTexture("_MainTex", UnselectedTexture);
                    m.SetTexture("_EmitMap", UnselectedTexture);
                    m.SetFloat("_EmitStrength", 1);
                }
            }
        }
    }

    public void EnableOutline()
    {
        var mr = GetComponent<MeshRenderer>();
        if (mr != null && SelectedTexture != null)
        {
            var m = mr.materials[0];
            m.SetTexture("_MainTex", SelectedTexture);
            m.SetTexture("_EmitMap", SelectedTexture);
            m.SetFloat("_EmitStrength", 1);
        }
    }

    public void DisableOutline()
    {
        var mr = GetComponent<MeshRenderer>();
        if (mr != null && UnselectedTexture != null)
        {
            var m = mr.materials[0];
            m.SetTexture("_MainTex", UnselectedTexture);
            m.SetTexture("_EmitMap", UnselectedTexture);
            if (c != null /*|| fp != null || fpi != null*/)
                m.SetFloat("_EmitStrength", 1);
            else
                m.SetFloat("_EmitStrength", 0);
        }
    }

}
//#endif