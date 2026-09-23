using System.Collections;
using UI;
using UnityEngine;
using Unity.Cinemachine;
using MoreMountains.Feedbacks;


public class BossFightStart : MonoBehaviour
{
    private Player _player;

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
    [SerializeField]
    public MMF_Player activeFeedback;

    [Header("Floating Variables")]
    [SerializeField]
    private float amplitude = 0.5f;

    [SerializeField]
    private float frequency = 1f;

    [SerializeField]
    private float degreePerSecond = 15.0f;
    
    [SerializeField]
    private MMF_Player floatingFeedback;

    private GameObject _ingredient;

    private Vector3 _startPos;
    private Vector3 _endPos;

    public bool isActive = true;

    public void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        _ingredient = GetComponentInChildren<IngredientRef>().gameObject;
        _startPos = _ingredient.transform.position;
        activeFeedback.PlayFeedbacks();
        // Get the active virtual camera via the Cinemachine Brain
    }

    public void Update()
    {
        if (degreePerSecond != 0)
        {
            _ingredient.transform.Rotate(new Vector3(0f, degreePerSecond * Time.deltaTime, 0f), Space.World);
        }

        _endPos = _startPos;
        _endPos.y += Mathf.Sin(Time.time * Mathf.PI * frequency) * amplitude;

        _ingredient.transform.position = _endPos;
    }

    public void ShowTooltip()
    {
        HUD.Instance.ShowTooltip(tooltipText);
    }

    public void HideTooltip()
    {
        HUD.Instance.HideTooltip();
    }

    [Header("Audio")]
    [SerializeField]
    private AudioSource _audioSource;

    public void Trigger(Player callee)
    {
        _audioSource.Play();
        callee.LockControls(true);
        HUD.Instance.FadeOut(0.5f, () => StartCoroutine(DoTeleport(callee)));
    }

    private IEnumerator DoTeleport(Player callee)
    {
        callee.MoveTo(teleportTarget.position);
        Instantiate(boss, bossPosition.transform.position, bossPosition.transform.rotation);

        yield return new WaitForSeconds(0.5f);
        callee.LockControls(false);
        HUD.Instance.FadeIn(0.5f);

        isActive = false;
        floatingFeedback.StopFeedbacks();
    }

    public void Reset()
    {
        isActive = true;
        floatingFeedback.PlayFeedbacks();
    }
}
