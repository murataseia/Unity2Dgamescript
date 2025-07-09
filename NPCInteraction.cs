using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPCInteraction : MonoBehaviour
{
    public TextMeshProUGUI npcText;          // 表示するテキスト
    public GameObject pinIcon;               // ピンとして表示するアイコン（プレハブやUI）
    public GameObject npcBackground;         // 背景ウィンドウのオブジェクト（ImageなどのUI）
    public GameObject statsUpgradePanel;     // ステータス強化画面のオブジェクト
    public TextMeshProUGUI hpCostText;       // HP強化のコスト表示用テキスト
    public TextMeshProUGUI staminaCostText;  // スタミナ強化のコスト表示用テキスト

    public List<string> npcDialogues;        // 複数のテキストを格納するリスト
    private int currentDialogueIndex = 0;    // 現在のテキストインデックス

    private bool isPlayerInRange = false;
    private bool isTextVisible = false;      // テキストが表示中かどうか
    public Button dialogueButton;  // 追加: ダイアログ表示用のボタン


    // 強化コスト
    private int hpUpgradeCost = 20;          // 初期HP強化コスト
    private int staminaUpgradeCost = 15;     // 初期スタミナ強化コスト
    private int hpIncreaseAmount = 10;       // HPの増加量
    private int staminaIncreaseAmount = 5;   // スタミナの増加量

    public PlayerStats playerStats;          // PlayerStatsスクリプトの参照

    void Start()
    {
        // Playerオブジェクトをタグで探してPlayerStatsスクリプトを取得
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>(); // PlayerStatsスクリプトを取得
        }

        // 初期表示設定：テキスト、ピン、背景、強化画面を非表示に
        npcText.gameObject.SetActive(false);
        pinIcon.SetActive(false); 
        npcBackground.SetActive(false);
        statsUpgradePanel.SetActive(false); // 強化画面を非表示に

        UpdateUpgradeCostTexts(); // 初期コスト表示を更新
    }

    void Update()
    {
        // プレイヤーが範囲内にいてエンターキーが押されたとき
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Return))
        {
            if (!isTextVisible)
            {
                // 初めてエンターが押されたらテキストを表示
                ShowDialogue();
            }
            else
            {
                // 次のテキストを表示
                ShowNextDialogue();
            }
        }
    }
    public void OnDialogueButtonClick()
    {
        if (!isTextVisible)
        {
            ShowDialogue();
        }
        else
        {
            ShowNextDialogue();
        }
    }

    private void ShowDialogue()
    {
        // テキストと背景の表示を開始
        npcText.gameObject.SetActive(true);
        npcBackground.SetActive(true);
        npcText.text = npcDialogues[currentDialogueIndex]; // 最初のテキストを表示
        isTextVisible = true;
        pinIcon.SetActive(false);  // テキストが表示されているのでピンを非表示に
    }

    private void ShowNextDialogue()
    {
        // 次のテキストを表示
        currentDialogueIndex++;

        if (currentDialogueIndex < npcDialogues.Count)
        {
            // 次のテキストをセット
            npcText.text = npcDialogues[currentDialogueIndex];
        }
        else
        {
            // すべてのテキストが表示されたら終了
            npcText.gameObject.SetActive(false);
            npcBackground.SetActive(false);
            isTextVisible = false;
            currentDialogueIndex = 0;  // インデックスをリセット
            ShowStatsUpgradePanel();   // 強化画面を表示
        }
    }

    private void ShowStatsUpgradePanel()
    {
        // 強化画面の表示
        statsUpgradePanel.SetActive(true);
    }

    // コストを表示するテキストを更新
    private void UpdateUpgradeCostTexts()
    {
        hpCostText.text = "HP+10: " + hpUpgradeCost + "G";
        staminaCostText.text = "Stamina+10: " + staminaUpgradeCost + "G";
    }

    // HP強化ボタン用メソッド
    public void UpgradeHP()
    {
        // ゴールドが足りるかチェックし、足りれば強化を行う
        if (playerStats.gold >= hpUpgradeCost) // goldをPlayerStatsから参照
        {
            playerStats.SpendGold(hpUpgradeCost);  // ゴールドを消費
            playerStats.IncreaseMaxHP(hpIncreaseAmount);  // HPを増加
            hpUpgradeCost += 10;                          // コストを増加
            UpdateUpgradeCostTexts();                     // コスト表示を更新
        }
        else
        {
            Debug.Log("ゴールドが足りません！"); // ゴールド不足の通知
        }
    }

    // スタミナ強化ボタン用メソッド
    public void UpgradeStamina()
    {
        // ゴールドが足りるかチェックし、足りれば強化を行う
        if (playerStats.gold >= staminaUpgradeCost) // goldをPlayerStatsから参照
        {
            playerStats.SpendGold(staminaUpgradeCost); // ゴールドを消費
            playerStats.IncreaseMaxStamina(staminaIncreaseAmount); // スタミナを増加
            staminaUpgradeCost += 7;                               // コストを増加
            UpdateUpgradeCostTexts();                              // コスト表示を更新
        }
        else
        {
            Debug.Log("ゴールドが足りません！"); // ゴールド不足の通知
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            pinIcon.SetActive(true); // プレイヤーが近づいたらピンを表示
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            npcText.gameObject.SetActive(false);  // プレイヤーが離れたらテキストを非表示に
            npcBackground.SetActive(false);       // 背景も非表示に
            pinIcon.SetActive(false);             // ピンも非表示に
            isTextVisible = false;                // テキスト非表示状態をリセット
            currentDialogueIndex = 0;             // インデックスをリセット
            statsUpgradePanel.SetActive(false);   // 強化画面も非表示に
        }
    }
}
