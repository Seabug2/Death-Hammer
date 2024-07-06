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

        // groundCheckPoint 가 없고
        if (!groundCheckPoint)
        {
            // 자식 객체중에 Ground Check Point 가 없다면
            if (!transform.Find("Ground Check Point"))
            {
                // Ground Check Point 를 새로 만들고
                groundCheckPoint = new GameObject().transform;

                // 이름을 바꿔주고 부모도 바꿔준다.
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

    #region 체력 관리
    [Header("체력"), SerializeField]
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

        //체력을 회복하면 발생하는 이벤트
        //ex) 체력회복 파티클 생성
    }
    */

    public void ResetHealthPoint()
    {
        currentHp = maxHp;
    }
    #endregion

    #region 피격 이벤트
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
    
    //피격을 당하면 뒤로 넉백을 당하고, 데미지를 입는다. 방향도 바꾼다.
    //'누가' '어느 정도의 힘' 으로 '몇 데미지' 를 주었는가?
    IEnumerator Hit_co(Transform _attacker, Vector2 _knockBack, int _damage)
    {
        myAudio.Play(); //피격음 재생

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

    #region 이동
    protected Rigidbody2D rb;

    [SerializeField]
    Transform groundCheckPoint; // 이 위치에서 부터 콜라이더 검출을 위한 가상의 Ray를 생성합니다.

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

    //이동 하는 데 사용하는 속도
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
    /// _target을 바라본다.
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
        //진행 방향의 반대로 팅겨 나가야하므로 dir 에 -1을 곱한다.
        rb.AddForce(new Vector2(-dir * _force.x, _force.y), ForceMode2D.Impulse);
    }
    
    /// <summary>
    /// Shelf 넉백
    /// </summary>
    /// <param name="_force"></param>
    public void KnockBack(Vector2 _force)
    {
        rb.velocity = Vector2.zero;
        //진행 방향의 반대로 팅겨 나가야하므로 dir 에 -1을 곱한다.
        rb.AddForce(new Vector2(-dir * _force.x, _force.y), ForceMode2D.Impulse);
    }

    public void Jump()
    {
        //바닥에 붙어 있을 때만 점프가 가능
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