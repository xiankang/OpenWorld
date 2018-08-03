using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;
using Core.Asset.Pool;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ABFileStream : FileStream
{
    public ABFileStream(string path, FileMode mode, FileAccess access) : base(path, mode, access)
    {

    }
    public override int Read(byte[] array, int offset, int count)
    {
        int n = base.Read(array, offset, count);
        for (int i = 0; i < n; i += 1)
        {
            byte tmp = array[i];
            array[i] = (byte)(tmp ^ '&');
        }

        //Debug.LogErrorFormat("Read Count={0}", n);
        return n;
    }
}

namespace Core.Asset
{
    public class BundleManager
    {
        public static AssetBundle _CoreBundle = null;
        public static AssetBundle _GameMainBundle = null;
        public static AssetBundle _GameSceneBundle = null;

        public static IEnumerator InitCoreBundle()
        {
            if (AppEnv.IsDevMode)
            {
                string coreBundlePath = GetCoreBundlePath();
                WWW www = new WWW(coreBundlePath);
                yield return www;

                if (string.IsNullOrEmpty(www.error))
                {
                    // 解密代码 
                    byte[] sourcedata = www.bytes;
                    float before_msTime = Time.realtimeSinceStartup * 1000;
                    byte[] result = EnAndDecode.EncodeAndDecode.Decode(sourcedata);
                    float after_msTime = Time.realtimeSinceStartup * 1000;
                    Debug.Log("Decode cast time is : " + (after_msTime - before_msTime).ToString());
                    // 从内存中创建 AssetBundle 
                    Debug.Log("start loadAssetBundle for Memory");
                    _CoreBundle = AssetBundle.LoadFromMemory(result);
                    Debug.LogFormat("InitCoreBundle Success Path={0} Count={1}", coreBundlePath, _CoreBundle.GetAllAssetNames().GetLength(0));
                }
                else
                {
                    Debug.LogErrorFormat("InitCoreBundle Error={0} Path={1}", www.error, coreBundlePath);
                }
            }
            else
            {
                string coreBundlePath = GetCoreBundlePath();
                Debug.LogFormat("Begin InitCoreBundle Path={0}", coreBundlePath);
                //AssetBundleCreateRequest bundleLoadRequest = null;
                //try
                //{
                //    ABFileStream fileStream = new ABFileStream(coreBundlePath, FileMode.Open, FileAccess.Read);
                //    fileStream.ReadTimeout = 100;
                //    if (fileStream.CanRead)
                //    {
                //        bundleLoadRequest = AssetBundle.LoadFromStreamAsync(fileStream);
                //    }
                //    else
                //    {
                //        Debug.LogFormat("core can not be read {0}", fileStream);
                //    }
                //}
                //catch(Exception _e)
                //{
                //    Debug.LogFormat("LoadFromStreamAsync exception {0}", _e.StackTrace);
                //}

                //while (bundleLoadRequest != null && !bundleLoadRequest.isDone)
                //{
                //    yield return null;    // 协程等待
                //}

                //if (bundleLoadRequest != null)
                //{
                //    _CoreBundle = bundleLoadRequest.assetBundle;
                //    if (_CoreBundle != null)
                //    {
                //        Debug.LogFormat("InitCoreBundle Success Path={0} Count={1}", coreBundlePath, _CoreBundle.GetAllAssetNames().GetLength(0));
                //    }
                //    else{
                //        Debug.LogError("!!!!!!!!!!!!!!!!!!!!!_CoreBundle is null!!!!!!!!!!!!!!!!!!!!!");
                //    }
                //}
                try
                {
                    ABFileStream fileStream = new ABFileStream(coreBundlePath, FileMode.Open, FileAccess.Read);
                    _CoreBundle = AssetBundle.LoadFromStream(fileStream);
                    if (_CoreBundle != null)
                    {
                        Debug.LogFormat("InitCoreBundle Success Path={0} Count={1}", coreBundlePath, _CoreBundle.GetAllAssetNames().GetLength(0));
                    }
                    else
                    {
                        Debug.LogError("!!!!!!!!!!!!!!!!!!!!!_CoreBundle is null!!!!!!!!!!!!!!!!!!!!!");
                    }
                }
                catch (Exception _e)
                {
                    Debug.LogFormat("LoadFromStream exception {0}", _e.StackTrace);
                }
            }
        }

        public static IEnumerator InitGameBundle(string gameName)
        {
            if (AppEnv.IsDevMode)
            {
                string gameMainBundlePath = GetGameMainBundlePath(gameName);
                WWW www = new WWW(gameMainBundlePath);
                yield return www;

                if (string.IsNullOrEmpty(www.error))
                {
                    // 解密代码 
                    byte[] sourcedata = www.bytes;
                    float beforGameTime = Time.realtimeSinceStartup * 1000;
                    byte[] result = EnAndDecode.EncodeAndDecode.Decode(sourcedata);
                    float afterGameTime = Time.realtimeSinceStartup * 1000;
                    Debug.Log("Decode the game assetBundle cast time " + (afterGameTime - beforGameTime).ToString());
                    _GameMainBundle = AssetBundle.LoadFromMemory(result);
                    Debug.LogFormat("InitGameMainBundle Success Path={0} Count={1}", gameMainBundlePath, _GameMainBundle.GetAllAssetNames().GetLength(0));
                }
                else
                {
                    Debug.LogErrorFormat("InitGameMainBundle Error={0} Path={1}", www.error, gameMainBundlePath);
                }

                string gameSceneBundlePath = GetGameSceneBundlePath(gameName);
                WWW www2 = new WWW(gameSceneBundlePath);
                yield return www2;

                if (string.IsNullOrEmpty(www2.error))
                {
                    // 解密 scene Assetbundle 
                    byte[] sceneSourceData = www2.bytes;
                    float bSceneTime = Time.realtimeSinceStartup * 1000;
                    byte[] sceneResult = EnAndDecode.EncodeAndDecode.Decode(sceneSourceData);
                    float aSceneTime = Time.realtimeSinceStartup * 1000;
                    Debug.Log("Decode the scene assetBundle cast time : " + (aSceneTime - bSceneTime).ToString());
                    _GameSceneBundle = AssetBundle.LoadFromMemory(sceneResult);
                    Debug.LogFormat("InitGameSceneBundle Success Path={0} Count={1}", gameSceneBundlePath, _GameSceneBundle.GetAllAssetNames().GetLength(0));
                }
                else
                {
                    Debug.LogErrorFormat("InitGameSceneBundle Error={0} Path={1}", www2.error, gameSceneBundlePath);
                }
            }
            else
            {
                string gameMainBundlePath = GetGameMainBundlePath(gameName);
                ABFileStream fileStreamMain = new ABFileStream(gameMainBundlePath, FileMode.Open, FileAccess.Read);
                AssetBundleCreateRequest bundleLoadRequestMain = AssetBundle.LoadFromStreamAsync(fileStreamMain);
                while (!bundleLoadRequestMain.isDone)
                {
                    yield return null;    // 协程等待
                }

                _GameMainBundle = bundleLoadRequestMain.assetBundle;
                Debug.LogFormat("InitCoreBundle Success Path={0} Count={1}", gameMainBundlePath, _GameMainBundle.GetAllAssetNames().GetLength(0));

                string gameSceneBundlePath = GetGameSceneBundlePath(gameName);
                ABFileStream fileStreamScene = new ABFileStream(gameSceneBundlePath, FileMode.Open, FileAccess.Read);
                AssetBundleCreateRequest bundleLoadRequestScene = AssetBundle.LoadFromStreamAsync(fileStreamScene);
                while (!bundleLoadRequestScene.isDone)
                {
                    yield return null;    // 协程等待
                }

                _GameSceneBundle = bundleLoadRequestScene.assetBundle;
                Debug.LogFormat("InitCoreBundle Success Path={0} Count={1}", gameSceneBundlePath, _GameSceneBundle.GetAllAssetNames().GetLength(0));
            }
        }

        public static void UnInitGameBundle()
        {
            if (_GameSceneBundle != null)
            {
                _GameSceneBundle.Unload(true);
                _GameSceneBundle = null;
            }

            if (_GameMainBundle != null)
            {
                _GameMainBundle.Unload(true);
                _GameMainBundle = null;
            }
        }

        public static Object LoadFromAssetBundle(string assetPathWithExt, Type type)
        {
            // assetPath=Game/Brawl/Res/Avatar/keven.prefab
            // type=GameObject

            Object asset = null;

            //Debug.LogWarning("====> assetPathWithExt : " + assetPathWithExt + "type : " + type);

            if (assetPathWithExt.StartsWith("Core/"))
            {
                asset = _CoreBundle.LoadAsset(ResourceDefine.ASSET_ROOT_PATH + assetPathWithExt, type);
            }
            else if (assetPathWithExt.StartsWith("Game/"))
            {
                asset = _GameMainBundle.LoadAsset(ResourceDefine.ASSET_ROOT_PATH + assetPathWithExt, type);
            }
            return asset;
        }

        public static IEnumerator LoadFromAssetBundleAsync(string assetPathWithExt, Type type, ResourcePool resourcePool, bool shouldCacheAsset, bool hasCached, Action<int> _CachedDoneCallback)
        {
            AssetBundleRequest request = null;
            if (assetPathWithExt.StartsWith("Core/"))
            {
                request = _CoreBundle.LoadAssetAsync(ResourceDefine.ASSET_ROOT_PATH + assetPathWithExt, type);
            }
            else if (assetPathWithExt.StartsWith("Game/"))
            {
                request = _GameMainBundle.LoadAssetAsync(ResourceDefine.ASSET_ROOT_PATH + assetPathWithExt, type);
            }
            yield return request;

            if (shouldCacheAsset && !hasCached)
            {
                if (request.asset != null && request.isDone)
                {
                    resourcePool = new ResourcePool(Object.Instantiate(request.asset as GameObject), ResourcePoolManager._poolParent);
                    ResourcePoolManager._resourcePoolDict.Add(assetPathWithExt, resourcePool);
                    _CachedDoneCallback?.Invoke(ResourcePoolManager._resourcePoolDict.Count);
                }
            }
        }

        public static byte[] LoadLuaFile(string assetPathWithOutExt)
        {
            TextAsset asset = null;
            if (assetPathWithOutExt.StartsWith("Core/Lua/"))
            {
                asset = _CoreBundle.LoadAsset(ResourceDefine.ASSET_ROOT_PATH + assetPathWithOutExt + ".txt", typeof(TextAsset)) as TextAsset;
            }
            else if (assetPathWithOutExt.StartsWith("Game/"))
            {
                asset = _GameMainBundle.LoadAsset(ResourceDefine.ASSET_ROOT_PATH + assetPathWithOutExt + ".txt", typeof(TextAsset)) as TextAsset;
            }
            return asset.bytes;
        }

        private static string GetCoreBundlePath()
        {
            if (AppEnv.IsDevMode)// streaming
            {
                // 加密之后不再使用 unity3d 的后缀
#if UNITY_EDITOR
                return "file://" + Application.streamingAssetsPath + "/core/core.unity3d";
#elif UNITY_STANDALONE_WIN
                return "file://" + Application.streamingAssetsPath + "/core/core.unity3d";
#elif UNITY_ANDROID
                return Application.streamingAssetsPath + "/core/core.unity3d";
#elif UNITY_IOS
                return "file://" + Application.streamingAssetsPath + "/core/core.unity3d";
#endif
            }
            else // persistent
            {
#if UNITY_EDITOR
                return Application.persistentDataPath + "/core/core.unity3d";
#elif UNITY_STANDALONE_WIN
                return Application.persistentDataPath + "/core/core.unity3d";
#elif UNITY_ANDROID
                return Application.persistentDataPath + "/core/core.unity3d";
#elif UNITY_IOS
                return Application.persistentDataPath + "/core/core.unity3d";
#endif
            }
        }

        private static string GetGameMainBundlePath(string gameName)
        {
            if (AppEnv.IsDevMode)// streaming
            {
#if UNITY_EDITOR
                return $"file://{Application.streamingAssetsPath}/game/{gameName.ToLower()}/{gameName.ToLower()}.unity3d";
#elif UNITY_STANDALONE_WIN
                return $"file://{Application.streamingAssetsPath}/game/{gameName.ToLower()}/{gameName.ToLower()}.unity3d";
#elif UNITY_ANDROID
                return $"{Application.streamingAssetsPath}/game/{gameName.ToLower()}/{gameName.ToLower()}.unity3d";
#elif UNITY_IOS
                return $"file://{Application.streamingAssetsPath}/game/{gameName.ToLower()}/{gameName.ToLower()}.unity3d";
#endif
            }
            else // persistent
            {
#if UNITY_EDITOR
                return $"{Application.persistentDataPath}/game/{gameName.ToLower()}/{gameName.ToLower()}.unity3d";
#elif UNITY_STANDALONE_WIN
                return $"{Application.persistentDataPath}/game/{gameName.ToLower()}/{gameName.ToLower()}.unity3d";
#elif UNITY_ANDROID
                return $"{Application.persistentDataPath}/game/{gameName.ToLower()}/{gameName.ToLower()}.unity3d";
#elif UNITY_IOS
                return $"{Application.persistentDataPath}/game/{gameName.ToLower()}/{gameName.ToLower()}.unity3d";
#endif
            }
        }

        private static string GetGameSceneBundlePath(string gameName)
        {
            if (AppEnv.IsDevMode)// streaming
            {
#if UNITY_EDITOR
                return $"file://{Application.streamingAssetsPath}/game/{gameName.ToLower()}/{gameName.ToLower()}_scenes.unity3d";
#elif UNITY_STANDALONE_WIN
                return $"file://{Application.streamingAssetsPath}/game/{gameName.ToLower()}/{gameName.ToLower()}_scenes.unity3d";
#elif UNITY_ANDROID
                return $"{Application.streamingAssetsPath}/game/{gameName.ToLower()}/{gameName.ToLower()}_scenes.unity3d";
#elif UNITY_IOS
                return $"file://{Application.streamingAssetsPath}/game/{gameName.ToLower()}/{gameName.ToLower()}_scenes.unity3d";
#endif
            }
            else // persistent
            {
#if UNITY_EDITOR
                return $"{Application.persistentDataPath}/game/{gameName.ToLower()}/{gameName.ToLower()}_scenes.unity3d";
#elif UNITY_STANDALONE_WIN
                return $"{Application.persistentDataPath}/game/{gameName.ToLower()}/{gameName.ToLower()}_scenes.unity3d";
#elif UNITY_ANDROID
                return $"{Application.persistentDataPath}/game/{gameName.ToLower()}/{gameName.ToLower()}_scenes.unity3d";
#elif UNITY_IOS
                return $"{Application.persistentDataPath}/game/{gameName.ToLower()}/{gameName.ToLower()}_scenes.unity3d";
#endif
            }
        }
    }
}
