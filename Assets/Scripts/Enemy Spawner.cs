using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> enemyPrefabs;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Transform enemyGroupingObejctTransform;

    [SerializeField] bool isSpawning;
    [SerializeField] float spawnDelay;
    [SerializeField] int minSpawnHeight;
    [SerializeField] int maxSpawnHeight;


    //[SerializeField]Transform lastBaseBlock;
    //[SerializeField] float SpawnDistanceFromStack;

    private void Start()
    {
        if (spawnPoint == null)
        {
            Debug.Log("u fucked up the spawn point refrence to enemies dumbass");
        }
        SpawnEnemies();
    }

    public void SpawnEnemies()
    {
        int randomHeight = Random.Range(minSpawnHeight,maxSpawnHeight+1);

        Vector3 spawnPosition = new Vector3(spawnPoint.position.x, randomHeight, spawnPoint.position.z);

        GameObject enemy = Instantiate(enemyPrefabs[0], spawnPosition, Quaternion.identity,enemyGroupingObejctTransform);

    }
}
