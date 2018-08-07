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

        //
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
