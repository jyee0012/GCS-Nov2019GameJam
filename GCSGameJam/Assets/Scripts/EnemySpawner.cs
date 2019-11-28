using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawn
{
    public GameObject enemyPrefab;
    public int chanceToSpawn = 0;
    public int divideSize = 0;
}

public class EnemySpawner : MonoBehaviour
{
    public bool gamePlaying = true;
    public int enemyCount = 0;

    [SerializeField]
    [Tooltip("Whether to have enemies spawn immediately or wait for a period of time")]
    bool initialDelay = false;
    [SerializeField]
    float initialDelayAmount = 0.0f;
    float initialDelayTimeStamp = 0.0f;

    [Space]
    [SerializeField]
    float timeBetweenSpawns = 0.0f;
    float SpawnTimeStamp = 0.0f;

    [Space]
    [SerializeField]
    int minEnemiesPerSpawn = 1;
    [SerializeField]
    int maxEnemiesPerSpawn = 1;

    [SerializeField]
    List<EnemySpawn> enemiesToSpawn = new List<EnemySpawn>();

    // Start is called before the first frame update
    void Start()
    {
        gamePlaying = true;

        if (initialDelay)
            initialDelayTimeStamp = Time.time + initialDelayAmount;
        else
            SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        int numEnemies = Random.Range(minEnemiesPerSpawn, maxEnemiesPerSpawn);
        int chancePool = GetTotalPool();

        for (int i = 0; i < numEnemies; i ++)
        {
            int randChance = Random.Range(0, chancePool);
            int totalpool = 0;

            for(int n = 0; n < enemiesToSpawn.Count; n ++)
            {
                totalpool += enemiesToSpawn[n].chanceToSpawn;
                if (randChance < totalpool)
                {
                    EnemyAI enemy = Instantiate(enemiesToSpawn[n].enemyPrefab).GetComponent<EnemyAI>();

                    enemy.spawner = this;

                    enemy.transform.position = new Vector3(Random.Range(-enemy.areaBounds.x, enemy.areaBounds.x),
                        Random.Range(-enemy.areaBounds.y, enemy.areaBounds.y),
                        transform.position.z);
                    enemy.divideSize = enemiesToSpawn[n].divideSize;

                    enemyCount++;

                    break;
                }
            }
        }

        SpawnTimeStamp = Time.time + timeBetweenSpawns;
    }

    public int GetTotalPool()
    {
        int totalChance = 0;

        for (int i = 0; i < enemiesToSpawn.Count; i ++)
        {
            totalChance += enemiesToSpawn[i].chanceToSpawn;
        }

        return totalChance;
    }

    // Update is called once per frame
    void Update()
    {
        if (gamePlaying)
        {
            if (Time.time >= SpawnTimeStamp && Time.time >= initialDelayTimeStamp)
                SpawnEnemy();
        }
    }
}
