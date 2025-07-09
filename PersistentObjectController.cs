using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentObjectController : MonoBehaviour
{
    private static PersistentObjectController instance;

    // 非表示にするシーン名を指定
    public string[] scenesToDisable;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject); // すでにインスタンスが存在する場合は破棄
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // シーンが変更されたときに呼ばれるイベントを登録
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 現在のシーンが非表示対象かを確認
        if (System.Array.Exists(scenesToDisable, sceneName => sceneName == scene.name))
        {
            gameObject.SetActive(false); // 非表示にする
        }
        else
        {
            gameObject.SetActive(true); // 表示に戻す
        }
    }

    private void OnDestroy()
    {
        // イベントの解除
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
