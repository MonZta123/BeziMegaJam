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

        [SerializeField]
        private List<BurgerPartMatch> parts;

        [SerializeField]
        public bool debugFinishedBurger;

        public static Burger CurrentBurger { get; private set; }

        private readonly List<BurgerPart> activeParts = new();

        public (Vector3, Quaternion) GetOriginalPosition() => (_originalPosition, _originalRotation);

        public bool GetIsFinished()
        {
            var ingredients = OrderSystem.Instance.GetIngredients();
            var isFinished = ingredients.All(ingredient => activeParts.Contains(ingredient));

            return debugFinishedBurger || isFinished;
        }

        private void Awake()
        {
            _originalPosition = transform.position;
            _originalRotation = transform.rotation;
            CurrentBurger = this;

            parts.ForEach(part => part.gameObject.SetActive(false));
        }

        public void Carry(Player player)
        {
            transform.SetParent(player.GetAttachmentPoint());
            transform.rotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;
            HideTooltip();
        }

        public void Drop()
        {
            transform.parent = null;
            transform.position = _originalPosition;
            transform.rotation = _originalRotation;
        }

        public void AddBurgerPart(BurgerPart part)
        {
            var partMatch = parts.Find(candidate => candidate.part == part);
            if (partMatch == null || !partMatch.gameObject)
                return;

            partMatch.gameObject.SetActive(true);
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
