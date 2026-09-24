using System.Collections;
using UnityEngine;

namespace BossBehaviours.PattyBoss
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(AudioSource))]
    public class BombScript : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody rb;

        [SerializeField]
        private GameObject explosion;

        [SerializeField]
        private AudioSource audioSource;

        private void OnValidate()
        {
            if (!rb)
                rb = GetComponent<Rigidbody>();
            
            if (!audioSource)
                audioSource = GetComponent<AudioSource>();
        }

        public void Drop()
        {
            StartCoroutine(DoDrop());
        }

        private IEnumerator DoDrop()
        {
            yield return null;
            
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;

            yield return new WaitForSeconds(0.5f);
            
            var go = Instantiate(explosion, transform.position, Quaternion.identity);
            
            go.transform.localScale = Vector3.one * 0.2f;
            
            BossMonoBehaviour.Instance.TakeDamage(1);
            // Play boom sound
            
            Destroy(gameObject);
        }
    }
}
