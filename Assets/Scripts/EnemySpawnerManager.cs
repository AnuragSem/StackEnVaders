using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour
{


    [SerializeField] List<SpawnerData> spawnerList;
    [SerializeField] List<bool> hasSpawnerFinishedSpawning;
    [SerializeField] GameObject spawnedEnemyGroupingObject;

    //assume there are enemies on screen when a level start always (this is just a flag so that it doesn't trgger victory condition again and again)
    bool areThereAnyEnemiesOnTheScene = true;
    bool haveAllSpawnerFinishedSpawning;

    int currentSpawnerIndex = 0;
    int numberOfSpawnersCompletedSpawning = 0;

    private void Start()
    {
        if (spawnerList != null)
        {
            hasSpawnerFinishedSpawning = Enumerable.Repeat(false, spawnerList.Count).ToList();
        }

    }

    private void Update()
    {
        if (!haveAllSpawnerFinishedSpawning)
        {
            return;
        }
        //statemanager . isvictoriuous is a nullable boolean
        else if (areThereAnyEnemiesOnTheScene && spawnedEnemyGroupingObject.transform.childCount <= 0 && StateManager.instance.isVictorious ==null)
        {
            areThereAnyEnemiesOnTheScene = false;
            LevelManager.Instance.SetLevelVictoryStatus(true);
        }
    }

    public int GetSpawnerCountInSpawnerList()
    { 
        return spawnerList.Count;
    }

    public void SetSpawnersFinishedSpawning(bool forceStatus)
    { 
        haveAllSpawnerFinishedSpawning = forceStatus;
    }

    public void ReleaseEnemyHordes()
    {
        StartCoroutine(ActivateSpawners());
    }

    public IEnumerator ActivateSpawners()
    {
        while (currentSpawnerIndex < spawnerList.Count && spawnerList[currentSpawnerIndex] != null)
        {
            yield return new WaitForSecondsRealtime(spawnerList[currentSpawnerIndex].timeToActivateSpawner);
            spawnerList[currentSpawnerIndex].enemySpawner.gameObject.SetActive(true);
            spawnerList[currentSpawnerIndex].enemySpawner.spawnerIndexInSpawnerManager = currentSpawnerIndex;
            currentSpawnerIndex++;
        }
    }

    public void SetIthSpawnerFinishedSpanwning(int i)
    {
        hasSpawnerFinishedSpawning[i] = true;
        numberOfSpawnersCompletedSpawning++;

        //checks the number of spawner done spawning and check if any enemy is alive or not
        if (numberOfSpawnersCompletedSpawning == spawnerList.Count)
        {
            Debug.Log("W gamplay");
            haveAllSpawnerFinishedSpawning = true;
        }
    }

    //please have the last spawner in the list as the farthest spawner from the base block 
    public int GetTheMaxHeightWhereEnemiesCanSpawn()
    {
        int maxHeight = 0;
        if (spawnerList.Count > 0)
        {
            foreach (var spawner in spawnerList)
            {   
                int spawnerMaxHeight = spawner.enemySpawner.GetMaxSpawnHeight();
                if (maxHeight < spawnerMaxHeight)
                { 
                    maxHeight = spawnerMaxHeight;
                }
            }
            return maxHeight;
        }
        return -1;
    }

}
