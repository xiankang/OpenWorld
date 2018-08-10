using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : Tool {
    public bool _noSilence = false;
    public List<uint> _requiredLevel;

    //public void LevelUp()
    //{

    //}

    //public bool CanLevelUp()
    //{
    //    return true;
    //}

    //public virtual bool IsDisabled()
    //{
    //    return false;
    //}

    //public virtual bool CanOrder()
    //{
    //    return true;
    //}

    //public virtual bool CanActivate()
    //{
    //    return true;
    //}

    // Use this for initialization
    protected override void Start () {
        Debug.Log("Ability Start");
        base.Start();
	}

    // Update is called once per frame
    protected override void Update () {
        base.Update();
	}
}
