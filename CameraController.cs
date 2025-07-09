using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float scrollSpeed = 2.0f;  // カメラのスクロール速度

    void Update()
    {
        // 毎フレームごとにX方向に移動する
        transform.position += new Vector3(scrollSpeed * Time.deltaTime, 0, 0);
    }
}
