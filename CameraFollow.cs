using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Vector3 offset;   // カメラとキャラクターのオフセット

    public float minX; // カメラが移動できるX軸の最小値
    public float maxX; // カメラが移動できるX軸の最大値
    public float minY; // カメラが移動できるY軸の最小値（必要に応じて）
    public float maxY; // カメラが移動できるY軸の最大値（必要に応じて）

    private Transform target;

    void Start()
    {
        // "Player"タグのオブジェクトをターゲットとして設定
        GameObject targetObject = GameObject.FindGameObjectWithTag("Player");
        if (targetObject != null)
        {
            target = targetObject.transform;
        }
        else
        {
            Debug.LogError("Player tagged object not found.");
        }
    }

    void LateUpdate()
    {
        if (target == null) return; // ターゲットが設定されていない場合は何もしない

        // 目標となるカメラの位置
        Vector3 targetPosition = target.position + offset;

        // カメラのX軸とY軸の位置を制限する
        float clampedX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(targetPosition.y, minY, maxY);

        // カメラの位置を更新する
        transform.position = new Vector3(clampedX, clampedY, targetPosition.z);
    }
}
