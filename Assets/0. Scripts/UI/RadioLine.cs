using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class RadioLine : MonoBehaviour
{
    RectTransform rect;
    Text text;
    [SerializeField]
    Vector3 originPos;

    [SerializeField]
    float speed;

    private void Awake()
    {
        text = GetComponent<Text>();
        index = 0;
        text.text = lines[index];
        rect = GetComponent<RectTransform>();
        originPos = transform.position;
    }

    private void FixedUpdate()
    {
        transform.position -= new Vector3(speed * Time.fixedDeltaTime, 0, 0);
        CheckIfInvisible();
    }

    string[] lines = {
        "����� �����Ϸ� �� ���� �༮���� ������ �����̼���!",
        "��ü�� �������� ������! �Ÿ��� �Ǿ� ���ִ� ����� ���� ����� �˴ϴ�!",
        "ȭ���� �� ������ ���⸦ �ٲ� �� �ֽ��ϴ�.",
        "������ �ٲٸ� ������ �ʱ�ȭ �˴ϴ�.",
        "���� ���̶� �ӵ��� �ٸ��ϴ�.",
        "����� ȹ���ϸ� ������ �����ϴ�.",
        "����� ���� ��Ȯ�� ���� ���� �� ���� ������ �����ɴϴ�.",
        "�� ���� ����� ȹ���ϼ���!"
    };

    int index;

    private void CheckIfInvisible()
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);

        bool isVisible = false;
        foreach (Vector3 corner in corners)
        {
            Vector3 viewportPoint = Camera.main.WorldToViewportPoint(corner);
            if (viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1)
            {
                isVisible = true;
                break;
            }
        }

        if (!isVisible)
        {
        }
    }
}
