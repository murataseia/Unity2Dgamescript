using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageHandler : MonoBehaviour
{
    public float damageAmount = 10;         // Enemyから受けるダメージ量
    public float detectionRadius = 0.5f;  // ダメージを与える判定の半径
    public LayerMask enemyLayer;          // Enemy用のLayer

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();

        
    }

    private void Update()
    {
        // 半径内にEnemyがいるかどうかを判定
        Collider2D enemyCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, enemyLayer);

        if (enemyCollider != null)
        {
            // ダメージを受ける処理
            playerStats.currentHP -= damageAmount;
            
            // 一度ダメージを受けたらそのEnemyには再び当たらないようにしたい場合などは追加の処理が可能です
        }
    }

    // 視覚的に判定範囲を確認するためのデバッグ描画
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
