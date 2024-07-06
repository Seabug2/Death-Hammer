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
        //무기 객체의 레이어는 Enemy 레이어만 충돌할 수 있으므로 충돌한 객체는 전부 Enemy이다.
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
        //무기가 공격하면 벌어지는 이벤트
        //데미지를 준다?
        //어떤 파티클을 생성한다?
    }

    public void Replay()
    {
        anim.Play("Attack", -1, 0f);
        anim.Update(0f);
    }

    public void Stop()
    {
        //무기 멈추고
        anim.enabled = false;
        gameObject.layer = 0;
    }
}
