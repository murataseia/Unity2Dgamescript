using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactEffect : MonoBehaviour
{
    /// <summary>
    /// アニメーション終了後にこのメソッドを呼び出してオブジェクトを削除する
    /// </summary>
    public void DestroyAfterAnimation()
    {
        Destroy(gameObject);
    }
}

