using UnityEngine;

public class TouchPlease : MonoBehaviour
{
    [SerializeField]
    float fadeInTime;

    float time;


    Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Open"))
        {
            if(Input.anyKeyDown)
            {
                anim.SetTrigger("Close");
                time = 0;
            }

            return;
        }

        else        {
            time += Time.deltaTime;

            if (time > fadeInTime)
                anim.SetTrigger("Open");
        }

    }
}
