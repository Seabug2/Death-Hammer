using UnityEngine;

[CreateAssetMenu]
public class StageData : ScriptableObject
{
    [Tooltip("�� ������ �޼��ϸ� ���� ������ �Ѿ�ϴ�.")]
    public int nextLevelScore;

    [Tooltip("���ο� ������ �߰��Ǵ� �� ����Ʈ")]
    public MonsterData[] addEnemyList;
    [Tooltip("�ش� ������ �� ���� �ֱ�")]
    public float enemyRespawnTerm;

    [Space(20)]
    [Tooltip("��� ���� �ֱ�")]
    public int carrotRespawnTerm;
    [Tooltip("�̹� ���忡 ��� ����")]
    public int carrotPoint;
}

[System.Serializable]
public class MonsterData
{
    public string enemyName;
    public int addCount;
}