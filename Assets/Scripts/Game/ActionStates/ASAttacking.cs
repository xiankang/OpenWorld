using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASAttacking : ActionState {


    public ASAttacking(Brain brain) : base(brain)
    {

    }

    public override bool BeginState()
    {
        return true;
    }
    public override bool ContinueStateAction()
    {
        return false;
    }
    public override bool EndState(uint priority)
    {
        return true;
    }
}
