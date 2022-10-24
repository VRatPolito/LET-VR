//#if (UNITY_EDITOR)

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class VRUIElement : MonoBehaviour
{
    [HideInInspector]
    public ActionBasedController c;
    /*[HideInInspector]
    public FirstPersonMainView fp;
    [HideInInspector]
    public ItemControllerNet icn;
    [HideInInspector]
    public FirstPersonItemController fpi;*/
    [HideInInspector]
    public bool transition = false;
    [HideInInspector]
    public AudioSource Source;
    public bool ActiveSinceStart = true;
    public bool EnabledSinceStart = true;
    [HideInInspector]
    public bool Active = true;

    // Use this for initialization
    void Start()
    {

    }


    public virtual void Awake()
    {
        Source = GetComponent<AudioSource>();
        if (!ActiveSinceStart)
            ElementNotActive();
        if (!EnabledSinceStart)
            gameObject.SetActive(false);
    }

    public virtual void ElementNotActive()
    {
        throw new NotImplementedException();
    }
    public virtual void ElementActive()
    {
        throw new NotImplementedException();
    }

    public bool IsElementActive()
    {
        return Active;
    }
    // Update is called once per frame
    void Update()
    {

    }

    public virtual void Selected(ActionBasedController controller)
    {
        if (Source.isPlaying)
            Source.Stop();
        Source.Play();
        transition = true;
        c = controller;
        ElementSelected();
        transition = false;
    }

    public virtual void ElementSelected()
    {
        throw new NotImplementedException();
    }

    public virtual void ElementUnSelected()
    {
        if(EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
        if(c != null)
        {
            var i = c.GetComponent<ControllerManager>();
            if (i != null)
                i.StopPulsePublic();
            c = null;
        }
        /*fp = null;
        fpi = null;
        icn = null;*/
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (Active && other.gameObject.tag == "Controller" && c != null)
            UnSelected();
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (Active && other.gameObject.tag == "Controller" && c == null)
        {
            c = other.GetComponent<ActionBasedController>();
            if (c != null)
            {
                var i = c.GetComponent<ControllerManager>();
                if (i != null)
                {
                    i.StartPulsePublic(ControllerButtonByFunc.ButtonToInteract);
                    i.vibrationController.ShortVibration();
                }
            }
            Selected(c);
        }
    }

    /*public void Selected(FirstPersonMainView controller)
    {
        if (Source.isPlaying)
            Source.Stop();
        Source.Play();
        transition = true;
        fp = controller;
        ElementSelected();
        transition = false;
    }*/

    public virtual void Click(InputAction.CallbackContext e) // done: controllare se vr button controller needed, in tal caso correggere click _LV 
    {
        if(c != null)
            OnTriggerExit(c.GetComponent<Collider>());
    }

    public void UnSelected()
    {
        transition = true;
        ElementUnSelected();
        transition = false;
    }
    public virtual void OnDisable()
    {
        gameObject.SetActive(false);
        UnSelected();
    }
    public virtual void OnDestroy()
    {
        if (c != null)
            ElementUnSelected();
    }

    /*public void Selected(ItemControllerNet controller)
    {
        if (Source.isPlaying)
            Source.Stop();
        Source.Play();
        transition = true;
        icn = controller;
        ElementSelected();
        transition = false;
    }

    public void Selected(FirstPersonItemController controller)
    {
        if (Source.isPlaying)
            Source.Stop();
        Source.Play();
        transition = true;
        fpi = controller;
        ElementSelected();
        transition = false;
    }*/
}
//#endif