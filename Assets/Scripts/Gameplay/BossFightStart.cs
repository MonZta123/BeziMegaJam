using UI;
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

    [SerializeField]
    private string tooltipText = "Press E to start the fight!";

    public void ShowTooltip()
    {
        HUD.Instance.ShowTooltip(tooltipText);
    }

    public void HideTooltip()
    {
        HUD.Instance.HideTooltip();
    }

    public void Trigger(Player callee)
    {
        // Screen fade and stuff in Coroutine
        callee.transform.position = teleportTarget.position;

        Instantiate(boss, bossPosition.transform.position, bossPosition.transform.rotation);
    }
}
