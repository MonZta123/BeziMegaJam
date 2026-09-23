using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class EndScreenManager : MonoBehaviour
    {
        [SerializeField]
        private Image fade;

        [SerializeField]
        private GameObject winScreen;

        [SerializeField]
        private GameObject loseScreen;

        [SerializeField]
        private GameObject blocker;

        [SerializeField]
        private TextMeshProUGUI scoreText;

        public bool ScreenLocked => winScreen.activeSelf || loseScreen.activeSelf;

        public void ShowWinScreen(int time)
        {
            Player.Instance.LockControls(true);
            Time.timeScale = 0f;
            winScreen.SetActive(true);
            blocker.SetActive(true);
            scoreText.text = "Your time: " + time;
        }

        public static EndScreenManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void ShowLoseScreen()
        {
            Player.Instance.LockControls(true);
            Time.timeScale = 0f;
            loseScreen.SetActive(true);
            blocker.SetActive(true);
        }

        public void OnRetryClicked()
        {
            Time.timeScale = 1f;
            loseScreen.SetActive(false);
            winScreen.SetActive(false);

            HUD.Instance.FadeOut(0.5f, () =>
            {
                SceneManager.LoadScene("GameScene");
            });
        }

        public void OnQuitClicked()
        {
            Time.timeScale = 1f;
            loseScreen.SetActive(false);
            winScreen.SetActive(false);

            HUD.Instance.FadeOut(0.5f, () =>
            {
                SceneManager.LoadScene("MainMenu");
            });
        }
    }
}
