using UnityEngine;

[CreateAssetMenu]
public class GameData : ScriptableObject
{
    [SerializeField]
    Range fieldRange;
    public Range FieldRange => fieldRange;

    [SerializeField]
    Range playerLimitRange;
    public Range PlayerLimitRange => playerLimitRange;

    [SerializeField]
    Range deadRange;
    public Range DeadRange => deadRange;
}

[System.Serializable]
public class Range
{
    [SerializeField]
    float left, right;

    public float Left => left;
    public float Right => right;

    public Range(float left, float right)
    {
        this.left = left;
        this.right = right;
    }
}
