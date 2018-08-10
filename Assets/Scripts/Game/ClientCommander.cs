using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientCommander : MonoBehaviour
{

    private uint _commanderIndex = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        //给localplayer发移动指令
        Vector3 moveDir = GetMoveDirInput();
        if (!moveDir.Equals(Vector3.zero))
        {
            GiveOrder(ECommanderOrder.CMDR_ORDER_MOVE, moveDir, QueueType.QUEUE_NONE);
        }

        //施放技能
        if (Input.GetKey(KeyCode.E))
        {
            ActivateTool(0, Game._instance._localPlayer._hero, true, QueueType.QUEUE_BACK);
        }
    }

    void GiveOrder(ECommanderOrder eOrder, Vector3 v3, byte yQueue, uint param = 0)
    {
        Player localPlayer = Game._instance._localPlayer;
        if (localPlayer == null)
            return;

        if (eOrder == ECommanderOrder.CMDR_ORDER_MOVE)
        {
            bool mobile = false;

            if (localPlayer._hero._isMobile)
            {
                mobile = true;
            }

            if (!mobile)
                return;
        }

        switch (eOrder)
        {
            case ECommanderOrder.CMDR_ORDER_MOVE:
                //这里先直接把指令发给player所控制的单位执行，
                if (localPlayer._hero)
                {
                    UnitCommand cmd = new UnitCommand(EUnitCommand.UNITCMD_MOVE);
                    cmd._v3Dir = v3;
                    cmd._param = 0;
                    cmd._yQueue = yQueue;
                    //这里先设置默认0
                    cmd._clientNumber = 0;
                    uint orderSequence = localPlayer._hero.PlayerCommand(cmd);
                }
                break;
            default:
                break;
        }

        _commanderIndex++;
    }
    Vector3 GetMoveDirInput()
    {
        Vector3 dir;
        if (GameEnv._InputManager.JoyStickVec.magnitude > 0)
        {
            dir = new Vector3(GameEnv._InputManager.JoyStickVec.x, 0, GameEnv._InputManager.JoyStickVec.y);
        }
        else
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            dir = new Vector3(h, 0, v);
        }

        return dir;
    }

    public void ActivateTool(byte slot, Unit ownerUnit, bool isClick, byte yQueue)
    {
        if (ownerUnit == null)
            return;

        Tool tool = ownerUnit._inventory.GetTool(slot);
        if (tool == null)
            return;

        if (tool.GetLevel() < 1 && tool.GetMaxLevel() > 0)
            return;

        //不能使用tool,提示
        if (!tool.CanOrder())
        {
            Debug.LogFormat("Can'n order this tool in slot {0}", slot);
            return;
        }

        switch (tool.ActionType)
        {
            case EEntityToolAction.TOOL_ACTION_PASSIVE:
                break;
            case EEntityToolAction.TOOL_ACTION_TOGGLE:
            case EEntityToolAction.TOOL_ACTION_NO_TARGET:
            case EEntityToolAction.TOOL_ACTION_GLOBAL:
            case EEntityToolAction.TOOL_ACTION_TARGET_SELF:
            case EEntityToolAction.TOOL_ACTION_FACING:
            case EEntityToolAction.TOOL_ACTION_SELF_POSITION:
            case EEntityToolAction.TOOL_ACTION_ATTACK_TOGGLE:
                {

                    UnitCommand cmd = new UnitCommand(EUnitCommand.UNITCMD_TOOL);
                    cmd._param = tool.gameObject.GetInstanceID();
                    cmd._yQueue = (byte)yQueue;
                    //这里先设置默认0
                    cmd._clientNumber = 0;
                    uint orderSequence = ownerUnit.PlayerCommand(cmd);
                }
                break;
        }
    }
}
