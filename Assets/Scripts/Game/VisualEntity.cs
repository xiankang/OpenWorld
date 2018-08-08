using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualEntity : GameEntity {
    Animator _animator = null;
	// Use this for initialization
	protected void Start () {
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
	void Update () {
		
	}

    public void SetAnim(EActionStateAnim eActionStateAnim)
    {
        _animator.SetInteger("State", (int)eActionStateAnim);
    }
}
