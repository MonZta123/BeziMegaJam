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

        private int _errors;

        private float _lastOrderStarted;

        private Order _currentOrder;

        private Order _collected;

        private Order BuildNewOrder()
        {
            return new Order() { Bun = true, Cheese = true, Meat = true, Lettuce = true };
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
    }
}
