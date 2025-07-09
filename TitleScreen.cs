using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public void ContinueGame()
    {
        if (PlayerPrefs.HasKey("MaxHP")) // セーブデータが存在する場合
        {
            Debug.Log("Continuing game...");
            SceneManager.LoadScene("Stage"); // "GameScene"に遷移
        }
        else
        {
            Debug.Log("No save data found."); // セーブデータがない場合の処理
        }
    }

    public void NewGame()
    {
        Debug.Log("Starting new game...");
        PlayerPrefs.DeleteAll(); // 既存のセーブデータをリセット
        SceneManager.LoadScene("Stage"); // "GameScene"に遷移
    }
}
