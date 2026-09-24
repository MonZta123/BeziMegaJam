using UnityEngine;

namespace Gameplay
{
    public class FootstepScript : MonoBehaviour
    {
        [SerializeField]
        private AudioSource audioSource;
        private Player player;

        private void Start()
        {
            player = GetComponentInParent<Player>();
        }
        public void PlayFootstep()
        {
            if(player != null)
                audioSource.Play();
        }
    }
}
