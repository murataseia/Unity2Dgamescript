using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TeleportTo : MonoBehaviour
{
    public string sceneName; // テレポート先のシーン名
    public string teleportID; // ユニークなテレポートID
    public bool requireEnterKey = false; // エンターキーが必要かどうか

    public Button teleportButton; // 追加: テレポートボタン

    private bool playerInZone = false; // プレイヤーがゾーンにいるかどうか

    private void Update()
    {
        // エリア内にプレイヤーがいる場合にエンターキーが押されたらシーン遷移
        if (playerInZone && requireEnterKey && Input.GetKeyDown(KeyCode.Return))
        {
            Teleport();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = true;
            teleportButton.gameObject.SetActive(true); // ボタンを表示

            // エンターキーが不要な場合、エリアに入るだけでシーン遷移
            if (!requireEnterKey)
            {
                Teleport();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = false;
            teleportButton.gameObject.SetActive(false); // ボタンを非表示
        }
    }

    public void Teleport() // ボタンから呼び出すためのパブリックメソッド
    {
        // テレポートIDを保存してシーンをロード
        PlayerPrefs.SetString("TeleportID", teleportID);
        SceneManager.LoadScene(sceneName);
    }
}