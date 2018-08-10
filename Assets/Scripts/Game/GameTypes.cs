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

public enum EEntityToolAction
{
    TOOL_ACTION_INVALID,

    TOOL_ACTION_PASSIVE,
    TOOL_ACTION_TOGGLE,
    TOOL_ACTION_NO_TARGET,
    TOOL_ACTION_TARGET_POSITION,
    TOOL_ACTION_TARGET_ENTITY,
    TOOL_ACTION_GLOBAL,
    TOOL_ACTION_TARGET_SELF,
    TOOL_ACTION_FACING,
    TOOL_ACTION_SELF_POSITION,
    TOOL_ACTION_ATTACK,
    TOOL_ACTION_ATTACK_TOGGLE,
    TOOL_ACTION_TARGET_DUAL,
    TOOL_ACTION_TARGET_DUAL_POSITION,
    TOOL_ACTION_TARGET_VECTOR,
    TOOL_ACTION_TARGET_ENTITY_VECTOR,
    TOOL_ACTION_TARGET_CURSOR,
};

public class DefaultValue<T>
{
    public static T GetDefaultEmptyValue()
    {
        T t = default(T);
        return t;
    }
}

//public class DefaultValue<float>
//{
//    public static float GetDefaultEmptyValue() { return 0.0f; }
//}





