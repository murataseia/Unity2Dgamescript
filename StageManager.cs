using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public Transform cameraTransform; // 自動スクロールするカメラのTransform
    public GameObject[] stagePrefabs; // ステージのプレハブ配列
    public float stageLength = 10f; // ステージの長さ
    public int initialStages = 3; // 初期のステージ数
    public float spawnOffset = 15f; // カメラの位置からどれだけ先にスポーンするか
    private int stageProgress = 0; // 進行度（ステージが進むごとに増加）

    private List<GameObject> activeStages = new List<GameObject>();
    private float spawnPos = 38.4f; // スポーン位置
    private float deletePos = 100f; // 削除位置のオフセット

    void Start()
    {
        // 初期ステージの生成
        for (int i = 0; i < initialStages; i++)
        {
            SpawnStage();
        }
    }

    void Update()
    {
        // カメラがスポーン位置に近づいたら新しいステージを生成
        if (cameraTransform.position.x + spawnOffset > spawnPos)
        {
            SpawnStage();
        }

        // 最も古いステージが削除位置を通過したら削除
        if (activeStages.Count > 0 && cameraTransform.position.x - deletePos > activeStages[0].transform.position.x)
        {
            DeleteStage();
        }
    }

    void SpawnStage()
{
    GameObject newStage = Instantiate(stagePrefabs[Random.Range(0, stagePrefabs.Length)], 
                                      new Vector3(spawnPos, 0, 0), Quaternion.identity);
    activeStages.Add(newStage);
    spawnPos += stageLength;
    stageProgress++; // ステージごとに進行度を増加

    // ステージに配置されている敵にステータスを設定
    Enemy[] enemies = newStage.GetComponentsInChildren<Enemy>();
    foreach (Enemy enemy in enemies)
    {
        int scaledHp = GetScaledEnemyHp();
        int scaledAttack = GetScaledEnemyAttack();
        enemy.SetStats(scaledHp, scaledAttack);
    }
}

    void DeleteStage()
    {
        Destroy(activeStages[0]);
        activeStages.RemoveAt(0);
    }
    public int GetScaledEnemyHp()
{
    return Mathf.RoundToInt(100 * Mathf.Pow(1.1f, stageProgress)); // 基本HPを10％増加
}

public int GetScaledEnemyAttack()
{
    return Mathf.RoundToInt(10 * Mathf.Pow(1.05f, stageProgress)); // 基本攻撃力を5％増加
}

}
