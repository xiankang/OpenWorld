using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EActionStateIDs
{
    ASID_ATTACKING = 0,
    ASID_MOVING,
    ASID_COUNT
}

public enum EActionStateAnim
{
    IDLE = 0, //空闲
    RUN = 1,//
    JUMP = 2,//
    DEAD = 3,//
    ABILITY1 = 4,//
    ABILITY2 = 5,
    ABILITY3 = 6,
    ABILITY4 = 7
}

public abstract class ActionState {
    public static readonly uint ASR_ACTIVE = Bit.BIT(0);
    public static readonly uint ASR_COMMITTED = Bit.BIT(1);
    public static readonly uint ASR_COMPLETED = Bit.BIT(2);
    public static readonly uint ASR_ALLDONE = Bit.BIT(3);
    public static readonly uint ASR_INTERRUPTED = Bit.BIT(4);
    public static readonly uint ASR_PAUSED = Bit.BIT(5);

    private uint _actionStateFlags = 0;

    protected Brain _brain;

    public ActionState(Brain brain)
    {
        _actionStateFlags = 0;
        _brain = brain;
    }

    public bool IsActive()
    {
        return (_actionStateFlags & ASR_ACTIVE) != 0;
    }

    public bool IsCommitted()
    {
        return (_actionStateFlags & ASR_COMMITTED) != 0;
    }

    public bool IsCompleted()
    {
        return (_actionStateFlags & ASR_COMPLETED) != 0;
    }

    public bool IsAllDone()
    {
        return (_actionStateFlags & ASR_ALLDONE) != 0;
    }

    public bool IsInterrupted()
    {
        return (_actionStateFlags & ASR_INTERRUPTED) != 0;
    }

    public bool IsPaused()
    {
        return (_actionStateFlags & ASR_PAUSED) != 0;
    }

    public Brain GetBrain()
    {
        return _brain;
    }

    public uint GetFlags()
    {
        return _actionStateFlags;
    }

    protected void SetFlag(uint flag)
    {
        //Debug.LogFormat("flag {0}", flag);
        _actionStateFlags |= flag;
    }

    protected void ClearFlag(uint flag)
    {
        //Debug.LogFormat("clear flag {0}", flag);
        _actionStateFlags &= ~flag;
    }

    protected void ClearAllFlags()
    {
        Debug.LogFormat("ClearAllFlags");
        _actionStateFlags = 0;
    }

    void CopyActionStateFrom(ActionState actionState)
    {
        _actionStateFlags = actionState._actionStateFlags;
    }

    public abstract bool BeginState();
    public abstract bool ContinueStateAction();
    public abstract bool EndState(uint priority);
    public void PauseState()
    {
        if (IsActive())
        {
            ClearFlag(ASR_ACTIVE);
            SetFlag(ASR_PAUSED);
        }
    }

    public void UnpauseState()
    {
        if (IsPaused())
        {
            SetFlag(ASR_ACTIVE);
            ClearFlag(ASR_PAUSED);
        }
    }
}
