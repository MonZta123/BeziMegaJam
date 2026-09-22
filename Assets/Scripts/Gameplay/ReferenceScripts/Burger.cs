using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

namespace Gameplay.ReferenceScripts
{
    [Serializable]
    public class BurgerPartMatch
    {
        public BurgerPart part;
        public GameObject gameObject;
    }

    public enum BurgerPart
    {
        TopBun, Patty, BottomBun
    }

    public class Burger : MonoBehaviour
    {
        private Vector3 _originalPosition;
        private Quaternion _originalRotation;

        private Player _carrier;

        [SerializeField]
        private List<BurgerPartMatch> parts;

        public static Burger CurrentBurger { get; private set; }
        
        public (Vector3, Quaternion) GetOriginalPosition() => (_originalPosition, _originalRotation);

        public bool GetIsFinished()
        {
            var ingredients = OrderSystem.Instance.GetIngredients();

            var activeGameObjects = activeParts.ToList();

            var isFinished = ingredients.All(n => activeGameObjects.Contains(n));
            
            return isFinished;
        }

        public bool GetHasMistakes()
        {
            var ingredients = OrderSystem.Instance.GetIngredients();

            var activeGameObjects = activeParts.ToList();

            return activeGameObjects.Any(m => !ingredients.Contains(m));
        }        
        
        public void Awake()
        {
            _originalPosition = transform.position;
            _originalRotation = transform.rotation;

            CurrentBurger = this;
            
            parts.ForEach(x => x.gameObject.SetActive(false));
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

        private List<BurgerPart> activeParts = new();

        public List<BurgerPart> GetList()
        {
            return activeParts;
        }

        public bool HasAnyParts()
        {
            return activeParts.Count > 0;
        }
        
        public void AddBurgerPart(BurgerPart part)
        {
            var partObj = parts.Find(x => x.part == part);
            if (partObj == null)
                return;

            partObj.gameObject.SetActive(true);
            if (!activeParts.Contains(part))
                activeParts.Add(part);
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
