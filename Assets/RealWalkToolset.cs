﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealWalkToolset : MonoBehaviour
{
    [SerializeField]
    private List<Vector3> _teleportPoints;
    private InputManagement _input;
    private int _currPoint = 0;

    private void Awake()
    {
        _input = GetComponent<InputManagement>();
        _input.OnLeftPadPressed += LeftPadPressed;
        _input.OnRightPadPressed += RightPadPressed;
        _input.OnLeftPadTouched += LeftPadTouched;
        _input.OnRightPadTouched += RightPadTouched;
    }


    private void LeftPadPressed(object sender, ClickedEventArgs arg)
    {
        if (arg.padY > -arg.padX && arg.padY >= arg.padX)         //up
        {
            Translate2D(0, 1f);
        }
        else if (arg.padY < arg.padX && arg.padY >= -arg.padX)   //right
        {
            Translate2D(1f, 0);
        }
        else if (arg.padY < arg.padX && arg.padY <= -arg.padX)   //down
        {
            Translate2D(0, -1f);
        }
        else //if(padY > padX && padY <= -padX)  left
        {
            Translate2D(-1f, 0);
        }
    }

    private void LeftPadTouched(object sender, ClickedEventArgs arg)
    {
        if (arg.padY > -arg.padX && arg.padY >= arg.padX)         //up
        {
            Translate2D(0, .1f);
        }
        else if (arg.padY < arg.padX && arg.padY >= -arg.padX)   //right
        {
            Translate2D(.1f, 0);
        }
        else if (arg.padY < arg.padX && arg.padY <= -arg.padX)   //down
        {
            Translate2D(0, -.1f);
        }
        else //if(padY > padX && padY <= -padX)  left
        {
            Translate2D(-.1f, 0);
        }
    }

    private void RightPadTouched(object sender, ClickedEventArgs arg)
    {
        if (arg.padY > -arg.padX && arg.padY >= arg.padX)         //up
        {
        }
        else if (arg.padY < arg.padX && arg.padY >= -arg.padX)   //right
        {
            Rotate(1);
        }
        else if (arg.padY < arg.padX && arg.padY <= -arg.padX)   //down
        {
        }
        else //if(padY > padX && padY <= -padX)  left
        {
            Rotate(-1);
        }
    }

    private void RightPadPressed(object sender, ClickedEventArgs arg)
    {
       if(arg.padY > -arg.padX && arg.padY >= arg.padX)         //up
        {
            TeleportNext();
        }
       else if (arg.padY < arg.padX && arg.padY >= -arg.padX)   //right
        {
            Rotate(15);
        }
       else if (arg.padY < arg.padX && arg.padY <= -arg.padX)   //down
        {
            TeleportPrev();
        }
       else //if(padY > padX && padY <= -padX)  left
        {
            Rotate(-15);
        }
    }

    private void TeleportNext()
    {
        if(_teleportPoints.Count > 0)
        {
            _currPoint++;
            if (_currPoint > _teleportPoints.Count - 1)
                _currPoint = _teleportPoints.Count - 1;
            transform.position = _teleportPoints[_currPoint];
        }
    }

    private void TeleportPrev()
    {
        _currPoint--;
        if (_currPoint < 0)
            _currPoint = 0;
        transform.position = _teleportPoints[_currPoint];
    }

    private void Rotate(float a)
    {
        transform.Rotate(new Vector3(0, a, 0));
    }

    private void Translate2D(float x, float z)
    {
        transform.Translate(new Vector3(x, 0, z));
    }
}
