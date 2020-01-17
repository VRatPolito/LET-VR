using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualDoorController : DoorController
{


    private bool _gateOpened = false;

    public bool GateOpened
    {
        get { return _gateOpened; }
        set
        {
            _gateOpened = value;
            if (PlayerInRange)
                OpenGate(_playerInRange);
        
            
        }
    }

    protected override bool PlayerInRange
    {
        get { return _playerInRange; }
        set
        {
            if (_playerInRange == value) return;
            _playerInRange = value;
            if (!_playerInRange || _gateOpened)
                OpenGate(_playerInRange && _gateOpened);
        }
    }

    protected override void OpenGate(bool openOrClose)
    {
        base.OpenGate(openOrClose);
        _gateOpened = openOrClose;
    }

}
