using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public float sensitivityX = 0.6f;
    public float sensitivityY = 0.2f;

    public float minimumX = -360.0f;
    public float maximumX = 360.0f;

    public float minimumY = -75.0f;
    public float maximumY = 89.0f;

    private float rotationYaxis = 0.0f;
    private float rotationXaxis = 0.0f;
    public Transform _target = null;

    private Quaternion originalCameraRotation;
    private Quaternion originalPlayerRotation;

    private void Awake()
    {
        originalCameraRotation = Camera.main.transform.localRotation;
        originalPlayerRotation = transform.localRotation;
    }
    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (GameEnv._InputManager.SwipeVec.Equals(Vector2.zero))
            return;

        if (_target == null)
        {
            if (transform.parent.parent)
                _target = transform.parent.parent;
        }

        if (_target == null)
            return;

        UpdateCameraRotation();

        GameEnv._InputManager.SwipeVec = Vector2.zero;
    }

    
    private void UpdateCameraRotation()
    {
        rotationYaxis += GameEnv._InputManager.SwipeVec.x * sensitivityX;
        rotationXaxis += GameEnv._InputManager.SwipeVec.y * sensitivityY;
        rotationYaxis = ClampAngle(rotationYaxis, minimumX, maximumX);
        rotationXaxis = ClampAngle(rotationXaxis, minimumY, maximumY);
        Quaternion xQuaternion = Quaternion.AngleAxis(rotationYaxis, Vector3.up);
        Quaternion yQuaternion = Quaternion.AngleAxis(rotationXaxis, Vector3.left);
        _target.transform.localRotation = originalPlayerRotation * xQuaternion;
        transform.localRotation = originalCameraRotation * yQuaternion;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
