
namespace Core.Asset
{
    public enum LoadPriority //加载优先级，值越大优先级越高
    {
        Normal = 0,
        Medium,
        High,
    }

    public delegate void AssetLoadedEvent(AssetHandle Sender);

    public class ResourceHandle
    {
        public float Progress = 0.0f;
        public bool IsDone = false;
        public bool IsStop = false;
        public System.Object UserData = null;
    }

    public class AssetHandle : ResourceHandle
    {
        public UnityEngine.Object Asset = null;
        public event AssetLoadedEvent Callback;

        public string CallbackFunName
        {
            get
            {
#if UNITY_EDITOR
                if (Callback != null)
                {
                    return Callback.Method.Name;
                }
#endif
                return string.Empty;
            }

        }

        public void OnCallBack()
        {
            if (Callback != null)
                Callback(this);
        }
    }
}
