using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialSpawnPoint : MonoBehaviour
{
    private void Start()
    {
        // ゲーム開始時にTeleportIDをクリア
        PlayerPrefs.DeleteKey("TeleportID");

        // キャラクターの位置をそのままスポーン位置として使用
        transform.position = transform.position;
    }
}
