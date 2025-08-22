using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{

    [SerializeField] Block defaultSpawnableBlockPrefab;
    GameObject lastSpawnedBlock;

    [Header("Movement path")]
    [SerializeField] List<WaypointManager> possiblePaths;
    WaypointManager selectedPath = null;


    float orignalPrefabsVolume;

    float currentBlockVolume;

    private void Start()
    {
        orignalPrefabsVolume = CalculateVolume(defaultSpawnableBlockPrefab);
    }
    float CalculateVolume(Block block)
    { 
        float volume =  block.transform.localScale.x * block.transform.localScale.y * block.transform.localScale.z;
        return volume;
    }

    public void SpawnBlock(Vector3 currentBaseBlockPosition,Transform spawnedBlockParent, Block blockPrefab = null)
    {

        if (blockPrefab == null)
            blockPrefab = defaultSpawnableBlockPrefab;

        if (Block.currentBlock != null)
        { 
            Block.SetPreviousBlock(Block.currentBlock);
        }

        Block previousBlock = Block.previousBlock;


        //this is soo scuffed OMG

        selectedPath = null;
        if (possiblePaths == null)
        {
            Debug.Log("no paths assigned for blocks to follow");
            return;
        }
        else
        { 
            selectedPath = (possiblePaths.Count == 1)? possiblePaths[0] : possiblePaths[Random.Range(0,possiblePaths.Count)];

            if (selectedPath.GetWaypointCount() < 3 && selectedPath.GetTrackingIndex() < 0)
            {
                Debug.LogError(
                                $"[Path Error] Invalid path setup! WaypointCount = {selectedPath.GetWaypointCount()}," +
                                $" TrackingIndex = {selectedPath.GetTrackingIndex()}"
                              );
                return;
            }
            else
            {
                selectedPath.SetWaypointPositionAtTrackedIndex(previousBlock.transform.position);
            }
        }

        
        float previousBlockHeight = previousBlock.transform.localScale.y;
        float spawnedBlockHeight = blockPrefab.transform.localScale.y;
        float stackHeight = previousBlock.transform.position.y;

        Vector3 spawnPos = transform.position;
        spawnPos.y = previousBlock.transform.position.y + (previousBlockHeight / 2) + (spawnedBlockHeight / 2);

        var blockInstance = Instantiate(blockPrefab,spawnPos,Quaternion.identity,spawnedBlockParent);

        Block.SetCurrentBlock(blockInstance);


        

        if (!HasCustomScaling(blockInstance))
        {
            // sets newly's spawned block's scale to the last's blocks scale
            blockInstance.transform.localScale = new Vector3(previousBlock.transform.localScale.x,
                                                        blockInstance.transform.localScale.y,
                                                        previousBlock.transform.localScale.z);
        }

        //Calculatevolume
        currentBlockVolume = CalculateVolume(blockInstance);
        //sets hp
        blockInstance.SetHP(orignalPrefabsVolume, currentBlockVolume);

        
        blockInstance.InitializeMovement(selectedPath);

    }

    bool HasCustomScaling(Block block)
    {
        return block.CompareTag("NoScale");
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireCube(transform.position, defaultSpawnableBlockPrefab.transform.localScale);
    //}
}
