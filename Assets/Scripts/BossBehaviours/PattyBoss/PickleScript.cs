using UnityEngine;

namespace BossBehaviours.PattyBoss
{
    [RequireComponent(typeof(Rigidbody))]
    public class PickleScript : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody rb;
        private AudioSource _audioSource;

        private void OnValidate()
        {
            if (!rb)
                rb = GetComponent<Rigidbody>();
        }

        private void Awake()
        {
            _audioSource = GetComponentInChildren<AudioSource>();
            rb.linearVelocity = transform.rotation * new Vector3(0, 0, 3f);
            _audioSource.Play();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<Player>(out var player))
            {
                player.TakeDamage(1);
            }
            
            Destroy(gameObject);
        }
    }
}
