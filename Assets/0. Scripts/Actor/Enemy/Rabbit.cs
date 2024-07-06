using System.Collections;
using UnityEngine;

public class Rabbit : Enemy
{
    [SerializeField, Range(1,10)]
    float jumpTerm = 1;
    bool isViolent = false;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (Jumping != null)
            StopCoroutine(Jumping);
        Jumping = StartCoroutine(Jumping_co());

        isViolent = Random.Range(0, 2).Equals(1);
    }

    private void OnDisable()
    {
        if(Jumping != null)
            StopCoroutine(Jumping);
    }

    Coroutine Jumping;

    IEnumerator Jumping_co()
    {
        WaitForSeconds _jumpTerm = new WaitForSeconds(jumpTerm);
        while (true)
        {
            OutOfRangeCheck();

            yield return _jumpTerm;

            if (isViolent)
            {
                Transform target = GameManager.instance.Player.transform;
                SetRotation(target);
            }
            if(IsGrounded())
                anim.SetTrigger("Jump");
            Jump();
        }
    }

    protected override void Movement()
    {
        //is Empty.
    }
}