using System;
using System.Collections;
using BossBehaviours;
using BossBehaviours.BottomBunBoss;
using Gameplay;
using Gameplay.ReferenceScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UI;
using UnityEngine.Animations.Rigging;

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
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
    private float moveAcceleration = 40f;

    [SerializeField]
    private float moveDeceleration = 50f;

    [SerializeField]
    public Transform carryingAttachmentPoint;

    [SerializeField]
    private Rig rightHandRig;

    [SerializeField]
    private SphereCollider trigger;

    [Header("Audio")]
    [SerializeField]
    private AudioSource dieAudio;

    [SerializeField]
    private AudioSource getHitAudio;

    [SerializeField]
    private AudioSource hitAudio;

    [SerializeField]
    private AudioSource deliverAudio;


    public bool IsGrounded { get; private set; } = true;

    public Transform GetAttachmentPoint()
    {
        return carryingAttachmentPoint;
    }

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
        _playerInput.actions["pause"].performed += OnPause;
    }

    private void OnDestroy()
    {
        if (!_playerInput)
            return;

        _playerInput.actions["attack"].performed -= OnAttack;
        _playerInput.actions["interact"].performed -= OnInteract;
        _playerInput.actions["pause"].performed -= OnPause;
    }

    private int _health;
    private int _maxHealth;

    public void SetCheckpoint(Vector3 position)
    {
        _checkpointPosition = position;
    }

    private Vector3 _checkpointPosition;

    public void SetHealth(int value)
    {
        _health = value;
        _maxHealth = value;
        HealthSystem.Instance.SetPlayerHealth(value);
    }

    public void TakeDamage(int value)
    {
        if (OrderSystem.Instance.HasLost)
        {
            return;
        }

        _health -= value;
        _health = Math.Max(_health, 0);

        HealthSystem.Instance.TakeDamagePlayer(value);

        if (_health <= 0)
        {
            dieAudio.Play();
            // EndScreenManager.Instance.ShowLoseScreen();
            HealthSystem.Instance.SetCurrentHealthPlayer(_maxHealth);
            LockControls(true);


            HUD.Instance.FadeOut(0.5f, () =>
            {
                transform.position = _checkpointPosition;

                LockControls(false);
                BossMonoBehaviour.Instance.AddHealth(_maxHealth);
                HUD.Instance.FadeIn(0.5f);
            });

            _health = _maxHealth;
            // get cooked
        }
        else
        {
            getHitAudio.Play();
        }
    }

    private void OnPause(InputAction.CallbackContext ctx)
    {
        if (EndScreenManager.Instance.ScreenLocked)
            return;

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
    private float _timeLastAttack;
    private PauseMenuManager _pauseMenuManager;
    private Vector2 _moveInput;

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        _attack = true;
    }

    private bool _controlsLocked;

    public void LockControls(bool locked)
    {
        _controlsLocked = locked;
        if (!locked)
            return;

        _moveInput = Vector2.zero;
        StopHorizontalMovement();
    }

    private void StopHorizontalMovement()
    {
        if (!rb)
            return;

        var velocity = rb.linearVelocity;
        velocity.x = 0f;
        velocity.z = 0f;
        rb.linearVelocity = velocity;
    }

    private Burger _carryingBurger;

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (_carryingBurger)
        {
            if (_deliveryInView)
            {
                // Deliver Burger
                OrderSystem.Instance.DeliverBurger(_carryingBurger);
                deliverAudio.Play();
                _deliveryInView.Deliver();
                _deliveryInView.HideTooltip();
                _deliveryInView = null;
            }
            else
            {
                _carryingBurger.Drop();
            }

            _carryingBurger = null;
            rightHandRig.weight = 0.0f;

            trigger.enabled = false;
            trigger.enabled = true;
            return;
        }

        if (_burgerInView)
        {
            _carryingBurger = _burgerInView;
            _burgerInView?.Carry(this);
            rightHandRig.weight = 1.0f;
            _burgerInView = null;
            _bossFightStart = null;
            return;
        }

        _bossFightStart?.Trigger(this);
        _bossFightStart = null;
    }

    public void FixedUpdate()
    {
        CheckIsGrounded();

        if (_controlsLocked)
        {
            StopHorizontalMovement();
            return;
        }

        if (!_playerInput || _pauseMenuManager.PauseIsActive)
            return;

        var currentVelocity = rb.linearVelocity;
        var currentHorizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
        var targetHorizontalVelocity = new Vector3(
            _moveInput.x * moveSpeed,
            0f,
            _moveInput.y * moveSpeed);
        var acceleration = _moveInput.sqrMagnitude > 0f ? moveAcceleration : moveDeceleration;

        currentHorizontalVelocity = Vector3.MoveTowards(
            currentHorizontalVelocity,
            targetHorizontalVelocity,
            acceleration * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector3(
            currentHorizontalVelocity.x,
            currentVelocity.y,
            currentHorizontalVelocity.z);
    }

    private void Start()
    {
        _pauseMenuManager = PauseMenuManager.Instance;
        CheckIsGrounded();
        animator.SetBool(s_isGrounded, IsGrounded);
        SetHealth(DifficultyOptions.PlayerHealth);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position + Vector3.up * 1.5f, transform.forward * 1.5f, Color.red);
    }
#endif


    private readonly Collider[] _results = new Collider[50];

    public void Update()
    {
        if (!_playerInput || _pauseMenuManager.PauseIsActive)
            return;

        var move = Vector2.ClampMagnitude(_playerInput.actions["Move"].ReadValue<Vector2>(), 1);
        _moveInput = _controlsLocked ? Vector2.zero : move;

        if (!_controlsLocked)
        {
            if (_attack && !_carryingBurger)
            {
                if (_timeLastAttack + attackCooldown < Time.timeSinceLevelLoad)
                {
                    _timeLastAttack = Time.timeSinceLevelLoad;
                    animator.SetTrigger(s_attack);


                    var size = Physics.OverlapSphereNonAlloc(
                        transform.position + Vector3.up * 1.302f + transform.forward * 0.5f, 0.5f, _results,
                        attackMask);

                    for (var i = 0; i < size; i++)
                    {
                        if (_results[i].transform.gameObject.TryGetComponent<TopBunBossBehaviour>(out var behaviour))
                        {
                            hitAudio.Play();
                            behaviour.TakeDamage(1);
                        }

                        var ball = BallManager.Instance.CurrentBall;

                        if (ball && ball == _results[i].gameObject)
                        {
                            var attackDirection = ball.transform.position - transform.position;

                            attackDirection.y = 0;

                            var rb = ball.GetComponent<Rigidbody>();
                            rb.AddForce(attackDirection * 10f, ForceMode.Impulse);
                        }
                    }
                }
            }


            if (move != Vector2.zero)
                transform.LookAt(transform.position + new Vector3(move.x, 0, move.y), Vector3.up);

            if (animator)
            {
                animator.SetBool(s_isGrounded, IsGrounded);
                animator.SetFloat(s_moveSpeed, move.magnitude);
            }
        }


        _attack = false;

    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        var origin = transform.position + Vector3.up * (groundCheckRadius + 0.05f);
        Gizmos.DrawWireSphere(origin + Vector3.down * (groundCheckDistance + 0.05f), groundCheckRadius);
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
        if (other.TryGetComponent<Delivery>(out var delivery) && _carryingBurger)
        {
            _deliveryInView = delivery;
            _deliveryInView.ShowTooltip();
        }

        if (_carryingBurger)
            return;

        if (other.TryGetComponent<BossFightStart>(out var bossFightStart) && bossFightStart.isActive)
        {
            Debug.Log("INTERACTIVE ENTERED");
            _bossFightStart = bossFightStart;
            _bossFightStart.ShowTooltip();
        }

        if (other.TryGetComponent<Burger>(out var burger) && burger.GetIsFinished())
        {
            _burgerInView = burger;
            _burgerInView.ShowTooltip();
        }
    }

    private Burger _burgerInView;

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<BossFightStart>(out var bossFightStart))
        {
            Debug.Log("INTERACTIVE EXIT");
            _bossFightStart = null;
            bossFightStart.HideTooltip();
        }

        if (other.TryGetComponent<Burger>(out var burger))
        {
            Debug.Log("INTERACTIVE EXIT");
            _burgerInView = null;
            burger.HideTooltip();
        }

        if (other.TryGetComponent<Delivery>(out var delivery))
        {
            Debug.Log("INTERACTIVE EXIT");
            _deliveryInView = delivery;
            delivery.HideTooltip();
        }
    }

    private Delivery _deliveryInView;

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

    public void MoveTo(Vector3 teleportTargetPosition)
    {
        rb.MovePosition(teleportTargetPosition);
    }


}
