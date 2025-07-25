using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TargetAmbientLightAttributes
{
    public Color skyColor;
    public Color equatorColor;
    public Color groundColor;

    public float skyExp;
    public float equatorExp;
    public float groundExp;

    public TargetAmbientLightAttributes(Color skyColor , Color equatorColor , Color groundColor)
    { 
        this.skyColor = skyColor;
        this.equatorColor = equatorColor;
        this.groundColor = groundColor;
    }

    public TargetAmbientLightAttributes(Color skyColor, Color equatorColor, Color groundColor,
                                        float skyExp , float equatorExp , float groundExp)
    {
        this.skyColor = skyColor;
        this.equatorColor = equatorColor;
        this.groundColor = groundColor;

        this.skyExp = skyExp;
        this.equatorExp = equatorExp;
        this.groundExp = groundExp;
    }

}
