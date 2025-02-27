using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField]
    Vector3 rotateVectSpeed;

    private void FixedUpdate()
    {
        transform.rotation *= Quaternion.Euler(rotateVectSpeed * Time.fixedDeltaTime);
    }
}
