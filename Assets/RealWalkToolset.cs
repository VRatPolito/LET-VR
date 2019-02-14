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
        _input.OnLeftPadPressed += RightPadPressed;
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
            RotateRight();
        }
       else if (padY < padX && padY <= -padX)   //down
        {
            TeleportPrev();
        }
       else //if(padY > padX && padY <= -padX)  left
        {
            RotateLeft();
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

    private void RotateRight()
    {
        transform.Rotate(new Vector3(0, 90, 0));
    }

    private void TeleportPrev()
    {
        _currPoint--;
        if (_currPoint < 0)
            _currPoint = 0;
        transform.position = _teleportPoints[_currPoint];
    }

    private void RotateLeft()
    {
        transform.Rotate(new Vector3(0, -90, 0));
    }
}
