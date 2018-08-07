using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : VisualEntity {
    public bool _isMobile = true;
    public bool _canAttack = true;
    public Inventory _inventory = null;
    public Brain _brain = null;
    public uint _orderSequence = 1;

	// Use this for initialization
	protected void Start () {
        base.Start();
        if (_inventory == null)
        {
            _inventory = transform.Find("inventory").GetComponent<Inventory>();
        }
        if (_brain == null)
        {
            _brain = transform.Find("brain").GetComponent<Brain>();
        }
        _brain.SetUnit(this);
        _brain.Init();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public uint PlayerCommand(UnitCommand cmd)
    {
        cmd._orderSequence = _orderSequence;
        ++_orderSequence;
        _brain.AddCommand(cmd);
        return cmd._orderSequence;
    }

    public virtual void Interrupt(EUnitAction eAction)
    {
        foreach(Slave slave in _inventory.slaveList)
        {
            Tool tool = slave as Tool;
            if(tool !=null && tool.IsActive()){
                tool.Interrupt(eAction);
            }

            EntityState state = slave as EntityState;
            if(state != null)
            {
                state.Interrupt(eAction);
            }
        }

    }

    public virtual bool IsImmobilized()
    {
        return false;
    }

    public virtual bool IsStunned()
    {
        return false;
    }

    public bool IsGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.1f, ~(1 << 26));
    }


}
