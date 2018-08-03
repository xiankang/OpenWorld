using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Utils.Log;

namespace Core.Asset.Pool
{
    public class ResourcePoolManager
    {
        public static Transform _poolParent;
        public static Dictionary<string, ResourcePool> _resourcePoolDict = new Dictionary<string, ResourcePool>();

        public static Object GetResource(string assetPathWithExt)
        {
            ResourcePool resourcePool;
            if (_resourcePoolDict.TryGetValue(assetPathWithExt, out resourcePool))
                return resourcePool.Resource;

            return null;
        }

        public static void ClearResourcePools()
        {
            foreach(var iterator in _resourcePoolDict)
            {
                var resourcePool = iterator.Value;
                resourcePool.Clear();
            }

            _resourcePoolDict.Clear();

            if (_poolParent != null)
            {
                for(int i=_poolParent.childCount -1; i >= 0; i--)
                {
                    GameObject.Destroy(_poolParent.GetChild(i).gameObject);
                }
            }
        }

        public static void ClearPools()
        {
#if UNITY_EDITOR
            LogHelper.WARN("ResourcePoolManager", "ClearPools resourcePoolDict.size :" + _resourcePoolDict.Count);
#endif
            ClearResourcePools();
        }
    }
}

