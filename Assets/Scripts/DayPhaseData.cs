using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DayPhaseData
{
    public string phaseName;
    public int index;
    public float desiredAngle;
    public Color desiredSunColor;
    public float desiredSunIntensity;
    public Color desiredMoonColor;
    public float desiredMoonIntensity;

    //Ambient Lighthting -_-
    public Color skyColor;
    public Color equatorColor;
    public Color groundColor;

    //HDR
    public float skyExposure;
    public float equatorExposure;
    public float groundExposure;

}
