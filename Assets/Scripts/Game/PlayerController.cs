using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Utils.Log;

public class PlayerController : MonoBehaviour
{
    CharacterController _characterController;
    Vector3 _moveSpeed;
    public float _moveSpeedMagnitude { get; set; } = 2.5f;

    //跳跃高度
    public float _jumpHeight = 1.0f;
    //重力加速度
    public float _gravity = 17.5f;

    //
    //protected bool touchGround = true;

    protected Vector3 _targetPos = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        _characterController = gameObject.GetComponent<CharacterController>();
        if (!_characterController)
            LogHelper.FATAL("PlayerController", "no Character Controller");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
    }

    protected virtual Vector3 GetMoveDir()
    {
        return GetMoveDirInput();
    }

    protected Vector3 GetMoveDirInput()
    {
        Vector3 dir;
        if(GameEnv._InputManager.JoyStickVec.magnitude > 0)
        {
            dir = (transform.forward * GameEnv._InputManager.JoyStickVec.y) + (transform.right * GameEnv._InputManager.JoyStickVec.x);
        } else
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector3 dirWalk = transform.forward * v;
            Vector3 dirStrafe = transform.right * h;
            dir = dirWalk + dirStrafe;
        }
        dir.y = 0;
        dir.Normalize();

        return ClampMoveDir(dir, transform.right, transform.forward);
    }

    //移动方向归约到八个方位
    private Vector3 ClampMoveDir(Vector3 dir, Vector3 right, Vector3 forward)
    {
        float d2rAngle = Vector3.Angle(dir, right);
        float d2fAngle = Vector3.Angle(dir, forward);
        if (d2rAngle < 90 && d2fAngle < 90)      //第一象限
        {
            if (d2rAngle <= 22.5f)
                return right.normalized;
            else if (d2rAngle > 22.5f && d2fAngle > 22.5f)
                return (forward + right).normalized;
            else
                return forward.normalized;
        }
        else if (d2rAngle > 90 && d2fAngle < 90)  //第二象限
        {
            if (d2fAngle <= 22.5f)
                return forward.normalized;
            else if (d2fAngle > 22.5f && d2fAngle < 90.0f - 22.5f)
                return (forward + (-right)).normalized;
            else
                return (-right).normalized;
        }
        else if (d2rAngle > 90 && d2fAngle > 90) //第三象限
        {
            if (d2fAngle <= 90.0f + 22.5f)
                return (-right).normalized;
            else if (d2fAngle > 90.0f + 22.5f && d2fAngle < 180.0f - 22.5f)
                return ((-forward) + (-right)).normalized;
            else
                return (-forward).normalized;
        }
        else if (d2rAngle < 90 && d2fAngle > 90) //第四象限
        {
            if (d2rAngle <= 22.5f)
                return right.normalized;
            else if (d2rAngle > 22.5f && d2rAngle < 90.0f - 22.5f)
                return (right + (-forward)).normalized;
            else
                return (-forward).normalized;
        }

        return dir;
    }

    void UpdateMovement()
    {
        //移动方向
        Vector3 dir = GetMoveDir();

        _moveSpeed.x = dir.x * _moveSpeedMagnitude;
        _moveSpeed.z = dir.z * _moveSpeedMagnitude;
        _moveSpeed.y = IsGround() ? -0.1f : _moveSpeed.y - _gravity * Time.deltaTime;

        _characterController.Move(_moveSpeed * Time.deltaTime);
    }

    private bool IsGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.1f, ~(1 << 26));
    }
}
