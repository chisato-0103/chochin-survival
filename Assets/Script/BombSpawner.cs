using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    public GameObject bombPrefab;

    [Header("Spawn Area")]
    public Transform center;
    public float radius = 12f;        // ステージ上のランダム範囲（半径）
    public float spawnHeight = 8f;    // 上から落とす高さ

    [Header("Spawn Rate")]
    public float startInterval = 1.2f;
    public float minInterval = 0.35f;
    public float rampPerSecond = 0.01f;

    [Header("Limits")]
    public int maxAlive = 25;

    [Header("Burst")]
    public int spawnPerTick = 3;      // 1回のタイミングで出す数

    [Header("Random Kick (optional)")]
    public float kickStrength = 0f;   // 0なら初速なし。少し暴れさせたいなら 1〜3
    public float upwardKick = 0f;     // 上向きに少し足す（基本0）

    float timer;
    float elapsed;

    void Reset()
    {
        center = transform;
    }

    void Update()
    {
        if (bombPrefab == null) return;
        if (center == null) center = transform;

        elapsed += Time.deltaTime;
        float interval = Mathf.Max(minInterval, startInterval - elapsed * rampPerSecond);

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;

            int alive = CountAliveBombs();
            int canSpawn = Mathf.Min(spawnPerTick, maxAlive - alive);

            for (int i = 0; i < canSpawn; i++)
                SpawnOne();
        }
    }

    int CountAliveBombs()
    {
        return GameObject.FindGameObjectsWithTag("Bomb").Length;
    }

    void SpawnOne()
    {
        // 円の中でランダム（ステージ上に投下）
        Vector2 r = Random.insideUnitCircle * radius;
        Vector3 pos = center.position + new Vector3(r.x, spawnHeight, r.y);

        GameObject go = Instantiate(bombPrefab, pos, Random.rotation);
        go.tag = "Bomb";

        // 必要なら初速を少し付ける（投下の不規則さ）
        var rb = go.GetComponent<Rigidbody>();
        if (rb != null && kickStrength > 0f)
        {
            Vector3 kick = new Vector3(Random.Range(-kickStrength, kickStrength), upwardKick,
                                       Random.Range(-kickStrength, kickStrength));
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity += kick;
#else
            rb.velocity += kick;
#endif
        }
    }
}
