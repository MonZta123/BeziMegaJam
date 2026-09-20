using System.Collections;
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
        callee.LockControls(true);
        HUD.Instance.FadeOut(0.5f, () => StartCoroutine(DoTeleport(callee)));
    }

    private IEnumerator DoTeleport(Player callee)
    {
        player.GetComponent<Rigidbody>().MovePosition(teleportTarget.position);
        Instantiate(boss, bossPosition.transform.position, bossPosition.transform.rotation);

        yield return new WaitForSeconds(0.5f);
        callee.LockControls(true);
        HUD.Instance.FadeIn(0.5f);
    }
}
