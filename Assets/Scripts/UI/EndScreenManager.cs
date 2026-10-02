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
            LockActivePlayerControls();
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
            LockActivePlayerControls();
            Time.timeScale = 0f;
            loseScreen.SetActive(true);
            blocker.SetActive(true);
        }

        private static void LockActivePlayerControls()
        {
            if (Player.Instance)
            {
                Player.Instance.LockControls(true);
                return;
            }

            var controller = UnityEngine.Object.FindAnyObjectByType<StarterAssets.ThirdPersonController>();
            if (!controller)
                return;

            var inputState = controller.GetComponent<StarterAssets.StarterAssetsInputs>();
            if (inputState)
            {
                inputState.move = Vector2.zero;
                inputState.look = Vector2.zero;
                inputState.jump = false;
                inputState.sprint = false;
                inputState.aim = false;
            }

            var playerInput = controller.GetComponent<UnityEngine.InputSystem.PlayerInput>();
            if (playerInput)
                playerInput.DeactivateInput();

            controller.enabled = false;
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
