using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField]
        private GameObject mainMenuPanel;

        [SerializeField]
        private GameObject settingsPanel;

        [SerializeField]
        private GameObject creditsPanel;

        [SerializeField]
        private GameObject confirmQuitPanel;

        [SerializeField]
        private GameObject blocker;

        [SerializeField]
        private Slider masterVolumeSlider;

        [SerializeField]
        private Slider musicVolumeSlider;

        [SerializeField]
        private Slider sfxVolumeSlider;

        [SerializeField]
        private AudioMixer audioMixer;

        private void Awake()
        {
        }

        public void StartGameClicked()
        {
            SceneManager.LoadScene("GameScene");
        }

        public void SettingsClicked()
        {
            blocker.SetActive(true);
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

        public void CreditsClicked()
        {
            blocker.SetActive(true);
            creditsPanel.SetActive(true);
        }

        public void QuitClicked()
        {
            blocker.SetActive(true);
            confirmQuitPanel.SetActive(true);
        }

        public void ConfirmQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void CancelQuitClicked()
        {
            blocker.SetActive(false);
            confirmQuitPanel.SetActive(false);
        }

        public void CloseCreditsClicked()
        {
            blocker.SetActive(false);
            creditsPanel.SetActive(false);
        }

        private float _masterVolume;
        private float _musicVolume;
        private float _sfxVolume;

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
            blocker.SetActive(false);
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
            blocker.SetActive(false);
            settingsPanel.SetActive(false);

            var instance = Settings.Instance;

            audioMixer.SetFloat("master", Mathf.Log10(Mathf.Max(instance.MasterVolume, 0.0001f)) * 20f);
            audioMixer.SetFloat("music", Mathf.Log10(Mathf.Max(instance.MusicVolume, 0.0001f)) * 20f);
            audioMixer.SetFloat("sfx", Mathf.Log10(Mathf.Max(instance.SfxVolume, 0.0001f)) * 20f);

            _masterVolume = 0;
            _musicVolume = 0;
            _sfxVolume = 0;
        }
    }
}
