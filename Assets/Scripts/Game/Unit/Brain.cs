using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour {
    public static readonly uint BS_READ = Bit.BIT(0);
    public static readonly uint BS_PROCESSED = Bit.BIT(1);
    public static readonly uint BS_MOVING = Bit.BIT(2);

    private uint _flags;
    private Deque<UnitCommand> _commands = new Deque<UnitCommand>();
    
    private Deque<Behavior> _behaviors = new Deque<Behavior>();
    private const uint MAX_COMMANDS = 200;
    private const uint MAX_BEHAVIORS = 200;

    private Unit _unit = null;

    private ActionState[] _actionStates = new ActionState[(int)EActionStateIDs.ASID_COUNT];

    public ActionState GetActionState(EActionStateIDs eASID)
    {
        return _actionStates[(int)eASID];
    }

    public ActionState AttemptActionState(uint stateID, uint priority)
    {
        if (!GetReady())
            return null;

        for (int i=0; i < (int)EActionStateIDs.ASID_COUNT; ++i)
        {
            if (_actionStates[i].IsActive())
            {
                if(stateID == i)
                {
                    return _actionStates[i];
                } else if (!_actionStates[i].EndState(priority))
                {
                    //无法结束此action state
                    return _actionStates[i];
                }
            }
        }

        if (!_actionStates[stateID].IsActive())
        {
            if (!_actionStates[stateID].BeginState())
                return null;
        }

        return _actionStates[stateID];
    }
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (!GetReady())
            return;

        Behavior behavior = null;
        if (_behaviors.Count < MAX_BEHAVIORS)
        {
            foreach(UnitCommand cmd in _commands)
            {
                if(_behaviors.Count > 0)
                {
                    behavior = _behaviors.Peek();
                    if(behavior.IsForced() && !cmd._forced)
                    {
                        continue;
                    }
                }

                if (ProcessCommand(cmd))
                {
                    _commands.Erase(cmd);
                    break;
                }
                else
                {
                    _commands.Erase(cmd);
                    continue;
                }
            }
        }

        if (_behaviors.Count > 0)
        {
            behavior = _behaviors.Peek();

            if(behavior.GetBrain() != this)
            {
                Debug.LogWarning("Behavior's unit does not match it's owner's unit");
            }
            if(behavior.GetSelf() != _unit)
            {
                Debug.LogWarning("Behavior's unit does not match brain's unit, the behavior will try to control a different unit");
            }

            if (behavior.Validate())
            {
                if ((behavior.GetFlags() & Behavior.BSR_NEW) != 0)
                    behavior.BeginBehavior();

                behavior.Update();
            }
            
            if (behavior.IsEnd())
            {
                behavior.EndBehavior();

                //这里根据toll判断是否可以被打断
                bool nonInterupting = false;

                _behaviors.Dequeue();

                //reset next behavior
                if(_behaviors.Count>0 && _behaviors.Peek().ShouldReset() && !nonInterupting)
                {
                    _behaviors.Peek().Reset();
                }
            }
        }

        for(int i=0; i<(int)EActionStateIDs.ASID_COUNT; ++i)
        {
            if (_actionStates[i].IsActive())
            {
                if (!_actionStates[i].ContinueStateAction())
                {
                    _actionStates[i].EndState(3);
                }
            }
        }
	}

    public void SetUnit(Unit unit)
    {
        _unit = unit;
    }

    public Unit GetUnit()
    {
        return _unit;
    }

    public void Init()
    {
        if (GetReady())
            return;

        _actionStates[(int)EActionStateIDs.ASID_ATTACKING] = new ASAttacking(this);
        _actionStates[(int)EActionStateIDs.ASID_MOVING] = new ASMoving(this);

        SetReady(true);
    }

    private bool ProcessCommand(UnitCommand cmd)
    {
        if (!GetReady())
            return false;

        EUnitCommand eCmd = cmd._eCommandID;

        if(eCmd == EUnitCommand.UNITCMD_MOVE)
        {
            if (!_unit._isMobile)
            {
                return false;
            }
        }else if(eCmd == EUnitCommand.UNITCMD_ATTACK)
        {
            if (!_unit._canAttack)
            {
                return false;
            }
        }

        Behavior behavior = null;
        if (cmd._yQueue == QueueType.QUEUE_NONE)
        {
            //
            ClearBrainQueue();
        }
        else
        {
            if (_behaviors.Count > 0)
            {
                behavior = _behaviors.Peek();

                if (behavior.GetDefault())
                {

                }
                else
                {
                    bool popFront = true;
                    if (eCmd == EUnitCommand.UNITCMD_TOOL)
                    {
                        GameObject obj = GameEntity.GetEntity(cmd._param);
                        if (obj != null)
                        {
                            Tool tool = obj.GetComponent<Tool>();
                            if (tool != null)
                            {
                                popFront = !tool.NonInterrupting;
                            }
                        }
                    }

                    if (popFront)
                    {
                        behavior.EndBehavior();
                        _behaviors.Dequeue();
                    }
                }
            }
        }

        switch (eCmd)
        {
            case EUnitCommand.UNITCMD_MOVE:
                behavior = new BMove(this, _unit);
                break;
            case EUnitCommand.UNITCMD_ATTACK:
                behavior = new BAttack(this, _unit);
                break;
            case EUnitCommand.UNITCMD_TOOL:
                behavior = new BTool(this, _unit, cmd._param);
                break;
            default:
                Debug.LogWarning("Brain ProcessCommand: Unrecognized Unit Command");
                break;
        }

        if (behavior != null)
        {
            behavior.SetDir(cmd._v3Dir);
            behavior.SetIssuedClientNumber(cmd._clientNumber);
            behavior.SetDefault(cmd._default);
            behavior.SetLevel(cmd._level);

            if (cmd._forced)
            {
                behavior.SetForced(true);
                behavior.SetForcedTime(Game._instance.GetGameTime() + cmd._forcedDuration);
            }
            if (cmd._restricted)
            {
                behavior.SetRestricted(true);
            }
            if (cmd._duration != uint.MaxValue)
            {
                behavior.SetEndTime(Game._instance.GetGameTime() + cmd._duration);
            } else
            {
                behavior.SetEndTime(uint.MaxValue);
            }

            if(cmd._yQueue == QueueType.QUEUE_FRONT)
            {
                if (_behaviors.Count > 0)
                {
                    Behavior front = _behaviors.Peek();
                    if(front!=null && front.GetRestricted())
                    {
                        bool popFront = true;
                        if (eCmd == EUnitCommand.UNITCMD_TOOL)
                        {
                            GameObject obj = GameEntity.GetEntity(cmd._param);
                            if (obj != null)
                            {
                                Tool tool = obj.GetComponent<Tool>();
                                if (tool != null)
                                {
                                    popFront = !tool.NonInterrupting;
                                }
                            }
                        }

                        if (popFront)
                        {
                            front.EndBehavior();
                            _behaviors.Dequeue();
                        }
                    }
                }
                _behaviors.PushFront(behavior);
            } else if(cmd._yQueue == QueueType.QUEUE_FRONT_CLEAR_MOVES)
            {
                foreach(Behavior temp in _behaviors)
                {
                    if (temp == null)
                        continue;

                    if(temp.GetBType() == Behavior.EBehaviorType.EBT_MOVE)
                    {
                        _behaviors.Erase(temp);
                        continue;
                    }
                }
                _behaviors.PushFront(behavior);
            } else
            {
                _behaviors.Enqueue(behavior);
            }

            return true;
        }
        return false;
    }

    public void AddCommand(UnitCommand cmd)
    {
        if (!GetReady())
            return;

        if (_commands.Count >= MAX_COMMANDS)
        {
            if (cmd._yQueue == QueueType.QUEUE_BACK)
                return;

            _commands.Dequeue();
        }

        _commands.Enqueue(cmd);
    }

    private void ClearBrainQueue()
    {
        if (!GetReady())
            return;

        foreach(Behavior behavior in _behaviors)
        {
            behavior.EndBehavior();
        }
        _behaviors.Clear();
    }

    private void SetFlags(uint flags)
    {
        _flags |= flags;
    }
    private void ClearFlags(uint flags)
    {
        _flags &= ~flags;
    }

    private void ClearAllFlags()
    {
        _flags = 0;
    }

    public void SetMoving(bool isMoving)
    {
        if (isMoving)
        {
            _flags |= BS_MOVING;
        } else
        {
            _flags &= ~BS_MOVING;
        }
    }

    public bool GetMoving()
    {
        return (_flags & BS_MOVING) != 0;
    }

    public void SetReady(bool isReady)
    {
        if (isReady)
        {
            _flags |= BS_READ;
        }
         else
        {
            _flags &= ~BS_READ;
        }
    }
    public bool GetReady()
    {
        return (_flags & BS_READ) != 0;
    }

    public uint EndActionStates(uint priority)
    {
        if (!GetReady())
            return 0;

        uint active = 0;
        for(int i=0; i < (int)EActionStateIDs.ASID_COUNT; ++i)
        {
            if (_actionStates[i].IsActive())
            {
                if (!_actionStates[i].EndState(priority))
                    ++active;
            }
        }

        return active;
    }
}
