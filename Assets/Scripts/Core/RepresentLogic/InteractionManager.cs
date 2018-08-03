using System.Collections;
using UnityEngine;

namespace Core.RepresentLogic
{
    public class InteractionManager : Singleton<InteractionManager>
    {
        public GameObject easyTouchObject;
        public GameObject inputObject;
        protected override IEnumerator OnInitCoroutine(UpdateProgressDelegate updateProgressHandler, int currentStep, int totalStep)
        {
            GameObject interactionObject = new GameObject("_InteractionManager");

            // EasyTouch
            easyTouchObject = new GameObject("EasyTouch");
            easyTouchObject.transform.parent = interactionObject.transform;
            EasyTouch com = easyTouchObject.AddComponent<EasyTouch>();
            com.enableRemote = false;
            com.autoUpdatePickedUI = false;
            com.enabledNGuiMode = false;
            com.enableUIMode = true;
            com.autoSelect = false;
            com.StationaryTolerance = 5;

            yield return updateProgressHandler?.Invoke(this.ToString(), currentStep, totalStep, $"初始化交互模块");
        }
    }
}
