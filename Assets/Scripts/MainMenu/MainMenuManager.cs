using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TMPro;
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
        private GameObject instPanel;



        private static readonly CreditEntry[] BundledCredits =
        {
            new CreditEntry("Design", "Jesse", "Design/Production/Asset Research", "https://jessemauritz1.wixsite.com/website"),
            new CreditEntry("Art", "Cris", "Environment/Prop Artist", "https://crisrvs.artstation.com/"),
            new CreditEntry("Developer", "TJ", "Coding/Mechanics", "tj-codez.itch.io"),
            new CreditEntry("Art", "GraphicSauce", "UI Font", "https://www.1001fonts.com/sketch-chalk-font.html"),
            new CreditEntry("Art", "Stylized Core By Z", "Character", "https://assetstore.unity.com/packages/3d/characters/humanoids/stylized-player-character-free-371506"),
            new CreditEntry("Sound", "Sadiquecat", "enemy fire1", "https://freesound.org/people/Sadiquecat/sounds/784041/"),
            new CreditEntry("Art", "etman", "pickle turret", "https://sketchfab.com/3d-models/pickles-4b8c7e8412fb409285528e79b70d1891"),
            new CreditEntry("Sound", "rayray_cruze", "Pickup Sound", "https://freesound.org/people/rayray_cruze/sounds/734296/"),
            new CreditEntry("Sound", "Joao_Janz", "Player Hit Target", "https://freesound.org/people/Joao_Janz/sounds/485279/"),
            new CreditEntry("Sound", "Under7Dude", "Enemy Hit Player", "https://freesound.org/people/Under7dude/sounds/163441/"),
            new CreditEntry("Sound", "mickey13", "Enemy Die", "https://freesound.org/people/mrickey13/sounds/515620/"),
            new CreditEntry("Sound", "F.M. Audio", "Player Die", "https://freesound.org/people/F.M.Audio/sounds/695386/"),
            new CreditEntry("Sound", "Catch22Music", "Music", "https://pixabay.com/users/catch22music-43977658/"),
            new CreditEntry("Sound", "SonicSoundFX", "Ambient Noise Kitchen", "https://www.zapsplat.com/author/sonic-soundfx/?_gl=1*1hghjf2*_up*MQ..*_gs*MQ..&gclid=Cj0KCQjwlNPVBhCMARIsAPZ5Rqj3GwqvuO1nto0YiWleRVW7mOmv8yBx6zTfQAwaPSzWWIRDQPma3FMaAumMEALw_wcB"),
            new CreditEntry("Sound", "Kenneth_Cooney", "Deliver Burger", "https://freesound.org/people/Kenneth_Cooney/sounds/609335/"),
            new CreditEntry("Sound", "B_Sean", "Fight Music", "https://freesound.org/people/B_Sean/sounds/421886/"),
            new CreditEntry("Sound", "LukeUPF", "Turret Raise", "https://freesound.org/people/LukeUPF/sounds/233058/"),
            new CreditEntry("Sound", "Breviceps", "Pickle Shot", "https://freesound.org/people/Breviceps/sounds/445109/"),
            new CreditEntry("Sound", "EtherAudi", "SwitchClick", "https://freesound.org/people/EtherAudio/sounds/825502/"),
            new CreditEntry("Sound", "Airstream", "MenuMusic", "https://uppbeat.io/c/airstream"),
            new CreditEntry("Sound", "Artninja", "Explode", "https://freesound.org/people/Artninja/sounds/786111/"),
            new CreditEntry("HUD", "Stockio", "UI health", "https://www.flaticon.com/free-icons/burger"),
            new CreditEntry("HUD", "Knokapp", "Boss Health", "https://www.flaticon.com/free-icon/plate_2848763?term=diner&page=1&position=56&origin=search&related_id=2848763")
        };

        private readonly struct CreditEntry
        {
            public readonly string Section;
            public readonly string Name;
            public readonly string Contribution;
            public readonly string Link;

            public CreditEntry(string section, string name, string contribution, string link)
            {
                Section = section;
                Name = name;
                Contribution = contribution;
                Link = link;
            }
        }

        [SerializeField]
        private GameObject confirmQuitPanel;
        [SerializeField]
        private GameObject closeInstructionsPanel;

        [SerializeField]
        private GameObject splashScreen;

        [SerializeField]
        private GameObject blocker;

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

        private void Start()
        {
            if (splashScreen)
                splashScreen.SetActive(false);

            mainMenuPanel.SetActive(true);
        }

        private void PlayAudioButton()
        {
            if (audioSource != null)
            {
                print("Playing audio button sound");
                audioSource.Play();
            }
        }

        public void StartGameClicked()
        {
            SceneManager.LoadScene("GameScene");
        }

        /// <summary>Opens the difficulty settings panel and loads the saved values.</summary>
        public void DifficultyClicked()
        {
            PlayAudioButton();
            var panel = GetDifficultyPanel();
            panel.SetActive(true);
            PopulateDifficultyFields();
            blocker.SetActive(true);
        }

        /// <summary>Validates and saves the difficulty settings entered in the panel.</summary>
        public void ConfirmDifficultyClicked()
        {
            if (!TryReadDifficultyValue("PlayerHealthInput", out var playerHealth) ||
                !TryReadDifficultyValue("TopBunHealthInput", out var topBunHealth) ||
                !TryReadDifficultyValue("PattyHealthInput", out var pattyHealth) ||
                !TryReadDifficultyValue("BottomBunHealthInput", out var bottomBunHealth) ||
                !TryReadDifficultyValue("OrdersToWinInput", out var ordersToWin) ||
                !TryReadDifficultyValue("OrderTimerInput", out var orderTimer) ||
                !TryReadDifficultyValue("OrdersToLoseInput", out var ordersToLose))
            {
                GetDifficultyErrorText().text = "Health and order values must be 1-999; the timer must be 1-3600 seconds.";
                return;
            }

            DifficultyOptions.Save(
                playerHealth,
                topBunHealth,
                pattyHealth,
                bottomBunHealth,
                ordersToWin,
                orderTimer,
                ordersToLose);

            PlayAudioButton();
            GetDifficultyPanel().SetActive(false);
            blocker.SetActive(false);
        }

        /// <summary>Closes the difficulty panel without changing saved values.</summary>
        public void CancelDifficultyClicked()
        {
            PlayAudioButton();
            GetDifficultyPanel().SetActive(false);
            blocker.SetActive(false);
        }

        private void PopulateDifficultyFields()
        {
            GetDifficultyInput("PlayerHealthInput").text = DifficultyOptions.PlayerHealth.ToString(CultureInfo.InvariantCulture);
            GetDifficultyInput("TopBunHealthInput").text = DifficultyOptions.GetBossHealth(
                DifficultyOptions.TopBunBossId, DifficultyOptions.DefaultTopBunBossHealth).ToString(CultureInfo.InvariantCulture);
            GetDifficultyInput("PattyHealthInput").text = DifficultyOptions.GetBossHealth(
                DifficultyOptions.PattyBossId, DifficultyOptions.DefaultPattyBossHealth).ToString(CultureInfo.InvariantCulture);
            GetDifficultyInput("BottomBunHealthInput").text = DifficultyOptions.GetBossHealth(
                DifficultyOptions.BottomBunBossId, DifficultyOptions.DefaultBottomBunBossHealth).ToString(CultureInfo.InvariantCulture);
            GetDifficultyInput("OrdersToWinInput").text = DifficultyOptions.OrdersToWin.ToString(CultureInfo.InvariantCulture);
            GetDifficultyInput("OrderTimerInput").text = DifficultyOptions.OrderTimerSeconds.ToString(CultureInfo.InvariantCulture);
            GetDifficultyInput("OrdersToLoseInput").text = DifficultyOptions.OrdersToLose.ToString(CultureInfo.InvariantCulture);
            GetDifficultyErrorText().text = string.Empty;
        }

        private bool TryReadDifficultyValue(string inputName, out int value)
        {
            var maximum = inputName == "OrderTimerInput" ? 3600 : 999;
            return int.TryParse(
                GetDifficultyInput(inputName).text,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out value) && value > 0 && value <= maximum;
        }

        private GameObject GetDifficultyPanel()
        {
            return mainMenuPanel.transform.parent.Find("DifficultyPanel").gameObject;
        }

        private TMP_InputField GetDifficultyInput(string inputName)
        {
            return GetDifficultyPanel().transform.Find(inputName).GetComponent<TMP_InputField>();
        }

        private TMP_Text GetDifficultyErrorText()
        {
            return GetDifficultyPanel().transform.Find("DifficultyErrorText").GetComponent<TMP_Text>();
        }

        public void InstructionsClicked()
        {
            PlayAudioButton();
            blocker.SetActive(true);
            instPanel.SetActive(true);
        }

        public void CloseInstructionsClicked()
        {
            PlayAudioButton();
            blocker.SetActive(false);
            instPanel.SetActive(false);
        }

        public void SettingsClicked()
        {
            PlayAudioButton();
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
            _mouseSensitivity = Settings.Instance.MouseSensitivity;
            mouseSensitivitySlider.value = _mouseSensitivity;
        }

        public void CreditsClicked()
        {
            PlayAudioButton();
            blocker.SetActive(true);
            creditsPanel.SetActive(true);

            var creditsText = FindCreditsText(creditsPanel.transform);
            if (creditsText == null)
            {
                Debug.LogError("CreditsPanel needs a child named CreditsText with a TextMeshPro text component.");
                return;
            }

            var linkHandler = creditsText.GetComponent<CreditsLinkHandler>();
            if (linkHandler == null)
            {
                linkHandler = creditsText.gameObject.AddComponent<CreditsLinkHandler>();
            }

            var linkTargets = new List<string>();
            creditsText.text = FormatBundledCredits(linkTargets);
            linkHandler.SetLinkTargets(linkTargets);
        }



        private static TMP_Text FindCreditsText(Transform root)
        {
            var textComponents = root.GetComponentsInChildren<TMP_Text>(true);
            foreach (var textComponent in textComponents)
            {
                if (textComponent.gameObject.name == "CreditsText")
                {
                    return textComponent;
                }
            }

            return null;
        }

        private static string FormatBundledCredits(List<string> linkTargets)
        {
            var output = new StringBuilder();
            var sections = new List<string>();

            foreach (var credit in BundledCredits)
            {
                if (!sections.Contains(credit.Section))
                {
                    sections.Add(credit.Section);
                }
            }

            foreach (var section in sections)
            {
                if (output.Length > 0)
                {
                    output.AppendLine();
                }

                output.Append("<b>")
                    .Append(EscapeRichText(section))
                    .AppendLine("</b>");

                foreach (var credit in BundledCredits)
                {
                    if (!string.Equals(credit.Section, section, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    output.Append("• ")
                        .Append(EscapeRichText(credit.Name));

                    if (!string.IsNullOrWhiteSpace(credit.Contribution))
                    {
                        output.Append(" — ").Append(EscapeRichText(credit.Contribution));
                    }

                    if (TryGetWebUri(credit.Link, out var linkUri))
                    {
                        var linkIndex = linkTargets.Count;
                        linkTargets.Add(linkUri.AbsoluteUri);
                        output.Append("\n  <link=\"")
                            .Append(linkIndex.ToString(CultureInfo.InvariantCulture))
                            .Append("\"><u>")
                            .Append(EscapeRichText(GetLinkLabel(linkUri)))
                            .Append("</u></link>");
                    }

                    output.AppendLine();
                }
            }

            return output.ToString().TrimEnd();
        }

        private static string GetLinkLabel(Uri uri)
        {
            return uri.Host.StartsWith("www.", StringComparison.OrdinalIgnoreCase)
                ? uri.Host.Substring(4)
                : uri.Host;
        }

        private static bool TryGetWebUri(string value, out Uri uri)
        {
            var normalizedValue = value.Contains("://") ? value : "https://" + value;
            return Uri.TryCreate(normalizedValue, UriKind.Absolute, out uri) &&
                   (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        private static string EscapeRichText(string value)
        {
            return value.Replace("<", "＜")
                .Replace(">", "＞");
        }








        public void QuitClicked()
        {
            PlayAudioButton();
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
            PlayAudioButton();
        }

        public void CancelQuitClicked()
        {
            PlayAudioButton();
            blocker.SetActive(false);
            confirmQuitPanel.SetActive(false);
        }

        public void CloseCreditsClicked()
        {
            PlayAudioButton();
            blocker.SetActive(false);
            creditsPanel.SetActive(false);
        }

        private float _masterVolume;
        private float _musicVolume;
        private float _sfxVolume;
        private float _mouseSensitivity;

        /// <summary>
        /// Previews the selected mouse sensitivity until the settings are confirmed.
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
            blocker.SetActive(false);
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
        }

        public void CancelSettingsClicked()
        {
            PlayAudioButton();
            blocker.SetActive(false);
            settingsPanel.SetActive(false);

            var instance = Settings.Instance;
            _mouseSensitivity = instance.MouseSensitivity;
            if (mouseSensitivitySlider)
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
    }
}
