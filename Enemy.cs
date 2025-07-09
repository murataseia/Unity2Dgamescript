using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int maxHp = 100;
    public int hp;
    public int goldReward = 10;
    public int attackDamage = 10;

    [Header("UI Elements")]
    public Image hpBar;
    public Vector3 hpBarOffset = new Vector3(0, 1.5f, 0);

    [Header("Damage Feedback")]
    public GameObject damageTextPrefab;
    public Color damageColor = Color.red;
    public float flashDuration = 0.1f;
    public Vector3 damageTextOffsetRange = new Vector3(0.5f, 0.5f, 0);

    [Header("Attack Settings")]
    public GameObject attackCollider;
    public float attackCooldown = 1f;
    public float colliderEnableDelay = 0.3f;
    public float attackDuration = 0.5f;

    [Header("AI Settings")]
    public Transform player;
    public float detectionRange = 5f;
    public float attackRange = 1f;
    public float moveSpeed = 2f;

    private PlayerStats playerStats;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Color originalColor;

    private bool isChasing = false;
    private bool isAttacking = false;
    private float lastAttackTime = 0f;
    public bool isDead { get; private set; } = false;
    private bool isDamaged = false;

    [Header("Impact Effect")]
    public GameObject impactEffectPrefab;


    private void Start()
    {
        hp = maxHp;
        UpdateHpBar();
        playerStats = FindObjectOfType<PlayerStats>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        originalColor = spriteRenderer.color;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (animator != null)
        {
            animator.SetBool("isDead", false);
        }

        if (attackCollider != null)
        {
            attackCollider.SetActive(false);
        }
        Debug.Assert(playerStats != null, "PlayerStatsが正しく設定されていません");
        Debug.Assert(player != null, "Playerが正しく設定されていません");
    }

    private void Update()
    {
        if (isDead) return;

        UpdateHpBarPosition();
        HandleAI();
    }

    public void SetStats(int hp, int attack)
{
    maxHp = hp;
    this.hp = hp; // 現在のHPも最大HPに合わせる
    attackDamage = attack;
    UpdateHpBar();
}

    private void SpawnImpactEffect(Vector3 enemyPosition)
{
    if (impactEffectPrefab != null)
    {
        // ランダムなオフセットを計算
        float randomX = Random.Range(-1f, 1f); // 左右に±の範囲
        float randomY = Random.Range(-1f, 2f); // 上下に±の範囲
        Vector3 randomOffset = new Vector3(randomX, randomY, 0);

        // エフェクトを生成
        Instantiate(impactEffectPrefab, enemyPosition + randomOffset, Quaternion.identity);
    }
}

    private void UpdateHpBarPosition()
    {
        if (hpBar != null)
        {
            hpBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + hpBarOffset);
        }
    }

    private void HandleAI()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange && distanceToPlayer > attackRange && !isAttacking)
        {
            FacePlayer();
            ChasePlayer();
        }
        else if (distanceToPlayer <= attackRange)
        {
            if (Time.time > lastAttackTime + attackCooldown)
            {
                AttackPlayer();
            }
            else
            {
                Idle();
            }
        }
        else
        {
            StopChasing();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.gameObject.CompareTag("Attack") && !isDead && !isDamaged)
    {
        TakeDamage(playerStats.attackPower);
        isDamaged = true;
    }
}

private void OnTriggerExit2D(Collider2D collision)
{
    if (collision.gameObject.CompareTag("Attack"))
    {
        isDamaged = false;
    }
}

    private void TakeDamage(int damage)
{
    hp -= damage;
    UpdateHpBar();
    ShowDamageText(damage);
    SpawnImpactEffect(transform.position); // ランダム位置でエフェクト生成
    StartCoroutine(FlashRed());

    if (hp <= 0)
    {
        Die();
    }
}

    private void UpdateHpBar()
    {
        if (hpBar != null)
        {
            hpBar.fillAmount = (float)hp / maxHp;
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        if (animator != null)
        {
            animator.ResetTrigger("attack");
            animator.SetBool("isRunning", false);
            animator.SetBool("isDead", true);
        }

        if (GetComponent<Collider2D>() != null)
        {
            GetComponent<Collider2D>().enabled = false;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        if (playerStats != null)
        {
            playerStats.gold += goldReward;
        }

        StartCoroutine(WaitForDieAnimation());
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    private void ShowDamageText(int damageAmount)
    {
        if (damageTextPrefab != null)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-damageTextOffsetRange.x, damageTextOffsetRange.x),
                Random.Range(2, damageTextOffsetRange.y),
                0
            );

            GameObject damageText = Instantiate(damageTextPrefab, transform.position + randomOffset, Quaternion.identity);
            damageText.GetComponent<TextMesh>().text = damageAmount.ToString();
            Destroy(damageText, 1f);
        }
    }

    private IEnumerator WaitForDieAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    private void ChasePlayer()
    {
        if (!isChasing)
        {
            isChasing = true;
            isAttacking = false;
            animator.SetBool("isRunning", true);
        }

        Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    private void AttackPlayer()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            isChasing = false;
            lastAttackTime = Time.time;
            animator.SetTrigger("attack");
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        yield return new WaitForSeconds(colliderEnableDelay); // アニメーションの一部が進んだタイミングでコライダーを有効化

        if (attackCollider != null)
        {
            attackCollider.SetActive(true); // 攻撃コライダー有効化
            yield return new WaitForSeconds(attackDuration); // 攻撃有効時間
            attackCollider.SetActive(false); // 攻撃コライダー無効化
        }

        isAttacking = false;
    }

    private void Idle()
    {
        if (!isAttacking && !isChasing)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdle", true);
        }
    }

    private void FacePlayer()
    {
        if (player.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
        FlipAttackCollider();
    }

    private void FlipAttackCollider()
    {
        if (spriteRenderer.flipX)
        {
            attackCollider.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            attackCollider.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void StopChasing()
    {
        if (isChasing)
        {
            isChasing = false;
            animator.SetBool("isRunning", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}