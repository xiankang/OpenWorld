using Core.Utils.Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GameEntity : MonoBehaviour {
    private static Dictionary<int, GameObject> _entityDirectory = new Dictionary<int, GameObject>();



    public static GameObject GetEntity(int instanceId)
    {
        GameObject entity;
        if(_entityDirectory.TryGetValue(instanceId, out entity))
        {
            return entity;
        }

        return null;
    }

    protected virtual void Start()
    {
        _entityDirectory[gameObject.GetInstanceID()] = gameObject;
    }

    protected virtual void OnDestroy()
    {
        _entityDirectory.Remove(GetInstanceID());
    }

    protected virtual void Update()
    {

    }

    public virtual void Spawn()
    {

    }

    public virtual bool IsActive()
    {
        return true;
    }

    public virtual uint GetLevel()
    {
        return 1;
    }

    public virtual uint GetCharges()
    {
        return 0;
    }

    protected virtual T GetAttribute<T>(uint levelIndex, string attributeName)
    {
        Type t = this.GetType();
        PropertyInfo property = t.GetProperty("_" + Lib.FirstCharToLower(attributeName));
        List<T> list = null;
        if (property != null)
            list = (List<T>)property.GetValue(this);
        if (!IsActive() || list == null || list.Count == 0)
            return default(T);
        return list[Math.Min((int)levelIndex, list.Count)];
    }

    protected virtual T GetAttribute<T>(string attributeName)
    {
        uint levelIndex = Math.Max(1u, GetLevel()) - 1;
        return GetAttribute<T>(levelIndex, attributeName);
    }

    protected virtual T GetAttribute<T>(List<T> attributeList)
    {
        uint levelIndex = Math.Max(1u, GetLevel()) - 1;
        return GetAttribute<T>(levelIndex, attributeList);
    }

    protected virtual T GetAttribute<T>(uint levelIndex, List<T> attributeList)
    {
        if (!IsActive() || attributeList == null || attributeList.Count == 0)
            return default(T);
        return attributeList[Math.Min((int)levelIndex, attributeList.Count)];
    }

    protected virtual T GetMultiLevelMutedAttribute<T>(string attributeName) where T : struct
    {
        Type t = this.GetType();
        uint levelIndex = Math.Max(1u, GetLevel()) - 1;
        uint chargePoint = GetCharges();
        float level = 0;
        float charge = 0;
        string standardAttributeName = "_" + Lib.FirstCharToLower(attributeName);
        PropertyInfo propertyPerLevel = t.GetProperty(standardAttributeName + "PerLevel");
        if (propertyPerLevel !=null && levelIndex > 0)
        {
            level = levelIndex * (dynamic)propertyPerLevel.GetValue(this);
        }
        if (chargePoint > 0)
        {
            charge = chargePoint * (dynamic)GetAttribute<T>(levelIndex, attributeName + "PerParge");
        }

        return (dynamic)GetAttribute<T>(levelIndex, attributeName) + level + charge;
    }
}
