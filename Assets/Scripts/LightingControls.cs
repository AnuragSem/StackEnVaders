using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingControls : MonoBehaviour
{
    float degreeToRotate = 0f;

    [Header("Sun Attribues")]
    [SerializeField] Light sun;
    [SerializeField] float trackedSunAngle = 0f;
    float trackedSunIntensity;
    Color trackedSunColor;
    private float initialSunY;
    private float initialSunZ;

    [Header("Moon Attributes")]
    [SerializeField] Light moon;
    [SerializeField] float trackedMoonAngle = 180f;
    float trackedMoonIntensity;
    Color trackedMoonColor;
    private float initialMoonY;
    private float initialMoonZ;

    [Header("Others")]
    [SerializeField] float moonOffsetFromSunOnX = 180f;
    private Coroutine currentRotationCoroutine;
    [SerializeField] float timeToRotate = 3f;
    [SerializeField] List<DayPhaseData> dayPhaseData;

    //ambient lighting

    Color trackedSkyColor;
    Color trackedEquatorColor;
    Color trackedGroundColor;

    //optional 
    float trackedSkyExposure;
    float trackedEquatorExposure;
    float trackedGroundExposure;

    private void Start()
    {
        Vector3 sunEuler = sun.transform.localEulerAngles;
        trackedSunAngle = NormalizeAngle(sunEuler.x);
        initialSunY = sunEuler.y;
        initialSunZ = sunEuler.z;

        Vector3 moonEuler = moon.transform.localEulerAngles;
        trackedMoonAngle = moonEuler.x;
        initialMoonY = moonEuler.y;
        initialMoonZ = moonEuler.z;

        trackedSunIntensity = sun.intensity;
        trackedSunColor = sun.color;

        trackedMoonIntensity  = moon.intensity;
        trackedMoonColor = moon.color;

        trackedSkyColor = RenderSettings.ambientSkyColor;
        trackedEquatorColor = RenderSettings.ambientEquatorColor;
        trackedGroundColor = RenderSettings.ambientGroundColor;

        CalculateLightAttributesForCurrentBackground(0);
    }

    public void CalculateLightAttributesForCurrentBackground(int currentColorIndex)
    {
        if (dayPhaseData == null || dayPhaseData.Count < 2)
            return;

        for (int i = 0; i < dayPhaseData.Count - 1; i++)
        {
            DayPhaseData from = dayPhaseData[i];
            DayPhaseData to = dayPhaseData[i + 1];

            if (from.index <= currentColorIndex &&  currentColorIndex <= to.index)
            {

                float t = (currentColorIndex - from.index) / (float)(to.index - from.index);
                float targetAngle = Mathf.LerpAngle(from.desiredAngle, to.desiredAngle, t);

                float targetSunIntensity = Mathf.Lerp(from.desiredSunIntensity, to.desiredSunIntensity, t);
                float targetMoonIntensity = Mathf.Lerp(from.desiredMoonIntensity, to.desiredMoonIntensity, t);

                Color targetSunColor = Color.Lerp(from.desiredSunColor, to.desiredSunColor, t);
                Color targetMoonColor = Color.Lerp(from.desiredMoonColor, to.desiredMoonColor, t);


                //Ambient Lighting
                Color targetSkyColor  = Color.Lerp(from.skyColor, to.skyColor, t);
                Color targetEquatorColor = Color.Lerp(from.equatorColor, to.equatorColor, t);
                Color targetGroundColor = Color.Lerp(from.groundColor, to.groundColor, t);


                TargetLightAttributes sunTarget = new TargetLightAttributes(targetAngle, targetSunIntensity, targetSunColor);
                TargetLightAttributes moonTarget = new TargetLightAttributes(targetAngle + moonOffsetFromSunOnX,targetMoonIntensity, targetMoonColor);

                TargetAmbientLightAttributes ambientLightTarget = new TargetAmbientLightAttributes(targetSkyColor, targetEquatorColor, targetGroundColor);


                if (currentRotationCoroutine != null)
                {
                    StopCoroutine(currentRotationCoroutine);
                }
                currentRotationCoroutine =StartCoroutine(InterpolateLightAttributes(sun,moon,sunTarget,moonTarget,ambientLightTarget,timeToRotate));
                break;

            }
        }
    }

    IEnumerator InterpolateLightAttributes(Light sun ,Light moon,TargetLightAttributes sunTarget,TargetLightAttributes moonTarget , TargetAmbientLightAttributes ambientLightTarget, float duration)
    {
        float sunStart = trackedSunAngle;
        float moonStart = trackedMoonAngle;

        float sunStartIntensity  = trackedSunIntensity;
        Color sunStartColor = trackedSunColor;

        float moonStartIntensity = trackedMoonIntensity;
        Color moonStartColor = trackedMoonColor;

        Color ambientLightSkyColorStart = trackedSkyColor;
        Color ambientLightEquatorColorStart = trackedEquatorColor;
        Color ambientLightGroundColorStart = trackedGroundColor;

        if (sunTarget.intensity > 0f && !sun.gameObject.activeSelf)
            sun.gameObject.SetActive(true);

        if (moonTarget.intensity > 0f && !moon.gameObject.activeSelf)
            moon.gameObject.SetActive(true);


        Debug.Log($"moonstart {moonStart} moontarget {moonTarget.angle}");

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Lerp both rotation (angles on x)
            float newSunAngle = Mathf.LerpAngle(sunStart, sunTarget.angle, t);
            trackedSunAngle = newSunAngle;

            float newMoonAngle = LerpAngleForward(moonStart, moonTarget.angle, t); //custom lerp lol
            trackedMoonAngle = newMoonAngle;

            //lerp both Intensities
            float newSunIntensity = Mathf.Lerp(sunStartIntensity, sunTarget.intensity, t);
            trackedSunIntensity = newSunIntensity;

            float newMoonIntensity = Mathf.Lerp(moonStartIntensity, moonTarget.intensity, t);
            trackedMoonIntensity = newMoonIntensity;

            //lerp both Colors
            Color newSunColor = Color.Lerp(sunStartColor, sunTarget.color, t);
            trackedSunColor = newSunColor;

            Color newMoonColor = Color.Lerp(moonStartColor,moonTarget.color, t);
            trackedMoonColor = newMoonColor;

            //AmbientLights
            Color newSkyColor = Color.Lerp(ambientLightSkyColorStart, ambientLightTarget.skyColor, t);
            trackedSkyColor = newSkyColor;

            Color newEquatorColor = Color.Lerp(ambientLightEquatorColorStart,ambientLightTarget.equatorColor, t);
            trackedEquatorColor = newEquatorColor;

            Color newGroundColor = Color.Lerp(ambientLightGroundColorStart,ambientLightTarget.groundColor, t);
            trackedGroundColor = newGroundColor;

            //apply the short rotation
            sun.transform.localEulerAngles = new Vector3(newSunAngle, initialSunY, initialSunZ);
            moon.transform.localEulerAngles = new Vector3(newMoonAngle, initialMoonY, initialMoonZ);

            //apply intensities
            sun.intensity = newSunIntensity;
            moon.intensity = newMoonIntensity;

            //apply colors
            sun.color = newSunColor;
            moon.color = newMoonColor;

            //apply ambient Light Color
            RenderSettings.ambientSkyColor = newSkyColor;
            RenderSettings.ambientEquatorColor = newEquatorColor;
            RenderSettings.ambientGroundColor = newGroundColor;

            yield return null;
        }

        sun.transform.localEulerAngles = new Vector3(sunTarget.angle, initialSunY, initialSunZ);
        moon.transform.localEulerAngles = new Vector3(moonTarget.angle, initialMoonY, initialMoonZ);

        sun.intensity = sunTarget.intensity;
        moon.intensity = moonTarget.intensity;

        sun.color = sunTarget.color;
        moon.color = moonTarget.color;

        RenderSettings.ambientSkyColor = ambientLightTarget.skyColor;
        RenderSettings.ambientEquatorColor = ambientLightTarget.equatorColor;
        RenderSettings.ambientGroundColor = ambientLightTarget.groundColor;

        trackedSunAngle = sunTarget.angle;
        trackedSunIntensity = sunTarget.intensity;
        trackedSunColor = sunTarget.color;

        trackedMoonAngle = moonTarget.angle;
        trackedMoonIntensity = moonTarget.intensity;
        trackedMoonColor = moonTarget.color;

        trackedSkyColor = ambientLightTarget.skyColor;
        trackedEquatorColor = ambientLightTarget.equatorColor;
        trackedGroundColor = ambientLightTarget.groundColor;

        if (sunTarget.intensity == 0f)
            sun.gameObject.SetActive(false);

        if (moonTarget.intensity == 0f)
            moon.gameObject.SetActive(false);

    }

    float NormalizeAngle(float angle)
    {
        angle %= 360f;
        return angle < 0 ? angle + 360f : angle;
    }

    float LerpAngleForward(float from, float to, float t)
    {
        from = NormalizeAngle(from);
        to = NormalizeAngle(to);

        float delta = (to - from + 360f) % 360f; // force positive direction
        return NormalizeAngle(from + delta * t);
    }

    void OnDrawGizmos()
    {
        Vector3 dir = moon.transform.forward;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(moon.transform.position, moon.transform.position + dir * 3f);
    }
}
