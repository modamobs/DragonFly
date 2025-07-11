using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // 현재 코인 개수
    public int coin=0;

    // 코인 개수를 표시할 텍스트
    public TextMeshProUGUI textMeshProCoin;

    // 게임 오버 UI 패널
    public GameObject gameOverPanel;
    
    // 게임 오버 텍스트
    public TextMeshProUGUI gameOverText;
    
    // 재시작 버튼
    public Button restartButton;
    
    // 특수 미사일 쿨타임 UI
    public Image specialMissileCooldownOverlay; // 자식 이미지 (어두운 쿨타임 오버레이)
    public TextMeshProUGUI specialMissileCooldownText;

    // 게임 상태
    public bool isGameOver = false;

    // 싱글톤 인스턴스
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        // 싱글톤 인스턴스 설정 (씬 재로드 시 새로운 인스턴스 생성)
        Instance = this;
    }

    void Start()
    {
        // 게임 시작 시 게임 오버 UI 비활성화
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        // 버튼 이벤트 연결
        if (restartButton != null)
        {
            Debug.Log("RestartButton이 연결되었습니다!");
            restartButton.onClick.RemoveAllListeners(); // 기존 리스너 제거
            restartButton.onClick.AddListener(RestartGame);
        }
        else
        {
            Debug.LogWarning("RestartButton이 할당되지 않았습니다!");
        }
    }

    // 코인 개수를 증가시키고 UI에 표시, 2개마다 미사일 업그레이드
    public void ShowCoinCount()
    {
        coin++;
        textMeshProCoin.SetText(coin.ToString()); // 코인 개수 UI 갱신

        // 코인이 2의 배수일 때마다 미사일 업그레이드
        if(coin %2 ==0)
        {
            Player player = FindFirstObjectByType<Player>();
            if (player != null)
            {
                player.MissileUp(); // 2개마다 미사일 업그레이드
            }
        }
    }
    
    // 게임 오버 처리
    public void GameOver()
    {
        if (isGameOver) return; // 이미 게임 오버 상태면 리턴
        
        isGameOver = true;
        Time.timeScale = 0f; // 게임 일시정지
        
        // 게임 오버 UI 활성화
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            // UI가 Time.timeScale의 영향을 받지 않도록 설정
            Canvas canvas = gameOverPanel.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = 100; // 최상위에 표시
            }
        }
        
        // 게임 오버 텍스트 설정
        if (gameOverText != null)
        {
            gameOverText.SetText($"Game Over!\nFinal Score: {coin}");
        }
        
        Debug.Log("게임 오버 처리가 완료되었습니다!");
    }
    
    // 특수 미사일 쿨타임 UI 업데이트
    public void UpdateSpecialMissileCooldown(float cooldownRemaining, float maxCooldown)
    {
        // 자식 이미지(어두운 오버레이)로 쿨타임 표시
        if (specialMissileCooldownOverlay != null)
        {
            // 쿨타임 중일 때는 어두운 이미지가 표시됨 (fillAmount = 1에서 0으로 감소)
            // 쿨타임이 끝나면 어두운 이미지가 사라짐 (fillAmount = 0)
            specialMissileCooldownOverlay.fillAmount = cooldownRemaining / maxCooldown;
        }
        
        if (specialMissileCooldownText != null)
        {
            if (cooldownRemaining > 0)
            {
                specialMissileCooldownText.SetText($"{cooldownRemaining:F1}s");
            }
            else
            {
                specialMissileCooldownText.SetText("READY!");
            }
        }
    }
    
    // 게임 재시작
    public void RestartGame()
    {
        Debug.Log("RestartGame 함수가 호출되었습니다!"); // 디버그 로그 추가
        
        Time.timeScale = 1f; // 게임 시간 복원
        
        // 게임 상태 초기화
        isGameOver = false;
        
        // 씬 다시 로드
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}
