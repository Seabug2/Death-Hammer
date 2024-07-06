using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetAlphaHit : MonoBehaviour
{
    public float AlphaThreshold = 0.1f;
    private void Awake()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = AlphaThreshold;
    }
}
