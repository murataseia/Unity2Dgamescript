using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 7f;
    public float jumpForce = 11f;
    public float rollMultiplier = 2f;
    public LayerMask groundLayer;

    [Header("ジャンプ設定")]
    public int maxExtraJumps = 1;

    [Header("スタミナ設定")]
    public float rollStaminaCost = 20f;

    [Header("UIコントロール")]
    public Joystick joystick;  // Inspector からアタッチ
    public Button jumpButton;
    public Button rollButton;
    public Button attackButton;

    [Header("攻撃設定")]
    public GameObject attackCollider;

    private static PlayerController instance;  // Singleton インスタンス

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded = false;
    private bool facingRight = true;
    private bool isAttacking = false;
    private bool isRolling = false;
    private int extraJumps;
    private Vector2 moveInput;
    private bool isDead = false; // 死亡状態を管理
    public GameObject gameOverUI; // ゲームオーバー画面のUI

    void Awake()
    {
        // シングルトンパターン: シーン間でオブジェクトを保持
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);  // すでに存在する場合は重複を避けるため削除
        }
        
    }
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        extraJumps = maxExtraJumps;
        // GameOverタグのついたキャンバスを自動設定
        SetupGameOverUI();

        // UI ボタンにイベントを追加
        jumpButton.onClick.AddListener(Jump);
        rollButton.onClick.AddListener(() => StartCoroutine(PerformRoll()));
        attackButton.onClick.AddListener(() => StartCoroutine(PerformAttack()));
        gameOverUI.SetActive(false); // ゲームオーバー画面を非表示

    }

    void Update()
    {
        if (isDead) return; // 死亡時は入力処理を停止

        float horizontalInput = joystick.Horizontal != 0 ? joystick.Horizontal : Input.GetAxis("Horizontal");
        moveInput = new Vector2(horizontalInput, 0);

        if (!isRolling && !isAttacking)
        {
            rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);

            if (moveInput.x != 0)
            {
                animator.SetBool("IsRunning", true);
            }
            else
            {
                animator.SetBool("IsRunning", false);
            }

            if (moveInput.x < 0 && facingRight)
            {
                Flip();
            }
            else if (moveInput.x > 0 && !facingRight)
            {
                Flip();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(PerformRoll());
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(PerformAttack());
        }
    

        // キーボード操作
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(PerformRoll());
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(PerformAttack());
        }
    }
    private void SetupGameOverUI()
{
    if (gameOverUI != null)
    {
        // GameOverUIを破壊不能に設定
        DontDestroyOnLoad(gameOverUI);
    }
    else
    {
        Debug.LogWarning("GameOverUIがアタッチされていません。");
    }
}

    public void OnPlayerDie()
    {
        if (isDead) return; // すでに死亡処理が行われている場合は無視

        isDead = true; // 死亡状態を設定
        animator.SetBool("IsRunning", false);
        rb.velocity = Vector2.zero; // 移動を停止
        DisablePlayerControls();
        StartCoroutine(HandleDeath());
    }

    private IEnumerator HandleDeath()
    {
        if (animator != null)
        {
            animator.Play("Die"); // "Die" アニメーションを再生
        }

        yield return new WaitForSeconds(2f); // アニメーション再生時間

        // すべての Canvas を非表示にする
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            if (canvas.gameObject != gameOverUI) // ゲームオーバー画面以外を非表示
            {
                canvas.gameObject.SetActive(false);
            }
        }

        // ゲームオーバー画面を表示
        gameOverUI.SetActive(true);
    }

    private void DisablePlayerControls()
    {
        // 入力と移動を無効化
        rb.velocity = Vector2.zero; // 強制停止
    }

    public void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.Play("Jump");
            extraJumps = maxExtraJumps;
            isGrounded = false;
        }
        else if (extraJumps > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.Play("Jump");
            extraJumps--;
        }
    }

    public IEnumerator PerformAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            animator.Play("Attack");

            if (attackCollider != null)
            {
                attackCollider.SetActive(true);
            }

            yield return new WaitForSeconds(0.24f);

            if (attackCollider != null)
            {
                attackCollider.SetActive(false);
            }

            isAttacking = false;
        }
    }

    public IEnumerator PerformRoll()
    {
        if (!isRolling && PlayerStats.Instance.UseStamina(rollStaminaCost)) // スタミナを消費可能かチェック
        {
            isRolling = true;
            animator.Play("Roll");
            float horizontalInput = joystick.Horizontal != 0 ? joystick.Horizontal : Input.GetAxis("Horizontal");

            rb.velocity = new Vector2(horizontalInput * moveSpeed * rollMultiplier, rb.velocity.y);
            yield return new WaitForSeconds(0.2f);
            isRolling = false;

            // ロール後スタミナ回復を開始
            PlayerStats.Instance.StartRecovery();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("IsGrounded", true);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetBool("IsGrounded", false);
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
    public void ResetController()
    {
        isDead = false;
        gameOverUI.SetActive(false); // ゲームオーバー画面を非表示

        // アニメーションを初期状態にリセット
        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }
        
    }
    
}
