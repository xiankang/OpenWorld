using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASAttacking : ActionState {

    private int _toolIndex;
    private Vector3 _v3TargetPostion;
    private Vector3 _v3Delta;
    private int _issuedClientNumber;


    public void SetToolIndex(int index) { _toolIndex = index; }
    public void SetTargetPosition(Vector3 pos) { _v3TargetPostion = pos; }
    public void SetIssuedClientNumber(int clientNumber) { _issuedClientNumber = clientNumber; }
    public void SetDelta(Vector3 delta) { _v3Delta = delta; }

    private Tool GetTool()
    {
        GameObject obj = GameEntity.GetEntity(_toolIndex);
        return obj ? obj.GetComponent<Tool>() : null;
    }

    public ASAttacking(Brain brain) : base(brain)
    {

    }

    public override bool BeginState()
    {
        Unit unit = _brain.GetUnit();
        if (unit == null)
            return false;

        Tool tool = GetTool();
        if (tool == null)
            return false;
        if (tool.IsDisabled())
            return false;
        if (unit.IsStunned() && !tool.NoStun)
            return false;

        if (!IsPaused())
            Reset();

        SetFlag(ASR_ACTIVE);
        ClearFlag(ASR_PAUSED);
        return true;
    }
    public override bool ContinueStateAction()
    {
        Unit unit = _brain.GetUnit();
        Tool tool = GetTool();
        if (unit == null || tool == null)
            return false;

        tool.SetFlag(Tool.ENTITY_TOOL_FLAG_IN_USE);

        if((GetFlags() & ASR_COMPLETED) == 0 && !tool.CanOrder())
            return false;

        if (tool.IsDisabled())
            return false;

        if(unit.IsStunned() && !tool.NoStun)
        {
            SetFlag(ASR_INTERRUPTED);
            return false;
        }

        bool ret = true;

        //动画
        if (unit.GetAnim()!= EActionStateAnim.ATTACK)
        {
            unit.SetAnim(EActionStateAnim.ATTACK);
        }
        //及调用tool的activate
        if((GetFlags() & ASR_COMPLETED) == 0)
        {
            tool.Activate(null, _v3TargetPostion, _v3Delta, _issuedClientNumber);
            SetFlag(ASR_COMPLETED);
        }

        if((GetFlags() & ASR_COMPLETED) != 0)
        {
            SetFlag(ASR_ALLDONE);
            ret = false;
        }

        return ret;
    }
    public override bool EndState(uint priority)
    {
        Unit unit = _brain.GetUnit();
        Tool tool = GetTool();
        bool ret = false;

        if ((GetFlags() & ASR_ALLDONE)!=0)
        {
            Reset();
            ret = true;
        }
         else if((GetFlags() & ASR_COMPLETED)!=0 && priority > 0)
        {
            ClearFlag(ASR_ACTIVE);
            ClearFlag(ASR_PAUSED);
            ret = true;
        } else if((GetFlags() & ASR_COMPLETED) != 0 && priority < 2)
        {
            ret = false;
        } else if((~GetFlags() & ASR_COMPLETED) !=0 && priority > 1)
        {
            bool interrupted = (GetFlags() & ASR_INTERRUPTED) != 0;
            Reset();
            ret = true;
            //停止动画
            if (interrupted)
                SetFlag(ASR_INTERRUPTED);
        } else if((GetFlags() & ASR_COMPLETED) != 0 && priority > 2)
        {
            bool interrupted = (GetFlags() & ASR_INTERRUPTED) != 0;
            Reset();
            ret = true;
            //停止动画
            if (interrupted)
                SetFlag(ASR_INTERRUPTED);
        }

        if(ret && tool != null)
        {
            tool.ClearFlag(Tool.ENTITY_TOOL_FLAG_IN_USE);
        }
        return ret;
    }

    private void Reset()
    {
        ClearAllFlags();
    }
}
