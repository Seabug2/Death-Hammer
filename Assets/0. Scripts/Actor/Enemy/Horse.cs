using UnityEngine;

public class Horse : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        OnHitEvent += () =>
        {
            additionalSpeed = 0;
        };
    }

    [SerializeField]
    float maxSpeed;

    protected override void Movement()
    {
        base.Movement();
        if (IsGrounded())
        {
            if(maxSpeed > additionalSpeed)
            {
                additionalSpeed += Time.fixedDeltaTime;
            }
            rb.position += new Vector2(Speed * dir * Time.fixedDeltaTime, 0);
        }
    }
}