using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Behavior
{
    public enum EBehaviorType
    {
        EBT_INVALID = 0,
        EBT_ABILITY,
        EBT_ATTACK,
        EBT_MOVE,
    }

    public static readonly uint BSR_NEW = Bit.BIT(0);
    public static readonly uint BSR_MOVING = Bit.BIT(1);
    public static readonly uint BSR_END = Bit.BIT(2);
    public static readonly uint BSR_DEFAULT = Bit.BIT(3);
    public static readonly uint BSR_FORCED = Bit.BIT(4);
    public static readonly uint BSR_RESTRICTED = Bit.BIT(5);
    public static readonly uint BSR_SUCCESS = Bit.BIT(6);
    public static readonly uint BSR_SHARED = Bit.BIT(7);
    public static readonly uint BSR_FAILURE = Bit.BIT(8);

    protected Brain _brain;
    protected uint _flags;
    protected uint _forcedTime;
    protected Unit _self;
    protected uint _endTime;
    private EBehaviorType _type;
    public Vector3 _v3Dir = Vector3.zero;
    protected int _issuedClientNumber = -1;
    protected uint _level = 1;

    public void SetLevel(uint level)
    {
        _level = level;
    }

    public uint GetLevel()
    {
        return _level;
    }

    public void SetIssuedClientNumber(int clientNumber)
    {
        _issuedClientNumber = clientNumber;
    }
    public int GetIssuedClientNumber()
    {
        return _issuedClientNumber;
    }

    public void SetDir(Vector3 dir)
    {
        _v3Dir = dir;
    }
    public Vector3 GetDir()
    {
        return _v3Dir;
    }

    public EBehaviorType GetBType()
    {
        return _type;
    }
    public Behavior(EBehaviorType type, Brain brain, Unit self)
    {
        _type = type;
        _brain = brain;
        _self = self;
    }

    protected void SetFlag(uint flags)
    {
        _flags |= flags;
    }
    public uint GetFlags()
    {
        return _flags;
    }
    protected void ClearFlag(uint flags)
    {
        _flags &= ~flags;
    }

    public void SetDefault(bool isDefault)
    {
        if (isDefault)
        {
            _flags |= BSR_DEFAULT;
        }
        else
        {
            _flags &= ~BSR_DEFAULT;
        }
    }
    public bool GetDefault()
    {
        return (_flags & BSR_DEFAULT) != 0;
    }

    public void SetShared(bool isShared)
    {
        if (isShared)
        {
            _flags |= BSR_SHARED;
        }
        else
        {
            _flags &= ~BSR_SHARED;
        }
    }

    public bool GetShared()
    {
        return (_flags & BSR_SHARED) != 0;
    }

    public void SetMoving(bool isMoving)
    {
        if (isMoving)
        {
            _flags |= BSR_MOVING;
        }
        else
        {
            _flags &= ~BSR_MOVING;
        }
    }

    public bool GetMoving()
    {
        return (_flags & BSR_MOVING) != 0;
    }

    public void SetRestricted(bool isRestricted)
    {
        if (isRestricted)
        {
            _flags |= BSR_RESTRICTED;
        }
        else
        {
            _flags &= ~BSR_RESTRICTED;
        }
    }

    public bool GetRestricted()
    {
        return (_flags & BSR_RESTRICTED) != 0;
    }

    public void SetForced(bool isForced)
    {
        if (isForced)
        {
            SetFlag(BSR_FORCED);
        }
        else
        {
            ClearFlag(BSR_FORCED);
        }
    }
    public void SetForcedTime(uint endTime)
    {
        _forcedTime = endTime;
    }

    public bool IsForced()
    {
        return (_flags & BSR_FORCED) != 0 && _forcedTime > Game._instance.GetGameTime();
    }

    public bool IsEnd()
    {
        return (_flags & BSR_END) != 0;
    }

    public void SetEndTime(uint endTime)
    {
        _endTime = endTime;
    }

    public void Reset()
    {
        _flags = BSR_NEW;
    }

    public virtual bool ShouldReset()
    {
        return (_flags & BSR_END) == 0;
    }

    public abstract void Update();
    public abstract void BeginBehavior();
    public abstract void EndBehavior();
}


