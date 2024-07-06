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
        "당근을 서리하러 온 못된 녀석들을 모조리 쳐죽이세요!",
        "시체는 걱정하지 마세요! 거름이 되어 맛있는 당근을 위한 양분이 됩니다!",
        "화면을 꾹 누르면 무기를 바꿀 수 있습니다.",
        "방향을 바꾸면 공격이 초기화 됩니다.",
        "같은 적이라도 속도가 다릅니다.",
        "당근을 획득하면 점수가 오릅니다.",
        "당근을 많이 수확할 수록 더욱 더 많은 적들이 몰려옵니다.",
        "더 많은 당근을 획득하세요!"
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
