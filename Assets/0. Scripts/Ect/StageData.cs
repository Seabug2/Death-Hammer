using UnityEngine;

[CreateAssetMenu]
public class StageData : ScriptableObject
{
    [Tooltip("이 점수를 달성하면 다음 레벨로 넘어갑니다.")]
    public int nextLevelScore;

    [Tooltip("새로운 레벨에 추가되는 적 리스트")]
    public MonsterData[] addEnemyList;
    [Tooltip("해당 레벨의 적 생성 주기")]
    public float enemyRespawnTerm;

    [Space(20)]
    [Tooltip("당근 생성 주기")]
    public int carrotRespawnTerm;
    [Tooltip("이번 라운드에 당근 점수")]
    public int carrotPoint;
}

[System.Serializable]
public class MonsterData
{
    public string enemyName;
    public int addCount;
}