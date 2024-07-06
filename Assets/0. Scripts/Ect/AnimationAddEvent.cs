using UnityEngine;
using UnityEngine.UI;

public class AnimationAddEvent : MonoBehaviour
{
    public void AddEvent_Disabled()
    {
        GetComponent<Image>().enabled = enabled;
    }
}
