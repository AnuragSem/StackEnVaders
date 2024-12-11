using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] CameraController cameraController;

    [SerializeField] List<Block> baseBlocks;
    [SerializeField] List<Vector3> baseBlockPositions;

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
            cameraController.SetTarget(baseBlocks[currentBaseBlockIndex].transform);
            Block.SetPreviousBlock(baseBlocks[currentBaseBlockIndex]);
            Debug.Log("current block set to "+ Block.previousBlock.gameObject.name);
            Block.SetCurrentBlock(baseBlocks[currentBaseBlockIndex]);
            Debug.Log("previous block set to " + Block.currentBlock.gameObject.name);
        }
        else
            Debug.Log("base Block Indexing Fucked");
    }

    public void GetBaseBlocksPositions()
    {
        for (int i = 0; i < baseBlocks.Count; i++)
        {
            baseBlockPositions.Add(baseBlocks[i].gameObject.transform.position);
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
}
