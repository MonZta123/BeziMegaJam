using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings Instance { get; set; }

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        DontDestroyOnLoad(gameObject);
    }
    
    public const string MouseSensitivityPreferenceKey = "PlayerMouseSensitivity";
    public const float DefaultMouseSensitivity = 0.12f;
    public const float MinimumMouseSensitivity = 0.01f;
    public const float MaximumMouseSensitivity = 0.5f;

    public float MusicVolume { get; set; } = 1f;
    public float SfxVolume { get; set; } = 1f;
    public float MasterVolume { get; set; } = 1f;

    public static float SavedMouseSensitivity =>
        PlayerPrefs.GetFloat(MouseSensitivityPreferenceKey, DefaultMouseSensitivity);

    public float MouseSensitivity
    {
        get => SavedMouseSensitivity;
        set
        {
            PlayerPrefs.SetFloat(
                MouseSensitivityPreferenceKey,
                Mathf.Clamp(value, MinimumMouseSensitivity, MaximumMouseSensitivity));
            PlayerPrefs.Save();
        }
    }
}
