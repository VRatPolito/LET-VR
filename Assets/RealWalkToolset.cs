using System;
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

    private void LeftPadTouched(object sender, ClickedEventArgs arg)
    {
        PadTouched(arg.padX, arg.padY);
    }

    private void RightPadTouched(object sender, ClickedEventArgs arg)
    {
        PadTouched(arg.padX, arg.padY);
    }

    private void PadTouched(float padX, float padY)
    {
        if (padY > -padX && padY >= padX)         //up
        {
        }
        else if (padY < padX && padY >= -padX)   //right
        {
            Rotate(1);
        }
        else if (padY < padX && padY <= -padX)   //down
        {
        }
        else //if(padY > padX && padY <= -padX)  left
        {
            Rotate(-1);
        }
    }

    private void RightPadPressed(object sender, ClickedEventArgs arg)
    {
        PadPressed(arg.padX, arg.padY);
    }

    private void LeftPadPressed(object sender, ClickedEventArgs arg)
    {
        PadPressed(arg.padX, arg.padY);
    }

    private void PadPressed(float padX, float padY)
    {
       if(padY > -padX && padY >= padX)         //up
        {
            TeleportNext();
        }
       else if (padY < padX && padY >= -padX)   //right
        {
            Rotate(15);
        }
       else if (padY < padX && padY <= -padX)   //down
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
}
