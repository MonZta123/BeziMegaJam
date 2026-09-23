using System;
using System.Collections.Generic;
using Gameplay.ReferenceScripts;
using TMPro;
using UI;
using UnityEngine;

namespace Gameplay
{
    public class OrderSystem : MonoBehaviour
    {
        [SerializeField]
        private int timeInSecondsUntilOrderCancelled = 180;

        [SerializeField]
        private int howManyFailedOrderUntilDead = 3;

        [SerializeField]
        private int howManyOrdersToWin = 5;

        [SerializeField]
        private Burger burgerPrefab;

        [SerializeField]
        private TextMeshPro timer;
        
        [SerializeField]
        private TextMeshPro ingredients;
        
        [SerializeField]
        private TextMeshPro ordersLeft;

        private int _errors;

        private float _lastOrderStarted;

        private List<BurgerPart> _currentOrder;

        private List<BurgerPart> _collected;

        private int _timeLeftInSeconds;

        private float _timer;

        private int _ordersLeft;

        private int _totalTimeInSeconds;

        private string SecondsIntoText(int seconds)
        {
            var min = seconds / 60;
            var sec = seconds % 60;

            return $"{min}m {sec}s";
        }
        
        public static OrderSystem Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            _ordersLeft = howManyOrdersToWin;

            InitNewOrder();
        }

        private List<BurgerPart> BuildNewOrder()
        {
            // Todo: Change order maybe?
            return new List<BurgerPart>() { BurgerPart.TopBun, BurgerPart.Patty, BurgerPart.BottomBun };
        }

        public void DeliverBurger(Burger burger)
        {
            Destroy(burger.gameObject);
            var originalPosition = burger.GetOriginalPosition();
            Instantiate(burgerPrefab, originalPosition.Item1, originalPosition.Item2);

            _ordersLeft--;

            if (_ordersLeft <= 0)
            {
                Win();
            }

            InitNewOrder();
        }

        private void InitNewOrder()
        {
            _timer = 0;
            _timeLeftInSeconds = timeInSecondsUntilOrderCancelled;
            timer.text = SecondsIntoText(_timeLeftInSeconds);
            ordersLeft.text = _ordersLeft.ToString();
        }

        private bool _hasLost;

        private void Update()
        {
            if (_hasLost) return;

            _timer += Time.deltaTime;

            if (_timer >= 1f)
            {
                _timer -= 1f;
                _timeLeftInSeconds--;

                timer.text = SecondsIntoText(_timeLeftInSeconds);

                _totalTimeInSeconds++;
                
                if (_timeLeftInSeconds <= 0)
                {
                    Dead();
                }
            }
            
            if (_currentOrder == null)
            {
                _lastOrderStarted = Time.timeSinceLevelLoad;

                _currentOrder = BuildNewOrder();
                _collected = new List<BurgerPart>();
            }

            if (Time.timeSinceLevelLoad - _lastOrderStarted > timeInSecondsUntilOrderCancelled)
            {
                _currentOrder = null;

                _errors++;

                if (_errors >= howManyFailedOrderUntilDead)
                {
                    Dead();
                }
            }
        }

        private void Dead()
        {
            _hasLost = true;
            
            Player.Instance.LockControls(true);
            
            // ShowUI
        }

        private void Win()
        {
            EndScreenManager.Instance.ShowWinScreen(_totalTimeInSeconds);
        }

        public List<BurgerPart> GetIngredients()
        {
            return new List<BurgerPart>() { BurgerPart.TopBun, BurgerPart.BottomBun, BurgerPart.Patty };
        }
    }
}
