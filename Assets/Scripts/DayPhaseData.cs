using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DayPhaseData
{
    [Header("Directional Lighting")]
    public string phaseName;
    public int index;
    public float desiredAngle;
    public Color desiredSunColor;
    public float desiredSunIntensity;
    public Color desiredMoonColor;
    public float desiredMoonIntensity;

    [Header("Ambient Lighting")]
    //Ambient Lighthting -_-
    public Color skyColor;
    public Color equatorColor;
    public Color groundColor;

    //HDR
    public float skyExposure;
    public float equatorExposure;
    public float groundExposure;

    [Header("Post Processing")]
    //Post Processing Settings
    public float cgTemperature;
    public float cgTint;
    public float cgPostExposure;
    public float cgSaturation;
    public float cgContrast;

    public float vignetteIntensity;
    public float vignetteSmoothness;

    public float aoIntensity;
    public float aoRadius;

}
