using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;
using UI;

[RequireComponent(typeof(Rigidbody))]
[SelectionBase]
public class Player : MonoBehaviour
{
    private static readonly int s_isGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int s_moveSpeed = Animator.StringToHash("MoveSpeed");
    private static readonly int s_attack = Animator.StringToHash("Attack");

    [SerializeField]
    private LayerMask attackMask;

    public static Player Instance { get; private set; }

    private Vector3 _startPosition;

    private void Awake()
    {
        Instance = this;

        _startPosition = transform.position;
    }

    [Header("Components")]
    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private Animator animator;

    [Header("Physics")]
    [SerializeField]
    private float groundCheckRadius = 0.5f;

    [SerializeField]
    private float groundCheckDistance = 0.1f;

    [SerializeField]
    private LayerMask groundLayers;

    [Header("Balancing")]
    [SerializeField]
    private float attackCooldown = 0.5f;

    [SerializeField]
    private float moveSpeed = 7.5f;

    [SerializeField]
    private float jumpForce = 10.0f;

    [SerializeField]
    public MMF_Player FootstepFeedback;

    public bool IsGrounded { get; private set; } = true;

    private PlayerInput _playerInput;

    private void OnValidate()
    {
        if (!rb)
            rb = GetComponent<Rigidbody>();
    }

    public void SetPlayerInput(PlayerInput playerInput)
    {
        _playerInput = playerInput;

        _playerInput.actions["attack"].performed += OnAttack;
        _playerInput.actions["interact"].performed += OnInteract;
        _playerInput.actions["jump"].performed += OnJumping;
        _playerInput.actions["pause"].performed += OnPause;
    }

    private void OnDestroy()
    {
        _playerInput.actions["attack"].performed -= OnAttack;
        _playerInput.actions["interact"].performed -= OnInteract;
        _playerInput.actions["jump"].performed -= OnJumping;
        _playerInput.actions["pause"].performed -= OnPause;
    }

    private int _health;

    public void SetHealth(int value)
    {
        _health = value;
        HealthSystem.Instance.SetCurrentHealthPlayer(value);
    }

    public void TakeDamage(int value)
    {
        _health -= value;
        _health = Math.Max(_health, 0);

        if (_health <= 0)
        {
            EndScreenManager.Instance.ShowLoseScreen();
            // get cooked
        }
    }

    private void OnPause(InputAction.CallbackContext ctx)
    {
        Debug.Log("ESC HIT");
        var instance = PauseMenuManager.Instance;

        if (instance.PauseIsActive)
        {
            instance.ContinueGame();
            Debug.Log("Opening");
        }
        else
            instance.ShowPauseMenu();
    }

    private bool _attack;
    private bool _jump;

    private float _debugMove;

    private float _timeLastAttack;
    private PauseMenuManager _pauseMenuManager;

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        _attack = true;
    }

    private void OnJumping(InputAction.CallbackContext ctx)
    {
        _jump = true;
    }

    private bool _controlsLocked;

    public void LockControls(bool locked)
    {
        _controlsLocked = locked;
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        _bossFightStart?.Trigger(this);
        _bossFightStart = null;
    }

    public void FixedUpdate()
    {
        CheckIsGrounded();
    }

    private void Start()
    {
        _pauseMenuManager = PauseMenuManager.Instance;
        CheckIsGrounded();
        animator.SetBool(s_isGrounded, IsGrounded);
        SetHealth(5);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position + Vector3.up * 1.5f, transform.forward * 1.5f, Color.red);
    }
#endif

    public void Update()
    {
        if (!_playerInput || _pauseMenuManager.PauseIsActive)
            return;

        var move = Vector2.ClampMagnitude(_playerInput.actions["Move"].ReadValue<Vector2>(), 1);

        if (!_controlsLocked)
        {
            if (_attack)
            {
                if (_timeLastAttack + attackCooldown < Time.timeSinceLevelLoad)
                {
                    _timeLastAttack = Time.timeSinceLevelLoad;
                    animator.SetTrigger(s_attack);
                    if (Physics.Raycast(transform.position + Vector3.up * 1.5f, transform.forward, out var hitInfo,
                            1.5f,
                            attackMask))
                    {
                        if (hitInfo.transform.gameObject.TryGetComponent<BossMonoBehaviour>(out var behaviour))
                        {
                            behaviour.TakeDamage(1);
                        }
                    }        
                }
            }

            if (_jump)
            {
                if (IsGrounded && _hasLostGround && !_jumping)
                {
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                    _jumping = true;
                }

                if (!IsGrounded && !_hasLostGround)
                {
                    _hasLostGround = true;
                }

                if (IsGrounded && _jumping && _hasLostGround)
                {
                    _jumping = false;
                }
            }

            if (move != Vector2.zero)
                transform.LookAt(transform.position + new Vector3(move.x, 0, move.y), Vector3.up);

            rb.linearVelocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.y * moveSpeed);
            
            if (animator)
            {
                animator.SetBool(s_isGrounded, IsGrounded);
                animator.SetFloat(s_moveSpeed, move.magnitude);
            }
        }
        
        _debugMove = move.magnitude;

        _attack = false;
        _jump = false;
    }

    private bool _jumping;
    private bool _hasLostGround = true;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        var origin = transform.position + Vector3.up * (groundCheckRadius + 0.05f);
        Gizmos.DrawWireSphere(origin + Vector3.down * (groundCheckDistance + 0.05f), groundCheckRadius);
    }

    public void CallFootstepFeedback()
    {
        FootstepFeedback.PlayFeedbacks();
    }

    private void CheckIsGrounded()
    {
        var origin = transform.position + Vector3.up * (groundCheckRadius + 0.05f);

        IsGrounded = Physics.SphereCast(
            origin,
            groundCheckRadius,
            Vector3.down,
            out _,
            groundCheckDistance + 0.05f,
            groundLayers,
            QueryTriggerInteraction.Ignore);
    }

    private BossFightStart _bossFightStart;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BossFightStart>(out var bossFightStart))
        {
            Debug.Log("INTERACTIVE ENTERED");
            _bossFightStart = bossFightStart;
            _bossFightStart.ShowTooltip();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<BossFightStart>(out var bossFightStart))
        {
            Debug.Log("INTERACTIVE EXIT");
            _bossFightStart = null;
            bossFightStart.HideTooltip();
        }
    }

    public void GoBackToKitchen(GameObject callee)
    {
        LockControls(true);
        HUD.Instance.FadeOut(0.5f, () => StartCoroutine(DoTeleport(callee, _startPosition)));
    }

    private IEnumerator DoTeleport(GameObject callee, Vector3 position)
    {
        rb.MovePosition(position);

        Destroy(callee);
        
        yield return new WaitForSeconds(0.5f);

        LockControls(false);
        HUD.Instance.FadeIn(0.5f);

        yield return new WaitForSeconds(0.5f);
    }
}
