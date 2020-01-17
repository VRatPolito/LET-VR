using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputConsoleManager : MonoBehaviour
    {
    public static InputConsoleManager Instance;
    public VRButtonController KeyEnter;
    [HideInInspector]
    public VRInputFieldController CurrentField;
    VRUICanvas Console;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    // Use this for initialization
    void Start () {
        Console = GetComponent<VRUICanvas>();
    }

    public void EnableConsole(VRInputFieldController input)
    {
        CurrentField = input;
        Console.EnableElements();
    }

    public void InputRight()
    {
    if (CurrentField != null)
        CurrentField.MoveCursorRight();
    }

    public void InputLeft()
    {
    if (CurrentField != null)
    CurrentField.MoveCursorLeft();
    }
    
    public void InputEnter()
    {
    if (CurrentField != null)
        CurrentField.Close();
    CurrentField = null;
    if(KeyEnter.OnButtonUnSelected != null)
        KeyEnter.OnButtonUnSelected.Invoke(this);
    Console.DisableElements();
    }

    public void InputLocal()
    {
    if (CurrentField != null)
    { 
    CurrentField.InputString("localhost");
    CurrentField.Close();
    }
    }

    public void InputBack()
    {
    if (CurrentField != null)
    CurrentField.DeleteChar();
    }

    public void InputDot()
    {
    if (CurrentField != null)
    CurrentField.InputChar('.');
    }

    public void Input9()
    {
    CurrentField.InputChar('9');
    }

    public void Input8()
    {
    CurrentField.InputChar('8');
    }

    public void Input7()
    {   
    CurrentField.InputChar('7');
    }

    public void Input6()
    {
    CurrentField.InputChar('6');
    }

    public void Input5()
    {
    CurrentField.InputChar('5');
    }

    public void Input4()
    {
    CurrentField.InputChar('4');
    }

    public void Input3()
    {
    CurrentField.InputChar('3');
    }

    public void Input2()
    {
    CurrentField.InputChar('2');
    }

    public void Input1()
    {
    CurrentField.InputChar('1');
    }

    public void Input0()
    {
    CurrentField.InputChar('0');
    }

    // Update is called once per frame
    void Update () {
		
	}
}
