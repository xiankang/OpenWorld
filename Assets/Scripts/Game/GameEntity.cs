using System.Collections;
using System.Collections.Generic;
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
        _entityDirectory[GetInstanceID()] = gameObject;
    }

    protected virtual void OnDestroy()
    {
        _entityDirectory.Remove(GetInstanceID());
    }
}
