using Core.Utils;
using Core.Utils.Log;
using Core.Asset.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Asset
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager _Instance = null;
        public static float _lastReleaseTime = 0;
        public static float Interval;
        public static bool IsClearAsset = true;
        public static long AllocatedMB;
        public const long MemoryMb = 1024 * 1024;
        public static float RecoveryTime = 5.0f;
        public static bool DisableObjectPool = false;
#if UNITY_EDITOR
        public const uint LimitMemory = 500;
#elif UNITY_ANDROID
        public const uint LimitMemory = 400;
#else
        public const uint LimitMemory = 200;
#endif

        public static IEnumerator Init()
        {
            if (!Application.isPlaying)
                yield break;

            var resManager = FindObjectOfType(typeof(ResourceManager)) as ResourceManager;
            if (resManager == null)
            {
                var gameObj = new GameObject("_ResourceManager");
                resManager = gameObj.AddComponent<ResourceManager>();
                _Instance = resManager;
            }

            var child = new GameObject("_Pool");
            child.transform.SetParent(resManager.transform, false);
            child.SetActive(false);
            ResourcePoolManager._poolParent = child.transform;

            AsyncImageDownload.Init();

            _lastReleaseTime = Time.time;
            _Instance.StartCoroutine(OnReleaseUnusedAssets());

            if (AppEnv.IsUseAB)
            {
                yield return BundleManager.InitCoreBundle();
            }
        }

        public static IEnumerator InitGameResource(string gameName)
        {
            if (AppEnv.IsUseAB)
            {
                yield return BundleManager.InitGameBundle(gameName);
            }
            yield return 1;
        }

        public static void UnInitGameResource()
        {
            if (AppEnv.IsUseAB)
            {
                BundleManager.UnInitGameBundle();
            }
            ResourcePoolManager.ClearPools();
        }

        public static Object InstantiateObject(Object original)
        {
            return Object.Instantiate(original);
        }

        public static Object InstantiateObject(Object original, Vector3 pos)
        {
            return Object.Instantiate(original, pos, Quaternion.identity);
        }

        public static TextAsset LoadTextAsset(string path)
        {
            return (TextAsset)LoadAsset(path, typeof(TextAsset), false);
        }

        public static byte[] LoadText(string path)
        {
            TextAsset assetObj = LoadTextAsset(path);
            if (assetObj == null)
            {
                return null;
            }
            return assetObj.bytes;
        }

        public static Sprite LoadSprite(string path)
        {
            Sprite assetObj = (Sprite)LoadAsset(path, typeof(Sprite), false);
            return assetObj;
        }

        public static Texture2D LoadTexture2D(string path)
        {
            Texture2D assetObj = (Texture2D)LoadAsset(path, typeof(Texture2D), false);
            return assetObj;
        }

        public static GameObject LoadPrefab(string path)
        {
            var instance = GetCacheObject(path);
            if (instance != null)
                return instance;

            Object obj = LoadAsset(path, typeof(GameObject), false);
            if (obj == null)
                return null;
            return InstantiateObject(obj) as GameObject;
        }

        public static GameObject LoadPrefabAndSetPos(string path, Vector3 pos)
        {
            var instance = GetCacheObjectAndSetPos(path, pos);
            if (instance != null)
                return instance;

            Object obj = LoadAsset(path, typeof(GameObject), false);
            if (obj == null)
                return null;
            return InstantiateObject(obj, pos) as GameObject;
        }

        public static Material LoadMaterial(string path)
        {
            Material assetObj = (Material)LoadAsset(path, typeof(Material), false);
            return assetObj;
        }

        public static AudioClip LoadAudioClip(string path)
        {
            AudioClip assetObj = (AudioClip)LoadAsset(path, typeof(AudioClip), false);
            return assetObj;
        }

        public static RuntimeAnimatorController LoadAnimatorController(string path)
        {
            RuntimeAnimatorController assetObj = (RuntimeAnimatorController)LoadAsset(path, typeof(RuntimeAnimatorController), false);
            return assetObj;
        }

        public static AnimationClip LoadAnimationClip(string path)
        {
            AnimationClip assetObj = (AnimationClip)LoadAsset(path, typeof(AnimationClip), false);
            return assetObj;
        }

        //public static byte[] LoadLuaFile(string path)
        //{
        //    byte[] content = null;

        //    if (!AppEnv.IsUseAB)
        //    {
        //        if (!path.EndsWith(".lua"))
        //            path += ".lua";

        //        string fullPath = $"{Application.dataPath}/{path}";
        //        FileHelper.LoadFileByFileStream(fullPath, out content);
        //    }
        //    else
        //    {
        //        path = path.Replace(".lua", "");
        //        content = BundleManager.LoadLuaFile(path);
        //    }

        //    return content;
        //}

        public static void AddCacheObject(string path, int count)
        {
            ResourcePool resourcePool;
            if (ResourcePoolManager._resourcePoolDict.TryGetValue(path, out resourcePool))
                return;

            Object asset = LoadAsset(path, typeof(GameObject), false);
            var obj = InstantiateObject(asset) as GameObject;
            resourcePool = new ResourcePool(obj, ResourcePoolManager._poolParent);
            ResourcePoolManager._resourcePoolDict.Add(path, resourcePool);

            for (int i = 0; i < count; i++)
            {
                resourcePool.Despawn(InstantiateObject(asset) as GameObject);
            }
        }

        public static void AddCacheObjectAsync(string path, Action<int> _CachedDoneCallback)
        {
            ResourcePool resourcePool;
            if (ResourcePoolManager._resourcePoolDict.TryGetValue(path, out resourcePool))
                return;

            _Instance.StartCoroutine(LoadAssetAsync(path, typeof(GameObject), _CachedDoneCallback, true));
        }

        public static GameObject GetCacheObject(string path)
        {
            ResourcePool resourcePool;
            if (!ResourcePoolManager._resourcePoolDict.TryGetValue(path, out resourcePool))
                return null;
            var obj = resourcePool.Spawn();
            if (obj != null)
            {
                obj.transform.parent = null;
            }
            return obj;
        }

        public static GameObject GetCacheObjectAndSetPos(string path, Vector3 pos)
        {
            ResourcePool resourcePool;
            if (!ResourcePoolManager._resourcePoolDict.TryGetValue(path, out resourcePool))
                return null;
            var obj = resourcePool.SpawnAndSetPos(pos);
            if (obj != null)
            {
                obj.transform.parent = null;
            }
            return obj;
        }

        public static IEnumerator LoadWWWTexture(string path, AssetHandle handle)
        {
#if UNITY_STANDALONE_WIN || UNITY_WP_8_1 || UNITY_WSA
            var fullpath = "file:///" + path;
#else
            string fullpath = "file://" + path;
#endif
            var www = new WWW(fullpath);
            yield return www;

            handle.Asset = www.texture;

            handle.OnCallBack();
        }

        private static IEnumerator LoadAssetAsync(string assetPath, Type type, Action<int> _CachedDoneCallback, bool shouldCacheAsset = false)
        {
            if (string.IsNullOrEmpty(assetPath))
                yield break;

            var extensions = ResourceExtensions.GetExtOfType(type);
            foreach (var ext in extensions)
            {
                var assetPathWithExt = assetPath + ext;
                Object asset = null;
                ResourcePool resourcePool;
                if (ResourcePoolManager._resourcePoolDict.TryGetValue(assetPathWithExt, out resourcePool))
                {
                    asset = resourcePool.Resource as GameObject;
                    (asset as GameObject).SetActive(true);
                    var obj = InstantiateObject(asset) as GameObject;
                    resourcePool = new ResourcePool(obj, ResourcePoolManager._poolParent);
                    ResourcePoolManager._resourcePoolDict.Add(assetPath, resourcePool);
                    _CachedDoneCallback?.Invoke(ResourcePoolManager._resourcePoolDict.Count);
                    yield break;
                }

                var hasCached = false;
                if (!AppEnv.IsUseAB)
                {
#if UNITY_EDITOR
                    _Instance.StartCoroutine(AssetManager.LoadFromAssetDatabaseAsync(assetPathWithExt, type, resourcePool, shouldCacheAsset, hasCached, _CachedDoneCallback));
#endif
                }
                else
                {
                    _Instance.StartCoroutine(BundleManager.LoadFromAssetBundleAsync(assetPathWithExt, type, resourcePool, shouldCacheAsset, hasCached, _CachedDoneCallback));
                }
            }
        }

        private static Object LoadAsset(string assetPath, Type type, bool shouldCacheAsset = false)
        {
            if (string.IsNullOrEmpty(assetPath))
                return null;

            var extensions = ResourceExtensions.GetExtOfType(type);
            foreach (var ext in extensions)
            {
                // 先找缓存
                var assetPathWithExt = assetPath + ext;
                ResourcePool resourcePool;
                if (ResourcePoolManager._resourcePoolDict.TryGetValue(assetPathWithExt, out resourcePool))
                {
                    (resourcePool.Resource as GameObject).SetActive(true);
                    return resourcePool.Resource;
                }

                var hasCached = false;
                Object asset = null;
                if (!AppEnv.IsUseAB)
                {
#if UNITY_EDITOR
                    asset = AssetManager.LoadFromAssetDatabase(assetPathWithExt, type);
#endif
                }
                else
                {
                    asset = BundleManager.LoadFromAssetBundle(assetPathWithExt, type);
                }

                if (asset == null)
                    continue;

                if (shouldCacheAsset && !hasCached)
                {
                    resourcePool = new ResourcePool(asset, ResourcePoolManager._poolParent);
                    ResourcePoolManager._resourcePoolDict.Add(assetPathWithExt, resourcePool);
                }

                return asset;
            }

            LogHelper.INFO("ResourceManager", "loading file {0} failed, type {1}", assetPath, type);
            return null;
        }

        private static IEnumerator OnReleaseUnusedAssets()
        {
            while (true)
            {
                Interval = Time.time - _lastReleaseTime;

                var fTime = 15.0f;
                if (IsClearAsset)
                    fTime = 3.0f;

                if (Interval > fTime)
                {
                    _lastReleaseTime = Time.time;

                    var allocatedSize = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();
                    AllocatedMB = allocatedSize / MemoryMb;
                    if (AllocatedMB > LimitMemory)
                    {
                        yield return ClearResourcePoolsByTimeAsync(RecoveryTime, IsClearAsset);
                        UnloadUnusedAssets(false, false);
                    }
                }

                yield return new WaitForSeconds(1.0f);
            }
        }

        private static void UnloadUnusedAssets(bool forceGc = false, bool clearPool = false)
        {
#if UNITY_EDITOR
            var watch = Stopwatch.StartNew();
            long totalTime = 0;
            long clearPoolTime = 0;
            long unloadResTime = 0;
            long gcTime = 0;
#endif
            ResourceManager._lastReleaseTime = Time.time;
            if (clearPool)
            {
                ResourcePoolManager.ClearPools();
            }

#if UNITY_EDITOR
            watch.Stop();
            totalTime += watch.ElapsedMilliseconds;
            clearPoolTime = watch.ElapsedMilliseconds;
            watch.Reset();
            watch.Start();
#endif
            Resources.UnloadUnusedAssets();
#if UNITY_EDITOR
            watch.Stop();
            totalTime += watch.ElapsedMilliseconds;
            unloadResTime = watch.ElapsedMilliseconds;
            watch.Reset();
            watch.Start();
#endif
            if (forceGc)
            {
                GC.Collect(0, GCCollectionMode.Forced);
            }

#if UNITY_EDITOR
            watch.Stop();
            totalTime += watch.ElapsedMilliseconds;
            gcTime = watch.ElapsedMilliseconds;
            watch.Reset();
            watch.Start();

            watch.Stop();
            LogHelper.WARN(
                "ResManager", "UnloadUnusedAssets({0}, {1}) used {2}ms(CP{3},UL{4},GC{5})",
                forceGc ? "GC" : "--", clearPool ? "CP" : "--", totalTime, clearPoolTime, unloadResTime, gcTime);
#endif
        }

        private static IEnumerator ClearResourcePoolsByTimeAsync(float fTime, bool bClearAsset)
        {
            var watch = Stopwatch.StartNew();
            var curTime = Time.time;

            var resources = new List<string>(ResourcePoolManager._resourcePoolDict.Keys);
            for (var i = 0; i < resources.Count; i++)
            {
                watch.Stop();
                if (watch.ElapsedMilliseconds > 2)
                {
                    yield return null;
                    watch.Reset();
                }
                watch.Start();

                ResourcePool resourcePool = null;
                if (ResourcePoolManager._resourcePoolDict.TryGetValue(resources[i], out resourcePool))
                {
                    if (resourcePool != null && curTime - resourcePool.LastUseTime > fTime)
                    {
                        resourcePool.Clear();
                        ResourcePoolManager._resourcePoolDict.Remove(resources[i]);
                    }
                }
            }
        }

        void Update()
        {
        }

        void LateUpdate()
        {
        }

        void OnDestroy()
        {
            ResourcePoolManager.ClearPools();
        }
    }
}
