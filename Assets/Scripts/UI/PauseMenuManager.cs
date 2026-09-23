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
    private AudioMixer audioMixer;

    public bool PauseIsActive => pauseMenuPanel.activeSelf;

    private void Awake()
    {
        Instance = this;
    }

    private float _masterVolume;
    private float _musicVolume;
    private float _sfxVolume;

    public void ShowPauseMenu()
    {
        blocker.SetActive(true);
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ContinueGame()
    {
        blocker.SetActive(false);
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void SettingsClicked()
    {
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);

        audioMixer.GetFloat("master", out var db);
        masterVolumeSlider.value = Mathf.Pow(10f, db / 20f);

        audioMixer.GetFloat("music", out var db1);
        musicVolumeSlider.value = Mathf.Pow(10f, db1 / 20f);

        audioMixer.GetFloat("sfx", out var db2);
        sfxVolumeSlider.value = Mathf.Pow(10f, db2 / 20f);

        _masterVolume = masterVolumeSlider.value;
        _musicVolume = musicVolumeSlider.value;
        _sfxVolume = sfxVolumeSlider.value;
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
        pauseMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);

        Settings.Instance.MasterVolume = _masterVolume;
        Settings.Instance.MusicVolume = _musicVolume;
        Settings.Instance.SfxVolume = _sfxVolume;

        _masterVolume = 0;
        _musicVolume = 0;
        _sfxVolume = 0;
    }

    public void CancelSettingsClicked()
    {
        pauseMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);

        var instance = Settings.Instance;

        audioMixer.SetFloat("master", Mathf.Log10(Mathf.Max(instance.MasterVolume, 0.0001f)) * 20f);
        audioMixer.SetFloat("music", Mathf.Log10(Mathf.Max(instance.MusicVolume, 0.0001f)) * 20f);
        audioMixer.SetFloat("sfx", Mathf.Log10(Mathf.Max(instance.SfxVolume, 0.0001f)) * 20f);

        _masterVolume = 0;
        _musicVolume = 0;
        _sfxVolume = 0;
    }

    public void QuitClicked()
    {
        pauseMenuPanel.SetActive(false);
        confirmQuitPanel.SetActive(true);
    }

    public void ConfirmQuitClicked()
    {
        Time.timeScale = 1f;
        HUD.Instance.FadeOut(0.5f, () => SceneManager.LoadScene("MainMenu"));
    }

    public void CancelQuitClicked()
    {
        pauseMenuPanel.SetActive(true);
        confirmQuitPanel.SetActive(false);
    }
}
