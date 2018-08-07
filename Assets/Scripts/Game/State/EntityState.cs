using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityState : Slave {

	// Use this for initialization
	void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void Interrupt(EUnitAction eAction)
    {

    }
}
