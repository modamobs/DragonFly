using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 플레이어 이동 속도
    [SerializeField]
    float moveSpeed = 1f;
    // 현재 미사일 프리팹 인덱스
    int missIndex = 0;
    // 미사일 프리팹 배열
    public GameObject[] missilePrefab;
    // 미사일 생성 위치
    public Transform spPostion;

    // 미사일 발사 간격(초)
    [SerializeField]
    private float shootInverval = 0.05f;

    // 마지막 발사 시간
    private float lastshotTime = 0f;

    // 애니메이터 컴포넌트 참조
    private Animator animator;
    
    // 특수 미사일 관련
    public GameObject specialMissilePrefab; // 특수 미사일 프리팹
    [SerializeField]
    private float specialMissileCooldown = 5f; // 특수 미사일 쿨타임 (초)
    private float lastSpecialMissileTime = -5f; // 마지막 특수 미사일 발사 시간
    
    // 플레이어 체력
    [SerializeField]
    private int health = 1;
    
    // 플레이어가 죽었는지 여부
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>(); // Animator 컴포넌트 가져오기
    }

    // 매 프레임마다 이동 및 발사 처리
    void Update()
    {
        if (isDead) return; // 죽었으면 Update 처리 건너뜀

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        
        Vector3 moveTo= new Vector3(horizontalInput, 0,0);
        transform.position += moveTo*moveSpeed*Time.deltaTime; // 좌우 이동
        
        // 애니메이션 상태 변경
        if (horizontalInput < 0)
        {
            animator.Play("Left"); // 왼쪽 이동 애니메이션
        }
        else if (horizontalInput > 0)
        {
            animator.Play("Right"); // 오른쪽 이동 애니메이션
        }
        else
        {
            animator.Play("Idle"); // 가운데(정지) 애니메이션
        }
        Shoot(); // 미사일 발사
        
        // 특수 미사일 발사 (스페이스바)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShootSpecialMissile();
        }
        
        // 특수 미사일 쿨타임 UI 업데이트
        UpdateSpecialMissileCooldown();
    }

    // 미사일 발사 함수
    void Shoot()
    {
        if (Time.time - lastshotTime > shootInverval)
        {

            Instantiate(missilePrefab[missIndex], spPostion.position, Quaternion.identity); // 미사일 생성
            lastshotTime = Time.time; // 마지막 발사 시간 갱신
            
        }

    }

    // 특수 미사일 발사 함수
    void ShootSpecialMissile()
    {
        // 쿨타임 체크
        if (Time.time - lastSpecialMissileTime >= specialMissileCooldown)
        {
            if (specialMissilePrefab != null)
            {
                // 특수 미사일 생성 (2배 크기)
                GameObject specialMissile = Instantiate(specialMissilePrefab, spPostion.position, Quaternion.identity);

                // 크기를 2배로 설정
                specialMissile.transform.localScale = Vector3.one * 2f;

                lastSpecialMissileTime = Time.time; // 마지막 특수 미사일 발사 시간 갱신
                Debug.Log("Time.time: " + Time.time + ", lastSpecialMissileTime: " + lastSpecialMissileTime);
            }
        }
    }
    
    // 특수 미사일 쿨타임 UI 업데이트
    void UpdateSpecialMissileCooldown()
    {
        float cooldownRemaining = Mathf.Max(0f, specialMissileCooldown - (Time.time - lastSpecialMissileTime));
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateSpecialMissileCooldown(cooldownRemaining, specialMissileCooldown);
        }
    }

    // 미사일 업그레이드 함수
    public void MissileUp()
    {
        missIndex++; // 미사일 종류 업그레이드
        shootInverval = shootInverval - 0.1f; // 발사 간격 감소(더 빠르게)
        if (shootInverval <=0.1f)
        {
            shootInverval = 0.1f; // 최소 발사 간격 제한
        }
        if (missIndex>=missilePrefab.Length)
        {
            missIndex=missilePrefab.Length-1; // 인덱스 범위 제한
        }
    }
    
    // 적과의 충돌 처리
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return; // 이미 죽었으면 리턴
        
        // 적 또는 적의 미사일과 충돌 시
        if (other.CompareTag("Enemy") || other.CompareTag("EnemyMissile"))
        {
            TakeDamage(1);
        }
    }
    
    // 데미지 받기
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        health -= damage;
        
        if (health <= 0)
        {
            Die();
        }
    }
    
    // 플레이어 사망 처리
    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        // 게임 매니저에 게임 오버 알림
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        
        // 플레이어 오브젝트 비활성화 또는 파괴
        gameObject.SetActive(false);
    }
}
