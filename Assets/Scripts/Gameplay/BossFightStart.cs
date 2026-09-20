using UnityEngine;

public class BossFightStart : MonoBehaviour
{
    [SerializeField]
    private Player player;

    [SerializeField]
    private Transform teleportTarget;

    [SerializeField]
    private GameObject boss;

    [SerializeField]
    private GameObject bossPosition;

    public void ShowTooltip()
    {
        
    }

    public void HideTooltip()
    {
        
    }

    public void Trigger()
    {
        
    }
    
    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         other.transform.position = teleportTarget.position;
    //         
    //         Instantiate(boss, bossPosition.transform.position, bossPosition.transform.rotation);
    //     }
    // }
}
