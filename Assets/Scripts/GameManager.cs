using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Text;
using System.Runtime.CompilerServices;

public class GameManager : MonoBehaviour
{
    [Header("GO Refrences")]

    [SerializeField] BackgroundColourChanger bgColourChanger;

    [SerializeField]CameraController cameraController;

    [SerializeField] LevelManager levelManager;

    [SerializeField] UIManager uiManager;

    [SerializeField] StateManager stateManager;

    public void OnScreenTapped(InputAction.CallbackContext ctx)
    {
        if (stateManager.isPlaying && ctx.performed && Block.currentBlock != null)
        {
            Block.currentBlock.Stop();

            if (stateManager.isPlaying)
            {
                levelManager.score++;
                levelManager.stackHeight++;
                Block.currentBlock.indexInStack  = levelManager.stackHeight;

                Debug.Log(Block.currentBlock.indexInStack);
                uiManager.SetScore(levelManager.score);

                //sets current block to camera's target
                cameraController.SetTarget(Block.previousBlock.transform);

                //changes bg colours
                bgColourChanger.UpdateStackHeight(levelManager.stackHeight);

                //spawns new block
                levelManager.SpawnBlock();
            }
        }
    }

    public void OnScrolling(InputAction.CallbackContext ctx)
    {
        float inputScrollValue;
        if (ctx.performed)
        {
            inputScrollValue = ctx.ReadValue<float>();
            float sv = inputScrollValue/Math.Abs(inputScrollValue);
            cameraController.Zoom(sv);
            //Debug.Log("Scroll value " + sv);
        }
    }

    public void OnPanning(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            //Debug.Log("started panning");
            cameraController.OnPanStart();
        }
        if (ctx.canceled)
        {
            //Debug.Log("ended panning");
            cameraController.OnPanStop();
        }
    }
}
