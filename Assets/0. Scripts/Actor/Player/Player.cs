using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : Actor
{
    [SerializeField]
    GameData fieldData;
    Range movementRange;

    [SerializeField]
    WeaponManager inventory;

    [SerializeField]
    HpBar myHpBar;

    protected override void Awake()
    {
        base.Awake();
        if(!inventory)
            inventory = transform.GetComponentInChildren<WeaponManager>();
    }

    void Start()
    {
        //ü�¹� ����
        myHpBar.SetTarget(this);
        //���� ���� UI�� ��Ȱ��ȭ
        popUpUI.SetActive(false);
        //�̵� ���� ���� ����
        movementRange = fieldData.PlayerLimitRange;

        inventory.WeaponEquip();
        //���� ���� = 0
        SwitchingWaepon(0);

        OnHitEvent += () =>
        {
            clickTime = 0;
            Time.timeScale = 1;
            popUpUI.SetActive(false);
            currentWeapon.Replay();
            anim.SetTrigger("Hit");
            stun.SetActive(true);
            GameManager.instance.MainCamCtrl.ShakeCamera(.3f, 2.5f);
        };
    }

    protected override void Movement()
    {
        base.Movement();
        float x = Mathf.Clamp(rb.position.x, movementRange.Left, movementRange.Right);
        rb.position = new Vector2(x, rb.position.y);
    }

    #region ����
    [SerializeField]
    GameObject popUpUI;
    [SerializeField]
    GraphicRaycaster gr;

    [SerializeField]
    float uiPopupTime;
    float clickTime;

    void Update()
    {
            //HandleWindowsInput();
#if UNITY_ANDROID
        HandleAndroidInput();
#elif UNITY_STANDALONE_WIN
            HandleWindowsInput();
#endif
    }

    void TouchCheck()
    {
        PointerEventData ped = new PointerEventData(EventSystem.current)
        {
#if UNITY_ANDROID
            position = Input.touches[0].position
#elif UNITY_STANDALONE_WIN
        position = Input.mousePosition
#endif
        };

        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(ped, results);

        //�հ����� �ϳ��� �ʿ��մϴ�
        if (results.Count > 0)
        {
            print(results[0].gameObject.name);

            if(results[0].gameObject.TryGetComponent<ButtonActive>(out ButtonActive button))
            {
                SwitchingWaepon(button.MyID);
            }
        }

        Time.timeScale = 1;
        popUpUI.SetActive(false);
    }

    [Header("����")]
    Weapon currentWeapon;
    public Weapon CurrentWeapon => currentWeapon;
    public void SwitchingWaepon(int _index)
    {
        //���� �����ϰ� �ִ� ���Ⱑ �ִٸ�, ���� ���� ���⸦ ��Ȱ��ȭ�Ѵ�.
        if(currentWeapon)
            currentWeapon.gameObject.SetActive(false);

        currentWeapon = inventory.GetWeapon(_index);
        currentWeapon.gameObject.SetActive(true);

        additionalSpeed = currentWeapon.Speed;
    }

    void HandleAndroidInput()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase.Equals(TouchPhase.Began))
            {
                clickTime = 0;
            }
            else if (Input.GetTouch(0).phase.Equals(TouchPhase.Moved) || Input.GetTouch(0).phase.Equals(TouchPhase.Stationary))
            {
                if (gameObject.layer.Equals(0)) return;

                if (clickTime > uiPopupTime)
                {
                    if (!popUpUI.activeSelf)
                    {
                        popUpUI.transform.localPosition = new Vector3(0, Input.GetTouch(0).position.y - Screen.height * .5f, 0);
                        popUpUI.SetActive(true);
                        Time.timeScale = .1f;
                        // UI ��ġ�� ����...?
                        //popUpUI.anchoredPosition = new Vector2(Mathf.Clamp(tch.position.x, 250, Screen.width - 250), Mathf.Clamp(tch.position.y, 280, Screen.height - 280));
                    }
                    return;
                }
                else
                {
                    clickTime += Time.deltaTime;
                    return;
                }
            }
            else if (Input.GetTouch(0).phase.Equals(TouchPhase.Ended))
            {
                if (clickTime < uiPopupTime)
                {
                    if (IsGrounded())
                    {
                        SetRotation();
                        currentWeapon.Replay();
                    }
                }

                if (clickTime > uiPopupTime)
                {
                    if (popUpUI.activeSelf)
                    {
                        TouchCheck();
                    }
                }

                clickTime = 0;
                return;
            }
        }
    }
    void HandleWindowsInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickTime = 0;
        }

        else if (Input.GetMouseButton(0))
        {
            if (gameObject.layer.Equals(0)) return;

            if (clickTime > uiPopupTime)
            {
                if (!popUpUI.activeSelf)
                {
                    popUpUI.transform.localPosition = new Vector3(0, Input.mousePosition.y - Screen.height * .5f, 0);
                    popUpUI.SetActive(true);
                    Time.timeScale = .1f;
                }
                return;
            }
            else
            {
                clickTime += Time.deltaTime;
                return;
            }
        }

        else if (Input.GetMouseButtonUp(0))
        {
            //ª�� ��ġ = ���� ��ȯ
            if (clickTime < uiPopupTime)
            {
                if (IsGrounded())
                {
                    SetRotation();
                    currentWeapon.Replay();
                }
            }

            else
            {
                if (popUpUI.activeSelf)
                {
                    TouchCheck();
                }
            }

            clickTime = 0;
            return;
        }
    }
    #endregion

    public IEnumerator GameOver_cor()
    {
        //Player�� OnHitEvent
        /*
            currentWeapon.Replay();
            anim.SetTrigger("Hit");
            GameManager.instance.MainCamCtrl.ShakeCamera(.2f, 2);
         */

        additionalSpeed = 0; //�÷��̾� �̵� ����
        currentWeapon.Stop();
        yield return new WaitWhile(() => myAudio.isPlaying);

        //���� ����
        anim.enabled = false;
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        enabled = false;
    }

    [SerializeField]
    GameObject stun;
}
