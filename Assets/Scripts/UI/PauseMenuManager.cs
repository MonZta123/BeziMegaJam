using UI;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager Instance { get; private set; }

    [SerializeField]
    private GameObject pauseMenuPanel;

    [SerializeField]
    private GameObject blocker;

    [SerializeField]
    private GameObject settingsPanel;

    [SerializeField]
    private GameObject confirmQuitPanel;

    [SerializeField]
    private Slider masterVolumeSlider;

    [SerializeField]
    private Slider musicVolumeSlider;

    [SerializeField]
    private Slider sfxVolumeSlider;

    [SerializeField]
    private Slider mouseSensitivitySlider;

    [SerializeField]
    private AudioMixer audioMixer;
    [SerializeField]
    private AudioSource audioSource;

    public bool PauseIsActive =>
        pauseMenuPanel.activeSelf || settingsPanel.activeSelf || confirmQuitPanel.activeSelf;

    private void Awake()
    {
        Instance = this;
    }

    private float _masterVolume;
    private float _musicVolume;
    private float _sfxVolume;
    private float _mouseSensitivity;

    public void ShowPauseMenu()
    {
        print("openingpausemenu");
        blocker.SetActive(true);
        pauseMenuPanel.SetActive(true);
        pauseMenuPanel.transform.SetAsLastSibling();
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    private void PlayAudioButton()
    {
        audioSource.Play();
    }

    public void ContinueGame()
    {
        PlayAudioButton();
        blocker.SetActive(false);
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        confirmQuitPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SettingsClicked()
    {
        PlayAudioButton();
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        settingsPanel.transform.SetAsLastSibling();

        audioMixer.GetFloat("master", out var db);
        masterVolumeSlider.value = Mathf.Pow(10f, db / 20f);

        audioMixer.GetFloat("music", out var db1);
        musicVolumeSlider.value = Mathf.Pow(10f, db1 / 20f);

        audioMixer.GetFloat("sfx", out var db2);
        sfxVolumeSlider.value = Mathf.Pow(10f, db2 / 20f);

        _masterVolume = masterVolumeSlider.value;
        _musicVolume = musicVolumeSlider.value;
        _sfxVolume = sfxVolumeSlider.value;
        _mouseSensitivity = Settings.SavedMouseSensitivity;
        mouseSensitivitySlider.value = _mouseSensitivity;
        if (Player.Instance)
            Player.Instance.SetMouseSensitivity(_mouseSensitivity);
    }

    /// <summary>
    /// Previews the mouse sensitivity selected in the pause-menu settings.
    /// </summary>
    public void SetMouseSensitivity(float sensitivity)
    {
        _mouseSensitivity = Mathf.Clamp(
            sensitivity,
            Settings.MinimumMouseSensitivity,
            Settings.MaximumMouseSensitivity);
        if (Player.Instance)
            Player.Instance.SetMouseSensitivity(_mouseSensitivity);
    }

    public void SetMasterVolume(float volume)
    {
        _masterVolume = volume;
        audioMixer.SetFloat("master", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f);
    }

    public void SetMusicVolume(float volume)
    {
        _musicVolume = volume;
        audioMixer.SetFloat("music", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f);
    }

    public void SetSFXVolume(float volume)
    {
        _sfxVolume = volume;
        audioMixer.SetFloat("sfx", Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f);
    }

    public void ConfirmSettingsClicked()
    {
        PlayAudioButton();
        pauseMenuPanel.SetActive(true);
        pauseMenuPanel.transform.SetAsLastSibling();
        settingsPanel.SetActive(false);

        Settings.Instance.MasterVolume = _masterVolume;
        Settings.Instance.MusicVolume = _musicVolume;
        Settings.Instance.SfxVolume = _sfxVolume;
        Settings.Instance.MouseSensitivity = _mouseSensitivity;

        if (Player.Instance)
            Player.Instance.SetMouseSensitivity(_mouseSensitivity);

        _masterVolume = 0;
        _musicVolume = 0;
        _sfxVolume = 0;
        _mouseSensitivity = 0f;
    }

    public void CancelSettingsClicked()
    {
        PlayAudioButton();
        pauseMenuPanel.SetActive(true);
        pauseMenuPanel.transform.SetAsLastSibling();
        settingsPanel.SetActive(false);

        var instance = Settings.Instance;
        _mouseSensitivity = instance.MouseSensitivity;
        mouseSensitivitySlider.value = _mouseSensitivity;
        if (Player.Instance)
            Player.Instance.SetMouseSensitivity(_mouseSensitivity);

        audioMixer.SetFloat("master", Mathf.Log10(Mathf.Max(instance.MasterVolume, 0.0001f)) * 20f);
        audioMixer.SetFloat("music", Mathf.Log10(Mathf.Max(instance.MusicVolume, 0.0001f)) * 20f);
        audioMixer.SetFloat("sfx", Mathf.Log10(Mathf.Max(instance.SfxVolume, 0.0001f)) * 20f);

        _masterVolume = 0;
        _musicVolume = 0;
        _sfxVolume = 0;
    }

    public void QuitClicked()
    {
        PlayAudioButton();
        pauseMenuPanel.SetActive(false);
        confirmQuitPanel.SetActive(true);
        confirmQuitPanel.transform.SetAsLastSibling();
    }

    public void ConfirmQuitClicked()
    {
        PlayAudioButton();
        Time.timeScale = 1f;
        HUD.Instance.FadeOut(0.5f, () => SceneManager.LoadScene("MainMenu"));
    }

    public void CancelQuitClicked()
    {
        PlayAudioButton();
        pauseMenuPanel.SetActive(true);
        pauseMenuPanel.transform.SetAsLastSibling();
        confirmQuitPanel.SetActive(false);
    }
}
