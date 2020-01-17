using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothAxes : MonoBehaviour
{
    enum InputMode { Touch, Click };
    SteamVR_TrackedController _controller;
    private Vector2 _axis = Vector2.zero, _target = Vector2.zero;
    [SerializeField]
    private InputMode _mode = InputMode.Click;
    [SerializeField]
    private float _smoothStep = 0.05f;
    private bool _padHeld = false;
    private Vector2 _pad = Vector2.zero;
    // Start is called before the first frame update
    void Awake()
    {
        _controller = GetComponent<SteamVR_TrackedController>();
        switch (_mode)
        {
            case InputMode.Click:
                _controller.PadClicked += PadDown;
                _controller.PadUnclicked += PadUp;
                break;
            case InputMode.Touch:
                _controller.PadTouched += PadDown;
                _controller.PadUntouched += PadUp;
                break;
        }
    }

    private void PadUp(object sender, ClickedEventArgs e)
    {
        _padHeld = false;
        _pad = Vector2.zero;
    }

    private void PadDown(object sender, ClickedEventArgs e)
    {
        _padHeld = true;
    }

    private void Update()
    {
        if (_controller != null)
            {
            float x = 0, y = 0;
            if(_padHeld)
            {
                _pad = _controller.touchPos;
                if (_pad.x > 0)
                    x = 1;
                else if (_pad.x < 0)
                    x = -1;
                if (_pad.y > 0)
                    y = 1;
                else if (_pad.y < 0)
                    y = -1;
            }
            _target = new Vector2(x, y);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        float x = _axis.x;
        float y = _axis.y;
        if (_axis.x > _target.x)
        {
            x = _axis.x - _smoothStep;
            if (x < _target.x)
                x = _target.x;
        }
        else if (_axis.x < _target.x)
        {
            x = _axis.x + _smoothStep;
            if (x > _target.x)
                x = _target.x;
        }
        if (_axis.y > _target.y)
        {
            y = _axis.y - _smoothStep;
            if (y < _target.y)
                y = _target.y;
        }
        else if (_axis.y < _target.y)
        {
            y = _axis.y + _smoothStep;
            if (y > _target.y)
                y = _target.y;
        }
        _axis = new Vector2(x, y);
    }

    public Vector2 GetAxis()
    {
        return _axis;
    }
}
