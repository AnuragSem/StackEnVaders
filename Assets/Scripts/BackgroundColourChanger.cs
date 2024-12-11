using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundColourChanger : MonoBehaviour
{
    public Camera mainCamera; // Assign the main camera in the Inspector
    private Color[] colors = {
                            new Color(1f, 0.8f, 0.9f), // Light Pink
                            new Color(0.9f, 0.85f, 1f), // Pale Lavender
                            new Color(0.8f, 0.9f, 1f), // Light Blue
                            new Color(0.6f, 0.8f, 1f), // Sky Blue
                            new Color(0.4f, 0.7f, 1f), // Vibrant Blue
                            new Color(0.95f, 0.95f, 1f), // Soft White
                            new Color(0.96f, 0.87f, 0.7f), // Beige
                            new Color(1f, 0.9f, 0.6f), // Golden Yellow
                            new Color(1f, 0.7f, 0.6f), // Peach
                            new Color(0.9f, 0.5f, 0.3f), // Soft Orange
                            new Color(0.7f, 0.5f, 0.9f), // Lavender
                            new Color(0.8f, 0.5f, 0.6f), // Dusty Rose
                            new Color(0.2f, 0.2f, 0.5f), // Deep Blue
                            new Color(0.1f, 0.05f, 0.3f) // Midnight Purple
                            };
    public int blocksPerColorChange = 5; // Change color every 5 blocks

    private int stackHeight = 0; // Tracks the stack height
    private int currentColorIndex = 0;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (colors.Length > 0)
            mainCamera.backgroundColor = colors[0];
    }

    public void UpdateStackHeight(int newHeight)
    {
        stackHeight = newHeight;

        // Calculate the next color index based on stack height
        int newColorIndex = Mathf.Min(stackHeight / blocksPerColorChange, colors.Length - 1);

        if (newColorIndex != currentColorIndex)
        {
            StartCoroutine(TransitionToColor(colors[newColorIndex]));
            currentColorIndex = newColorIndex;
        }
    }

    private System.Collections.IEnumerator TransitionToColor(Color targetColor)
    {
        Color initialColor = mainCamera.backgroundColor;
        float duration = 1f; // Duration of the transition
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            mainCamera.backgroundColor = Color.Lerp(initialColor, targetColor, elapsedTime / duration);
            yield return null;
        }

        mainCamera.backgroundColor = targetColor;
    }
}
