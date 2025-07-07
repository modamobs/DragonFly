using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 데미지 텍스트 프리팹
    public GameObject damageTextPrefab;

    // 스프라이트 렌더러 및 색상 관련 변수
    private SpriteRenderer spriteRenderer;
    public Color flashColor = Color.red; // 피격 시 깜빡일 색상
    public float flashDuration = 0.1f;   // 깜빡임 지속 시간
    private Color originalColor;         // 원래 색상 저장

    // 적의 체력
    public float enemyHp = 1;

    [SerializeField]
    public float moveSpeed = 1f; // 이동 속도

    // 코인 및 이펙트 프리팹
    public GameObject Coin;
    public GameObject Effect;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // 스프라이트 렌더러 컴포넌트 참조
        originalColor = spriteRenderer.color; // 원래 색상 저장
    }

    // 적이 피격 시 깜빡임 효과
    public void Flash()
    {
        StopAllCoroutines(); // 기존 코루틴 중지
        StartCoroutine(FlashRoutine());
    }

    // 피격 시 색상 변경 코루틴
    private IEnumerator FlashRoutine()
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    // 이동 속도 설정
    public void SetMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }

    // 매 프레임마다 아래로 이동, 화면 밖으로 나가면 삭제
    void Update()
    {
        transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        if (transform.position.y < -7f)
        {
            Destroy(this.gameObject);
        }
    }

    // 미사일과 충돌 시 처리
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Missile")
        {
            Missile missile = collision.GetComponent<Missile>();
            StopAllCoroutines(); // 기존 코루틴 중지
            StartCoroutine("HitColor"); // 피격 색상 코루틴 실행
            // Flash(); // 대체 가능

            enemyHp = enemyHp- missile.missileDamege; // 체력 감소
            if (enemyHp < 0 )
            {
                Destroy(gameObject); // 적 삭제
                Instantiate(Coin, transform.position, Quaternion.identity); // 코인 생성
                Instantiate(Effect, transform.position, Quaternion.identity); // 이펙트 생성
            }
            TakeDamage(missile.missileDamege); // 데미지 팝업 표시
        }
    }

    // 피격 시 색상 변경 코루틴
    IEnumerator HitColor()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }

    // 데미지 팝업 표시 함수
    void TakeDamage(int damage)
    {
        // 체력 감소 처리 등...
        DamagePopupManager.Instance.CreateDamageText(damage, transform.position);
    }
}
