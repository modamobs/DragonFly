using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    // Rigidbody2D 컴포넌트 참조
    Rigidbody2D rb;

    // 시작 시 호출됨
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D 컴포넌트 가져오기
        Jump(); // 코인 튀기기
    }

    // 코인을 위로 튀기는 함수
    void Jump()
    {
        // x축은 -1~0, y축은 3~5의 임의의 힘을 가함 (ForceMode2D.Impulse)
        rb.AddForce(new Vector2(Random.Range(-1,1),Random.Range(3,6)),ForceMode2D.Impulse);    
    }

    // 플레이어와 충돌 시 호출됨
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Player")
        {
            GameManager.Instance.ShowCoinCount(); // 코인 개수 표시
            Destroy(gameObject); // 코인 오브젝트 삭제
        }
    }
}
