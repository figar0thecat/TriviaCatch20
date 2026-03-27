using UnityEngine;

public class LanternController : MonoBehaviour
{
    [Header("References")]
    public Light lanternLight;           // Assign the Point Light inside lantern
    public Renderer lanternRenderer;     // Assign MeshRenderer that uses the glass material
    public int materialIndex = 1;        // Which material slot is the glowing one (default = 1)

    [Header("Glow Settings")]
    public Color glowColor = new Color(1f, 0.8f, 0.3f); // warm yellow
    public float glowIntensity = 3f;

    private Material _matInstance;

    void Awake()
    {
        if (lanternRenderer != null)
        {
            // Get a safe instance of the material (so we don't edit shared asset)
            var mats = lanternRenderer.materials;
            _matInstance = mats[materialIndex];
        }
    }

    public void SetLanternState(bool on)
    {
        if (lanternLight != null)
            lanternLight.enabled = on;

        if (_matInstance != null)
        {
            if (on)
            {
                _matInstance.EnableKeyword("_EMISSION");
                _matInstance.SetColor("_EmissionColor", glowColor * glowIntensity);
            }
            else
            {
                _matInstance.DisableKeyword("_EMISSION");
                _matInstance.SetColor("_EmissionColor", Color.black);
            }
        }
    }
}
