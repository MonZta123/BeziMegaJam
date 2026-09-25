using System.Collections.Generic;
using System.Linq;
using BossBehaviours.PattyBoss;
using UnityEngine;

public class PattyBossBehaviour : BossMonoBehaviour
{
    private static readonly int s_isGrounded = Animator.StringToHash("IsGrounded");

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private PickleCannonScript cannon;

    [SerializeField]
    private List<Vector3> cannonPositions;

    [SerializeField]
    private List<Quaternion> cannonRotations;

    private List<Vector3> _cannonPositions;
    private List<Quaternion> _cannonRotations;

    protected override void Awake()
    {
        base.Awake();

        _cannonPositions = cannonPositions.ToList();
        _cannonRotations = cannonRotations.ToList();

        animator.SetBool(s_isGrounded, true);

        SwitchManager.Instance.Reset();
    }

    public override void TakeDamage(int amount)
    {
        if (_cannonPositions.Count > 0)
        {
            var index = Random.Range(0, _cannonPositions.Count - 1);

            var position = _cannonPositions[index];

            _cannonPositions.RemoveAt(index);

            var rotation = _cannonRotations[index];
            _cannonRotations.RemoveAt(index);

            Instantiate(cannon, position, rotation);
        }

        base.TakeDamage(amount);
    }

    private void Update()
    {
        var lookatTarget = Player.Instance.transform.position;
        lookatTarget.y = transform.position.y;
        transform.LookAt(lookatTarget);
    }
}
