using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BossBehaviours.PattyBoss
{
    public class SwitchManager : MonoBehaviour
    {
        private List<SwitchScript> switches = new List<SwitchScript>();

        [SerializeField]
        private BombScript bomb;

        public static SwitchManager Instance { get; private set; }

        public void RegisterSwitch(SwitchScript switchScript)
        {
            switches.Add(switchScript);
        }
        
        private void Awake()
        {
            Instance = this;
        }

        private BombScript _currentBomb;
        
        public void Reset()
        {
            switches.ForEach(n => n.Lock(false));

            switches.ForEach(n => n.SetRandomColor());

            _currentBomb = Instantiate(bomb, transform.position, Quaternion.identity);

            _bombReady = true;
        }

        private bool _bombReady;

        private void ResetTimer()
        {
            StartCoroutine(DoReset());
        }

        private IEnumerator DoReset()
        {
            yield return new WaitForSeconds(0.5f);

            for (var i = 0; i < 5; i++)
            {
                switches.ForEach(n => n.SetRandomColor());
                yield return new WaitForSeconds(0.3f);
            }

            Reset();
        }

        public void Update()
        {
            if (!_bombReady)
                return;

            if (switches.All(n => n.IsOn))
            {
                _bombReady = false;
                _currentBomb.Drop();
                switches.ForEach(n => n.Lock(true));
                ResetTimer();
            }
        }
    }
}
