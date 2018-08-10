using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    public List<Slave> _slaveList;
    public List<Slave> SlaveList;

    public float GetMoveSpeed()
    {
        //装备加成，buff加成
        return 0;
    }

    public float GetCooldownSpeed()
    {
        return 0;
    }

    public float GetMaxCooldownSpeed()
    {
        return 0;
    }

    public float GetMaxManaMultiplier()
    {
        return 0;
    }

    public float GetMaxMana()
    {
        return 0;
    }

    public bool GetFreeCast()
    {
        return false;
    }

    // Use this for initialization
    void Start () {
        Spawn();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdateProperty()
    {
        //这里更新每个槽位的属性
    }

    public virtual void Spawn()
    {
        SlaveList = new List<Slave>();
        foreach(Slave slave in _slaveList)
        {
            Slave newSlave = null;
            if (slave != null)
            {
                GameObject obj = GameObject.Instantiate(slave.gameObject);
                obj.transform.parent = transform;
                newSlave = obj.GetComponent<Slave>();
                Debug.Log(transform.parent.gameObject.name);
                Debug.Log(transform.parent.gameObject.GetInstanceID());
                newSlave.SetOwnerIndex(transform.parent.gameObject.GetInstanceID());
                Debug.Log(newSlave.GetOwnerIndex());
            }

            SlaveList.Add(newSlave);
        }
    }
    public Tool GetTool(byte slot)
    {
        if (SlaveList == null)
            return null;
        if (SlaveList.Count < slot + 1)
            return null;
        return SlaveList[slot] ? SlaveList[slot] as Tool : null;
    }
}
