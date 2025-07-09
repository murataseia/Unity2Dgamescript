using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPersistence : MonoBehaviour
{
    private static PlayerPersistence instance;

    void Awake()
    {
        // 既に存在する場合はこのオブジェクトを破棄する
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            // 唯一のインスタンスとして設定し、破棄されないようにする
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
