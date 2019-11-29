using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EnemySpawn
{
    public GameObject enemyPrefab;
    [Tooltip("The chance to spawn this enemy out of the total amount for all enemeies in the list")]
    public int chanceToSpawn = 0;
    [Tooltip("How many times the enemy can split before it dies")]
    public int divideSize = 0;
    [Tooltip("The amount of seconds before the enemy is allowed to spawn")]
    public float initialSpawnDelay = 0.0f; 
    [HideInInspector]
    public float CanSpawnTimeStamp = 0.0f; //The time that the enemy can start spawning
}

public class EnemySpawner : MonoBehaviour
{
    [HideInInspector]
    public bool gamePlaying = true;
    public int enemyCount = 0;
    public Slider enemyCountSlider = null;
    [SerializeField]
    Image ecountSliderFill = null;

    [SerializeField]
    Vector2 AreaBounds = Vector2.zero;

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

        if (enemiesToSpawn != null && enemiesToSpawn.Count > 0)
        {
            //initialize the time at which the enemies are allowed to spawn
            foreach (EnemySpawn enemy in enemiesToSpawn)
                enemy.CanSpawnTimeStamp = Time.time + initialDelayAmount + enemy.initialSpawnDelay;
        }

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
                if (Time.time >= enemiesToSpawn[n].CanSpawnTimeStamp)
                {
                    totalpool += enemiesToSpawn[n].chanceToSpawn;
                    if (randChance < totalpool)
                    {
                        EnemyAI enemy = Instantiate(enemiesToSpawn[n].enemyPrefab).GetComponent<EnemyAI>();

                        enemy.spawner = this;

                        enemy.transform.position = new Vector3(Random.Range(-AreaBounds.x, AreaBounds.x),
                            Random.Range(-AreaBounds.y, AreaBounds.y),
                            transform.position.z);
                        enemy.areaBounds = AreaBounds;
                        enemy.divideSize = enemiesToSpawn[n].divideSize;

                        AddToEnemyCounter(1);

                        break;
                    }
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
            if (Time.time >= enemiesToSpawn[i].CanSpawnTimeStamp)
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

    public void AddToEnemyCounter(int amount)
    {
        enemyCount += amount;
        if (enemyCountSlider != null)
        {
            enemyCountSlider.value = enemyCount;
            if (enemyCountSlider.value < enemyCountSlider.maxValue * 0.5f)
                ecountSliderFill.color = Color.green;
            else if (enemyCountSlider.value >= enemyCountSlider.maxValue * 0.5f && enemyCountSlider.value < enemyCountSlider.maxValue * 0.8f)
                ecountSliderFill.color = Color.yellow;
            else if (enemyCountSlider.value < enemyCountSlider.maxValue)
                ecountSliderFill.color = Color.red;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(AreaBounds.x, AreaBounds.y, 0.1f));
    }
}
