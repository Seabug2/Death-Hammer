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

    //매 프레임마다 짝을 따라 다닙니다.
    private void FixedUpdate()
    {
        if(target)
            rect.position = Camera.main.WorldToScreenPoint(target.MyHpBarPos);
    }

    //짝이 정해지면 체력을 받아와 슬라이더 값을 바꿉니다.
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
        //사라질 때 파티클 추가?
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
