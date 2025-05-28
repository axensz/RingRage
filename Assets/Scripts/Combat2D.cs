using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Combat2D : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] float attackDamage = 10f;
    [SerializeField] float attackRate = 0.5f;
    [SerializeField] Transform hitPoint;
    [SerializeField] float hitRadius = 0.5f;
    [SerializeField] LayerMask enemyMask;

    float nextAttackTime;
    Animator anim;
    EntityMovement2D movement;

    void Awake()
    {
        anim = GetComponent<Animator>();
        movement = GetComponent<EntityMovement2D>();
    }

    /* Input */
    public void OnAttack(UnityEngine.InputSystem.InputValue v)
    {
        if (Time.time < nextAttackTime || movement.IsBlocking) return;
        DoAttack();
    }

    void DoAttack()
    {
        nextAttackTime = Time.time + attackRate;
        anim.SetTrigger("Attack");

        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPoint.position, hitRadius, enemyMask);
        foreach (var h in hits)
        {
            if (h.TryGetComponent<Health>(out var hp))
            {
                bool killed = hp.TakeDamage(attackDamage);
                /* Score sólo si somos el Player */
                if (CompareTag("Player")) ScoreManager.Add(10);
            }
        }
    }

    /* Gizmo para ver el área de golpe */
    void OnDrawGizmosSelected()
    {
        if (hitPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitPoint.position, hitRadius);
    }
}
