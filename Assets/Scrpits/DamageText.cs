using UnityEngine;
using TMPro;
using System.Collections;

public class DamageText : MonoBehaviour
{
    // 데미지 값을 표시할 텍스트
    public TextMeshProUGUI text;
    // 위로 떠오르는 속도
    public float floatUpSpeed = 50f;
    // 사라지는(페이드) 시간
    public float fadeDuration = 0.5f;

    private RectTransform rect;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        rect = GetComponent<RectTransform>(); // RectTransform 컴포넌트 참조
        canvasGroup = gameObject.AddComponent<CanvasGroup>(); // CanvasGroup 추가(투명도 조절용)
    }

    // 데미지 값을 표시하고 애니메이션 시작
    public void Show(int damage)
    {
        text.text = damage.ToString();
        StartCoroutine(FloatUp()); // 위로 떠오르며 사라지는 코루틴 실행
    }

    // 텍스트가 위로 떠오르며 점점 사라지는 코루틴
    private IEnumerator FloatUp()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            rect.anchoredPosition += Vector2.up * floatUpSpeed * Time.deltaTime; // 위로 이동
            canvasGroup.alpha = 1 - (elapsed / fadeDuration); // 점점 투명하게
            yield return null;
        }

        Destroy(gameObject); // 애니메이션 후 오브젝트 삭제
    }
}
