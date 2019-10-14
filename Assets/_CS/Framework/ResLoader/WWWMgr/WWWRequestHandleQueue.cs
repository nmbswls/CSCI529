using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WWWRequestHandleQueue
{

    Queue<WWWRequestHandle> queue = new Queue<WWWRequestHandle>();

    public WWWRequestHandle Fetch()
    {

        if(queue.Count == 0)
        {
            return null;
        }
        return queue.Dequeue();
    }


    public void Push(WWWRequestHandle handle)
    {
        queue.Enqueue(handle);
    }
}
