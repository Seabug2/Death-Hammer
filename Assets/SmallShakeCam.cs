using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallShakeCam : MonoBehaviour
{
    Vector3 origin;

    private void Awake()
    {
        origin = transform.position;
    }

    private void Start()
    {
        StartCoroutine(SmallShake_co());
    }

    [SerializeField]
    float size, lerp;

    IEnumerator SmallShake_co()
    {
        while (true)
        {
            Vector3 RandPos = Random.insideUnitSphere * size;
            Vector3 targetVect = new Vector3(origin.x + RandPos.x, origin.y + RandPos.y, origin.z);

            float randT = Random.Range(1f,1.8f);
            
            while (randT > 0)
            {
                transform.position = Vector3.Slerp(transform.position, targetVect, Time.fixedDeltaTime);
                randT -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
