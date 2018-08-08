using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASMoving : ActionState {

    private Vector3 _movement;
    private uint _moveTime;

    public ASMoving(Brain brain) : base(brain)
    {
        _movement = Vector3.zero;
        _moveTime = uint.MaxValue;
    }

    public override bool BeginState()
    {
        Unit unit = _brain.GetUnit();

        if (!unit)
            return false;

        if (!unit._isMobile)
            return false;

        //if (!IsPaused())
        //{

        //}

        SetFlag(ASR_ACTIVE);
        ClearFlag(ASR_PAUSED);

        unit.Interrupt(EUnitAction.UNIT_ACTION_MOVE);
        return true;
    }

    public override bool ContinueStateAction()
    {
        Unit unit = _brain.GetUnit();
        if (unit == null)
            return false;
        if (unit.IsImmobilized())
            return true;
        if (unit.IsStunned())
            return true;

        if (_movement != Vector3.zero)
        {
            _moveTime = Game._instance.GetGameTime();
        }
        
        if(_movement == Vector3.zero)
        {
            return false;
        }

        //
        _movement = Vector3.zero;

        if(_moveTime == Game._instance.GetGameTime())
        {
            unit.SetAnim(EActionStateAnim.RUN);
        } else
        {
            unit.SetAnim(EActionStateAnim.IDLE);
        }
        
        return true;
    }

    public override bool EndState(uint priority)
    {
        Unit unit = _brain.GetUnit();
        if (unit == null)
            return false;

        //动画
        unit.SetAnim(EActionStateAnim.IDLE);

        ClearFlag(ASR_ACTIVE);
        ClearFlag(ASR_PAUSED);

        return true;
    }

    public void SetMovement(Vector3 movement)
    {
        _movement = movement;
    }
}
