using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TargetLightAttributes
{
    public float angle;
    public float intensity;
    public Color color;

    public TargetLightAttributes(float angle, float intensity, Color color)
    { 
        this.angle = angle;
        this.intensity = intensity;
        this.color = color;
    }
}
