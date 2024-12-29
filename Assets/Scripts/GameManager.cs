using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public int score = 0;

    public int stackHeight = 0;

    [SerializeField] float timeForAutoStoppingBlock = 6f;

    [SerializeField] BackgroundColourChanger bgColourChanger;

    [SerializeField]CameraController cameraController;

    [SerializeField] LevelManager levelManager;

    [SerializeField] UIManager uiManager;

    [SerializeField] StateManager stateManager;

    public static bool isUsingXSpawner { get; private set; } = true;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        levelManager.GetBaseBlocksPositions();
        uiManager.SetScore(score);

        StartCoroutine(CountDownBeforeStartingGame());
    }

    public float GetTimeForAutoStoping()
    {
        return timeForAutoStoppingBlock;
    }

    public void StartCountDownCoroutie()
    {
        StartCoroutine(CountDownBeforeStartingGame());
    }
    void OnCountdownEndsAfterBaseBlockSwitch()
    {
        levelManager.SpawnBlockOnZ();
        stateManager.isPlaying = true;
    }
    

    public IEnumerator CountDownBeforeStartingGame()
    {
        stateManager.isPlaying = false;
        int countdown = 3;

        levelManager.SetNextBaseBlock();
        levelManager.SetSpawnerPosition();
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


    //Actual Controls

    public void OnScreenTapped(InputAction.CallbackContext ctx)
    {
        if (stateManager.isPlaying && ctx.performed && Block.currentBlock != null)
        {
            Block.currentBlock.Stop();

            if (stateManager.isPlaying)
            {
                score++;
                stackHeight++;
                uiManager.SetScore(score);

                //sets current block to camera's target
                cameraController.SetTarget(Block.previousBlock.transform);

                //changes bg colours
                bgColourChanger.UpdateStackHeight(stackHeight);

                //spawns new block
                SpawnBlock();
            }
        }
    }


    //Spawner Controller

    void SpawnBlock()
    {
        if (isUsingXSpawner)
            levelManager.SpawnBlockOnX();
        else
            levelManager.SpawnBlockOnZ();

        isUsingXSpawner = !isUsingXSpawner;// for alternating
    }



    // Game over Handlers


    public void HandleGameOver()
    {
        stateManager.isPlaying = false;
        Debug.Log(score);
        uiManager.SetPlayPauseUIMenuScore(score);


        if (!levelManager.IsLastCube())
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

    private void StackGameOver()
    {
    }
}
