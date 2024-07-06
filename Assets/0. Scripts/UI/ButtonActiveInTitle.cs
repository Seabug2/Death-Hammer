using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonActiveInTitle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    Image img;
    RectTransform rect;

    //bool ���� ���� myMark�� Ȱ��ȭ �ǰų� ��Ȱ��ȭ�ȴ�.
    List<int> targetList;

    public bool isSelected;
    public GameObject myMark;

    public Color selectedColor;
    public Color unselectedColor;
    public Vector2 selectedSize;
    public Vector2 unselectedSize;

    public void Init(int _id,Sprite _sprt, List<int> _targetList)
    {
        myID = _id;
        rect = GetComponent<RectTransform>();

        img = GetComponent<Image>();
        img.sprite = _sprt;

        myMark = transform.Find("Check Mark").gameObject;
        isSelected = false;
        myMark.SetActive(isSelected);

        targetList = _targetList;
    }
    private void OnEnable()
    {
        img.color = isSelected ? selectedColor : unselectedColor;
        rect.sizeDelta = unselectedSize;
        myMark.SetActive(isSelected);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        img.color = isSelected ? selectedColor : unselectedColor;
        rect.sizeDelta = selectedSize;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        img.color = isSelected ? selectedColor : unselectedColor;
        rect.sizeDelta = unselectedSize;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        img.color = isSelected ? selectedColor : unselectedColor;
        rect.sizeDelta = unselectedSize;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }

    int myID;
    
    public void OnClick()
    {
        //�������� ��ư�� ������.
        if (isSelected)
        {
            isSelected = false;
            myMark.SetActive(isSelected);
            if (targetList.Contains(myID))
            {
                targetList.Remove(myID);
            }
        }
        //�������� �ƴ� ��ư��
        else
        {
            // ���� ���� ���Ⱑ 4�� ������ ���
            if (targetList.Count < 4)
            {
                //Ȱ��ȭ �ȴ�.
                isSelected = true;
                myMark.SetActive(isSelected);
                targetList.Add(myID);
            }
        }

        img.color = isSelected ? selectedColor : unselectedColor;
        rect.sizeDelta = unselectedSize;
    }

}
