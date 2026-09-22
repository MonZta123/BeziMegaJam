using UI;
using UnityEngine;

namespace Gameplay.ReferenceScripts
{
    public class Burger : MonoBehaviour
    {
        private Vector3 _originalPosition;
        private Quaternion _originalRotation;

        private Player _carrier;

        public void Awake()
        {
            _originalPosition = transform.position;
            _originalRotation = transform.rotation;
        }

        public void Carry(Player player)
        {
            _carrier = player;
            transform.SetParent(player.GetAttachmentPoint());
            transform.rotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;
            HideTooltip();
            player.SetModeCarrying();
        }

        public void Drop()
        {
            transform.parent = null;
            _carrier.SetModeNotCarrying();
            transform.position = _originalPosition;
            transform.rotation = _originalRotation;
            _carrier = null;
        }

        public void ShowTooltip()
        {
            HUD.Instance.ShowTooltip("Press E to pick up Burger");
        }
        
        public void HideTooltip()
        {
            HUD.Instance.HideTooltip();
        }
    }
}
