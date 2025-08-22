using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    //health related parameters
    [Header("Health")]
    [SerializeField] float baseBlockHP = 1000f;
    [SerializeField] float maxBlockHealth = 100f;
    private float currentBlockHealth;

    [Header("Position in stack")]
    public int indexInStack = -1;

    [Header("Movement")]
    WaypointManager blockMovementPath;
    int movementPathIndex = 0;
    private float t = 0f; // interpolation factor for bezier
    [SerializeField] bool isMoving = false;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float minMoveSpeed = 1f;
    [SerializeField] float speedIncrementAfterEachBLock = 0.005f;
    [SerializeField] float maxMoveSpeed = 5f;

    [Header("Type of Block")]
    [SerializeField] bool isBaseBlock = false;
    public static Block currentBlock { get; private set; }
    public static Block previousBlock { get; private set; }
    bool moveAlongX = false;
    private Coroutine autostopCoroutine;
    //related to color
    Renderer blockRenderer;

    //setters
    public static void SetCurrentBlock(Block block)
    {
        currentBlock = block;
    }
    public static void SetPreviousBlock(Block block)
    {
        previousBlock = block;
    }
    
    public void SetCurrentHealth(float healthToBe)
    {
        currentBlockHealth = healthToBe;
    }

    public void SetMoveDirection(bool moveTowardsX)
    {
        moveAlongX = moveTowardsX;
    }

    public void SetSpeed(float newSpeed)
    { 
        moveSpeed = newSpeed;
    }

    //getters
    public float GetCurrentHeath()
    {
        return currentBlockHealth;
    }

    public float CalculateSpeed()
    {
        if (LevelManager.Instance != null)
        {
            int stackHeight = LevelManager.Instance.GetStackHeight();
            return Mathf.Clamp((minMoveSpeed + (speedIncrementAfterEachBLock * stackHeight)), minMoveSpeed, maxMoveSpeed);
        }
        else
        {
            Debug.Log("problem with level manager .instance");
            return minMoveSpeed;
        }
    }

    private void OnEnable()
    {
        //sets colour
        SetBlockColor();
        //sets speed
        SetSpeed(CalculateSpeed());

        //responsible for autostopping block 
        if (autostopCoroutine != null)
        {
            StopCoroutine(autostopCoroutine);
        }
        autostopCoroutine = StartCoroutine(StartAutoStopSequence());
    }

    private void Awake()
    {
        blockRenderer = GetComponent<Renderer>();
    }
    
    private void Update()
    {
        if (isMoving)
        {
            MoveBlock();
        }
    }


    public void SetHP(float refrenceVolume, float currentVolume)
    {
        if (gameObject.tag == "BaseBlock")
        {
            currentBlockHealth = baseBlockHP;
        }
        else
        {
            currentBlockHealth = (currentVolume / refrenceVolume) * maxBlockHealth;
            //Display health of block after placing it
            //Debug.Log($"current health {currentBlockHealth}");
        }
    }






    public void InitializeMovement(WaypointManager path)
    {
        blockMovementPath = path;
        movementPathIndex = 1; // start at segment between waypoint[0] and waypoint[1]
        t = 0f;
        isMoving = true;
    }

    private void MoveBlock()
    {
        //if (blockMovementPath == null || movementPathIndex >= blockMovementPath.GetWaypointCount())
        //    return;

        //// Get waypoints
        //Transform startT = blockMovementPath.GetWaypointAtIndex(movementPathIndex - 1);
        //Transform endT = blockMovementPath.GetWaypointAtIndex(movementPathIndex);

        //if (startT == null || endT == null) return;

        //Vector3 start = startT.position;
        //Vector3 end = endT.position;

        //// Control point (midpoint + sideways offset)
        //Vector3 mid = (start + end) / 2f;
        //Vector3 dir = (end - start).normalized;
        //Vector3 side = Vector3.Cross(Vector3.up, dir); // perpendicular to path


        //float adjustedStrength = blockMovementPath.GetCurveStrength() * (minMoveSpeed / moveSpeed)  + blockMovementPath.GetCurveStrength();
        //Debug.Log($"Adjusted CurveStrength {adjustedStrength}");

        //Vector3 control = mid + side * adjustedStrength;

        //// Keep Y constant
        //start.y = end.y = control.y = transform.position.y;

        //// Bezier interpolation
        //float segmentLength = Vector3.Distance(start, end);
        //t += moveSpeed * Time.deltaTime / Mathf.Max(segmentLength, 0.001f);

        //Vector3 pos = Mathf.Pow(1 - t, 2) * start +
        //              2 * (1 - t) * t * control +
        //              Mathf.Pow(t, 2) * end;

        //transform.position = pos;

        //// Segment complete
        //if (t >= 1f)
        //{
        //    t = 0f;
        //    movementPathIndex++;
        //    if (movementPathIndex >= blockMovementPath.GetWaypointCount())
        //    {
        //        Stop();
        //    }
        //}


        //////different logic ____________________________________________________________

        if (blockMovementPath == null || movementPathIndex >= blockMovementPath.GetWaypointCount())
            return;

        Transform startT = blockMovementPath.GetWaypointAtIndex(movementPathIndex - 1);
        Transform endT = blockMovementPath.GetWaypointAtIndex(movementPathIndex);
        if (startT == null || endT == null) return;

        // Keep movement strictly horizontal
        float y = transform.position.y;
        Vector3 start = startT.position; start.y = y;
        Vector3 end = endT.position; end.y = y;

        float L = Vector3.Distance(start, end);
        if (L < 1e-4f) { movementPathIndex++; t = 0f; return; }

        // h = "curveStrength" interpreted as sagitta (max sideways offset at midpoint).
        // h > 0 bends to the left of the travel direction; h < 0 bends to the right.
        float h = blockMovementPath.GetCurveStrength();

        // Straight segment if no curvature requested
        if (Mathf.Abs(h) < 1e-4f)
        {
            t += moveSpeed * Time.deltaTime / Mathf.Max(L, 1e-4f);
            transform.position = Vector3.Lerp(start, end, Mathf.Clamp01(t));
        }
        else
        {
            // Build the circle from chord (start->end) and sagitta h
            Vector3 dir = (end - start).normalized;
            Vector3 n = Vector3.Cross(Vector3.up, dir);
            if (n.sqrMagnitude < 1e-8f) n = Vector3.right; // degenerate fallback
            n.Normalize();

            float absH = Mathf.Abs(h);
            // Radius from chord length (L) and sagitta (absH):
            // R = (h/2) + L^2/(8h), with h positive in magnitude
            float R = (absH * 0.5f) + (L * L) / (8f * absH);
            float d = R - absH;                      // midpoint -> center distance
            float sign = Mathf.Sign(h);              // which side to bend toward

            Vector3 mid = 0.5f * (start + end);
            Vector3 C = mid + n * sign * d;          // circle center
            C.y = y;

            // Vectors from center to endpoints
            Vector3 from = start - C;
            Vector3 to = end - C;

            // Arc angle and length for speed-consistent traversal
            float angle = Mathf.Acos(Mathf.Clamp(Vector3.Dot(from.normalized, to.normalized), -1f, 1f));
            float arcLen = Mathf.Max(R * angle, 1e-4f);

            t += moveSpeed * Time.deltaTime / arcLen;      // advance by arc length
            Vector3 p = Vector3.Slerp(from, to, Mathf.Clamp01(t));
            transform.position = C + p;
        }

        if (t >= 1f)
        {
            t = 0f;
            movementPathIndex++;
            if (movementPathIndex >= blockMovementPath.GetWaypointCount())
                Stop();
        }

    }

    public void Stop()
    {
        if (autostopCoroutine != null)
        {
            StopCoroutine(autostopCoroutine);
            autostopCoroutine = null;
        }
        //Debug.Log("stop triggered by" + this.gameObject.name);                                                       
        isMoving = false;
        moveSpeed = 0f;



        CheckAndSliceBlock();

    }

    public void CheckAndSliceBlock()
    {
        Vector3 prevPos = previousBlock.transform.position;
        Vector3 prevScale = previousBlock.transform.localScale;
        Vector3 currPos = transform.position;
        Vector3 currScale = transform.localScale;

        // Calculate hangovers
        float xHangover = currPos.x - prevPos.x;
        float zHangover = currPos.z - prevPos.z;

        float maxXHang = prevScale.x;
        float maxZHang = prevScale.z;

        bool overhangX = Mathf.Abs(xHangover) >= maxXHang;
        bool overhangZ = Mathf.Abs(zHangover) >= maxZHang;

        // Game Over: no overlap in either axis
        if (overhangX || overhangZ)
        {
            OnNoBlockOverlapDetected();
            return;
        }

        // Determine directions
        float xDir = (xHangover > 0f) ? 1f : -1f;
        float zDir = (zHangover > 0f) ? 1f : -1f;

        bool sliceXFirst = UnityEngine.Random.value < 0.5f;

        if (sliceXFirst)
        {
            if (!Mathf.Approximately(xHangover, 0f))
                SplitBlockOnX(xHangover, xDir);
            if (!Mathf.Approximately(zHangover, 0f))
                SplitBlockOnZ(zHangover, zDir);
        }
        else
        {
            if (!Mathf.Approximately(zHangover, 0f))
                SplitBlockOnZ(zHangover, zDir);
            if (!Mathf.Approximately(xHangover, 0f))
                SplitBlockOnX(xHangover, xDir);
        }
    }

    void OnNoBlockOverlapDetected()
    {
        StateManager.instance.isPlaying = false;
        StateManager.instance.isGameOver = true;
        LevelManager.Instance.HandleGameOver();
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
        yield return new WaitForSeconds(LevelManager.Instance.GetTimeForAutoStoping());
        Debug.Log("auto Stopped");
        Stop();
    }

    public void TakeDmg(float dmg)
    { 
        currentBlockHealth -= dmg;
        Debug.Log($"{gameObject.name} hp : {currentBlockHealth}");
        if (currentBlockHealth <= 0)
        {
            DestroyBlock();
        }
    }

    private void DestroyBlock()
    {
        DestroyBlocksAbove();
        Destroy(gameObject);
    }

    void DestroyBlocksAbove()
    {
        Transform stackParent;
        int childCount;
        stackParent = transform.parent;
        childCount = stackParent.transform.childCount;
        for (int i = indexInStack + 1; i < childCount; i++)
        { 
            Destroy(stackParent.GetChild(i).gameObject);
        }
    }
}
