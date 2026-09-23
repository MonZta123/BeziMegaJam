using System;
using System.Collections.Generic;
using Gameplay.ReferenceScripts;
using UnityEngine;

namespace Gameplay
{
    public enum Ingredient { Bun, Cheese, Meat, Lettuce };

    public class Order
    {
        public bool Bun { get; set; }
        public bool Cheese { get; set; }
        public bool Meat { get; set; }
        public bool Lettuce { get; set; }
    }

    public class OrderSystem : MonoBehaviour
    {
        [SerializeField]
        private float timeInSecondsUntilOrderCancelled;

        [SerializeField]
        private int maxErrorsUntilDead;

        [SerializeField]
        private Burger burgerPrefab;

        private int _errors;

        private float _lastOrderStarted;

        private Order _currentOrder;

        private Order _collected;
        
        public static OrderSystem Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private Order BuildNewOrder()
        {
            return new Order() { Bun = true, Cheese = true, Meat = true, Lettuce = true };
        }

        public void DeliverBurger(Burger burger)
        {
            Destroy(burger.gameObject);
            var originalPosition = burger.GetOriginalPosition();
            Instantiate(burgerPrefab, originalPosition.Item1, originalPosition.Item2);
        }
        
        public void AddToCurrentOrder(Ingredient ingredient)
        {
        }

        private bool _hasLost;

        private void Update()
        {
            if (_hasLost) return;

            if (_currentOrder == null)
            {
                _lastOrderStarted = Time.timeSinceLevelLoad;

                _currentOrder = BuildNewOrder();
                _collected = new Order();
            }

            if (Time.timeSinceLevelLoad - _lastOrderStarted > timeInSecondsUntilOrderCancelled)
            {
                _currentOrder = null;

                _errors++;

                if (_errors >= maxErrorsUntilDead)
                {
                    // Dead
                }
            }
        }

        public List<BurgerPart> GetIngredients()
        {
            return new List<BurgerPart>() { BurgerPart.TopBun, BurgerPart.BottomBun, BurgerPart.Patty };
        }
    }
}
