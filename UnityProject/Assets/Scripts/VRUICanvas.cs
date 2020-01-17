using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class VRUICanvas : MonoBehaviour {

    public List<VRUIElement> Elements;
    public List<Text> TextElements;
    public List<Transform> GenericElements;
    public bool EnabledSinceStart = true;
    public bool ElementsEnabled;
    bool initialized = false;
    // Use this for initialization

    void Start()
    {
            Initialize();
    }

    private void Initialize()
    {
        if(!initialized)
        {
            if(!EnabledSinceStart)
                initialized = InitialDisableElements();
            else
                initialized = InitialEnableElements();
        }
    }


    // Update is called once per frame
    void Update() {
        if (!initialized)
            Initialize();       
    }
    bool InitialDisableElements()
    {
            foreach (VRUIElement e in Elements)
        {
            if (e == null)
                return false;
                e.enabled = false;
                e.gameObject.SetActive(false);
            }
            foreach (Text e in TextElements)
            {
            if (e == null)
                return false;
            e.enabled = false;
            }
            foreach (Transform e in GenericElements)
        {
            if (e == null)
                return false;
            e.gameObject.SetActive(false);
            }
        ElementsEnabled = false;
        return true;
    }
    public void DisableElements()
    {
        if (!initialized)
            Initialize();
            foreach (VRUIElement e in Elements)
            {
                e.enabled = false;
                e.gameObject.SetActive(false);
            }
            foreach (Text e in TextElements)
            {
                e.enabled = false;
            }
            foreach (Transform e in GenericElements)
            {
                e.gameObject.SetActive(false);
            }
        ElementsEnabled = false;
    }

    public void EnableElements()
    {
        if (!initialized)
            Initialize();
            foreach (VRUIElement e in Elements)
            {
                if (e.EnabledSinceStart)
                {
                    e.gameObject.SetActive(true);
                    e.enabled = true;
                }
            }
            foreach (Text e in TextElements)
            {
                e.enabled = true;
            }
            foreach (Transform e in GenericElements)
            {
                e.gameObject.SetActive(true);
            }
        ElementsEnabled = true;
    }

    public bool InitialEnableElements()
    {
        foreach (VRUIElement e in Elements)
        {
            if (e == null)
                return false;
            if (e.EnabledSinceStart)
            {
                e.gameObject.SetActive(true);
                e.enabled = true;
            }
        }
        foreach (Text e in TextElements)
        {
            if (e == null)
                return false;
            e.enabled = true;
        }
        foreach (Transform e in GenericElements)
        {
            if (e == null)
                return false;
            e.gameObject.SetActive(true);
        }
        ElementsEnabled = true;
        return true;
    }

    private void OnEnable()
    {
        if (initialized)
            EnableElements();
    }

    private void OnDisable()
    {
        if (initialized)
            DisableElements();
    }
}
