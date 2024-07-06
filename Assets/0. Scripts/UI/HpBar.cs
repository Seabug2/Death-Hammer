using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [HideInInspector]
    public RectTransform rect;

    [SerializeField]
    Slider slider;

    [SerializeField]
    Image fill;

    [SerializeField]
    Actor target;

    public Queue<HpBar> myQueue;

    //�� �����Ӹ��� ¦�� ���� �ٴմϴ�.
    private void FixedUpdate()
    {
        if(target)
            rect.position = Camera.main.WorldToScreenPoint(target.MyHpBarPos);
    }

    //¦�� �������� ü���� �޾ƿ� �����̴� ���� �ٲߴϴ�.
    public void SetTarget(Actor _target)
    {
        print(_target.gameObject.name);
        target = _target;
        target.OnHitEvent += ChangedValue;
        target.OnDeathEvent += Disconnected;

        slider.maxValue = target.MaxHp;
        slider.value = target.CurrentHp;
        ChangedValue();
    }

    public void Disconnected()
    {
        target.OnHitEvent -= ChangedValue;
        target.OnDeathEvent -= Disconnected;
        target = null;
        rect.localPosition = Vector3.zero;
        myQueue?.Enqueue(this);
        //����� �� ��ƼŬ �߰�?
        gameObject.SetActive(false);
    }

    public void ChangedValue()
    {
        slider.value = target.CurrentHp;

        if (slider.value <= slider.maxValue * .1f)
        {
            fill.color = Color.red;
        }
        else if (slider.value <= slider.maxValue *.5f)
        {
            fill.color = Color.yellow;
        }
        else
        {
            fill.color = Color.green;
        }
    }
}
