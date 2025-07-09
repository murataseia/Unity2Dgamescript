using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRange : MonoBehaviour
{
    public string sceneToLoad = "shop"; // Inspectorで設定するシーン名
    private bool isInRange = false; // プレイヤーが範囲内にいるかどうかを判定
    private GameObject player; // プレイヤーオブジェクトを保存する変数

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // プレイヤーが範囲内に入ったとき
        {
            isInRange = true;
            player = other.gameObject; // プレイヤーオブジェクトを保存
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // プレイヤーが範囲から出たとき
        {
            isInRange = false;
            player = null; // プレイヤーオブジェクトの参照をクリア
        }
    }

    private void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.Return)) // エンターキーが押されたとき
        {
            DontDestroyOnLoad(player); // プレイヤーオブジェクトをシーン遷移後も破壊しない
            SceneManager.sceneLoaded += OnSceneLoaded; // シーンがロードされた後のイベントを登録
            SceneManager.LoadScene(sceneToLoad); // 指定されたシーンをロード
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 新しくロードされたシーンでSpawnPointを探す
        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
        if (spawnPoint != null && player != null)
        {
            player.transform.position = spawnPoint.transform.position; // プレイヤーをSpawnPointの位置に移動
        }
        SceneManager.sceneLoaded -= OnSceneLoaded; // イベントを解除
    }
}
