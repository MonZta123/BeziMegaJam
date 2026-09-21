using System;
using System.Collections;
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
        private GameObject loseScreen;

        [SerializeField]
        private GameObject blocker;

        public static EndScreenManager Instance { get; private set; }
        
        private void Awake()
        {
            Instance = this;
        }
        
        public void ShowLoseScreen()
        {
            Time.timeScale = 0f;
            loseScreen.SetActive(true);
            blocker.SetActive(true);
        }

        public void OnRetryClicked()
        {
            StartCoroutine(FadeOut(0.5f, () =>
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("GameScene");
            }));
        }

        public void OnQuitClicked()
        {
            StartCoroutine(FadeOut(0.5f, () =>
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("MainMenu");
            }));
        }

        public IEnumerator FadeOut(float duration, Action then = null)
        {
            yield return null;

            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, 0);

            while (fade.color.a > 0)
            {
                fade.color = new Color(fade.color.r, fade.color.g, fade.color.b,
                    fade.color.a - Time.deltaTime / duration);
            }

            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, 1);

            then?.Invoke();
        }
    }
}
