using UnityEngine;

namespace Gameplay
{
    public class FootstepScript : MonoBehaviour
    {
        [SerializeField]
        private AudioSource audioSource;
        
        public void PlayFootstep()
        {
            audioSource.Play();
        }
    }
}
