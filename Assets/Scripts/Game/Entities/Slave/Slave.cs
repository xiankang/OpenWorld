using Core.Utils.Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Slave : GameEntity {
    public static readonly uint SLAVE_ACTIVE = Bit.BIT(0);
    public static readonly uint SLAVE_ACTIVE_NO_MUTE = Bit.BIT(1);
    public List<uint> _maxCharges;
    public List<uint> _maxChargesPerCharge;
    public uint _maxChargesPerLevel = 0;
    protected uint _slaveFlags = 0;
    protected int _ownerIndex = 0;
    protected uint _level = 1;
    protected uint _realLevel = 1;
    protected uint _yCharges = 0;
    protected float _accumulator;
    protected int _proxyUID;
    protected int _spawnerUID;

    protected float _maxShield;
    protected Inventory _inventory;
    protected CombatAction _combatAction;

    public List<float> _manaCostReduction;
    public float ManaCostReduction
    {
        get
        {
            return GetAttribute<float>(_manaCostReduction);
        }
    }
    public List<float> _manaCostMultiplier;
    public float ManaCostMultiplier
    {
        get
        {
            return GetAttribute<float>(_manaCostMultiplier);
        }
    }

    public void SetInventory(Inventory inventory)
    {
        _inventory = inventory;
    }


    public override bool IsActive()
    {
        return (_slaveFlags & SLAVE_ACTIVE) != 0;
    }

    public bool IsActiveNoMuteCheck()
    {
        return (_slaveFlags & SLAVE_ACTIVE_NO_MUTE) != 0;
    }

    public virtual void UpdateIsActive() { }

    public virtual void SetLevel(uint level)
    {
        _level = level;
        uint oldRealLevel = _realLevel;
        UpdateRealLevel();
        if(_realLevel != oldRealLevel)
        {
            MarkInventoryPropertyDirty();
        }
    }

    protected void MarkInventoryPropertyDirty()
    {
        if (_inventory)
            _inventory.UpdateProperty();
    }
    public void UpdateRealLevel()
    {
        if(GetMaxLevel() == 0)
        {
            if (_level == 0)
                _realLevel = 1;
            else
                _realLevel = _level + GetBaseLevel();
        } else
        {
            _realLevel = Math.Min(_level + GetBaseLevel(), GetMaxLevel());
        }
    }

    public virtual uint GetMaxLevel() { return 0; }
    public virtual uint GetBaseLevel() { return 0; }
    public override uint GetLevel()
    {
        return _realLevel;
    }

    public virtual void LevelUp() { }
    public virtual bool CanLevelUp() { return false; }

    public virtual uint GetInitialCharges() { return 0; }
    public override uint GetCharges() { return _yCharges; }
    public virtual void SetCharges(uint charges)
    {
        uint oldCharges = _yCharges;
        _yCharges = charges;
        if (oldCharges != _yCharges)
        {
            MarkInventoryPropertyDirty();
        }
    }
    public virtual void RemoveCharge()
    {
        uint oldCharges = _yCharges;
        if (_yCharges > 0)
            --_yCharges;
        if (oldCharges != _yCharges)
            MarkInventoryPropertyDirty();
    }
    public virtual void RemoveCharges(uint charges)
    {
        uint oldCharges = _yCharges;
        if ((int)(_yCharges - charges) > 0)
            _yCharges -= charges;
        else
            _yCharges = 0;
        if (oldCharges != _yCharges)
            MarkInventoryPropertyDirty();
    }
    public virtual void AddCharges(uint charges)
    {
        uint oldCharges = _yCharges;
        if (GetMaxCharges() == -1)
            _yCharges += charges;
        else if (_yCharges < GetMaxCharges())
            _yCharges = Math.Min(_yCharges + charges, (uint)GetMaxCharges());

        if (oldCharges != _yCharges)
            MarkInventoryPropertyDirty();
    }

    public virtual void ModifyCharges(int charges)
    {
        if (charges > 0)
            AddCharges((uint)charges);
        else
            RemoveCharges((uint)(-charges));
    }

    public virtual int GetMaxCharges()
    {
        return GetMultiLevelMutedAttribute<int>("MaxCharges");
    }

    public virtual uint GetMaxChargesPerCharge(uint levelIndex)
    {
        return GetAttribute<uint>(levelIndex, "MaxChargesPerCharge");
    }
 

    public int GetOwnerIndex() { return _ownerIndex; }
    public void SetOwnerIndex(int ownerIndex) { _ownerIndex = ownerIndex; }

    public Unit GetOwner() {
        GameObject obj = GetEntity(_ownerIndex);
        if (obj)
            return obj.GetComponent<Unit>();
        return null;
    }

    public virtual float GetManaCostReductionFactor()
    {
        Unit owner = GetOwner();
        if (!owner)
            return 1.0f;
        return 1.0f - owner.GetManaCostReduction();
    }

    public virtual float GetManaCostMultiplierFactor()
    {
        Unit owner = GetOwner();
        if (!owner)
            return 1.0f;
        return owner.GetManaCostMultiplier();
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
	}

    // Update is called once per frame
    protected override void Update () {
        base.Update();
	}

    public override void Spawn()
    {
        _combatAction = gameObject.GetComponent<CombatAction>();
    }
}
