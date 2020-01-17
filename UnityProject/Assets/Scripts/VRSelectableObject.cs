using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRSelectableObject : VRUIElement {

    public delegate void MyDelegate(object c);
    [HideInInspector]
    public MyDelegate OnObjectSelected, OnObjectClicked, OnObjectUnSelected;
    public Transform Highlight;
	// Use this for initialization
	void Start () {
        Highlight.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void ElementSelected()
    {
        Highlight.gameObject.SetActive(true);

        if (OnObjectSelected != null)
        {
            if (c != null)
                OnObjectSelected.Invoke(c);
            /*else if (fp != null)
                OnObjectSelected.Invoke(fp);
            else if (fpi != null)
                OnObjectSelected.Invoke(fpi);*/
        }

        /*if (fp != null)
            fp.OnButtonClicked += Click;
        else */if (c != null)
            c.TriggerClicked += Click;
        /*else if (fpi != null)
            fpi.OnButtonClicked += Click;*/
    }

    public override void Click(object sender, ClickedEventArgs e)
    {
        if (OnObjectClicked != null)
        {
            /*if (fp != null)
                OnObjectClicked(fp);
            else */if (c != null)
                OnObjectClicked(c);
            /*else if (fpi != null)
                OnObjectClicked(fpi);*/
        }
        Highlight.gameObject.SetActive(false);
        base.Click(sender, e);
    }

    public override void ElementUnSelected()
    {
        Highlight.gameObject.SetActive(false);

        if (c != null)
            c.TriggerClicked -= Click;
        /*else if (fp != null)
            fp.OnButtonClicked -= Click;
        else if (fpi != null)
            fpi.OnButtonClicked -= Click;*/

        if (OnObjectUnSelected != null)
            {
                if (c != null)
                    OnObjectUnSelected.Invoke(c);
                /*else if (fp != null)
                    OnObjectUnSelected.Invoke(fp);
                else if (fpi != null)
                    OnObjectUnSelected.Invoke(fpi);*/
            }
    }

    public override void ElementNotActive()
    {
        if (Active)
        {
            if (c != null)
                UnSelected();
            Active = false;
        }
    }

    public override void ElementActive()
    {
        if (!Active)
            Active = true;
    }

}
