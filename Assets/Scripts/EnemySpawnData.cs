using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnData
{
    public string name;
    public GameObject enemyPrefab;
    public int spawnCount;
    public float EnemySpawnDelay = 1f;
}
