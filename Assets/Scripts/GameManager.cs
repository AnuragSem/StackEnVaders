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


    [SerializeField] BlockSpawner blockSpawnerOnZ;
    [SerializeField] BlockSpawner blockSpawnerOnX;

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

        //delete
        //isplaying = uiManager.isTimerOver;
        

        //need to do theses two after the countdown ends //main problem + need to do this everytime base block sitches
        //moved the to on countdown ends after base block switch
    }

    public float GetTimeForAutoStoping()
    {
        return timeForAutoStoppingBlock;
    }

    void OnCountdownEndsAfterBaseBlockSwitch()
    {
        levelManager.SetNextBaseBlock();
        blockSpawnerOnZ.SpawnBlock();
    }

    public IEnumerator CountDownBeforeStartingGame()
    {
        stateManager.isPlaying = false ;
        uiManager.countdownText.color = Color.red;
        uiManager.DisplayCountdownText(true);

        int countdown = 3;

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

        //could be done by events???
        stateManager.isPlaying = true;
        OnCountdownEndsAfterBaseBlockSwitch();
        //resetting just in case // definitely not necessary
        uiManager.countdownText.text = "3";
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
                cameraController.SetTarget(Block.currentBlock.transform);

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
        if(isUsingXSpawner)
            blockSpawnerOnX.SpawnBlock();
        else
            blockSpawnerOnZ.SpawnBlock();

        isUsingXSpawner = !isUsingXSpawner;// for alternating
    }



    // Game over Handlers

    //might have become obsolute

    private bool isGameOver()
    {
        float hangoverValue = isUsingXSpawner
                             ? Block.currentBlock.transform.position.x - Block.previousBlock.transform.position.x
                             : Block.currentBlock.transform.position.z - Block.previousBlock.transform.position.z;

        float previousBlockSize = isUsingXSpawner
                                ? Block.previousBlock.transform.localScale.x
                                : Block.previousBlock.transform.localScale.z;


        return (Mathf.Abs(hangoverValue) >= previousBlockSize);
    }

    public void HandleGameOver()
    {
        stateManager.isPlaying = false;
        Debug.Log(score);
        uiManager.SetPlayPauseUIMenuScore(score);


        if (!levelManager.IsLastCube())
        {
            // make the Sbb block appear and coutdown and auto call switch base block is user taps it within the time
            //otherwise pull up the GO screen with the sbb option active along with the rest of the options
            //call this after the timer for continue
            Debug.Log("triggered W BB");
            uiManager.OnGameOverWithBaseBlocksRemaining();

            ///////////////////////IMPORTANT///////////////
            //prolly reset static variables and other stuff except score????

        }
        else
        {
            Debug.Log("triggered No BB");
            //pull up the GO screen with the sbb option disabled along with the rest of the options
            uiManager.OnGameOverWithNOBaseBlockRemaining(false);

        }

    }

    private void StackGameOver()
    {

        //camera zoom out
        //make enemies spawn
        //check condition of enemy game over


        //// temporary////
        Debug.Log("GameStackOVer");
        Block.SetCurrentBlock(null);
        Block.SetPreviousBlock(null);

        //delete when implement ui successfully
        SceneManager.LoadScene(0);
    }
}
