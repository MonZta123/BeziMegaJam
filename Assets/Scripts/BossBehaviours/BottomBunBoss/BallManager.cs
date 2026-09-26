using UnityEngine;

namespace BossBehaviours.BottomBunBoss
{
    public class BallManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject ballPrefab;
        
        public static BallManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
        
        public GameObject CurrentBall { get; private set; }
        
        public void SpawnBall()
        {
            CurrentBall = Instantiate(ballPrefab, transform.position, Quaternion.identity);
        }

        public void DestroyBall()
        {
            Destroy(CurrentBall.gameObject);
            CurrentBall = null;
        }
    }
}
