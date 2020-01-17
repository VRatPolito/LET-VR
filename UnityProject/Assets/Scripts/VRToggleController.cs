using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class VRToggleController : VRUIElement
{
    public Texture UnselectedTexture, SelectedTexture;
    public delegate void MyDelegate();
    [HideInInspector]
    public MyDelegate OnToggleSet, OnToggleUnSet, OnToggle;
    public bool CanUntoggle = false;
    public bool State { get; private set; }

    public bool ActiveFromTheBeginning;
    public List<VRToggleController> OtherExclusiveToggles;

    // Use this for initialization
    public override void Awake()
    {
        Source = GetComponent<AudioSource>();
        if (!ActiveSinceStart)
            ElementNotActive();
        else if (ActiveFromTheBeginning)
            SetState(true);
        else
            ToggleUnselected();
    }

    public void Toggle(object sender, ClickedEventArgs e)
    {
        if (!CanUntoggle && State)
            return;
        else
         SetState(!State);
    }

    public void SetState(bool v)
    {
        if(v != State)
        {
            State = v;
            if (State)
            {
                if (OnToggleSet != null)
                    OnToggleSet.Invoke();
                if (OnToggle != null)
                    OnToggle.Invoke();
                EnableOutline();
                foreach (VRToggleController t in OtherExclusiveToggles)
                    t.PropagateSetStateFalse();
            }
            else
            {
                if (OnToggleUnSet != null)
                    OnToggleUnSet.Invoke();
                if (OnToggle != null)
                    OnToggle.Invoke();
                DisableOutline();
            }
        }
    }

    public void PropagateSetStateFalse()
    {
        if (false != State)
        {
            State = false;
            if (OnToggleUnSet != null)
                OnToggleUnSet.Invoke();
            if (OnToggle != null)
                OnToggle.Invoke();
            /*fp = null;
            fpi = null;*/
            c = null;
            DisableOutline();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void ElementSelected()
    {
        if (c != null)
            c.TriggerClicked += Toggle;
        /*else if (fp != null)
            fp.OnButtonClicked += Toggle;
        else if (fpi != null)
            fpi.OnButtonClicked += Toggle;*/
        if (!State)
            ToggleSelected();
    }

     public override void OnTriggerEnter(Collider other)
     {
         /*if (Enabled && other.gameObject.tag == "Controller" && c == null)
         {
             c = other.GetComponent<SteamVR_TrackedController>();
             Selected(c);
         }*/
     }
     

    public override void ElementUnSelected()
    {
        ToggleUnselected();
        if (c != null && (OnToggle != null || OnToggleSet != null || OnToggleUnSet != null))
        {
            //OnToggleUnSelected(c);
            c.TriggerClicked -= Toggle;
        }
        /*else if (fp != null && (OnToggle != null || OnToggleSet != null || OnToggleUnSet != null))
        {
            //OnToggleUnSelected(fp);
            fp.OnButtonClicked -= Toggle;
        }
        else if (fpi != null && (OnToggle != null || OnToggleSet != null || OnToggleUnSet != null))
        {
            //OnToggleUnSelected(fp);
            fpi.OnButtonClicked -= Toggle;
        }*/
    }


    void ToggleUnselected()
    {
        if (!State)
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
        else
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
    }

    void ToggleSelected()
    {
        if (!State)
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
        else
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
    }

    public override void ElementNotActive()
    {
        if (Active)
        {
            if (c != null)
                UnSelected();
            Active = false;
            var mr = GetComponent<MeshRenderer>();
            if (mr != null && UnselectedTexture != null)
            {
                var m = mr.materials[0];
                m.SetTexture("_MainTex", UnselectedTexture);
                m.SetTexture("_EmitMap", UnselectedTexture);
                m.SetFloat("_EmitStrength", 0);
            }
            State = false;
        }
    }

    public override void ElementActive()
    {
        if (!Active)
        {
            Active = true;
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
        /*else if (mr != null)
        {
            var m = mr.materials[0];
            var c = m.GetColor("_OutlineColor");
            c.a = 255;
            m.SetColor("_OutlineColor", c);
            m.SetFloat("_EmitStrength", 1);
        }*/
    }

    public void DisableOutline()
    {
        var mr = GetComponent<MeshRenderer>();
        if (mr != null && UnselectedTexture != null)
        {
            var m = mr.materials[0];
            m.SetTexture("_MainTex", UnselectedTexture);
            m.SetTexture("_EmitMap", UnselectedTexture);
            if (c != null/* || fp != null || fpi != null*/)
                m.SetFloat("_EmitStrength", 1);
            else
                m.SetFloat("_EmitStrength", 0);
        }
        /*else if (mr != null)
        {
            var m = mr.materials[0];
            var c = m.GetColor("_OutlineColor");
            c.a = 0;
            m.SetColor("_OutlineColor", c);
            m.SetFloat("_EmitStrength", 0);
        }*/
    }
}
