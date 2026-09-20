using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField]
        private GameObject tooltipBox;

        [SerializeField]
        private TextMeshProUGUI tooltipText;

        [SerializeField]
        private Image fade;

        public void ShowTooltip(string text)
        {
            tooltipBox.SetActive(true);
            tooltipText.text = text;
        }

        public void HideTooltip()
        {
            tooltipBox.SetActive(false);
        }

        public static HUD Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void FadeOut(float duration, Action andThen = null)
        {
            StartCoroutine(DoFadeOut(duration, andThen));
        }

        public void FadeIn(float duration)
        {
            StartCoroutine(DoFadeIn(duration));
        }

        private IEnumerator DoFadeOut(float duration, Action andThen)
        {
            yield return null;

            float count = 0;

            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, 0);

            while (count < duration)
            {
                count += Time.deltaTime / duration;
                var color = new Color(fade.color.r, fade.color.g, fade.color.b, count / duration);
                fade.color = color;
                yield return null;
            }

            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, 1);
            andThen?.Invoke();
        }

        public IEnumerator DoFadeIn(float duration)
        {
            yield return null;

            float count = 0;

            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, 1);

            while (count < duration)
            {
                count += Time.deltaTime / duration;
                var color = new Color(fade.color.r, fade.color.g, fade.color.b, 1 - count / duration);
                fade.color = color;
                yield return null;
            }

            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, 0);
        }
    }
}
