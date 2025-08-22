using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockColourManager : MonoBehaviour
{
    public static BlockColourManager Instance { get; private set; }

    //private Color[] autumnColors = {
    //                                new Color(0.42f, 0.56f, 0.14f), // Olive Green
    //                                new Color(0.90f, 0.80f, 0.70f), // Light Tan
    //                                new Color(0.76f, 0.60f, 0.20f), // Muted Ochre
    //                                new Color(0.80f, 0.47f, 0.13f), // Golden Yellow
    //                                new Color(0.87f, 0.52f, 0.20f), // Burnt Orange
    //                                new Color(0.76f, 0.29f, 0.13f), // Maple Red
    //                                new Color(0.55f, 0.23f, 0.23f), // Muted Crimson
    //                                new Color(0.58f, 0.29f, 0.13f), // Dark Brown
    //                                new Color(0.50f, 0.40f, 0.50f)  // Dusty Purple
    //                                };

    private Color[] autumnColors = {
                                    new Color(0.9f, 0.8f, 0.7f), // Softer Light Tan
                                    new Color(0.80f, 0.47f, 0.13f), // Golden Yellow
                                    new Color(0.72f, 0.53f, 0.04f), // Mustard Yellow
                                    new Color(0.89f, 0.41f, 0.18f), // Pumpkin Orange
                                    new Color(0.87f, 0.52f, 0.20f), // Burnt Orange
                                    new Color(0.76f, 0.29f, 0.13f), // Maple Red
                                    new Color(0.58f, 0.29f, 0.13f)  // Dark Brown
                                    };


    [SerializeField] int stepBetweenColours = 5;
    int totalBlocksSpawned = 0;
    int currentColourIndex = 0;

    private void Awake()
    {
        if(Instance ==null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public Color GetNextBlockColor()
    {
        //interpolation factor
        float interpolFactor = (totalBlocksSpawned % stepBetweenColours)/(float)stepBetweenColours;
        Color startColor = autumnColors[currentColourIndex];
        Color endColor = autumnColors[(currentColourIndex+1) % autumnColors.Length];

        Color nextColor = Color.Lerp(startColor, endColor, interpolFactor);

        //increasing the total number of cubes
        totalBlocksSpawned++;

        //changin current color inzex
        if (totalBlocksSpawned % stepBetweenColours == 0)
        { 
            currentColourIndex = (currentColourIndex + 1) % autumnColors.Length;
        }

        return nextColor;
    }


}
