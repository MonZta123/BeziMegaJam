using UnityEngine;

namespace BossBehaviours.PattyBoss
{
    public class PickleCannonScript : MonoBehaviour
    {
        [SerializeField]
        private PickleScript pickle;

        private void Awake()
        {
            transform.position = new Vector3(transform.position.x, -1.5f, transform.position.z);
        }

        [SerializeField]
        private float shootCooldown = 0.5f;
        
        private bool _startShooting;

        private float _lastShoot;

        private void Update()
        {
            if (!_startShooting)
            {
                transform.position += new Vector3(0, 2.5f * Time.deltaTime, 0);

                if (transform.position.y >= 0.0f)
                {
                    transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
                    _startShooting = true;
                }
            }

            if (_startShooting && _lastShoot + shootCooldown < Time.time)
            {
                var go = Instantiate(pickle, transform.position, transform.rotation);
                
                go.transform.Rotate(Vector3.right, 90f);
                _lastShoot = Time.time;
            }

            if (BossMonoBehaviour.Instance.IsDead)
            {
                Destroy(gameObject);
            }
        }
    }
}
