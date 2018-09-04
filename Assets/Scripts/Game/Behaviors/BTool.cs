using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTool : Behavior
{
    public static readonly uint BSR_CAST = Bit.BIT(9);

    private int _index = 0;
    private Tool _tool;

    public BTool(Brain brain, Unit self, int index) : base(Behavior.EBehaviorType.EBT_ABILITY, brain, self)
    {
        _index = index;
    }

    public override void Update()
    {
        if (_self.IsStunned() || _self.IsImmobilized())
            return;

        Vector3 v3Pos = _self.GetPosition();

        switch (_tool.ActionType)
        {
            case EEntityToolAction.TOOL_ACTION_PASSIVE:
            case EEntityToolAction.TOOL_ACTION_TOGGLE:
            case EEntityToolAction.TOOL_ACTION_NO_TARGET:
            case EEntityToolAction.TOOL_ACTION_TARGET_POSITION:
            case EEntityToolAction.TOOL_ACTION_TARGET_ENTITY:
            case EEntityToolAction.TOOL_ACTION_GLOBAL:
            case EEntityToolAction.TOOL_ACTION_TARGET_SELF:
            case EEntityToolAction.TOOL_ACTION_FACING:
            case EEntityToolAction.TOOL_ACTION_SELF_POSITION:
            case EEntityToolAction.TOOL_ACTION_ATTACK:
            case EEntityToolAction.TOOL_ACTION_ATTACK_TOGGLE:
            case EEntityToolAction.TOOL_ACTION_TARGET_DUAL:
            case EEntityToolAction.TOOL_ACTION_TARGET_DUAL_POSITION:
            case EEntityToolAction.TOOL_ACTION_TARGET_VECTOR:
            case EEntityToolAction.TOOL_ACTION_TARGET_ENTITY_VECTOR:
            case EEntityToolAction.TOOL_ACTION_TARGET_CURSOR:
                break;
            case EEntityToolAction.TOOL_ACTION_INVALID:
                Debug.LogWarning("Invalid tool action type");
                SetFlag(BSR_END);
                return;
        }

        ASAttacking activatingState = _brain.GetActionState(EActionStateIDs.ASID_ATTACKING) as ASAttacking;
        if (activatingState.IsActive())
            return;

        if((GetFlags() & BSR_CAST) != 0)
        {
            ClearFlag(BSR_CAST);
            if (!activatingState.IsInterrupted())
                SetFlag(BSR_END);
            return;
        }

        BeginCast(_self.transform.forward);

        Debug.Log("111111111111111111111111111111");
        if((GetFlags() & BSR_CAST)!=0 && !activatingState.IsActive())
        {
            Debug.Log("22222222222222222222222222222222");
            ClearFlag(BSR_CAST);
            if (!activatingState.IsInterrupted())
            {
                SetFlag(BSR_END);
                Debug.Log("BTool BSR_END");
            }
        }
    }

    private void BeginCast(Vector3 v3Target)
    {
        Debug.Log("BTool BeginCast");
        ActionState goalState = _brain.GetActionState(EActionStateIDs.ASID_ATTACKING);
        ASAttacking asAttacking = goalState as ASAttacking;
        asAttacking.SetToolIndex(_index);
        asAttacking.SetTargetPosition(v3Target);
        asAttacking.SetIssuedClientNumber(_issuedClientNumber);
        ActionState activeState = _brain.AttemptActionState((uint)EActionStateIDs.ASID_ATTACKING, 0);
        
        if(activeState != goalState)
        {
            SetFlag(BSR_END);
            return;
        }
        SetFlag(BSR_CAST);
    }

    public override void BeginBehavior()
    {
        if (_self == null || _tool == null)
        {
            Debug.LogWarning("BTool: Behavior started without valid information");
            return;
        }

        if (_tool.NonInterrupting)
        {
            if (_brain.GetActionState(EActionStateIDs.ASID_ATTACKING).IsActive())
                _brain.GetActionState(EActionStateIDs.ASID_ATTACKING).EndState(1);
            _brain.GetActionState(EActionStateIDs.ASID_MOVING).PauseState();
        }
        else
        {
            _brain.EndActionStates(1);
        }

        ClearFlag(BSR_NEW);
    }


    public override void EndBehavior()
    {
    }

    public bool IsNonInterupting()
    {
        if (_tool == null)
            return false;
        return _tool.NonInterrupting;
    }

    public override bool Validate()
    {
        if (!base.Validate())
        {
            SetFlag(BSR_END);
            return false;
        }

        if (_self.IsIllusion())
        {
            SetFlag(BSR_END);
            return false;
        }

        GameObject obj = GameEntity.GetEntity(_index);
        _tool = obj != null ? obj.GetComponent<Tool>() : null;
        if(_tool == null)
        {
            SetFlag(BSR_END);
            return false;
        }

        if (_tool.IsDisabled())
        {
            SetFlag(BSR_END);
            return false;
        }

        return true;
    }

}
