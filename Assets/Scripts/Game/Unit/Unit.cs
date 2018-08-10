using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Utils.Log;
using System;
using Core.Utils.Lib;

public class Unit : VisualEntity {
    public static readonly ushort UNIT_FLAG_INVULNERABLE = (ushort)Bit.BIT(0);
    public static readonly ushort UNIT_FLAG_BOUND = (ushort)Bit.BIT(1);
    public static readonly ushort UNIT_FLAG_IGNORE_ATTACK_COOLDOWN = (ushort)Bit.BIT(2);
    public static readonly ushort UNIT_FLAG_STEALTH = (ushort)Bit.BIT(3);
    public static readonly ushort UNIT_FLAG_ILLUSION = (ushort)Bit.BIT(4);
    public static readonly ushort UNIT_FLAG_TERMINATED = (ushort)Bit.BIT(5);
    public static readonly ushort UNIT_FLAG_UNCONTROLLABLE = (ushort)Bit.BIT(6);
    public static readonly ushort UNIT_FLAG_REVEALED = (ushort)Bit.BIT(7);
    public static readonly ushort UNIT_FLAG_LOCKED_BACKPACK = (ushort)Bit.BIT(8);
    public static readonly ushort UNIT_FLAG_TEAM_SHARE = (ushort)Bit.BIT(9);
    public static readonly ushort UNIT_FLAG_NOT_CONTROLLABLE = (ushort)Bit.BIT(10);
    public static readonly ushort UNIT_FLAG_LAST_PROTECTED_DEATH = (ushort)Bit.BIT(11);
    public static readonly ushort UNIT_FLAG_DYNAMIC_POWER = (ushort)Bit.BIT(12);
    public static readonly ushort UNIT_FLAG_DEATH_ON_DAMAGE = (ushort)Bit.BIT(13);

    protected ushort _unitFlags;
    public bool _isMobile = true;
    public bool _canAttack = true;
    public Inventory _inventory = null;
    public Brain _brain = null;
    public uint _orderSequence = 1;
    public float _moveSpeed = 2.5f;
    public float _gravity = 17.5f;

    public List<float> _maxMana;
    public float _maxManaPerLevel = 0;
    public float BaseMaxMana
    {
        get
        {
            return GetMultiLevelMutedAttribute<float>("MaxMana");
        }
    }
    public float BaseMaxManaMultiplier
    {
        get
        {
            return 1.0f;
        }
    }
    protected float _mana = 0;
    protected float _currentMaxMana = 0;

    private CharacterController _characterController = null;
    // Use this for initialization
    protected override void Start () {
        base.Start();
        if (_inventory == null)
        {
            _inventory = transform.Find("inventory").GetComponent<Inventory>();
        }
        if (_brain == null)
        {
            _brain = transform.Find("brain").GetComponent<Brain>();
        }

        _characterController = gameObject.GetComponent<CharacterController>();
        if (!_characterController)
            LogHelper.FATAL("Unit", "no Character Controller");

        _brain.SetUnit(this);
        _brain.Init();
        Spawn();
    }
	
	// Update is called once per frame
	protected override void Update() {
		
	}

    public CharacterController GetCharacterController()
    {
        return _characterController;
    }

    public uint PlayerCommand(UnitCommand cmd)
    {
        cmd._orderSequence = _orderSequence;
        ++_orderSequence;
        _brain.AddCommand(cmd);
        return cmd._orderSequence;
    }

    public virtual void Interrupt(EUnitAction eAction)
    {
        foreach(Slave slave in _inventory.SlaveList)
        {
            Tool tool = slave as Tool;
            if(tool !=null && tool.IsActive()){
                tool.Interrupt(eAction);
            }

            EntityState state = slave as EntityState;
            if(state != null)
            {
                state.Interrupt(eAction);
            }
        }

    }

    public virtual bool IsImmobilized()
    {
        return false;
    }

    public virtual bool IsStunned()
    {
        return false;
    }

    public bool IsGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.1f, ~(1 << 26));
    }

    public float GetMoveSpeed()
    {
        return _moveSpeed + _inventory.GetMoveSpeed();
    }

    public virtual float GetMaxCooldownSpeed()
    {
        float maxCooldownSpeed = GetBaseMaxCooldownSpeed() + _inventory.GetMaxCooldownSpeed();
        return Mathf.Clamp(maxCooldownSpeed, 0, 1.0f);
    }

    public virtual float GetBaseCooldownSpeed()
    {
        return 0.0f;
    }

    public virtual float GetBaseMaxCooldownSpeed()
    {
        return 1.0f;
    }

    public virtual float GetCooldownSpeed()
    {
        float cooldownSpeed = GetBaseCooldownSpeed() + _inventory.GetCooldownSpeed();
        return Mathf.Clamp(cooldownSpeed, 0, GetMaxCooldownSpeed());
    }

    public float GetManaCostReduction()
    {
        float reduction = 0.0f;
        foreach(Slave slave in _inventory.SlaveList)
        {
            if (slave.IsActive())
                reduction += slave.ManaCostReduction;
        }

        return Mathf.Clamp(reduction, 0.0f, 1.0f);
    }

    public float GetManaCostMultiplier()
    {
        float multiplier = 1.0f;
        foreach(Slave slave in _inventory.SlaveList)
        {
            if (slave.IsActive())
                multiplier += slave.ManaCostMultiplier - 1.0f;
        }

        return Mathf.Max(multiplier, 1.0f);
    }

    public virtual float GetMana()
    {
        return _mana;
    }

    public override void Spawn()
    {
        base.Spawn();
        SetStatus((byte)EEntityStatus.ENTITY_STATUS_ACTIVE);
        _mana = _currentMaxMana = GetMaxMana();
    }

    public virtual float GetMaxManaMultiplier()
    {
        return BaseMaxManaMultiplier + _inventory.GetMaxManaMultiplier();
    }

    public virtual float GetMaxMana()
    {
        float bonus = _inventory.GetMaxMana();
        return Mathf.Floor((BaseMaxMana + bonus) * Mathf.Max(0.0f, GetMaxManaMultiplier()));
    }

    protected override T GetMultiLevelMutedAttribute<T>(string attributeName)
    {
        //是否是变身状态，如果是变身状态，则取变身状态的值
        //if(_morphState != null && _morphState.GetApply)
        //    return _morphState.
        return base.GetMultiLevelMutedAttribute<T>(attributeName);
    }

    public virtual bool IsFreeCast()
    {
        return _inventory.GetFreeCast();
    }


    public void RemoveUnitFlags(ushort flags)
    {
        _unitFlags &= (ushort)~flags;
    }

    public void SetUnitFlags(ushort flags)
    {
        _unitFlags |= flags;
    }

    public bool HasUnitFlags(ushort flags)
    {
        return (_unitFlags & flags) == flags;
    }

    public ushort GetUnitFlags()
    {
        return _unitFlags;
    }

    public virtual bool IsIllusion()
    {
        return HasUnitFlags(UNIT_FLAG_ILLUSION);
    }

}
