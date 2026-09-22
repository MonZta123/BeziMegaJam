using UI;
using UnityEngine;

public class Delivery : MonoBehaviour
{
    public void ShowTooltip()
    {
        HUD.Instance.ShowTooltip("Press E to deliver Burger");
    }

    public void HideTooltip()
    {
        HUD.Instance.HideTooltip();
    }
}
