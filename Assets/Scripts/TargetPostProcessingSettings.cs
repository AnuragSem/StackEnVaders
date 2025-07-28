[System.Serializable]
public class TargetPostProcessingSettings
{
    public float cgTemperature;
    public float cgTint;
    public float cgPostExposure;
    public float cgSaturation;
    public float cgContrast;

    public float vignetteIntensity;
    public float vignetteSmoothness;

    public float aoIntensity;
    public float aoRadius;

    public TargetPostProcessingSettings(float cgTemperature , float cgTint, float cgPostExposure, float cgSaturation,
                                        float cgContrast , float vignetteIntensity , float vignetteSmoothness , float aoIntensity , float aoRadius)
    { 
        this.cgTemperature = cgTemperature;
        this.cgTint = cgTint;
        this.cgPostExposure = cgPostExposure;
        this.cgSaturation = cgSaturation;
        this.cgContrast = cgContrast;

        this.vignetteIntensity = vignetteIntensity;
        this.vignetteSmoothness = vignetteSmoothness;

        this.aoIntensity = aoIntensity;
        this.aoRadius = aoRadius;
    }
}
