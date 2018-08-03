using System.Collections;
using UnityEngine;

namespace Core.Utils.Log
{
    public class LogManager : Singleton<LogManager>
    {
        const int LOG_KEEP_DAYS = 7;
        private Log2Screen log2Sceen = null;
        protected override IEnumerator OnInitCoroutine(UpdateProgressDelegate updateProgressHandler, int currentStep, int totalStep)
        {
            GameObject logObject = new GameObject("_LogManager");

            LogHelper.Enable = true;
            LogHelper.AddContextFilter("EventSystem");
            //LogHelper.AddContextFilter("CppCore");
#if UNITY_EDITOR
            LogHelper.FileBufferMode = false;
            LogHelper.OutputLevelFile = LogHelper.Level.Debug;
            LogHelper.OutputLevelUnity = LogHelper.Level.Debug;
#else
        if (Debug.isDebugBuild)
        {
            LogHelper.OutputLevelFile = LogHelper.Level.Debug;
            LogHelper.OutputLevelUnity = LogHelper.Level.Debug;
        }
        else
        {
            //LogHelper.OutputLevelFile = LogHelper.Level.Warn;
            //LogHelper.OutputLevelUnity = LogHelper.Level.Warn;
            LogHelper.OutputLevelFile = LogHelper.Level.Debug;
            LogHelper.OutputLevelUnity = LogHelper.Level.Debug;
        }
#endif
            // 清除过期文件
            LogHelper.CleanupOldFiles(LOG_KEEP_DAYS);

            log2Sceen = logObject.AddComponent<Log2Screen>();
            log2Sceen.IsVisible = false;

            //LogHelper.INFO("Log", "program started, PID={0}", System.Diagnostics.Process.GetCurrentProcess().Id);

            yield return updateProgressHandler?.Invoke(this.ToString(), currentStep, totalStep, $"初始化日志模块");
        }

        public void EnableShowLog(bool enable)
        {
            log2Sceen.ShowSwitch = enable;
            if (!enable)
                log2Sceen.IsVisible = false;
        }
    }
}