using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    private bool playerInZone = false;
    private PlayerStats playerStats;
    private float damageInterval = 1f; // 1秒おき
    private float timer = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // "Player"タグを持つオブジェクトがゾーンに入ったとき
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            playerStats = other.GetComponent<PlayerStats>();

            if (playerStats == null)
            {
                Debug.LogError("PlayerStatsスクリプトが見つかりません！");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // "Player"タグを持つオブジェクトがゾーンから出たとき
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            playerStats = null;
        }
    }

    private void Update()
    {
        if (playerInZone && playerStats != null)
        {
            timer += Time.deltaTime;

            if (timer >= damageInterval)
            {
                timer = 0f; // タイマーをリセット
                playerStats.TakeDamage(50); // TakeDamageメソッドを呼び出す
                Debug.Log("Playerにダメージを与えました。現在のHP: " + playerStats.currentHP);
            }
        }
    }
}
