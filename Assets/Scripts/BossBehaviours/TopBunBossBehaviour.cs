using System.Collections.Generic;
using System.Reflection.Metadata;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace BossBehaviours
{
    public enum TopBunBossMode
    {
        GetPath, Moving, Waiting, Shooting
    }

    public class TopBunBossBehaviour : BossMonoBehaviour
    {
        private static readonly int s_isGrounded = Animator.StringToHash("IsGrounded");
        private static readonly int s_attack = Animator.StringToHash("Attack");

        public override void OnDeath()
        {
        }

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private GameObject spike;

        [SerializeField]
        private GameObject projectile;

        [SerializeField]
        private float moveCooldown;

        [SerializeField]
        private float aoeAttackCooldown;

        [SerializeField]
        private int aoeAttackAmount = 4;

        private float _moveTimer;

        private float _aoeAttackTimer;

        [SerializeField]
        private NavMeshAgent agent;

        [SerializeField]
        private List<Vector3> movePositions = new();

        private TopBunBossMode _mode = TopBunBossMode.GetPath;

        private int _shootingCount;

        protected override void Awake()
        {
            _shootingCount = aoeAttackAmount;
            base.Awake();

            animator.SetBool(s_isGrounded, true);

            _previousPosition = transform.position;
        }

        private Vector3 _previousPosition;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Handles.Label(transform.position, $"Attack Count: {_shootingCount}");
            Handles.Label(transform.position + Vector3.up * 0.5f, $"Mode: {_mode}");
        }
#endif

        private float _startedWaitingTime;
        private Vector3 _targetPosition;

        private void Update()
        {
            animator.SetFloat("MoveSpeed", Vector3.Distance(_previousPosition, transform.position));

            if (_mode == TopBunBossMode.GetPath)
            {
                _targetPosition = movePositions[Random.Range(0, movePositions.Count)];
                agent.SetDestination(_targetPosition);
                _mode = TopBunBossMode.Moving;
            }

            if (_mode == TopBunBossMode.Moving && Vector3.Distance(_targetPosition, transform.position) <= 0.1f)
            {
                _mode = TopBunBossMode.Waiting;
                var nextPos = _targetPosition;
                nextPos.y = transform.position.y;
                transform.LookAt(nextPos);
                _startedWaitingTime = Time.time;
            }

            if (_mode == TopBunBossMode.Waiting)
            {
                if (_startedWaitingTime + 2f < Time.time)
                {
                    _mode = TopBunBossMode.Shooting;
                }
                else
                {
                    var pos = Player.Instance.transform.position;
                    pos.y = transform.position.y;
                    transform.LookAt(pos);
                }
            }

            if (_mode == TopBunBossMode.Shooting)
            {
                var pos = Player.Instance.transform.position;
                pos.y = transform.position.y;

                transform.LookAt(pos);
                _aoeAttackTimer += Time.deltaTime;

                if (_aoeAttackTimer >= aoeAttackCooldown)
                {
                    _shootingCount--;
                    // Do Attack attack
                    Instantiate(projectile, transform.position + Vector3.up * 0.5f, transform.rotation);
                    _aoeAttackTimer = 0;
                    animator.SetTrigger(s_attack);
                }
            }

            if (_shootingCount == 0)
            {
                _mode = TopBunBossMode.GetPath;
                _shootingCount = aoeAttackAmount;
            }

            _previousPosition = transform.position;
        }
    }
}
