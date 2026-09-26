using UnityEngine;

namespace BossBehaviours.BottomBunBoss
{
    public class Goal : MonoBehaviour
    {
        [SerializeField]
        private bool isBossGoal;

        [SerializeField]
        private GameObject splosion;

        private void OnTriggerEnter(Collider other)
        {
            if (isBossGoal)
                BossMonoBehaviour.Instance.TakeDamage(1);
            else
                Player.Instance.TakeDamage(1);

            BallManager.Instance.DestroyBall();
            
            Instantiate(splosion, other.gameObject.transform.position, Quaternion.identity);
 
            if (!BossMonoBehaviour.Instance.IsDead)
                BallManager.Instance.SpawnBall();
        }
    }
}
