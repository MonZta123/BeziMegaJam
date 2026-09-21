using UnityEngine;
using MoreMountains.Feedbacks;
using System.Security;


public class BouncingObjectr : MonoBehaviour
{
    [SerializeField]
    private BossFightStart bfs;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartLoop()
    {
        bfs.bouncingFeedback.PlayFeedbacks();
    }
}
