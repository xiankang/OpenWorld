using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCommand {
    public int _clientNumber = -1;
    public bool _forced = false;
    public uint _forcedDuration = uint.MaxValue;
    public bool _restricted = false;
    public uint _level = 1;
    public uint _duration = uint.MaxValue;
    public byte _yQueue = QueueType.QUEUE_NONE;
    public EUnitCommand _eCommandID = EUnitCommand.UNITCMD_INVALID;
    public int _param = 0;
    public uint _orderSequence = uint.MaxValue;
    public Vector3 _v3Dir = Vector3.zero;
    public bool _default = false;

    public UnitCommand()
    {
        _eCommandID = EUnitCommand.UNITCMD_INVALID;
    }

    public UnitCommand(EUnitCommand eCommandID)
    {
        _eCommandID = eCommandID;
    }
}
