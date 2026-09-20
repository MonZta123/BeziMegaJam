using TMPro;
using UnityEngine;

namespace UI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField]
        private GameObject tooltipBox;
        
        [SerializeField]
        private TextMeshProUGUI tooltipText;
        
        public void ShowTooltip(string text)
        {
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
    }
}
