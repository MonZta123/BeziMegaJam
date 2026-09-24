using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
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

        private const string CreditsCsvUrl = "https://docs.google.com/spreadsheets/d/1Rd0XkZEmcesZnthSxBUQRH42q8Mh-5LB_aMuqAepXg8/export?format=csv";
        private const int CreditsRequestTimeoutSeconds = 15;

        private Coroutine _creditsFetchRoutine;

        [SerializeField]
        private GameObject confirmQuitPanel;

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
        private AudioMixer audioMixer;

        private void Start()
        {
            if (!InputDeviceManager.Instance.PlayerInput)
            {
                splashScreen.SetActive(true);
                InputDeviceManager.Instance.playerJoined.AddListener(OnPlayerJoined);
            }
            else
            {
                mainMenuPanel.SetActive(true);
            }
        }

        private void OnPlayerJoined()
        {
            InputDeviceManager.Instance.playerJoined.RemoveListener(OnPlayerJoined);
            splashScreen.SetActive(false);
            mainMenuPanel.SetActive(true);
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

            if (_creditsFetchRoutine != null)
            {
                StopCoroutine(_creditsFetchRoutine);
            }

            _creditsFetchRoutine = StartCoroutine(LoadCreditsFromGoogleSheet());
        }

        private IEnumerator LoadCreditsFromGoogleSheet()
        {
            var creditsText = FindCreditsText(creditsPanel.transform);
            if (creditsText == null)
            {
                Debug.LogError("CreditsPanel needs a child named CreditsText with a TextMeshPro text component.");
                _creditsFetchRoutine = null;
                yield break;
            }

            var linkHandler = creditsText.GetComponent<CreditsLinkHandler>();
            if (linkHandler == null)
            {
                linkHandler = creditsText.gameObject.AddComponent<CreditsLinkHandler>();
            }

            linkHandler.SetLinkTargets(new List<string>());
            creditsText.text = "Loading credits...";

            using (var request = UnityWebRequest.Get(CreditsCsvUrl))
            {
                request.timeout = CreditsRequestTimeoutSeconds;
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning($"Unable to load credits from Google Sheets: {request.error}");
                    creditsText.text = "Credits couldn't be loaded. Check your connection and try again.";
                    _creditsFetchRoutine = null;
                    yield break;
                }

                try
                {
                    var linkTargets = new List<string>();
                    var formattedCredits = FormatCredits(ParseCsv(request.downloadHandler.text), linkTargets);
                    linkHandler.SetLinkTargets(linkTargets);
                    creditsText.text = formattedCredits;
                }
                catch (FormatException exception)
                {
                    Debug.LogWarning($"Unable to parse the credits sheet: {exception.Message}");
                    creditsText.text = "Credits couldn't be read. Check the sheet's Section, Name, Contribution, and Link columns.";
                }
            }

            _creditsFetchRoutine = null;
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

        private static string FormatCredits(List<List<string>> rows, List<string> linkTargets)
        {
            linkTargets.Clear();

            if (rows.Count == 0)
            {
                return "No credits are listed in the sheet yet.";
            }

            var header = rows[0];
            var sectionColumn = FindColumn(header, "Section");
            var nameColumn = FindColumn(header, "Name");
            var contributionColumn = FindColumn(header, "Contribution");
            var linkColumn = FindColumn(header, "Link");
            var linkTextColumn = FindColumn(header, "Link Text");

            if (sectionColumn < 0 || nameColumn < 0 || contributionColumn < 0)
            {
                throw new FormatException("The header row must include Section, Name, and Contribution.");
            }

            var output = new StringBuilder();
            var currentSection = string.Empty;
            var previousSection = string.Empty;

            for (var rowIndex = 1; rowIndex < rows.Count; rowIndex++)
            {
                var row = rows[rowIndex];
                var name = GetCell(row, nameColumn).Trim();
                if (name.Length == 0)
                {
                    continue;
                }

                var section = GetCell(row, sectionColumn).Trim();
                if (section.Length == 0)
                {
                    section = previousSection.Length > 0 ? previousSection : "Credits";
                }

                if (!string.Equals(section, currentSection, StringComparison.Ordinal))
                {
                    if (output.Length > 0)
                    {
                        output.AppendLine();
                    }

                    output.Append("<b>")
                        .Append(EscapeRichText(section))
                        .AppendLine("</b>");
                    currentSection = section;
                }

                previousSection = section;
                output.Append("• ")
                    .Append(EscapeRichText(name));

                var contribution = GetCell(row, contributionColumn).Trim();
                if (contribution.Length > 0)
                {
                    output.Append(" — ").Append(EscapeRichText(contribution));
                }

                var link = linkColumn >= 0 ? GetCell(row, linkColumn).Trim() : string.Empty;
                if (TryGetWebUri(link, out var linkUri))
                {
                    var linkLabel = linkTextColumn >= 0 ? GetCell(row, linkTextColumn).Trim() : string.Empty;
                    if (linkLabel.Length == 0)
                    {
                        linkLabel = linkUri.Host.StartsWith("www.", StringComparison.OrdinalIgnoreCase)
                            ? linkUri.Host.Substring(4)
                            : linkUri.Host;
                    }

                    var linkIndex = linkTargets.Count;
                    linkTargets.Add(linkUri.AbsoluteUri);
                    output.Append("\n  <link=\"")
                        .Append(linkIndex.ToString(CultureInfo.InvariantCulture))
                        .Append("\"><u>")
                        .Append(EscapeRichText(linkLabel))
                        .Append("</u></link>");
                }

                output.AppendLine();
            }

            return output.Length > 0 ? output.ToString().TrimEnd() : "No credits are listed in the sheet yet.";
        }

        private static bool TryGetWebUri(string value, out Uri uri)
        {
            return Uri.TryCreate(value, UriKind.Absolute, out uri) &&
                   (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        private static int FindColumn(List<string> header, string columnName)
        {
            for (var index = 0; index < header.Count; index++)
            {
                var value = header[index].Trim().TrimStart('\uFEFF');
                if (string.Equals(value, columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return index;
                }
            }

            return -1;
        }

        private static string GetCell(List<string> row, int columnIndex)
        {
            return columnIndex >= 0 && columnIndex < row.Count ? row[columnIndex] : string.Empty;
        }

        private static string EscapeRichText(string value)
        {
            return value.Replace("<", "＜")
                .Replace(">", "＞");
        }

        private static List<List<string>> ParseCsv(string csv)
        {
            var rows = new List<List<string>>();
            var row = new List<string>();
            var field = new StringBuilder();
            var insideQuotes = false;

            for (var index = 0; index < csv.Length; index++)
            {
                var character = csv[index];

                if (insideQuotes)
                {
                    if (character == '"' && index + 1 < csv.Length && csv[index + 1] == '"')
                    {
                        field.Append('"');
                        index++;
                    }
                    else if (character == '"')
                    {
                        insideQuotes = false;
                    }
                    else
                    {
                        field.Append(character);
                    }

                    continue;
                }

                if (character == '"' && field.Length == 0)
                {
                    insideQuotes = true;
                }
                else if (character == ',')
                {
                    row.Add(field.ToString());
                    field.Clear();
                }
                else if (character == '\r' || character == '\n')
                {
                    row.Add(field.ToString());
                    field.Clear();
                    if (row.Exists(value => value.Length > 0))
                    {
                        rows.Add(row);
                    }
                    row = new List<string>();
                    if (character == '\r' && index + 1 < csv.Length && csv[index + 1] == '\n')
                    {
                        index++;
                    }
                }
                else
                {
                    field.Append(character);
                }
            }

            if (insideQuotes)
            {
                throw new FormatException("A quoted CSV value wasn't closed.");
            }

            if (field.Length > 0 || row.Count > 0)
            {
                row.Add(field.ToString());
                rows.Add(row);
            }

            return rows;
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
