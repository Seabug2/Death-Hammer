using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Warning : MonoBehaviour
{
    public Text text;
    public void Messege(int count)
    {
        if(text == null)
        {
            text = transform.GetComponentInChildren<Text>();
        }
        text.text = $"������ ���Ⱑ �����մϴ�!\n({count} / 4)";
        StartCoroutine(CloseAnykeyDown());
    }
    IEnumerator CloseAnykeyDown()
    {
        yield return new WaitForSecondsRealtime(1);
        yield return new WaitUntil(() => Input.anyKey);
        gameObject.SetActive(false);
        yield break;
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
