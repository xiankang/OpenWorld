using UnityEngine;

public class AppEnv
{
    private static bool _DevMode = false;
    private static bool _UseAB = false;

    // 是否开发模式
    // 非Unity Editor环境下：通过设置_DevMode来确定是否为开发模式
    // Unity Editor环境下：true
    public static bool IsDevMode
    {
        get
        {
#if !UNITY_EDITOR
            return _DevMode;
#else
            return true;
#endif
        }
    }

    // 是否使用AB包
    // 非Unity Editor环境下：true
    // Unity Editor环境下：通过设置_UseAB来确定是否使用AB包
    public static bool IsUseAB
    {
        get
        {
#if !UNITY_EDITOR
            return true;
#else
            return _UseAB;
#endif
        }
    }

    //public static DownloadManager _DownloadManager = null;

    public static bool IsStandaloneIOS()
    {
        bool ret = false;
#if !UNITY_EDITOR && UNITY_IOS
            ret = true;
#endif
        return ret;
    }

    public static bool IsStandaloneAndroid()
    {
        bool ret = false;
#if !UNITY_EDITOR && UNITY_ANDROID
            ret = true;
#endif
        return ret;
    }
}
