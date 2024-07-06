using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Animator)),]
public class Actor : MonoBehaviour
{
    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        myAudio = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();

        // groundCheckPoint �� ����
        if (!groundCheckPoint)
        {
            // �ڽ� ��ü�߿� Ground Check Point �� ���ٸ�
            if (!transform.Find("Ground Check Point"))
            {
                // Ground Check Point �� ���� �����
                groundCheckPoint = new GameObject().transform;

                // �̸��� �ٲ��ְ� �θ� �ٲ��ش�.
                groundCheckPoint.name = "Ground Check Point";
                groundCheckPoint.transform.SetParent(gameObject.transform);
            }
            else
                groundCheckPoint = transform.Find("Ground Check Point");
        }
    }
    protected virtual void OnEnable()
    {
        Init();
    }
    private void Init()
    {
        ResetHealthPoint();
    }

    public Range deadLine;

    protected virtual void FixedUpdate()
    {
        Movement();
    }

    protected virtual void Movement(){
        if (IsGrounded())
            rb.position += new Vector2(Speed * dir * Time.fixedDeltaTime, 0);
    }

    protected void OutOfRangeCheck()
    {
        if (transform.position.x < deadLine .Left || transform.position.x > deadLine.Right)
        {
            currentHp = 0;
            OnDeathEvent?.Invoke();
        }
    }

    #region ü�� ����
    [Header("ü��"), SerializeField]
    int maxHp;
    [SerializeField]
    int currentHp;
    public int MaxHp => maxHp;
    public int CurrentHp => currentHp;

    /*
    void RecoverHealth(int _gain)
    {
        currentHp += _gain;

        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }

        //ü���� ȸ���ϸ� �߻��ϴ� �̺�Ʈ
        //ex) ü��ȸ�� ��ƼŬ ����
    }
    */

    public void ResetHealthPoint()
    {
        currentHp = maxHp;
    }
    #endregion

    #region �ǰ� �̺�Ʈ
    public delegate void OnHitEventHandler();
    public event OnHitEventHandler OnHitEvent;

    public delegate void OnDeathEventHandler();
    public event OnDeathEventHandler OnDeathEvent;

    protected Animator anim;
    protected AudioSource myAudio;

    Coroutine Hit;

    public void TakeDamage(Transform _attacker, Vector2 _knockBack, int _damage)
    {
        if (Hit != null)
            StopCoroutine(Hit);
        Hit = StartCoroutine(Hit_co(_attacker, _knockBack, _damage));
    }

    [SerializeField]
    float hitRecovery = 1;
    
    //�ǰ��� ���ϸ� �ڷ� �˹��� ���ϰ�, �������� �Դ´�. ���⵵ �ٲ۴�.
    //'����' '��� ������ ��' ���� '�� ������' �� �־��°�?
    IEnumerator Hit_co(Transform _attacker, Vector2 _knockBack, int _damage)
    {
        myAudio.Play(); //�ǰ��� ���

        LayerMask initialLayer = gameObject.layer;
        gameObject.layer = 0;

        currentHp -= _damage;

        OnHitEvent?.Invoke();
        KnockBack(_attacker, _knockBack);
        
        if (currentHp <= 0)
        {
            OnDeathEvent?.Invoke();
            yield break;
        }
        else
        {
            yield return new WaitForSeconds(hitRecovery);
            gameObject.layer = initialLayer;
        }
    }


    #endregion

    [SerializeField]
    float hpBarYOffset;
    public Vector3 MyHpBarPos
    {
        get
        {
            return transform.position + new Vector3(0, hpBarYOffset, 0);
        }
    } 

    #region �̵�
    protected Rigidbody2D rb;

    [SerializeField]
    Transform groundCheckPoint; // �� ��ġ���� ���� �ݶ��̴� ������ ���� ������ Ray�� �����մϴ�.

    [SerializeField]
    float checkDist;
    [SerializeField]
    LayerMask groundLayer;

    [SerializeField]
    protected int dir = 1;
    public int Dir => dir;

    [SerializeField]
    protected float originSpeed = 1;
    protected float additionalSpeed = 1;

    public void SetRandomAdditionalSpeed()
    {
        additionalSpeed = Random.Range(211, 255) * .0039f;
    }

    //public float originSpeed = 1;

    //�̵� �ϴ� �� ����ϴ� �ӵ�
    public  float Speed {
        get {
            return originSpeed * additionalSpeed;
        }
    }

    [SerializeField]
    protected Vector2 jumpForce;

    /// <summary>
    /// SheldTurn;
    /// </summary>
    /// <param name="_dir"></param>
    public void SetRotation()
    {
        dir = -dir;

        if (dir > 0)
            transform.rotation = Quaternion.Euler(0, 180, 0);
        else
            transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    /// <summary>
    /// _target�� �ٶ󺻴�.
    /// </summary>
    /// <param name="_target"></param>
    public void SetRotation(Transform _target)
    {
        dir = (int)Mathf.Sign(_target.position.x - transform.position.x);

        if (dir > 0)
            transform.rotation = Quaternion.Euler(0, 180, 0);
        else
            transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void KnockBack(Transform _hitPosition, Vector2 _force)
    {
        SetRotation(_hitPosition);
        rb.velocity = Vector2.zero;
        //���� ������ �ݴ�� �ð� �������ϹǷ� dir �� -1�� ���Ѵ�.
        rb.AddForce(new Vector2(-dir * _force.x, _force.y), ForceMode2D.Impulse);
    }
    
    /// <summary>
    /// Shelf �˹�
    /// </summary>
    /// <param name="_force"></param>
    public void KnockBack(Vector2 _force)
    {
        rb.velocity = Vector2.zero;
        //���� ������ �ݴ�� �ð� �������ϹǷ� dir �� -1�� ���Ѵ�.
        rb.AddForce(new Vector2(-dir * _force.x, _force.y), ForceMode2D.Impulse);
    }

    public void Jump()
    {
        //�ٴڿ� �پ� ���� ���� ������ ����
        if (!IsGrounded()) return;

        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(dir * jumpForce.x, jumpForce.y), ForceMode2D.Impulse);
    }

    protected bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheckPoint.position, -transform.up, checkDist, groundLayer);
        if (hit)
            return true;
        else
        {
            Debug.DrawRay(groundCheckPoint.position, -transform.up * checkDist, Color.red);
            return false;
        }
    }
    #endregion
}