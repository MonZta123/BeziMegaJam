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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = teleportTarget.position;
            
            Instantiate(boss, bossPosition.transform.position, bossPosition.transform.rotation);
        }
    }
}
