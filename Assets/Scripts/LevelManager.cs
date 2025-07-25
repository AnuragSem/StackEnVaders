using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelManager : MonoBehaviour
{
    [Header("Game Variables")]
    int currentBaseBlockIndex;
    public int score = 0;
    public int stackHeight = 0;
    [SerializeField] float timeForAutoStoppingBlock = 6f;
    public int pointsToMaxStars = 100;

    public static LevelManager Instance { get; private set; }
    public static bool isUsingXSpawner { get; private set; } = true;

    [Header("GO Refrences")]

    [SerializeField] BackgroundColourChanger bgColourChanger;
    [SerializeField] CameraController cameraController;
    [SerializeField] UIManager uiManager;
    [SerializeField] SceneController sceneController;
    [SerializeField] StateManager stateManager;
    [SerializeField] PlayerAttributeManager playerAttributeManager;
    [SerializeField] EnemySpawnerManager enemySpawnerManager;

    [Header("Base Block")]
    [SerializeField] List<Block> baseBlocks;
    [SerializeField] List<Transform> baseBlockParentTransform;

    

    [Header("block Spawner")]
    [SerializeField] BlockSpawner blockSpawnerOnX;
    [SerializeField] BlockSpawner blockSpawnerOnZ;
    
    [SerializeField] Vector3 distanceOfSpawnerOnXAxisFromCurBaseBlock;
    [SerializeField] Vector3 distanceOfSpawnerOnZAxisFromCurBaseBlock;

    private void OnEnable()
    {
        EnemyMovement.OnEnemyReachedGoal += HandleOnEnemyReachedGoal;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        uiManager.UpdatePlayerLifeCount(playerAttributeManager.getPlayerLives());
        currentBaseBlockIndex = -1;
        uiManager.SetScore(score);
        StartCountDownCoroutie();
        DrawEnemyMaxHeightOfSpawningMarker();
    }

    private void OnDisable()
    {
        EnemyMovement.OnEnemyReachedGoal -= HandleOnEnemyReachedGoal;
    }

    //Getters
    public float GetTimeForAutoStoping()
    {
        return timeForAutoStoppingBlock;
    }
    public int GetTotalBaseBLocks()
    {
        return baseBlocks.Count;
    }

    public int GetStackHeight()
    { 
        return stackHeight;
    }

    public bool IsLastCube()
    {
        return (currentBaseBlockIndex >= baseBlocks.Count - 1);
    }

    //drawing the max enemy spawn height marker 
    void DrawEnemyMaxHeightOfSpawningMarker()
    {

        int height = enemySpawnerManager.GetTheMaxHeightWhereEnemiesCanSpawn();
        if (height >= 0)
        {
            List<Vector3> endpoints = cameraController.GetTheHorizontalBounds();
            if (endpoints.Count > 0)
            {
                uiManager.SetSpawnHeightMarker(height, endpoints);
                uiManager.SetEnemyRelatedReminder($"Max Enemy Spawn Height: {height}",Color.white);
            }
            else
            {
                Debug.Log("Bounds are not set for camera , check the clamps for camera movement");
                
            }
        }
        else
        {
            uiManager.SetEnemyRelatedReminder($"Peaceful", Color.white);
            Debug.Log("spawner list is empty apprantely");
        }
    }

    //functions that make the game run

    public void StartCountDownCoroutie()
    {
        StartCoroutine(CountDownBeforeStartingGame());
    }

    public IEnumerator CountDownBeforeStartingGame()
    {
        stateManager.isPlaying = false;
        int countdown = 3;

        SetNextBaseBlock();
        SetSpawnerPosition();
        uiManager.countdownText.color = Color.red;
        uiManager.DisplayCountdownText(true);

        while (countdown > 0)
        {
            uiManager.countdownText.text = countdown.ToString();
            yield return new WaitForSeconds(1f);
            countdown--;
        }
        uiManager.countdownText.color = Color.green;
        uiManager.countdownText.text = "GO!";

        yield return new WaitForSeconds(1f);
        uiManager.DisplayCountdownText(false);
        uiManager.countdownText.text = "3";


        OnCountdownEndsAfterBaseBlockSwitch();
    }
    void OnCountdownEndsAfterBaseBlockSwitch()
    {
        SpawnBlock();
        stateManager.isPlaying = true;
    }
    public void SetNextBaseBlock()
    {
        currentBaseBlockIndex++;

        if (currentBaseBlockIndex<baseBlocks.Count)
        {
            stackHeight = 0;
            baseBlocks[currentBaseBlockIndex].gameObject.SetActive(true);
            
            Block.SetPreviousBlock(baseBlocks[currentBaseBlockIndex]);
            //Debug.Log("current block set to "+ Block.previousBlock.gameObject.name);
            Block.SetCurrentBlock(baseBlocks[currentBaseBlockIndex]);
            //Debug.Log("previous block set to " + Block.currentBlock.gameObject.name);
            
            Block.currentBlock.indexInStack = 0;
            Block.currentBlock.SetHP(1,1);

            Debug.Log(Block.currentBlock.indexInStack +" "+ Block.currentBlock.name);
            cameraController.SetTarget(baseBlocks[currentBaseBlockIndex].transform);
        }
        else
            Debug.Log("base Block Indexing Fucked");
    }

    public void SetSpawnerPosition()
    {
        blockSpawnerOnX.transform.position = baseBlockParentTransform[currentBaseBlockIndex].position + distanceOfSpawnerOnXAxisFromCurBaseBlock;
        blockSpawnerOnZ.transform.position = baseBlockParentTransform[currentBaseBlockIndex].position + distanceOfSpawnerOnZAxisFromCurBaseBlock;
    }

    public void SpawnBlock()
    {
        if (isUsingXSpawner)
            SpawnBlockOnX();
        else
            SpawnBlockOnZ();

        isUsingXSpawner = !isUsingXSpawner;// for alternating
    }

    public void SpawnBlockOnX()
    {
        blockSpawnerOnX.SpawnBlock(baseBlockParentTransform[currentBaseBlockIndex]);
    }

    public void SpawnBlockOnZ()
    { 
        blockSpawnerOnZ.SpawnBlock(baseBlockParentTransform[currentBaseBlockIndex]);
    }

    public void SetParentOfSpawnedBlockToCorrespondingBaseBlock(Transform currentSpawnedBlock)
    {
        currentSpawnedBlock.transform.SetParent(baseBlocks[currentBaseBlockIndex].transform);
    }
    


    // Game over Handlers
    public void HandleGameOver()
    {
        stateManager.isPlaying = false;
        Destroy(Block.currentBlock.gameObject);
        Block.SetCurrentBlock(null);
        Debug.Log(score);
        uiManager.SetPlayPauseUIMenuScore(score);
        if (!IsLastCube())
        {
            Debug.Log("triggered W BB");
            uiManager.OnGameOverWithBaseBlocksRemaining();
        }
        else
        {
            Debug.Log("triggered No BB");
            uiManager.OnGameOverWithNOBaseBlockRemaining(false);
        }
    }

    public void EndPlayerPhase()
    {
        uiManager.OnEndPlayerPhaseButtonClicked();

        // cal set announvement 
        ActivateEnemySpawners();
    }

    public void OnCamRecentreClicked()
    { 
        cameraController.ResetCameraPositionToMostRecentStackBase(baseBlockParentTransform);
    }

    public void ActivateEnemySpawners()
    {
        OnCamRecentreClicked();

        if (enemySpawnerManager.GetSpawnerCountInSpawnerList() > 0)
        {
            cameraController.canControlCamera = true;
            uiManager.EnableRecentreButton(true);
            uiManager.SetEnemyRelatedReminder("Enemies are spawning!\n Paning and Zoom Active",Color.red);
            uiManager.TogglePlayerLifeIndicatorOnGameScreenAndConverseOnIntermediateMenu(true,false);
            enemySpawnerManager.ReleaseEnemyHordes();
        }
        else
        {
            Debug.Log("no spawners in enemy spawner manager's spawner list");
            enemySpawnerManager.SetSpawnersFinishedSpawning(true);
            uiManager.enemyRelatedAnnoncementText.gameObject.SetActive(false);
        }
        
    }


    //Game Conclude when enemy reaches the goal 
    void HandleOnEnemyReachedGoal()
    {
        playerAttributeManager.DecrementLifeCount();
        uiManager.UpdatePlayerLifeCount(playerAttributeManager.getPlayerLives());

        if (playerAttributeManager.getPlayerLives() <= 0)
        {
            SetLevelVictoryStatus(false);
        }
    }

    public void SetLevelVictoryStatus(bool gameVictoryStatus)
    {
        LevelCompletionStatus(gameVictoryStatus);
    }

    void LevelCompletionStatus(bool wasVictorious)
    {
        stateManager.isPlaying = false;
        stateManager.isVictorious = wasVictorious;
        stateManager.isGameOver = true;
        uiManager.EnableRecentreButton(false);

        bool wasLastLevel = sceneController.wasLastLevelInBuildIndex();//SCENEM CONTROLLER CHECKS FOR LAST LEVEL AND SENDS IT OVER TO UI ON GAME OVER

        uiManager.UIOnGameOver(wasVictorious, score, CheckForStarsObtained(score),wasLastLevel);
    }

    public int CheckForStarsObtained(int finalScore)
    {
        if (finalScore <= 0 || pointsToMaxStars <= 0) return 0;

        double percentage = ((float)finalScore /(float) pointsToMaxStars) * 100;

        if (percentage >= 70.0) return 3;
        else if (percentage >= 30) return 2;
        else return 1;
    }

}
