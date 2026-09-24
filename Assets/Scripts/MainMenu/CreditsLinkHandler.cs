using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MainMenu
{
    public class CreditsLinkHandler : MonoBehaviour, IPointerClickHandler
    {
        private TMP_Text _creditsText;
        private List<string> _linkTargets = new List<string>();

        private void Awake()
        {
            _creditsText = GetComponent<TMP_Text>();
        }

        /// <summary>
        /// Sets the external URL targets used by the numeric link IDs in the credits text.
        /// </summary>
        /// <param name="linkTargets">URLs in the same order as their TextMeshPro link IDs.</param>
        public void SetLinkTargets(List<string> linkTargets)
        {
            _linkTargets = linkTargets != null ? new List<string>(linkTargets) : new List<string>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_creditsText == null || eventData == null)
            {
                return;
            }

            var linkIndex = TMP_TextUtilities.FindIntersectingLink(_creditsText, eventData.position, eventData.pressEventCamera);
            if (linkIndex < 0)
            {
                return;
            }

            var linkId = _creditsText.textInfo.linkInfo[linkIndex].GetLinkID();
            if (!int.TryParse(linkId, out var targetIndex) || targetIndex < 0 || targetIndex >= _linkTargets.Count)
            {
                return;
            }

            if (Uri.TryCreate(_linkTargets[targetIndex], UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                Application.OpenURL(uri.AbsoluteUri);
            }
        }
    }
}
