using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class OnGameManager : MonoBehaviour
{
    Player player;
    public Player Player
    {
        get
        {
            if (!player)
            {
                player = GameManager.instance.Player;
            }
            return player;
        }
    }

    [Header("���� ����")]
    [SerializeField]
    int stageLevel;
    int maxStageLevel;
    StageData nowStageData;
    [SerializeField]
    int score;

    [Header("�� ���� ����"), Space(25)]
    Transform enemyTop;
    [SerializeField]
    List<Actor> enemysList; //���� ��� ���� �� ����Ʈ
    [SerializeField]
    float respawnTerm;

    [Header("UI ����"), Space(25)]
    [SerializeField]
    Transform enemyHpBarsTop;
    [SerializeField]
    HpBar hpBar;
    Queue<HpBar> qHpBar;
    [SerializeField]
    TextMeshProUGUI scroeUI;
    Animator scroeAnim;
    [SerializeField]
    Animator fadeInOut;
    [SerializeField]
    GameObject fingerprint, resultBoard;
    [SerializeField]
    GameObject resetButton;

    [Header("���� ����"), Space(25)]
    [SerializeField]
    GameData fieldData;
    [SerializeField]
    ViewGuider guider;

    private void Awake()
    {
        Camera.main.orthographicSize = Screen.height * guider.ViewRange * .5f / Screen.width;

    }

    private void Start()
    {
        GameManager.instance.BGMManager.InGameBGMStart();
        StartCoroutine(Intro_co());
    }

    //�ʱ�ȭ
    void Init()
    {
        resetButton.SetActive(false);
        score = 0;
        stageLevel = 1;
        scroeUI.gameObject.SetActive(false);
        fingerprint.SetActive(false);
        enemysList = new List<Actor>();
        qHpBar = new Queue<HpBar>();

        if (!fadeInOut.gameObject.activeSelf)
        {
            fadeInOut.gameObject.SetActive(true);
        }

        this.Player.gameObject.layer = 0;

        maxStageLevel = Resources.LoadAll<StageData>("Level Data").Length;
    }

    IEnumerator Intro_co()
    {
        Init();
        Animator camAnim = Camera.main.GetComponent<Animator>();
        yield return new WaitWhile(() => camAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
        Destroy(camAnim);
        GameManager.instance.MainCamCtrl.SetCamOriginPos();
        this.Player.gameObject.layer = LayerMask.NameToLayer("Player");

        qBlodds = new Queue<GameObject>();
        bloodTop = new GameObject().transform;
        bloodTop.name = "Blood Particle Top";

        scroeUI.gameObject.SetActive(true);
        scroeAnim = scroeUI.GetComponent<Animator>();
        fingerprint.SetActive(true);

        LevelSetting();

        StartEnemySpawn();
        StartCarrotSpawn();

        Player.OnDeathEvent += () =>
        {
            //�� ������ ����
            StopEnemySpawn();
            //��� ������ ����
            StopCarrotSpawn();
            //���� ���� �̺�Ʈ
            StartCoroutine(GameOverEvent_cor());
        };

        resetButton.SetActive(true);
    }

    #region �� ����
    Coroutine SpawnRoutin;
    void StartEnemySpawn()
    {
        StopEnemySpawn();
        SpawnRoutin = StartCoroutine(EnemySpawn_co());
    }

    void StopEnemySpawn()
    {
        if (SpawnRoutin != null)
            StopCoroutine(SpawnRoutin);
    }

    IEnumerator EnemySpawn_co()
    {
        print("�� ���� ����");
        //������ ���� ������ ���ӵ�
        while (true)
        {
            Respwan();
            yield return new WaitForSeconds(nowStageData.enemyRespawnTerm);
        }
    }

    void Respwan()
    {
        if (enemysList.Count.Equals(0))
        {
            print("���� �ִ� ���� �����ϴ�.");
            return;
        }

        print("�� ����");

        Actor _enemy = enemysList[Random.Range(0, enemysList.Count)];

        float xPosition = (Random.Range(0, 2).Equals(0))
            ? fieldData.FieldRange.Left
            : fieldData.FieldRange.Right;

        _enemy.transform.position = new Vector3(xPosition, 1, 0);
        _enemy.SetRotation(Player.transform);

        enemysList.Remove(_enemy);
        _enemy.SetRandomAdditionalSpeed();
        _enemy.gameObject.SetActive(true);

        HpBar _hpBar;
        if (qHpBar.Count.Equals(0))
        {
            print("���� �ִ� ü�¹ٰ� �����ϴ�.");

            //ü�¹� ����, �θ� ��ü ������ ����
            _hpBar = Instantiate(hpBar);
            _hpBar.transform.SetParent(enemyHpBarsTop);
            _hpBar.myQueue = qHpBar;
        }
        else
            _hpBar = qHpBar.Dequeue();

        _hpBar.SetTarget(_enemy.GetComponent<Actor>());
        _hpBar.rect.position = Camera.main.WorldToScreenPoint(_enemy.MyHpBarPos);
        _hpBar.gameObject.SetActive(true);
    }
    #endregion

    void LevelSetting()
    {
        nowStageData = Resources.Load<StageData>($"Level Data/Level_{stageLevel:D2}");

        if (enemyTop is null)
        {
            enemyTop = new GameObject().transform;
            enemyTop.name = "Enemy Top";
        }

        for (int i = 0; i < nowStageData.addEnemyList.Length; i++)
        {
            Actor loadObject = Resources.Load<Actor>($"Enemy/{nowStageData.addEnemyList[i].enemyName}");
            print(loadObject.name);

            for (int j = 0; j < nowStageData.addEnemyList[i].addCount; j++)
            {
                Actor _enemy = Instantiate(loadObject, enemyTop);

                //���� �׾��� �� �̺�Ʈ �Լ� �Է�
                _enemy.OnDeathEvent += () =>
                {
                    enemysList.Add(_enemy);
                    _enemy.gameObject.SetActive(false);
                    _enemy.gameObject.layer = LayerMask.NameToLayer("Enemy");
                    Blood(_enemy.transform.position);
                };

                _enemy.deadLine = fieldData.DeadRange;
                _enemy.gameObject.SetActive(false);
                _enemy.transform.SetParent(enemyTop);
                enemysList.Add(_enemy);
            }
        }
    }

    #region ��� ����
    [Header("��� ����"), Space(25)]
    [SerializeField]
    GameObject carrot;
    [SerializeField]
    float regenTerm;

    Coroutine CarrotSpawn;

    void StartCarrotSpawn()
    {
        StopCarrotSpawn();
        CarrotSpawn = StartCoroutine(CarrotSpawn_co());
    }
    void StopCarrotSpawn()
    {
        if (CarrotSpawn != null)
            StopCoroutine(CarrotSpawn);
    }
    IEnumerator CarrotSpawn_co()
    {
        if (ReferenceEquals(carrot, null))
        {
            carrot = GameObject.Find("Carrot");
        }

        carrot.GetComponent<Carrot>().OnGetScoreEvent += () => GetScore();

        while (true)
        {
            yield return new WaitForSeconds(nowStageData.carrotRespawnTerm);
            if (!carrot.activeSelf)
                CarrotRespawn();
        }
    }

    [SerializeField]
    float avoidanceDistance = 1f;

    void CarrotRespawn()
    {
        float playerPosX = Player.transform.position.x;
        float posX;

        do
        {
            posX = Random.Range(fieldData.PlayerLimitRange.Left, fieldData.PlayerLimitRange.Right);
        } while (Mathf.Abs(playerPosX - posX) < avoidanceDistance);

        carrot.transform.position = new Vector3(posX, 0, 0);
        carrot.gameObject.SetActive(true);
    }

    //������ ó���ϴ� �κ�
    void GetScore()
    {
        scroeAnim.SetTrigger("Get");
        score += nowStageData.carrotPoint;
        scroeUI.text = $"Score\n{score}";
        //������ ���� ���������� ���� ������ �������� ��...
        if (score >= nowStageData.nextLevelScore)
        {
            print("�������� ������");

            if (stageLevel < maxStageLevel)
            {
                stageLevel++;
                LevelSetting();
            }
        }
        carrot.SetActive(false);
    }
    #endregion

    [SerializeField]
    GameObject blood;
    Transform bloodTop;
    Queue<GameObject> qBlodds;

    void Blood(Vector3 _pos)
    {
        GameObject b;
        if (qBlodds.Count.Equals(0))
        {
            b = Instantiate(blood);
            b.transform.SetParent(bloodTop);
            b.GetComponent<ReturnQueue>().SetMyQueue(qBlodds);
        }
        else
        {
            b = qBlodds.Dequeue();
        }
        b.transform.position = _pos;
        b.SetActive(true);
    }

    //�� ���� ����ǰ�...
    IEnumerator GameOverEvent_cor()
    {
        GameManager.instance.BGMManager.BGMStop();
        resetButton.SetActive(false);

        yield return StartCoroutine(Player.GameOver_cor());
        yield return StartCoroutine(GameManager.instance.BGMManager.GameOver_co());

        ShowResult sR = Instantiate(resultBoard, GameObject.Find("Canvas").transform).GetComponent<ShowResult>();
        sR.score = score;
        sR.fadeInOut = fadeInOut;
        yield return StartCoroutine(sR.ShowScore_co());
    }

    public void ResetGame()
    {
        StartCoroutine(ResetGame_co());
    }

    IEnumerator ResetGame_co()
    {
        GameManager.instance.BGMManager.BGMStop();
        Player.enabled = false;
        resetButton.SetActive(false);

        fadeInOut.gameObject.SetActive(true);
        fadeInOut.SetTrigger("Fade Out");

        yield return null;
        yield return new WaitWhile(() => fadeInOut.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);

        // Ÿ��Ʋ ȭ������ �ǵ��ư�
        SceneManager.LoadScene(0);
    }

}