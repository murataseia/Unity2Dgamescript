using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshProを使用するための名前空間

public class UIManager : MonoBehaviour
{
    public Image hpBar;
    public Image staminaBar;
    public TextMeshProUGUI goldText; // TMPのTextコンポーネントに変更

    private PlayerStats playerStats; // playerStatsを自動で設定するため、publicからprivateに変更

    private void Awake()
    {
        // すでにUIManagerが存在する場合、現在のオブジェクトを破棄する
        if (FindObjectsOfType<UIManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            // 現在のUIManagerオブジェクトをシーン間で保持する
            DontDestroyOnLoad(gameObject);
        }

        // PlayerStatsの自動設定
        FindPlayerStats();
    }

    private void Update()
    {
        if (playerStats == null)
        {
            FindPlayerStats(); // プレイヤーが存在しない場合、再度検索
            return;
        }

        UpdateHPBar();
        UpdateStaminaBar();
        UpdateGoldText();
    }

    private void UpdateHPBar()
    {
        // HPバーの更新
        if (playerStats != null && playerStats.maxHP > 0)
        {
            hpBar.fillAmount = playerStats.currentHP / playerStats.maxHP;
        }
    }

    private void UpdateStaminaBar()
    {
        // スタミナバーの更新
        if (playerStats != null && playerStats.maxStamina > 0)
        {
            staminaBar.fillAmount = playerStats.currentStamina / playerStats.maxStamina;
        }
    }

    private void UpdateGoldText()
    {
        // ゴールドの更新 (TMPを使用)
        if (playerStats != null)
        {
            goldText.text = playerStats.gold.ToString();
        }
    }

    private void FindPlayerStats()
    {
        // "Player" タグの付いたオブジェクトを検索
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerStats = playerObject.GetComponent<PlayerStats>();
        }
    }
}
