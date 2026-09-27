using BossBehaviours.BottomBunBoss;
using Gameplay;
using UnityEngine;
using UnityEngine.AI;

namespace BossBehaviours
{
    public enum BottomBunBossMode
    {
        Waiting, WalkingToBall, Punching
    }

    public class BottomBunBossBehaviour : BossMonoBehaviour
    {
        private static readonly int s_isGrounded = Animator.StringToHash("IsGrounded");
        private static readonly int s_attack = Animator.StringToHash("Attack");
        private static readonly int s_moveSpeed = Animator.StringToHash("MoveSpeed");

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private Vector3 ballSpawnPoint;

        [SerializeField]
        private NavMeshAgent agent;

        [SerializeField]
        private Vector3 playerGoal;

        protected override void Awake()
        {
            base.Awake();
            BallManager.Instance.SpawnBall();

            animator.SetBool(s_isGrounded, true);
        }

        private float _lastKick;

        private void Update()
        {
            var ball = BallManager.Instance.CurrentBall;

            if (OrderSystem.Instance.HasLost)
            {
                if (ball)
                    ball.GetComponent<Rigidbody>().isKinematic = true;

                animator.SetBool(s_isGrounded, true);
                animator.SetFloat(s_moveSpeed, 0);
                agent.ResetPath();
                return;
            }

            if (!ball)
                return;

            animator.SetFloat(s_moveSpeed, agent.velocity.magnitude / agent.speed);
            var ballPosition = ball.transform.position;

            ballPosition.y = transform.position.y;

            if (Vector3.Distance(ballPosition, transform.position) < 2f && Time.time - _lastKick > 2f)
            {
                ball.GetComponent<Rigidbody>().AddForce(transform.forward * 10f, ForceMode.Impulse);
                animator.SetTrigger(s_attack);
                _lastKick = Time.time;
            }

            var vector = ballPosition - playerGoal;
            vector.y = 0;
            vector = vector.normalized * 1.5f;

            var target = new Vector3(vector.x, transform.position.y, vector.z) + ballPosition;

            if (!agent.hasPath || Vector3.Distance(target, transform.position) > 0.5f)
            {
                agent.SetDestination(target);
                transform.LookAt(ballPosition);
            }
            else if (agent.remainingDistance < 0.1f)
            {
                transform.LookAt(ballPosition);

                agent.ResetPath();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            var currentBall = BallManager.Instance.CurrentBall;

            if (!currentBall)
                return;

            var targetPos = currentBall.transform.position;

            targetPos.y = transform.position.y;

            transform.LookAt(targetPos);

            if (collision.gameObject == currentBall)
            {
                animator.SetTrigger(s_attack);
                currentBall.GetComponent<Rigidbody>().AddForce(transform.forward * 10f);
            }
        }
    }
}
