using System.Collections.Generic;
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
        private KetchupBottleScript projectile;

        [SerializeField]
        private float moveCooldown;

        [SerializeField]
        private float aoeAttackCooldown;

        [SerializeField]
        private int aoeAttackAmount = 4;

        [SerializeField]
        private Vector3 ketchupShootRange = new(64.14f, 0f, -0.86f);

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
        }

        private float _startedWaitingTime;
        private Vector3 _targetPosition;

        private void Update()
        {
            animator.SetFloat("MoveSpeed", agent.velocity.magnitude / agent.speed);

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
                _aoeAttackTimer += Time.deltaTime;
                base._shootSound.Play();

                if (_aoeAttackTimer >= aoeAttackCooldown)
                {
                    _shootingCount--;
                    // Do Attack attack
                    var projectileScript = Instantiate(projectile, transform.position + Vector3.up * 0.5f,
                        transform.rotation);

                    if (_shootingCount != 0)
                    {
                        var targetPos = ketchupShootRange + new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));

                        transform.LookAt(targetPos);

                        projectileScript.SetTarget(targetPos);
                    }
                    else
                    {
                        var pos = Player.Instance.transform.position;
                        pos.y = transform.position.y;

                        transform.LookAt(pos);
                    }

                    _aoeAttackTimer = 0;
                    animator.SetTrigger(s_attack);
                }
            }

            if (_shootingCount == 0)
            {
                _mode = TopBunBossMode.GetPath;
                _shootingCount = aoeAttackAmount;
            }
        }
    }
}
