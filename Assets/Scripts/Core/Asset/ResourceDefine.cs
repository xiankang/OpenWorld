using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core.Asset
{
    public class ResourceDefine
    {
        public static string ASSET_ROOT_PATH = "Assets/";
        public static string AB_ROOT_PATH { get; private set; }
        public const String ASSET_FILE_HEAD = "file://";

        static ResourceDefine()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    AB_ROOT_PATH = "windows";
#if UNITY_EDITOR
                    switch (EditorUserBuildSettings.activeBuildTarget)
                    {
                        case BuildTarget.iOS:
                            AB_ROOT_PATH = "iphone";
                            break;
                        case BuildTarget.Android:
                            AB_ROOT_PATH = "android";
                            break;
                    }
#endif
                    break;
                case RuntimePlatform.OSXEditor:
                    AB_ROOT_PATH = "mac";
#if UNITY_EDITOR
                    switch (EditorUserBuildSettings.activeBuildTarget)
                    {
                        case BuildTarget.iOS:
                            AB_ROOT_PATH = "iphone";
                            break;
                        case BuildTarget.Android:
                            AB_ROOT_PATH = "android";
                            break;
                    }
#endif
                    break;
                case RuntimePlatform.WindowsPlayer:
                    AB_ROOT_PATH = "windows";
                    break;
                case RuntimePlatform.OSXPlayer:
                    AB_ROOT_PATH = "mac";
                    break;
                case RuntimePlatform.Android:
                    AB_ROOT_PATH = "android";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    AB_ROOT_PATH = "iphone";
                    break;
                case RuntimePlatform.LinuxPlayer:
                    AB_ROOT_PATH = "linux";
                    break;
                default:
                    Debug.LogError("Unknown RuntimePlatform.");
                    break;
            }
        }

        //        public static string ABPath
        //        {

        //        }

        //        // 外部资源目录
        //        public static String OutterAssetPath
        //        {
        //            get
        //            {
        //                if (Application.platform == RuntimePlatform.Android) //Android资源路径
        //                    return Application.persistentDataPath + "/" + OUTER_GAMERES_FOLDER + "/";//"/sdcard/gameres/";
        //                else if (Application.platform == RuntimePlatform.IPhonePlayer)
        //                    return Application.persistentDataPath + "/" + OUTER_GAMERES_FOLDER + "/";
        //                else if (Application.platform == RuntimePlatform.WindowsPlayer) //PC资源路径
        //                    return Application.dataPath + "/../" + OUTER_GAMERES_FOLDER + "/";
        //                else if (Application.platform == RuntimePlatform.WindowsEditor) //Windows编辑器资源路径
        //                    return Application.persistentDataPath + "/" + OUTER_GAMERES_FOLDER + "/";
        //                else if (Application.platform == RuntimePlatform.OSXEditor) //MAC编辑器资源路径
        //                    return Application.persistentDataPath + "/" + OUTER_GAMERES_FOLDER + "/";
        //                else
        //                    return "";
        //            }
        //        }

        //        // StreamingAssets目录下AssetBundle资源KeyURL => 全路径 （用www加载内容，启动时把文件从StreamingAssets目录拷贝出去时用）
        //        public static string GetStreamingAssetsAssetBundleURL(string KeyURLAndPostFix)
        //        {
        //            if (Application.platform == RuntimePlatform.Android)
        //                return string.Concat(Application.streamingAssetsPath, "/", GameConfig.INNER_ASSETBUNDLE_FOLDER, KeyURLAndPostFix);
        //            else if (Application.platform == RuntimePlatform.IPhonePlayer)
        //                return string.Concat(ASSET_FILE_HEAD, Application.streamingAssetsPath, "/", GameConfig.INNER_ASSETBUNDLE_FOLDER, KeyURLAndPostFix);
        //            else if (Application.platform == RuntimePlatform.WindowsPlayer) // windows上加载assetbundle路径前需要加file://
        //                return string.Concat(ASSET_FILE_HEAD, Application.streamingAssetsPath, "/", GameConfig.INNER_ASSETBUNDLE_FOLDER, KeyURLAndPostFix);
        //            else if (Application.platform == RuntimePlatform.WindowsEditor) // windows上加载assetbundle路径前需要加file://
        //                return string.Concat(ASSET_FILE_HEAD, Application.streamingAssetsPath, "/", GameConfig.INNER_ASSETBUNDLE_FOLDER, KeyURLAndPostFix);
        //            else if (Application.platform == RuntimePlatform.OSXEditor)
        //                return string.Concat(Application.streamingAssetsPath, "/", GameConfig.INNER_ASSETBUNDLE_FOLDER, KeyURLAndPostFix);

        //            LoggerHelper.Error("GetAssetBundleURL Invalid Platform!" + KeyURLAndPostFix);
        //            return "";
        //        }

        //        static public string GetStreamingAssetsPath()
        //        {
        //            string path = string.Empty;
        //#if UNITY_EDITOR
        //            path = ASSET_FILE_HEAD + Application.streamingAssetsPath + "/";
        //#elif UNITY_STANDALONE
        //            path = ASSET_FILE_HEAD + Application.streamingAssetsPath + "/";
        //#elif UNITY_IPHONE
        //            path = ASSET_FILE_HEAD + Application.streamingAssetsPath + "/";//Application.dataPath +"/Raw/";
        //#elif UNITY_ANDROID
        //            path = Application.streamingAssetsPath + "/";
        //#endif
        //            return path;
        //        }

        //        // AssetBundle资源KeyURL => 全路径 （用www加载内容时用）
        //        public static string GetAssetBundleURL(string KeyURL)
        //        {
        //            if (Application.platform == RuntimePlatform.Android)
        //                // android上用www加载persistentdatapath目录下assetbundle路径前需要加file://
        //                return string.Concat(ASSET_FILE_HEAD, OutterAssetPath, KeyURL, AssetDefine.ASSETBUNDLERESOURCE_POSTFIX);
        //            else if (Application.platform == RuntimePlatform.IPhonePlayer)
        //                return string.Concat(ASSET_FILE_HEAD, AssetBundlePath, KeyURL, AssetDefine.ASSETBUNDLERESOURCE_POSTFIX);
        //            else if (Application.platform == RuntimePlatform.WindowsPlayer) // windows上加载assetbundle路径前需要加file://
        //                return string.Concat(ASSET_FILE_HEAD, AssetBundlePath, KeyURL, AssetDefine.ASSETBUNDLERESOURCE_POSTFIX);
        //            else if (Application.platform == RuntimePlatform.WindowsEditor) // windows上加载assetbundle路径前需要加file://
        //                return string.Concat(ASSET_FILE_HEAD, AssetBundlePath, KeyURL, AssetDefine.ASSETBUNDLERESOURCE_POSTFIX);
        //            else if (Application.platform == RuntimePlatform.OSXEditor)
        //                return string.Concat(AssetBundlePath, KeyURL, AssetDefine.ASSETBUNDLERESOURCE_POSTFIX);
        //            else if (Application.platform == RuntimePlatform.OSXEditor)
        //                return string.Concat(AssetBundlePath, KeyURL, AssetDefine.ASSETBUNDLERESOURCE_POSTFIX);

        //            LoggerHelper.Error("GetAssetBundleURL Invalid Platform!" + KeyURL);
        //            return "";
        //        }

        public static string AssetKeyToUnityAssetPath(string filePath, Type type)
        {
            return ASSET_ROOT_PATH + filePath;
        }
    }
}
