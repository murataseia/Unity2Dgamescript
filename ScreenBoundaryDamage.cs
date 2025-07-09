using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBoundaryDamage : MonoBehaviour
{
    public float damagePerSecond = 5f; // 画面外で受けるダメージ量（毎秒）
    private PlayerStats playerStats;

    void Start()
    {
        // PlayerStatsコンポーネントを取得
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStatsコンポーネントがアタッチされていません。");
        }
    }

    void Update()
    {
        if (playerStats == null) return; // PlayerStatsがない場合は何もしない

        if (IsOutOfScreen())
        {
            // TakeDamageを毎秒ダメージ量に基づいて呼び出す
            playerStats.TakeDamage((int)(damagePerSecond * Time.deltaTime));

            // HPが0以下になった場合、PlayerStats内で死亡処理を行う前提
            if (playerStats.currentHP <= 0)
            {
                Debug.Log("Player is dead.");
            }
        }
    }

    bool IsOutOfScreen()
    {
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        // 画面の範囲外であるかどうかを判定
        return viewportPosition.x < 0 || viewportPosition.x > 1 || viewportPosition.y < 0 || viewportPosition.y > 1;
    }
}
