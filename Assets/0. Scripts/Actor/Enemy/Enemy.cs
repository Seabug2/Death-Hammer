using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

// �ܼ� �̵�, �׸��� �浹�� �������� �ִ� ��
public class Enemy : Actor
{
    protected override void Awake()
    {
        base.Awake();
        spriteResolver = transform.GetComponentsInChildren<SpriteResolver>();
    }
    
    //�÷��̾�� ���� ������
    [SerializeField]
    protected int damage = 1;

    //�÷��̾ ���� �� ��
    [SerializeField]
    protected Vector2 knockBack;

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.TryGetComponent<Actor>(out Actor _actor))
        {
            _actor.TakeDamage(transform, knockBack, damage);
        }
    }

    protected override void Movement()
    {
        base.Movement();
        OutOfRangeCheck();
    }

    SpriteResolver[] spriteResolver;

    [SerializeField]
    protected string[] labels;

    protected void RandomReplaceSprites()
    {
        //spriteResolver�� 1�� �̻� �ִٸ�
        if (spriteResolver.Length > 0)
        {
            string label = labels[Random.Range(0, labels.Length)];
            for (int i = 0; i < spriteResolver.Length; i++)
            {
                spriteResolver[i].SetCategoryAndLabel(spriteResolver[i].GetCategory(), label);
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        RandomReplaceSprites();
    }
}
