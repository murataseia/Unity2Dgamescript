using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSwitcher : MonoBehaviour
{
    public Animator animator; // アタッチされたAnimator
    public RuntimeAnimatorController defaultController; // 初期アニメーター
    public RuntimeAnimatorController alternativeController; // 切り替え先のアニメーター

    void Start()
    {
        // 初期設定としてデフォルトを設定
        animator.runtimeAnimatorController = defaultController;
    }

    void Update()
    {
        // エンターキーを監視
        if (Input.GetKeyDown(KeyCode.Return)) // エンターキー
        {
            SwitchAnimatorController();
        }
    }

    public void SwitchAnimatorController()
    {
        // 現在のアニメーターを切り替える
        if (animator.runtimeAnimatorController == defaultController)
        {
            animator.runtimeAnimatorController = alternativeController;
        }
        else
        {
            animator.runtimeAnimatorController = defaultController;
        }
    }
}
