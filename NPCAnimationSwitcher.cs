using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPCAnimationSwitcher : MonoBehaviour
{
    public TextMeshProUGUI questionText;       // 質問用テキスト ("装備を購入しますか？")
    public GameObject decisionPanel;          // 「はい」「いいえ」のボタンを含むパネル
    public Button yesButton;                  // 「はい」ボタン
    public Button noButton;                   // 「いいえ」ボタン

    public GameObject pinIcon;                // ピンアイコンのUIオブジェクト

    public Animator playerAnimator;           // プレイヤーのアニメーター
    public RuntimeAnimatorController defaultController;     // 通常アニメーション
    public RuntimeAnimatorController alternativeController; // 切り替え先アニメーション
    private int equipmentCost = 50;           // 装備購入に必要なコスト

    public PlayerStats playerStats;           // プレイヤーのステータス管理スクリプト
    private bool isPlayerInRange = false;     // プレイヤーが範囲内にいるかどうか

    void Start()
    {
        // PlayerStatsとAnimatorの参照を取得
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
            playerAnimator = player.GetComponent<Animator>();
        }

        // 初期状態でUIを非表示に
        decisionPanel.SetActive(false);
        pinIcon.SetActive(false);

        // ボタンのクリックイベント設定
        yesButton.onClick.AddListener(PurchaseEquipment);
        noButton.onClick.AddListener(ClosePanel);
    }

    void Update()
    {
        // プレイヤーが範囲内でエンターキーを押した場合
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Return))
        {
            OpenPanel();
        }
    }

    private void OpenPanel()
    {
        // 購入確認パネルと質問テキストを表示
        decisionPanel.SetActive(true);
        questionText.text = "装備を購入しますか？";
        pinIcon.SetActive(false); // 購入画面を開いたらピンを非表示に
    }

    private void PurchaseEquipment()
    {
        // 「はい」ボタンが押されたとき
        if (playerStats.gold >= equipmentCost)
        {
            // ゴールドを消費してアニメーションを切り替える
            playerStats.SpendGold(equipmentCost);

            // アニメーション切り替え
            if (playerAnimator.runtimeAnimatorController == defaultController)
            {
                playerAnimator.runtimeAnimatorController = alternativeController;
            }
            else
            {
                playerAnimator.runtimeAnimatorController = defaultController;
            }

            Debug.Log("装備を購入しました！アニメーションが変更されました。");
        }
        else
        {
            Debug.Log("ゴールドが足りません！");
        }

        ClosePanel(); // 購入後にパネルを閉じる
    }

    private void ClosePanel()
    {
        // 「いいえ」ボタンまたは処理終了時にパネルを閉じる
        decisionPanel.SetActive(false);
        if (isPlayerInRange) pinIcon.SetActive(true); // 範囲内ならピンを再表示
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            pinIcon.SetActive(true); // プレイヤーが範囲内に入ったらピンを表示
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            ClosePanel();           // 範囲外に出たらパネルを閉じる
            pinIcon.SetActive(false); // ピンを非表示に
        }
    }
}
