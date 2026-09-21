using UnityEngine;

namespace BossBehaviours
{
    public class TestBossBehaviour : BossMonoBehaviour
    {
        public override void OnDeath()
        {
            // Burger and Stuff;
            Debug.Log("Log stuff");
        }
    }
}
