using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    EnemySpawnerManager enemySpawnerManager;
    public int spawnerIndexInSpawnerManager;

    [SerializeField] bool isSpawning;
    [SerializeField] bool skipWaveTime;

    [SerializeField] int minSpawnHeight;
    [SerializeField] int maxSpawnHeight;

    [SerializeField] Transform spawnPoint;
    [SerializeField] WaypointManager waypointManager;
    [SerializeField] List<WaveData> enemyWave;
    int currentwaveIndex = 0;

    //just putting enem ies under a parent so that they do not pour into my scene to generate clutter
    [SerializeField] Transform enemyGroupingObejctTransform;

    private void OnEnable()
    {
        StartCoroutine(SpawnWave());
    }

    private void Awake()
    {
        enemySpawnerManager = transform.parent.GetComponent<EnemySpawnerManager>();
    }

    public int GetMaxSpawnHeight()
    {
        return maxSpawnHeight;
    }

    public IEnumerator SpawnWave()
    {
        while (currentwaveIndex < enemyWave.Count)
        {
            WaveData currentWave = enemyWave[currentwaveIndex];
            //comment it for wave skip function in future
            yield return new WaitForSecondsRealtime(enemyWave[currentwaveIndex].waveSpawnTime);



            foreach (EnemySpawnData data in currentWave.enemySpawnData)
            {
                for (int i = 0; i < data.spawnCount; i++)
                {
                    GameObject spawnedEnemyRefrence = Instantiate(data.enemyPrefab,
                                                      GetPositionWithRandomHeight(), data.enemyPrefab.transform.rotation,
                                                      enemyGroupingObejctTransform);

                    EnemyMovement movement = spawnedEnemyRefrence.GetComponent<EnemyMovement>();
                    if (movement != null)
                    {
                        movement.SetPath(waypointManager);
                    }
                    yield return new WaitForSecondsRealtime(data.EnemySpawnDelay);
                }
            }
            currentwaveIndex++;
            
        }
        Debug.Log("All waves Finished");
        transform.parent.GetComponent<EnemySpawnerManager>().SetIthSpawnerFinishedSpanwning(spawnerIndexInSpawnerManager);

    }

    Vector3 GetPositionWithRandomHeight()
    {
        float randomHeight = Random.Range(minSpawnHeight, maxSpawnHeight);
        return new Vector3(spawnPoint.position.x, randomHeight, spawnPoint.position.z);
    }

    
}
