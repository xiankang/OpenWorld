using System;
using Core.Asset.Pool;
using Object = UnityEngine.Object;
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core.Asset
{
    public class AssetManager
    {
        public static Object LoadFromAssetDatabase(string assetPathWithExt, Type type)
        {
            // assetPath=Brawl/Avatar/keven.prefab
            // type=GameObject
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath(ResourceDefine.ASSET_ROOT_PATH + assetPathWithExt, type);
#else
            return null;
#endif
        }

        public static IEnumerator LoadFromAssetDatabaseAsync(string assetPathWithExt, Type type, ResourcePool resourcePool, bool shouldCacheAsset, bool hasCached, Action<int> _CachedDoneCallback)
        {
#if UNITY_EDITOR
            Object obj = LoadFromAssetDatabase(assetPathWithExt, type);
            yield return obj;

            if (shouldCacheAsset && !hasCached)
            {
                if (obj != null)
                {
                    resourcePool = new ResourcePool(Object.Instantiate(obj as GameObject), ResourcePoolManager._poolParent);
                    ResourcePoolManager._resourcePoolDict.Add(assetPathWithExt, resourcePool);
                    _CachedDoneCallback?.Invoke(ResourcePoolManager._resourcePoolDict.Count);
                }
            }
#else
            yield return 1;
#endif
        }
    }
}
