using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text scoreText; // スコアを表示するUIテキスト
    private Vector3 startPosition; // カメラの初期位置
    private float score = 0f; // スコア変数

    void Start()
    {
        // カメラの初期位置を記録
        startPosition = transform.position;
    }

    void Update()
    {
        // カメラの現在位置から初期位置の距離を計算し、スコアにする
        score = transform.position.x - startPosition.x;

        // スコアを画面に表示
        scoreText.text =Mathf.FloorToInt(score).ToString() + "m";
    }
}
