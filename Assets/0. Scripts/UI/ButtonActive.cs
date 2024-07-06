using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonActive : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Image img;
    RectTransform rect;
    void Awake()
    {
        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        img.color = new Vector4(1,1,1,.4f);
        rect.sizeDelta= new Vector2(512, 512);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        img.color = new Vector4(1,1,1,1);
        rect.sizeDelta= new Vector2(640, 640);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        img.color = new Vector4(1,1,1,.4f);
        rect.sizeDelta= new Vector2(512, 512);
    }

    [SerializeField]
    int myID;
    public int MyID => myID;
}
