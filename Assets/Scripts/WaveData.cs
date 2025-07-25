using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveData
{
    public string waveName;
    public List<EnemySpawnData> enemySpawnData;
    public float waveSpawnTime;
}
