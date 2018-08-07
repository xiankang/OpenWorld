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
}
