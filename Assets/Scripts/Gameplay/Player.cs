using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[SelectionBase]
public class Player : MonoBehaviour
{
    private static readonly int s_isGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int s_moveSpeed = Animator.StringToHash("MoveSpeed");
    private static readonly int s_attack = Animator.StringToHash("Attack");

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

    private void OnPause(InputAction.CallbackContext ctx)
    {
        Debug.Log("ESC HIT");
        var instance = PauseMenuManager.Instance;

        if (instance.PauseIsActive)
            instance.ContinueGame();
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

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        Debug.Log("INTERACT");
        _bossFightStart?.Trigger();
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
    }

    public void Update()
    {
        if (!_playerInput || _pauseMenuManager.PauseIsActive)
            return;

        var move = Vector2.ClampMagnitude(_playerInput.actions["Move"].ReadValue<Vector2>(), 1);

        if (_attack)
        {
            if (_timeLastAttack + attackCooldown < Time.timeSinceLevelLoad)
            {
                _timeLastAttack = Time.timeSinceLevelLoad;
                animator.SetTrigger(s_attack);
                // Execute Attack
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

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), $"Move: {_debugMove}");
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
}
