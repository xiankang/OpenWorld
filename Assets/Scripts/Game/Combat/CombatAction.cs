using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatAction : MonoBehaviour{
    public virtual void OnFrame() { }
    public virtual void OnFrameImpact() { }
    public virtual void OnInterval() { }
    public virtual void OnBegin() { }
    public virtual void OnStart() { }
    public virtual void OnPreCost() { }
    public virtual void OnAction() { }

    public virtual void OnPreImpact() { }
    public virtual void OnPreDamage() { }
    public virtual void OnDamageEvent() { }
    public virtual void OnImpact() { }
    public virtual void OnImpactInvalid() { }

    public virtual void OnComplete() { }
    public virtual void OnCancel() { }

    public virtual void OnDamage() { }
    public virtual void OnDamaged() { }
    public virtual void OnDamagedApplied() { }

    //...
}
