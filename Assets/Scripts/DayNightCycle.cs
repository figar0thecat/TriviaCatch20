using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("References")]
    public Light directionalLight;

    [Header("Settings")]
    public float dayDurationInMinutes = 1f; // 1 minute for a full day cycle
    public float nightIntensity = 0.2f;     // Light intensity at "night"
    public float dayIntensity = 1f;        // Light intensity at "day"
    private float dayCycleSpeed;           // degrees per second

    [Header("Sounds")]
    public AudioSource dayAudio;
    public AudioSource nightAudio;

    [Header("Skybox")]
    public Material skyboxMaterial;
    public float dayExposure = 1f;
    public float nightExposure = 0.1f;
    private bool isNight;
    private void Start()
    {
        // 360 degrees in (dayDurationInMinutes * 60) seconds
        float totalSeconds = dayDurationInMinutes * 60f;
        dayCycleSpeed = 360f / totalSeconds;

        UpdateAudio(); // Initialize with the correct sound
    }

    void Update()
    {
        // Rotate the sun
        directionalLight.transform.Rotate(Vector3.right * (dayCycleSpeed * Time.deltaTime));

        // Optional: Adjust intensity based on angle
        float currentAngle = directionalLight.transform.eulerAngles.x;
        bool nowNight = currentAngle > 180f;


        // Adjust light intensity
        directionalLight.intensity = Mathf.Lerp(
            directionalLight.intensity,
            nowNight ? nightIntensity : dayIntensity,
            Time.deltaTime * 0.1f
        );
        // Switch audio only when state changes
        if (nowNight != isNight)
        {
            isNight = nowNight;
            UpdateAudio();
            LanternController[] lanterns = UnityEngine.Object.FindObjectsByType<LanternController>(FindObjectsSortMode.None);
            foreach (var lantern in lanterns)
            {
                lantern.SetLanternState(isNight);
            }
        }
        if (skyboxMaterial != null)
        {
            float exposure = Mathf.Lerp(dayExposure, nightExposure, isNight ? 1f : 0f);
            skyboxMaterial.SetFloat("_Exposure", exposure);
            RenderSettings.ambientIntensity = Mathf.Lerp(RenderSettings.ambientIntensity, isNight ? 0.1f : 1f, Time.deltaTime * 0.5f);
            RenderSettings.reflectionIntensity = Mathf.Lerp(RenderSettings.reflectionIntensity, isNight ? 0.1f : 1f, Time.deltaTime * 0.5f);
        }
    }
    void UpdateAudio()
    {
        if (isNight)
        {
            if (!nightAudio.isPlaying)
                nightAudio.Play();

            if (dayAudio.isPlaying)
                dayAudio.Stop();
        }
        else
        {
            if (!dayAudio.isPlaying)
                dayAudio.Play();

            if (nightAudio.isPlaying)
                nightAudio.Stop();
        }
    }
}
