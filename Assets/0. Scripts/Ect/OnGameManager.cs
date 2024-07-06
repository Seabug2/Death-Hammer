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

    [Header("점수 관리")]
    [SerializeField]
    int stageLevel;
    int maxStageLevel;
    StageData nowStageData;
    [SerializeField]
    int score;

    [Header("적 생성 관리"), Space(25)]
    Transform enemyTop;
    [SerializeField]
    List<Actor> enemysList; //생성 대기 중인 적 리스트
    [SerializeField]
    float respawnTerm;

    [Header("UI 관리"), Space(25)]
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

    [Header("게임 관리"), Space(25)]
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

    //초기화
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
            //적 생성을 멈춤
            StopEnemySpawn();
            //당근 생성을 멈춤
            StopCarrotSpawn();
            //게임 오버 이벤트
            StartCoroutine(GameOverEvent_cor());
        };

        resetButton.SetActive(true);
    }

    #region 적 생성
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
        print("적 생성 시작");
        //게임이 끝날 때까지 지속됨
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
            print("남아 있는 적이 없습니다.");
            return;
        }

        print("적 생성");

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
            print("남아 있는 체력바가 없습니다.");

            //체력바 생성, 부모 객체 하위로 관리
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

                //적이 죽었을 때 이벤트 함수 입력
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

    #region 당근 생성
    [Header("당근 관리"), Space(25)]
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

    //점수를 처리하는 부분
    void GetScore()
    {
        scroeAnim.SetTrigger("Get");
        score += nowStageData.carrotPoint;
        scroeUI.text = $"Score\n{score}";
        //점수가 다음 스테이지를 위한 점수에 도달했을 때...
        if (score >= nowStageData.nextLevelScore)
        {
            print("스테이지 레벨업");

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

    //한 번만 실행되게...
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

        // 타이틀 화면으로 되돌아감
        SceneManager.LoadScene(0);
    }

}