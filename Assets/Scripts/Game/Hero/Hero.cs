using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Unit {
	// Use this for initialization
	protected override void Start () {
        base.Start();
        Debug.LogFormat("hero {0}", gameObject.GetInstanceID());
    }


    // Update is called once per frame
    protected override void Update () {
		
	}
}
