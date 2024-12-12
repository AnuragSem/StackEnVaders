using System;
using System.Collections;
using UnityEngine;

public class Block : MonoBehaviour
{
    public static Block currentBlock { get; private set; }
    public static Block previousBlock { get; private set; }
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] bool isMoving = true;
    [SerializeField] bool isBaseBlock = false;
    bool moveAlongX = false;

    private Coroutine autostopCoroutine;
    //related to color
    Renderer blockRenderer;

    public static void SetCurrentBlock(Block block)
    {
        currentBlock = block;
    }
    public static void SetPreviousBlock(Block block)
    {
        previousBlock = block;
    }

    public void SetMoveDirection(bool moveTowardsX)
    {
        moveAlongX = moveTowardsX;
    }

    private void Awake()
    {
        blockRenderer = GetComponent<Renderer>();
    }
    private void OnEnable()
    {
        SetBlockColor();
        if(autostopCoroutine != null)
        {
            StopCoroutine(autostopCoroutine);
        }
        autostopCoroutine = StartCoroutine(StartAutoStopSequence());
    }
    private void Update()
    {
        if (isMoving)
        {
            MoveBlock();
        }
    }

    private void MoveBlock()
    {
        if (moveAlongX)
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.forward * moveSpeed * Time.deltaTime;
        }

    }

    public void Stop()
    {
        if (autostopCoroutine != null)
        {
            StopCoroutine(autostopCoroutine);
            autostopCoroutine = null;
        }
        Debug.Log("stop triggered by" + this.gameObject.name);                                                       
        isMoving = false;
        moveSpeed = 0f;

        float hangoverValue = moveAlongX
                             ? transform.position.x - previousBlock.transform.position.x
                             : transform.position.z - previousBlock.transform.position.z;

        float previousBlockSize = moveAlongX
                                ? previousBlock.transform.localScale.x
                                : previousBlock.transform.localScale.z;


        if (Mathf.Abs(hangoverValue) >= previousBlockSize)
        {
            //no overlap GameOVer
            StateManager.instance.isPlaying = false;
            StateManager.instance.isGameOver = true;
            GameManager.Instance.HandleGameOver();
        }
        else
        {
            // There's some overlap, split the block
            float direction = (hangoverValue > 0) ? 1f : -1f;

            if (moveAlongX)
                SplitBlockOnX(hangoverValue, direction);
            else
                SplitBlockOnZ(hangoverValue, direction);
        }
    }

    private void SplitBlockOnX(float hangoverValueX, float direction)
    {
        float newBlockSizeX = previousBlock.transform.localScale.x - Mathf.Abs(hangoverValueX);
        float fallingBlockSizeX = transform.localScale.x - newBlockSizeX;

        float newBlockXposition = previousBlock.transform.position.x + (hangoverValueX / 2);

        transform.localScale = new Vector3(newBlockSizeX, transform.localScale.y, transform.localScale.z);
        transform.position = new Vector3(newBlockXposition, transform.position.y, transform.position.z);

        float stackedBlockEdgePosition = transform.position.x + (newBlockSizeX / 2f * direction);
        float fallingBlockPositionOnX = stackedBlockEdgePosition + (fallingBlockSizeX / 2f * direction);

        SpawnDropBlock(fallingBlockPositionOnX, fallingBlockSizeX, true);
    }

    private void SplitBlockOnZ(float hangoverValueZ, float direction)
    {
        float newBlockSizeZ = previousBlock.transform.localScale.z - MathF.Abs(hangoverValueZ);
        float fallingBlockSizeZ = transform.localScale.z - newBlockSizeZ;

        float newBlockZposition = previousBlock.transform.position.z + (hangoverValueZ / 2);

        //setting the new scale and tranforms

        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newBlockSizeZ);
        transform.position = new Vector3(transform.position.x, transform.position.y, newBlockZposition);

        //for falling block
        float stackedBlockEdgePosition = transform.position.z + (newBlockSizeZ / 2f * direction);
        float fallingBlockPositionOnZ = stackedBlockEdgePosition + (fallingBlockSizeZ / 2f * direction);

        SpawnDropBlock(fallingBlockPositionOnZ, fallingBlockSizeZ, false);
    }



    private void SpawnDropBlock(float fallingBlockPosition, float fallingBlockSize, bool isOnXAxis)
    {
        Debug.Log("dropped block");
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.GetComponent<Renderer>().material.color = blockRenderer.material.color;
        if (isOnXAxis)
        {
            //drop on x
            cube.transform.localScale = new Vector3(fallingBlockSize, transform.localScale.y, transform.localScale.z);
            cube.transform.position = new Vector3(fallingBlockPosition, transform.position.y, transform.position.z);
        }

        else
        {
            //drop on z
            cube.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, fallingBlockSize);
            cube.transform.position = new Vector3(transform.position.x, transform.position.y, fallingBlockPosition);
        }
        cube.AddComponent<Rigidbody>();
        Destroy(cube.gameObject, 1f);
    }

    void SetBlockColor()
    {
        if (blockRenderer != null && BlockColourManager.Instance != null)
        {
            blockRenderer.material.color = BlockColourManager.Instance.GetNextBlockColor();
        }
        else if (blockRenderer == null)
        {
            Debug.Log("renderer is problemo");
        }
        else if (BlockColourManager.Instance == null)
        {
            Debug.Log("cannot get block color mnager");
        }
    }

    public IEnumerator StartAutoStopSequence()
    {
        if (isBaseBlock)
        {
            yield break;
        }   
        yield return new WaitForSeconds(GameManager.Instance.GetTimeForAutoStoping());
        Debug.Log("auto Stopped");
        Stop();
    }
}
