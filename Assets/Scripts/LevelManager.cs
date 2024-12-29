using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] CameraController cameraController;

    [SerializeField] List<Block> baseBlocks;
    [SerializeField] List<Vector3> baseBlockPositions;

    //spawners
    [SerializeField] BlockSpawner blockSpawnerOnX;
    [SerializeField] BlockSpawner blockSpawnerOnZ;
    
    [SerializeField] Vector3 distanceOfSpawnerOnXAxisFromCurBaseBlock;
    [SerializeField] Vector3 distanceOfSpawnerOnZAxisFromCurBaseBlock;

    int currentBaseBlockIndex;

    private void Start()
    {
        currentBaseBlockIndex = -1;
    }
    public void SetNextBaseBlock()
    {
        currentBaseBlockIndex++;

        if (currentBaseBlockIndex<baseBlocks.Count)
        {
            GameManager.Instance.stackHeight = 0;
            
            Block.SetPreviousBlock(baseBlocks[currentBaseBlockIndex]);
            Debug.Log("current block set to "+ Block.previousBlock.gameObject.name);
            Block.SetCurrentBlock(baseBlocks[currentBaseBlockIndex]);
            Debug.Log("previous block set to " + Block.currentBlock.gameObject.name);

            cameraController.SetTarget(baseBlocks[currentBaseBlockIndex].transform);
        }
        else
            Debug.Log("base Block Indexing Fucked");
    }

    public void GetBaseBlocksPositions()
    {
        for (int i = 0; i < baseBlocks.Count; i++)
        {
            baseBlockPositions.Add(baseBlocks[i].transform.position);
        }
    }

    public bool IsLastCube()
    { 
        return(currentBaseBlockIndex >= baseBlocks.Count-1);
    }

    public int GetTotalBaseBLocks()
    { 
        return baseBlocks.Count;
    }

    public void SetSpawnerPosition()
    {
        blockSpawnerOnX.transform.position = baseBlockPositions[currentBaseBlockIndex] + distanceOfSpawnerOnXAxisFromCurBaseBlock;
        blockSpawnerOnZ.transform.position = baseBlockPositions[currentBaseBlockIndex] + distanceOfSpawnerOnZAxisFromCurBaseBlock;
    }

    public void SpawnBlockOnX()
    {
        blockSpawnerOnX.SpawnBlock();
    }

    public void SpawnBlockOnZ()
    { 
        blockSpawnerOnZ.SpawnBlock();
    }
}
