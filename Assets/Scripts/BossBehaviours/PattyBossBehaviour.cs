using BossBehaviours.PattyBoss;
using UnityEngine;

public class PattyBossBehaviour : BossMonoBehaviour
{
    private static readonly int s_isGrounded = Animator.StringToHash("IsGrounded");

    [SerializeField]
    private Animator animator;
    
    public override void OnDeath()
    {
       
    }

    protected override void Awake()
    {
        base.Awake();
        
        animator.SetBool(s_isGrounded, true);

        SwitchManager.Instance.Reset();
    }

    private void Update()
    {
        var lookatTarget = Player.Instance.transform.position;
        lookatTarget.y = transform.position.y;
        transform.LookAt(lookatTarget);
    }
}
