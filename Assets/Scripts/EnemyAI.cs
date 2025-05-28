using UnityEngine;

[RequireComponent(typeof(EntityMovement2D))]
[RequireComponent(typeof(Combat2D))]
public class EnemyAI : MonoBehaviour
{
    public Transform target;

    [Header("Distancias")]
    [SerializeField] float detectRange = 10f;
    [SerializeField] float attackRange = 2f;

    [Header("Ataque")]
    [SerializeField] float attackInterval = 1f;

    [Header("Bloqueo Aleatorio")]
    [SerializeField] float blockChance = 0.25f;
    [SerializeField] float blockDuration = 0.7f;

    /* ─── Runtime ─── */
    EntityMovement2D movement;
    Combat2D combat;
    float attackTimer;
    float blockTimer;

    void Awake()
    {
        movement = GetComponent<EntityMovement2D>();
        combat = GetComponent<Combat2D>();

        if (TryGetComponent<Health>(out var hp))
            hp.OnDeath.AddListener(() => enabled = false);
    }

    void Update()
    {
        if (target == null) return;

        float dist = Mathf.Abs(target.position.x - transform.position.x);

        /* Bloqueo aleatorio */
        if (blockTimer > 0)
        {
            blockTimer -= Time.deltaTime;
            movement.SetBlocking(true);
        }
        else
        {
            movement.SetBlocking(false);
        }

        /* ----- Lógica principal ----- */
        if (dist > detectRange)                    // Idle
        {
            movement.SetMoveInput(Vector2.zero);
            return;
        }

        if (dist > attackRange)                    // Perseguir
        {
            float dir = Mathf.Sign(target.position.x - transform.position.x);
            movement.SetMoveInput(new Vector2(dir, 0));
            attackTimer = 0;
            return;
        }

        /* Atacar */
        movement.SetMoveInput(Vector2.zero);
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            combat.OnAttack(new UnityEngine.InputSystem.InputValue());
            attackTimer = 0;
        }
    }

    /* Llamada opcional desde Health para bloquear tras daño */
    public void TryBlockOnDamage()
    {
        if (Random.value < blockChance)
            blockTimer = blockDuration;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green; Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
