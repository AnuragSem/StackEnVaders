using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public int currentActiveBaseBlockIndex = -1;
    [SerializeField] Block spawnableBlockPrefab;
    [SerializeField] Transform BaseBlockParent;

    [SerializeField] bool spawnOnXAxis;// initial spawn to z preferably

    float orignalVolume;

    float currentBlockVolume;
    private void Start()
    {
        orignalVolume = spawnableBlockPrefab.transform.localScale.x * spawnableBlockPrefab.transform.localScale.y *
                              spawnableBlockPrefab.transform.localScale.z;
        //Debug.Log("orignal volume " + orignalVolume);
    }

    public void SpawnBlock(Transform spawnedBlockParent)
    {

        if (Block.currentBlock != null)
        { 
            Block.SetPreviousBlock(Block.currentBlock);
        }

        Block previousBlock = Block.previousBlock;

        var blockInstance = Instantiate(spawnableBlockPrefab,spawnedBlockParent);
        Block.SetCurrentBlock(blockInstance);

        //sets newly's spawned block's scale to the last's blocks scale
        blockInstance.transform.localScale = new Vector3(previousBlock.transform.localScale.x,
                                                        blockInstance.transform.localScale.y,
                                                        previousBlock.transform.localScale.z);

        currentBlockVolume = blockInstance.transform.localScale.x*
                             blockInstance.transform.localScale.y*
                             blockInstance.transform.localScale.z;

        //for custom position of spawned block
        float x = this.spawnOnXAxis  ? transform.position.x: previousBlock.transform.position.x;
        float previousBlockHeight = previousBlock.transform.localScale.y;
        float spawnedBlockHeight = blockInstance.transform.localScale.y;
        float z = !this.spawnOnXAxis ? transform.position.z: previousBlock.transform.position.z;
        
        //actually setting the position
        blockInstance.transform.position =  new Vector3(x,
                    previousBlock.transform.position.y + (previousBlockHeight / 2) + (spawnedBlockHeight / 2),
                                                        z);

        //sets hp
        blockInstance.SetHP(orignalVolume,currentBlockVolume);


        if (this.spawnOnXAxis)
        {
            blockInstance.SetMoveDirection(true);
        }
        else
        {
            blockInstance.SetMoveDirection(false);
        }


    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireCube(transform.position,spawnableBlockPrefab.transform.localScale);
    //}
}
