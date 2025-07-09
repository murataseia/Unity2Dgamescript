using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRun : MonoBehaviour
{
     public float moveSpeed = 5f; // キャラクターの移動速度

    void Update()
    {
        // キャラクターを右に移動させる
        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
    }
}
