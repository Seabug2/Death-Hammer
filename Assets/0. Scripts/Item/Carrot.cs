using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrot : MonoBehaviour
{
    public delegate void OnGetScoreEventHandelr();
    public event OnGetScoreEventHandelr OnGetScoreEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        particle.transform.position = transform.position;
        particle.SetActive(true);
        OnGetScoreEvent?.Invoke();
    }

    [SerializeField]
    protected float closeTime = 1;
    float time = 0;

    protected void OnEnable()
    {
        time = 0;
    }

    protected void FixedUpdate()
    {
        time += Time.fixedDeltaTime;

        if (closeTime < time)
        {
            gameObject.SetActive(false);
        }
    }

    [SerializeField]
    GameObject particle;
}