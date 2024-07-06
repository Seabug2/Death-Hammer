using UnityEngine;

public class ViewGuider : MonoBehaviour
{
    [SerializeField]
    float viewRange = 0;
    public float ViewRange => viewRange;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(viewRange,Camera.main.orthographicSize,0));   
    }
}
