using UnityEngine;
using UnityEngine.AI;

public class Npc : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private NavMeshAgent navMeshAgent;

    private static readonly int s_moveSpeed = Animator.StringToHash("MoveSpeed");
    private static readonly int s_isGrounded = Animator.StringToHash("IsGrounded");

    private bool _leaving;

    private void OnValidate()
    {
        if (!animator)
            animator = GetComponent<Animator>();

        if (!navMeshAgent)
            navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Awake()
    {
        if (animator)
            animator.SetBool(s_isGrounded, true);
    }

    public void MoveTo(Vector3 position)
    {
        navMeshAgent.SetDestination(position);
    }

    public void Leave(Vector3 exitPosition)
    {
        _leaving = true;
        MoveTo(exitPosition);
    }

    private bool HasArrived =>
        !navMeshAgent.pathPending &&
        navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + 0.1f;

    public void Update()
    {
        if (animator && navMeshAgent.speed > 0f)
            animator.SetFloat(s_moveSpeed, Mathf.Clamp01(navMeshAgent.velocity.magnitude / navMeshAgent.speed));

        if (_leaving && HasArrived)
            Destroy(gameObject);
    }
}
