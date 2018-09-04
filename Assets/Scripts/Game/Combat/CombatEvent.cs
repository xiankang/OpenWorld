using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEvent{
    int _initiatorIndex;
    public int InitiatorIndex{ set { _initiatorIndex = value;}}
    int _inflictorIndex;
    public int InflictorIndex { set { _inflictorIndex = value; } }
    int _targetIndex;
    public int TargetIndex { get { return _targetIndex; } set { _targetIndex = value; } }
    int _toolIndex;
    public int ToolIndex { get { return _toolIndex; } set { _toolIndex = value; } }
    Vector3 _targetPosition;
    Vector3 _targetDelta;
    public Vector3 Delta { get { return _targetDelta; } set { _targetDelta = value; } }
    int _proxyIndex;
    public int ProxyIndex { get { return _proxyIndex; } set { _proxyIndex = value; } }
    uint _effectType;
    public uint EffectType { get { return _effectType; } set { _effectType = value; } }
    DamageEvent _dmg;
    uint _damageType;
    public uint DamageType { set { _damageType = value; } }
    float _baseDamage;
    public uint BaseDamage { set { _baseDamage = value; } }
    float _additionalDamage;
    public float AdditionalDamage { set { _additionalDamage = value; } }
    float _damageMultiplier;
    public float DamageMultiplier { set { _damageMultiplier = value; } }
    float _bonusMultiplier;
    public float BonusMultiplier { get { return _bonusMultiplier; } set { _bonusMultiplier = value; } }
    float _bonusDamage;
    public float BonusDamage { get { return _bonusDamage; } set { _bonusDamage = value; } }
    float _totalAdjustedDamage;
    public float TotalAdjustedDamage { get { return _totalAdjustedDamage; } }
    float _pierce;
    public float Pierce { get { return _pierce; } set { _pierce = value; } }
    float _piercePercent;
    public float PiercePercent { get { return _piercePercent; } set { _piercePercent = value; } }
    float _armorPierce;
    public float ArmorPierce { get { return _armorPierce; } set { _armorPierce = value; } }
    float _armorPiercePercent;
    public float ArmorPiercePercent { get { return _armorPiercePercent; } set { _armorPiercePercent = value; } }
    //float _magicArmorPierce;
    //float _magicArmorPiercePercent;

    float _armor;
    public float Armor { get { return _armor; } set { _armor = value; } }
    //float _magicArmor;
    //float _mitigation;
    //float _resistance;

    float _lifeSteal;
    public float LifeSteal { get { return _lifeSteal; } set { _lifeSteal = value; } }
    bool _critical;
 
    float _criticalMultiplier;
    public float CriticalMultiplier { get { return _criticalMultiplier; } set { _criticalMultiplier = Mathf.Max(value, 1.0f); } }
    public bool GetIsCritical() { return _criticalMultiplier > 1.0f; }
    float _baseExperience;
    public float BaseExperience { get { return _baseExperience; } set { _baseExperience = value; } }
    float _baseExperienceMultiplier;
    public float BaseExperienceMultiplier { get { return _baseExperienceMultiplier; } set { _baseExperienceMultiplier = value; } }
    float _evasion;
    public float Evasion { get { return _evasion; } set { _evasion = value; } }
    float _missChance;
    public float MissChance { get { return _missChance; } set { _missChance = value; } }
    float _deflection;
    public float Deflection { get { return _deflection; } set { _deflection = value; } }
    bool _nonLethal;
    public bool NonLethal { get { return _nonLethal; } set { _nonLethal = value; } }
    bool _trueStrike;
    public bool TrueStrike { get { return _trueStrike; } set { _trueStrike = value; } } 
    bool _noResponse;
    public bool NoResponse { get { return _noResponse; } set { _noResponse = value; } }
    uint _responseType;
    public uint ResponseType { get { return _responseType; } set { _responseType = value; } }
    int _issuedClientNumber;
    public int IssuedClientNumber { get { return _issuedClientNumber; } set { _issuedClientNumber = value; } }
    bool _negated;
    public bool Negated { get { return _negated; } set { _negated = value; } }
    bool _successful;
    public bool Successful { get { return _successful; } }
    bool _invalid;
    public bool Invalid { get { return _invalid; } set { _invalid = value; } }

    //activate
    float _manaCost;
    public float ManaCost { get { return _manaCost; } set { _manaCost = value; } }
    uint _cooldownTime;
    public uint CooldownTime { get { return _cooldownTime; } set { _cooldownTime = value; } }
    public CombatEvent()
    {

    }

    public void Process()
    {
        if (_targetIndex != int.MaxValue)
        {
            GameObject targetObj = GameEntity.GetEntity(_targetIndex);
            if (targetObj != null)
                _targetPosition = targetObj.transform.position;
        }

        if (!PreImpact())
            return;
        Impact();
        PostImpact();
        _successful = true;
    }

    public bool PreImpact()
    {
        Unit target = GameEntity.GetGameEntity<Unit>(_targetIndex);
        Unit initiator = GameEntity.GetGameEntity<Unit>(_initiatorIndex);
        GameEntity proxy = GameEntity.GetGameEntity<GameEntity>(_proxyIndex);
        GameEntity inflictor = GameEntity.GetGameEntity<GameEntity>(_inflictorIndex);
        GameEntity tool = GameEntity.GetGameEntity<GameEntity>(_toolIndex);

        OnPreImpact();


        return true;
    }

    public void Impact()
    {
        OnPreDamage();

        OnDamageEvent();
        OnImpact.Invoke();
    }

    public void PostImpact()
    {
        
    }

    public delegate void OnPreImpactDelegate();
    public delegate void OnPreDamageDelegate();
    public delegate void OnDamageEventDelegate();
    public delegate void OnImpactDelegate();
    public delegate void OnImpactInvalidDelegate();

    public OnPreImpactDelegate OnPreImpact;
    public OnPreDamageDelegate OnPreDamage;
    public OnDamageEventDelegate OnDamageEvent;
    public OnImpactDelegate OnImpact;
    public OnImpactInvalidDelegate OnImpactInvalid;
}
