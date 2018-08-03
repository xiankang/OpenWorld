using UnityEngine;
using Core.RepresentLogic;
using Core.Utils.Log;


public class InputManager : MonoBehaviour
{
    public float _IgnoreSwipeTime = 0.05f;//执行滑动时忽略多少秒以内的操作（防止误操作）
    public Vector2 _IgnoreSwipeArea = new Vector2(400, 400);//执行滑动时忽略的区域（摇杆区域）
    public float _SwipeDistanceFactor = 0.125f;//滑动距离因子
    public float _SwipeSpeedFactor = 0.00325f;//滑动速度因子
    public float _MinSwipeSpeed = 0.6f;//滑动速度最小值
    public float _MaxSwipeSpeed = 1.25f;//滑动速度最大值
    public bool _Debug = false;

    private Vector2 _LastMouseVec = Vector2.zero; // 上一帧鼠标位置
    private float _MouseX = 0;
    private float _MouseY = 0;
    private Vector2 _SwipeVec = Vector2.zero;   //滑屏方向和距离
    public Vector2 SwipeVec
    {
        get { return _SwipeVec; }
        set { _SwipeVec = value; }
    }

    private Vector2 joystickVec;    //摇杆滑动方向和距离
    public Vector2 JoyStickVec
    {
        set { joystickVec = value; }
        get { return joystickVec; }
    }



    public static bool _IsSwipe = false;
    public static int _IsSwipeFingerIndex = -1;


    public void EnableUIMode(bool b)
    {
        EasyTouch touch = InteractionManager.Instance.easyTouchObject.GetComponent<EasyTouch>();
        touch.enableUIMode = b;
    }

    #region EasyTouch事件注册和处理
    void OnEnable()
    {
        RegisterEvent();
    }

    void OnDisable()
    {
        UnRegisterEvent();
    }

    void OnDestroy()
    {
        UnRegisterEvent();
    }

    private void RegisterEvent()
    {
        //单次按下
        EasyTouch.On_TouchStart += OnTouchStart;
        //单次松开
        EasyTouch.On_TouchUp += OnTouchUp;

        //滑屏
        EasyTouch.On_SwipeStart += OnSwipeStart;
        EasyTouch.On_Swipe += OnSwipe;
        EasyTouch.On_SwipeEnd += OnSwipeEnd;

        //单次点击
        EasyTouch.On_SimpleTap += OnSimpleTap;

        //长按
        EasyTouch.On_LongTap += OnLongTap;
        EasyTouch.On_LongTapEnd += OnLongTapEnd;
    }

    private void UnRegisterEvent()
    {
        //单次按下
        EasyTouch.On_TouchStart -= OnTouchStart;
        //单次松开
        EasyTouch.On_TouchUp -= OnTouchUp;

        //滑屏
        EasyTouch.On_SwipeStart -= OnSwipeStart;
        EasyTouch.On_Swipe -= OnSwipe;
        EasyTouch.On_SwipeEnd -= OnSwipeEnd;

        //单次点击
        EasyTouch.On_SimpleTap -= OnSimpleTap;

        //长按
        EasyTouch.On_LongTap -= OnLongTap;
        EasyTouch.On_LongTapEnd -= OnLongTapEnd;
    }

    private void OnTouchStart(Gesture gesture)
    {
        //LogHelper.DEBUG("InputManager", "OnTouchStart pos=({0},{1}),pickedUIElement={2},fingerIndex={3}", gesture.position.x,gesture.position.y,gesture.pickedUIElement,gesture.fingerIndex);
    }

    private void OnTouchUp(Gesture gesture)
    {
        //LogHelper.DEBUG("InputManager", "OnTouchUp pos=({0},{1})", gesture.position.x,gesture.position.y);
    }

    private float GetSwipeFactorByDistance(float distance)
    {
        float factor = distance * _SwipeSpeedFactor + _SwipeDistanceFactor;

        if (factor < _MinSwipeSpeed)
        {
            factor = _MinSwipeSpeed;
        }
        else if (factor > _MaxSwipeSpeed)
        {
            factor = _MaxSwipeSpeed;
        }
        return factor;
    }

    private void OnSwipeStart(Gesture gesture)
    {
        //LogHelper.DEBUG("InputManager", "OnSwipeStart pos=({0},{1})", gesture.position.x,gesture.position.y);
        if (_IsSwipe)
        {
            return;
        }
        //屏蔽起始点在摇杆区域的滑动
        if (gesture.startPosition.x < _IgnoreSwipeArea.x && gesture.startPosition.y < _IgnoreSwipeArea.y)
        {
            return;
        }
        //_luaOnSwipeStart.call(_luaInputManager);
        _IsSwipe = true;
        _IsSwipeFingerIndex = gesture.fingerIndex;
    }

    private void OnSwipe(Gesture gesture)
    {
        if (!_IsSwipe || _IsSwipeFingerIndex != gesture.fingerIndex)
        {
            return;
        }
        //屏蔽0.1秒内的滑动
        if (gesture.actionTime < _IgnoreSwipeTime)
        {
            return;
        }

        float distance = Vector2.Distance(gesture.position, gesture.startPosition);
        float swipeFactor = GetSwipeFactorByDistance(distance);
        if (_Debug)
        {
            LogHelper.DEBUG("InputManager", "OnSwipe distance={0},swipeFactor={1}", distance, swipeFactor);
        }

        _SwipeVec = gesture.deltaPosition * swipeFactor;
        //_luaOnSwipe.call(_luaInputManager, gesture.deltaPosition);
    }

    private void OnSwipeEnd(Gesture gesture)
    {
        //LogHelper.DEBUG("InputManager", "OnSwipeEnd pos=({0},{1})", gesture.position.x,gesture.position.y);
        if (_IsSwipe && _IsSwipeFingerIndex == gesture.fingerIndex)
        {
            _SwipeVec = Vector2.zero;
            _IsSwipe = false;
            _IsSwipeFingerIndex = -1;
        }

    }

    void OnSimpleTap(Gesture gesture)
    {
        //Debug.Log("Simple Tap" + gesture.position);
    }

    void OnLongTap(Gesture gesture)
    {
        //Debug.Log("Long Tap" + gesture.position);
        //Debug.Log("Long Tap DeltaTime" + gesture.deltaTime);
    }

    void OnLongTapEnd(Gesture gesture)
    {
        //Debug.Log("Long Tap End" + gesture.position);
    }
    #endregion
}
