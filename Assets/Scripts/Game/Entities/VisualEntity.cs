using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEntityStatus
{
    ENTITY_STATUS_DORMANT,
    ENTITY_STATUS_ACTIVE,
    ENTITY_STATUS_DEAD,
    ENTITY_STATUS_CORPSE,
    ENTITY_STATUS_HIDDEN,

    NUM_ENTITY_STATUSES
}

public class VisualEntity : GameEntity {
    protected byte _yStatus;
    
    Animator _animator = null;
	// Use this for initialization
	protected override void Start () {
        base.Start();
        Transform modelTrans = transform.Find("model");
        if (modelTrans)
        {
            _animator = modelTrans.GetComponent<Animator>();
        }
        if (!_animator)
        {
            Debug.LogErrorFormat("{0} no animator!", gameObject.name);
        }
	}

    // Update is called once per frame
    protected override void Update () {
		
	}

    public void SetAnim(EActionStateAnim eActionStateAnim)
    {
        _animator.SetInteger("State", (int)eActionStateAnim);
    }

    public EActionStateAnim GetAnim()
    {
        return (EActionStateAnim)_animator.GetInteger("State");
    }

    public byte GetStatus() { return _yStatus; }
    public void SetStatus(byte status) { _yStatus = status; }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
