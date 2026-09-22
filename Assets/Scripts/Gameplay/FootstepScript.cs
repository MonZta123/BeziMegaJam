using MoreMountains.Feedbacks;
using UnityEngine;

namespace Gameplay
{
    public class FootstepScript : MonoBehaviour
    {
        [SerializeField]
        public MMF_Player footstepFeedback;
        
        public void PlayFootstep()
        {
            footstepFeedback.PlayFeedbacks();
        }
    }
}
