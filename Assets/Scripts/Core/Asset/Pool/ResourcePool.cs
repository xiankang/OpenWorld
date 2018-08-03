using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Utils.Log;

namespace Core.Asset.Pool
{
    public class ResourcePool
    {
        Object _resource;
        readonly Transform _parent;
        readonly Stack<GameObject> _instanceList = new Stack<GameObject>();
        int _capacity = 20;


        public float LastUseTime { get; set; }
        public void UpdateLastUseTime()
        {
            LastUseTime = Time.time;
        }

        ResourcePool()
        {
            LastUseTime = 0;
        }

        public ResourcePool(Object obj, Transform parent)
        {
            Debug.Assert(obj != null, "invalid resource object!");
            Debug.Assert(parent != null, "invalid parent transform");
            _resource = obj;
            _parent = parent;
            if (obj is GameObject)
                (obj as GameObject).transform.parent = _parent;
            LastUseTime = Time.time;
        }

        public GameObject Spawn()
        {
            LastUseTime = Time.time;
            GameObject instance = null;
            if (_instanceList.Count > 0)
            {
                instance = _instanceList.Pop();
                return instance;
            }

            if(!(_resource is GameObject))
            {
                return null;
            }

            instance = (GameObject)Object.Instantiate(_resource);
            if(instance != null)
            {
                instance.name = _resource.name;
                instance.transform.SetParent(_parent, false);
            }

            return instance;
        }

        public GameObject SpawnAndSetPos(Vector3 pos)
        {
            LastUseTime = Time.time;
            GameObject instance = null;
            if (_instanceList.Count > 0)
            {
                instance = _instanceList.Pop();
                return instance;
            }

            if (!(_resource is GameObject))
            {
                return null;
            }

            instance = (GameObject)Object.Instantiate(_resource, pos, Quaternion.identity);
            if (instance != null)
            {
                //var resInfo = instance.MakeSureComponent<ResourceInfo>();
                //resInfo.pool = this;

                instance.name = _resource.name;
                instance.transform.SetParent(_parent, false);
            }

            return instance;
        }

        public void Despawn(GameObject instance)
        {
            if (instance == null)
                return;

            if (_instanceList.Contains(instance))
            {
                LogHelper.ERROR("ResourcePool", "resource object already in InstanceList! 1 {0}", instance.name);
                //这里已经存在了，为什么没有返回
            }

            if (_instanceList.Count >= _capacity)
            {
                GameObject.Destroy(instance);
            } else
            {
                instance.transform.SetParent(_parent, false);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localRotation = Quaternion.identity;
                _instanceList.Push(instance);
            }
        }
        public Object Resource
        {
            get
            {
                LastUseTime = Time.time;
                return _resource;
            }
        }

        public int Count
        {
            get
            {
                return _instanceList.Count;
            }
        }

        public int Capacity
        {
            get
            {
                return _capacity;
            }

            set
            {
                if (_capacity == value)
                    return;
                if(value < 0)
                {
                    LogHelper.ERROR("ResourcePool", "invalid capacity for resource({0})", _resource ? _resource.name : "null");
                    return;
                }
                _capacity = value;
                while (_instanceList.Count > _capacity)
                {
                    var instance = _instanceList.Pop();
                    GameObject.Destroy(instance);
                }
            }
        }

        public void Clear()
        {
            while (_instanceList.Count > 0)
            {
                var instance = _instanceList.Pop();
                GameObject.Destroy(instance);
            }

            if (_resource is GameObject)
                GameObject.Destroy(_resource as GameObject);

            _resource = null;
        }
    }
}

