using UnityEngine;

namespace MainMenu
{
    public class UIManager : MonoBehaviour
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

        private void Awake()
        {
        }

        public void StartGameClicked()
        {
            // SceneChange
        }

        public void SettingsClicked()
        {
            blocker.SetActive(true);
            settingsPanel.SetActive(true);
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

        public void ConfirmSettingsClicked()
        {
            blocker.SetActive(false);
            settingsPanel.SetActive(false);
        }

        public void CancelSettingsClicked()
        {
            blocker.SetActive(false);
            settingsPanel.SetActive(false);
        }
    }
}
