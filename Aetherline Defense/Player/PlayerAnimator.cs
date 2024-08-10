using UnityEngine;

public class PlayerAnimator : PlayerSystem
{
    protected Animator animator;
    protected ClientNetworkAnimator netAnimator;

    protected override void Awake()
    {
        player = transform.root.GetComponent<PlayerController>();
        animator = GetComponentInChildren<Animator>();
        netAnimator = GetComponentInChildren<ClientNetworkAnimator>();
    }

    protected override void SubscribeEvents()
    {
        player.Events.SetAnimatonBool += SetBool;
        player.Events.SetAnimatonFloat += SetFloat;
        player.Events.SetAnimatonTrigger += SetTrigger;
    }

    protected override void UnsubscribeEvents()
    {
        player.Events.SetAnimatonBool -= SetBool;
        player.Events.SetAnimatonFloat -= SetFloat;
        player.Events.SetAnimatonTrigger -= SetTrigger;
    }

    public void SetFloat(string name, float value)
    {
        animator.SetFloat("moveSpeed", value);
    }

    public void SetBool(string name, bool value)
    {
        animator.SetBool(name, value);
    }

    public void SetTrigger(string name)
    {
        netAnimator.SetTrigger(name);
    }
}
