using Unity.Netcode;
using UnityEngine;

public class PlayerCombatSystem : PlayerSystem
{
    [SerializeField] private int damage;
    [SerializeField] private float damageRadius;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void SubscribeEvents()
    {

    }

    protected override void UnsubscribeEvents()
    {

    }

    private void Update()
    {
        
    }

    public void OnAttack()
    {
        if (!IsOwner)
            return;

        AttackServerRPC();
    }

    [ServerRpc]
    private void AttackServerRPC()
    {
        if (!IsServer)
            return;

        var enemiesHit = Physics.OverlapSphere(attackPoint.position, damageRadius, enemyLayers);

        foreach (Collider col in enemiesHit)
        {
            var enemy = col.GetComponent<NetEnemyAI>();

            enemy.OnDamage(damage);
        }

        AttackClientRPC();
    }

    [ClientRpc]
    private void AttackClientRPC()
    {
        // Attack Animation
        // playerAnimator.SetTrigger("Attack");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(attackPoint.position, damageRadius);
    }
}
