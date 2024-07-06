using System.Collections;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    Vector3 originPos;
    public void SetCamOriginPos()
    {
        originPos = transform.position;
    }
    Coroutine camCoroutine;
    public void ShakeCamera(float _time = 1, float _power = 1)
    {
        if (!ReferenceEquals(camCoroutine, null))
            StopCoroutine(camCoroutine);

        camCoroutine = StartCoroutine(ShakeCamera_co(_time, _power));
    }
    IEnumerator ShakeCamera_co(float _time, float _power)
    {
        while (_time > 0)
        {
            float randX = Random.Range(-1f, 1f) * _power * _time;
            float randY = Random.Range(-1f, 1f) * _power * _time;
            transform.position = originPos + new Vector3(randX, randY, 0);
            _time -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        transform.position = originPos;
    }
}
