using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BMove : Behavior
{

    public BMove(Brain brain, Unit self) : base(Behavior.EBehaviorType.EBT_MOVE, brain, self)
    {

    }

    public override void Update()
    {
        //
        ActionState goalState = _brain.GetActionState(EActionStateIDs.ASID_MOVING);
        ActionState activeState = _brain.AttemptActionState((uint)EActionStateIDs.ASID_MOVING, 0);

        if (goalState != activeState)
            return;

        Unit unit = _brain.GetUnit();
        if (unit == null)
            return;

        //处理移动逻辑
        Vector3 dirWalk = unit.transform.forward * _v3Dir.z + unit.transform.right * _v3Dir.x;
        dirWalk.y = 0;
        dirWalk.Normalize();
        Vector3 deltaMove = dirWalk * unit.GetMoveSpeed() * Time.deltaTime;
        deltaMove.y = unit.IsGround() ? -0.1f : deltaMove.y - unit._gravity * Time.deltaTime;

        unit.GetCharacterController().Move(deltaMove);

        ASMoving movingState = goalState as ASMoving;
        movingState.SetMovement(deltaMove);
        //这里处理一帧就结束行为来进行处理，之后再优化
        SetFlag(BSR_END | BSR_SUCCESS);
    }
    public override void BeginBehavior()
    {
        if(_self == null || _v3Dir == Vector3.zero)
        {
            Debug.LogError("BMove: behavior started without valid information");
            return;
        }

        if (GetShared())
            return;

        _brain.EndActionStates(1);

        ClearFlag(BSR_NEW);
    }

    public override void EndBehavior()
    {
        _brain.SetMoving(false);
    }

}
