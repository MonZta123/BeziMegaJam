
using System.Collections.Generic;
using Gameplay.ReferenceScripts;
using MoreMountains.Feedbacks;
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
        [SerializeField]
        private AudioSource deliveryAudio;

        [SerializeField]
        private List<BossFightStart> bossFightStarts;
        
        private int _errors;

        private float _lastOrderStarted;

        private bool _orderIsActive;



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
            timeInSecondsUntilOrderCancelled = DifficultyOptions.OrderTimerSeconds;
            howManyFailedOrderUntilDead = DifficultyOptions.OrdersToLose;
            howManyOrdersToWin = DifficultyOptions.OrdersToWin;
            _ordersLeft = howManyOrdersToWin;

            InitNewOrder();
        }

        public void DeliverBurger(Burger burger)
        {
            print(burger);
            deliveryAudio.Play();
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

            bossFightStarts.ForEach(n =>
            {
                n.Reset();
            });
        }

        public bool HasLost { get; private set; }

        private void Update()
        {
            if (HasLost) return;

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
            
            if (!_orderIsActive)
            {
                _lastOrderStarted = Time.timeSinceLevelLoad;
                _orderIsActive = true;
            }

            if (Time.timeSinceLevelLoad - _lastOrderStarted > timeInSecondsUntilOrderCancelled)
            {
                _orderIsActive = false;

                _errors++;

                if (_errors >= howManyFailedOrderUntilDead)
                {
                    Dead();
                }
            }
        }

        private void Dead()
        {
            HasLost = true;
            
            Player.Instance.LockControls(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            EndScreenManager.Instance.ShowLoseScreen();
        }

        private void Win()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            EndScreenManager.Instance.ShowWinScreen(_totalTimeInSeconds);

        }

        public List<BurgerPart> GetIngredients()
        {
            return new List<BurgerPart>() { BurgerPart.TopBun, BurgerPart.BottomBun, BurgerPart.Patty };
        }
    }
}
