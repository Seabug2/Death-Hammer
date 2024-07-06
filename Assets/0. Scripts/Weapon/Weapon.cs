using UnityEngine;

[RequireComponent(typeof(Animator)), RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    [SerializeField]
    int id;
    public int ID => id;

    [SerializeField]
    protected int damage;
    public int Damage => damage;

    [SerializeField]
    protected float speed = 1;
    public float Speed => speed;

    [SerializeField]
    protected Vector2 knockBackForce;

    protected Animator anim;

    protected Actor controler;

    [SerializeField]
    Sprite iconImage;
    public Sprite IconImage=> iconImage;

    public Actor Controler
    {
        get {
            if (!controler)
            {
                controler = GameManager.instance.Player;
            }
            return controler;
        }
    }

    [SerializeField]
    AudioClip[] audios;
    AudioSource myAudio;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        myAudio = GetComponent<AudioSource>();
        myAudio.loop = false;
        myAudio.playOnAwake = false;
    }

    protected void AudioPlay()
    {
        if (!audios.Length.Equals(0))
            myAudio.clip = audios[Random.Range(0, audios.Length)];
        if (myAudio.clip!=null)
            myAudio.Play();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        //���� ��ü�� ���̾�� Enemy ���̾ �浹�� �� �����Ƿ� �浹�� ��ü�� ���� Enemy�̴�.
        if (collision.transform.TryGetComponent<Actor>(out Actor _actor))
        {
            AttackEvent(_actor);
        }
        //else if (collision.transform.parent.TryGetComponent<Actor>(out _actor))
        //{
        //    _actor.KnockBack(transform,new Vector2(knockBackForce.x, 0));
        //}
    }

    protected virtual void AttackEvent(Actor _actor)
    {
        _actor.TakeDamage(transform, knockBackForce, damage);
        //���Ⱑ �����ϸ� �������� �̺�Ʈ
        //�������� �ش�?
        //� ��ƼŬ�� �����Ѵ�?
    }

    public void Replay()
    {
        anim.Play("Attack", -1, 0f);
        anim.Update(0f);
    }

    public void Stop()
    {
        //���� ���߰�
        anim.enabled = false;
        gameObject.layer = 0;
    }
}
