using UnityEngine;

public class Spawner : MonoBehaviour
{
    float timer;
    Transform[] spawnPoints;
    int level;
    float levelTime; // 몬스터 진화에 걸리는 시간

    public SpawnData[] spawnData;

    private void Awake()
    {
        spawnPoints = GetComponentsInChildren<Transform>();
        levelTime = BattleSession.instance.maxGameTime / spawnData.Length;
    }
    void Update()
    {
        if (!BattleSession.instance.isLive) return;
        
        timer += Time.deltaTime;
        level = Mathf.Min(Mathf.FloorToInt(BattleSession.instance.gameTime / levelTime), spawnData.Length - 1); // 이후 적이 추가된다면 1 대신 프리펩 배열의 길이로 수정한다.

        if (timer > spawnData[level].spawnTiming)
        {
            Spawn();
            timer = 0;
        }

    }

    void Spawn()
    {
        int ranSpawnIndex = Random.Range(1, spawnPoints.Length);

        GameObject enemy = BattleSession.instance.pool.Get(0);
        enemy.transform.position = spawnPoints[ranSpawnIndex].position;
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
    }

    
}

[System.Serializable]
public class SpawnData
{
    public int monsterID;
    public float spawnTiming;
    public int health;
    public float speed;
}
