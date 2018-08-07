using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EUnitCommand
{
    UNITCMD_INVALID,
    UNITCMD_ATTACK,
    UNITCMD_MOVE,
    UNITCMD_STOP,
    UNITCMD_TOOL,
    NUM_UNITCMDS
}

public enum EUnitAction
{
    UNIT_ACTION_ATTACK = 0,
    UNIT_ACTION_CAST,
    UNIT_ACTION_MOVE,
    UNIT_ACTION_STOP,
    UNIT_ACTION_DEATH,
    UNIT_ACTION_INTERRUPT
}

public enum ECommanderOrder
{
    CMDR_ORDER_INVALID = -1,
    CMDR_ORDER_AUTO = 0,
    CMDR_ORDER_CLEAR,
    CMDR_ORDER_MOVE,
    CMDR_ORDER_STOP,
    CMDR_ORDER_HOLD,
    CMDR_ORDER_PATROL,
    CMDR_ORDER_GIVEITEM,
    CMDR_ORDER_TOUCH,
    CMDR_ORDER_ATTACK,
    CMDR_ORDER_FOLLOW,
    CMDR_ORDER_EMOTE
}

public class QueueType
{
    public static byte QUEUE_NONE = 0;
    public static byte QUEUE_BACK = 1;
    public static byte QUEUE_FRONT = 2;
    public static byte QUEUE_FRONT_CLEAR_MOVES = 3;
    public static byte NUM_QUEUE_TYPES = 4;
}




