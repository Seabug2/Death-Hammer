using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnQueue : MonoBehaviour
{
    Queue<GameObject> myQueue;

    public void SetMyQueue(Queue<GameObject> _myQueue)
    {
        myQueue = _myQueue;
    }

    private void OnDisable()
    {
        myQueue.Enqueue(gameObject);
    }
}
