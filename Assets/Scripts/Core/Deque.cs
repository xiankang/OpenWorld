using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Deque<T>
{
    private LinkedList<T> list = new LinkedList<T>();

    public int Count
    {
        get
        {
            return list.Count;
        }
    }

    public T Dequeue()
    {
        T t = list.First.Value;
        list.RemoveFirst();
        return t;
    }

    public void Enqueue(T t)
    {
        list.AddLast(t);
    }

    public T Peek()
    {
        T t = list.First.Value;
        return t;
    }

    public void PushFront(T t)
    {
        list.AddFirst(t);
    }

    public void Clear()
    {
        list.Clear();
    }

    public void Erase(T t)
    {
        //不是很高效
        list.Remove(t);
    }

    public IEnumerator GetEnumerator()
    {
        return list.GetEnumerator();
    }
}
