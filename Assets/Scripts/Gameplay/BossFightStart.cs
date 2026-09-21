using System.Collections;
using UI;
using UnityEngine;
using MoreMountains.Feedbacks;


public class BossFightStart : MonoBehaviour
{
    
    private Player player;

    [SerializeField]
    private Transform teleportTarget;

    [SerializeField]
    private GameObject boss;

    [SerializeField]
    private GameObject bossPosition;

    [SerializeField]
    private string tooltipText = "Press E to start the fight!";

    [SerializeField]
    public MMF_Player bouncingFeedback;

    [Header("Floating Variables")]
    [SerializeField]
    float amplitude = 0.5f;
    [SerializeField]
    float frequency = 1f;
    [SerializeField]
    float degreePerSecond = 15.0f;
    GameObject ingreident;
    
    Vector3 startPos;
    Vector3 endPos;


    public void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        ingreident = GetComponentInChildren<IngredientRef>().gameObject;
        startPos = ingreident.transform.position;
    }

    public void Update()
    {
        if (degreePerSecond != 0)
        {
            ingreident.transform.Rotate(new Vector3(0f, degreePerSecond * Time.deltaTime, 0f), Space.World);
        }

        endPos = startPos;
        endPos.y += Mathf.Sin(Time.time * Mathf.PI * frequency) * amplitude;

        ingreident.transform.position = endPos;
        
    }

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
