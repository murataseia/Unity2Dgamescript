using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    public Canvas mainCanvas; // 通常ゲーム用のCanvas
    public Canvas subCanvas;
        public float maxHP = 100f;
    public float currentHP;
    public float maxStamina = 100f;
    public float currentStamina;
    public int gold = 0;
    public int attackPower = 10;
    public float recoveryRate = 5f; // スタミナ回復速度
    public float recoveryDelay = 2f; // ロール後のスタミナ回復開始までの遅延時間
    public static PlayerStats Instance { get; private set; }


    private Coroutine recoveryCoroutine;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    
    private Vector3 initialPosition;
    private bool canTakeDamage = true; // ダメージを受けるかどうかを判定するフラグ


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialPosition = transform.position; // キャラクターの初期位置を記録
        LoadPlayerStats(); // スタート時にセーブデータをロード
        
    }
    void Update()
    {
        if (currentHP <= 0)
            {
                currentHP = 0;
                NotifyDeath(); // 死亡を通知
                SavePlayerStats();
            }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ゴールドを支払うメソッド
    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            return true;  // 支払い成功
        }
        return false;      // ゴールド不足で支払い失敗
    }

    // HPを強化するメソッド
    public void IncreaseMaxHP(int amount)
    {
        maxHP += amount;
    }

    // スタミナを強化するメソッド
    public void IncreaseMaxStamina(int amount)
    {
        maxStamina += amount;
    }

    
    public void TakeDamage(int damage)
    {
        if (canTakeDamage)
        {
            currentHP -= damage;

            if (currentHP <= 0)
            {
                currentHP = 0;
                NotifyDeath(); // 死亡を通知
                SavePlayerStats();
            }
            else
            {
                StartCoroutine(FlashRed());
            }

            canTakeDamage = false;
        }
    }

    private void NotifyDeath()
{
    // PlayerController に通知
    var playerController = GetComponent<PlayerController>();
    if (playerController != null)
    {
        playerController.OnPlayerDie();
    }

    // メインCanvasを非表示にする
    if (mainCanvas != null && subCanvas != null)
    {
        mainCanvas.gameObject.SetActive(false);
        subCanvas.gameObject.SetActive(false);
    }
}


public void RetryGame()
{
    SceneManager.LoadScene("Stage");
    StartCoroutine(ResetAfterSceneLoad());
}

public void ReturnToTitle()
{
    SceneManager.LoadScene("Title");
    StartCoroutine(ResetAfterSceneLoad());
}

private IEnumerator ResetAfterSceneLoad()
{
    yield return new WaitForSeconds(0.1f); // シーンロードが完了するのを少し待つ
    ResetPlayer();                        // リセット処理を呼び出す
}
public void ResetPlayer()
{
    LoadPlayerStats();
    transform.position = initialPosition; // 初期位置に戻す
    currentHP = maxHP;                   // HPを最大値に戻す
    currentStamina = maxStamina;         // スタミナを最大値に戻す

    // メインCanvasを再表示する
    if (mainCanvas != null && subCanvas != null)
    {
        mainCanvas.gameObject.SetActive(true);
        subCanvas.gameObject.SetActive(true);
    }

    if (animator != null)
    {
        animator.Rebind();               // アニメーションをリセット
        animator.Update(0f);             // アニメーションを初期フレームに設定
    }
    
    // PlayerController のリセットを呼び出す
    var playerController = GetComponent<PlayerController>();
    if (playerController != null)
    {
        playerController.ResetController();
    }
}


    public bool UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            return true;
        }
        return false;
    }

    public void RecoverStamina(float amount)
    {
        currentStamina += amount;
        if (currentStamina > maxStamina) currentStamina = maxStamina;
    }

    public void StartRecovery()
    {
        if (recoveryCoroutine != null)
        {
            StopCoroutine(recoveryCoroutine);
        }
        recoveryCoroutine = StartCoroutine(StaminaRecovery());
    }

    private IEnumerator StaminaRecovery()
    {
        yield return new WaitForSeconds(recoveryDelay);
        while (currentStamina < maxStamina)
        {
            RecoverStamina(recoveryRate * Time.deltaTime);
            yield return null;
        }
        recoveryCoroutine = null;
    }

    // プレイヤーがコインに触れたときにゴールドを増やす処理
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            gold += 1; // ゴールドを1増やす
            Destroy(other.gameObject); // コインを消す
        }
        // EnemyAttackタグを持つオブジェクトと重なった場合のダメージ処理
        if (other.gameObject.CompareTag("EnemyAttack"))
        {
            TakeDamage(10); // ここでダメージ値を設定
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // ここで重なりが無くなった時にダメージを再度受けられるようにする
        if (other.gameObject.CompareTag("EnemyAttack"))
        {
            canTakeDamage = true; // 再度ダメージを受けられるように設定
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red; // 一時的に赤くする
        yield return new WaitForSeconds(0.5f); // 0.5秒間赤いままにする
        spriteRenderer.color = Color.white; // 元の色に戻す
    }

    private void SavePlayerStats()
    {
        PlayerPrefs.SetFloat("MaxHP", maxHP);
        PlayerPrefs.SetFloat("CurrentHP", currentHP);
        PlayerPrefs.SetFloat("MaxStamina", maxStamina);
        PlayerPrefs.SetFloat("CurrentStamina", currentStamina);
        PlayerPrefs.SetInt("Gold", gold);
        PlayerPrefs.SetInt("AttackPower", attackPower);
        PlayerPrefs.Save();
        Debug.Log("Player stats saved.");
    }

    private void LoadPlayerStats()
    {
        if (PlayerPrefs.HasKey("MaxHP"))
        {
            maxHP = PlayerPrefs.GetFloat("MaxHP");
            currentHP = PlayerPrefs.GetFloat("CurrentHP");
            maxStamina = PlayerPrefs.GetFloat("MaxStamina");
            currentStamina = PlayerPrefs.GetFloat("CurrentStamina");
            gold = PlayerPrefs.GetInt("Gold");
            attackPower = PlayerPrefs.GetInt("AttackPower");
            Debug.Log("Player stats loaded.");
        }
        else
        {
            Debug.Log("No saved data found. Starting with default stats.");
            currentHP = maxHP;
            currentStamina = maxStamina;
        }
    }
}
