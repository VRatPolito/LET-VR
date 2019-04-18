using System;
using System.Collections;
using System.Net;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class VRInputFieldController : VRUIElement {
    //InputConsoleManager ic = null;
    InputField Field;
    int _myCaret = 0;
    int MyCaret
        {
        get {
            if (Field.caretPosition != _myCaret)
                Field.caretPosition = _myCaret;
            return Field.caretPosition; }
        set
        {
            Field.caretPosition = _myCaret = value;
        }
        }
    bool InputActive = false;


    public override void Awake()
    {
        //var canvas = transform.Find("Canvas");
        Field = GetComponent<InputField>();
        base.Awake();
    }

    internal void MoveCursorRight()
    {
        if(MyCaret < Field.characterLimit +1)
            MyCaret++;        
    }

    //NON FUNZIONA
    /*void SetCaretVisible(int pos)
    {
        Field.caretPosition = pos; // desired cursor position

        Field.GetType().GetField("m_AllowInput", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Field, true);
        Field.GetType().InvokeMember("SetCaretVisible", BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance, null, Field, null);

    }*/


    internal void MoveCursorLeft()
    {
        if (MyCaret > 0)
            MyCaret--;
    }

    internal void Close()
    {
        InputActive = false;
        UnSelected();
        Field.DeactivateInputField();
    }

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }

    public override void ElementSelected()
    {
        /*if (fp != null)
            fp.OnButtonClicked += Click;
        else */if (c != null)
            c.TriggerClicked += Click;

        Field.Select();
    }

    public override void Click(object sender, ClickedEventArgs e)
    {
        if (!InputActive)
        {
            Field.ActivateInputField();
            InputConsoleManager.Instance.EnableConsole(this);
            InputActive = true;
        }
        MyCaret = Field.text.Length;
        //SetCaretVisible(MyCaret);
    }

    internal void InputString(string v)
    {
        int c = MyCaret;
        if (v.Length > Field.characterLimit)
        {
            var s = v.Substring(0, Field.characterLimit);
            c = s.Length + 1;
            Field.text = s;
            MyCaret = c;
            return;
        }
        c = v.Length + 1;
        Field.text = v;
        MyCaret = c;
    }

    internal void DeleteChar()
    {
        if(Field.text.Length > 0)
        {
            var text = Field.text;
            string v = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (i != MyCaret - 1)
                    v = String.Concat(v, text[i]);
            }
            int c = MyCaret;
            if(MyCaret > 0)
                c--;
            Field.text = v;
            MyCaret = c;
        }
    }

    internal void InputChar(char c)
    {
    if (Field.text.Length == Field.characterLimit)
        return;
    var text = Field.text;
    string v = "";
    if (MyCaret == text.Length)
        {
            Field.text = String.Concat(text, c);
            MyCaret++;
            return;
        }
    for (int i = 0; i < Field.text.Length; i++)
        {
            if (i == MyCaret)
                v = String.Concat(v, c);
            v = String.Concat(v, Field.text[i]);
        }
        Field.text = v;
        MyCaret++;
    }
    
    internal bool IsValidIP()
    {
        if (Field.text == "")
            return false;
        if (Field.text == "localhost")
            return true;

        try
            {
                IPAddress ServerIP;
                IPAddress.TryParse(Field.text, out ServerIP);
            }
        catch
            {
                return false;
            }
        return true;
    }

    public override void OnTriggerEnter(Collider other)
    {
    if (Active && other.gameObject.tag == "Controller" && c == null)
        {
            if (!InputActive)
            {
                c = other.GetComponent<SteamVR_TrackedController>();
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
    }

    internal void SetField(string v)
    {
        Field.text = v;
        MyCaret = v.Length+1;
    }

    internal void ClearField()
    {
        Field.text = string.Empty;
        MyCaret = 0;    
    }

    internal string GetIP()
    {
        if (IsValidIP())
            return Field.text;
        else
            return "localhost";
    }

    public override void OnTriggerExit(Collider other)
    {
        if (Active && other.gameObject.tag == "Controller" && c != null)
        {
            if (!InputActive)
            {
                var i = c.GetComponent<ControllerManager>();
                if (i != null)
                    i.StopPulsePublic();
                UnSelected();
            }
        }
    }

    public override void ElementUnSelected()
    {
        /*if (fp != null)
        {
            fp.OnButtonClicked -= Click;
            fp = null;
        }
        else */if (c != null)
        { 
            c.TriggerClicked -= Click;
            c = null;
        }
        //InputActive = false;
        //base.ElementUnSelected();
    }

    public override void ElementNotActive()
    {
        if (Active)
        {
            UnSelected();
            Active = false;
        }
    }

    public override void ElementActive()
    {
        if (!Active)
        {
            Active = true;
        }
    }
}
