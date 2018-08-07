using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slave : GameEntity {
    public static readonly uint SLAVE_ACTIVE = Bit.BIT(0);
    public static readonly uint SLAVE_ACTIVE_NO_MUTE = Bit.BIT(1);

    protected uint _slaveFlags = 0;


    public bool IsActive()
    {
        return (_slaveFlags & SLAVE_ACTIVE) != 0;
    }

    public bool IsActiveNoMuteCheck()
    {
        return (_slaveFlags & SLAVE_ACTIVE_NO_MUTE) != 0;
    }

    public virtual void UpdateIsActive() { }


	// Use this for initialization
	protected void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
