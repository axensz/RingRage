using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Combat2D : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] float minDamage = 8f; // Daño mínimo
    [SerializeField] float maxDamage = 10f; // Daño máximo
    [SerializeField] float criticalChance = 0.08f; 
    [SerializeField] float criticalMultiplier = 1.5f; // Daño crítico x2
    [SerializeField] float attackRate = 1.0f;          // Tiempo entre ataques (cooldown)
    [SerializeField] Transform hitPoint;
    [SerializeField] float hitOffset = 0.7f; // Nuevo: offset configurable para el ataque
    [SerializeField] float hitRadius = 0.7f;           // Radio de golpe ligeramente mayor
    [SerializeField] LayerMask enemyMask;

    [Header("Damage Feedback")]
    [SerializeField] float hitCooldown = 0.3f;         // Tiempo mínimo entre animaciones de daño

    float nextAttackTime;
    float nextHitTime;
    Animator anim;
    EntityMovement2D movement;

    // [Header("Feedback")]
    // [SerializeField] AudioClip attackSound;
    // [SerializeField] AudioClip hitSound;
    // [SerializeField] ParticleSystem hitEffect;
    // [SerializeField] AudioSource audioSource;
    // TODO: Agregar feedback visual/sonoro en el futuro

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

        // Determinar dirección y origen del ataque (como ya tienes)
        var sr = GetComponent<SpriteRenderer>();
        float dir = (sr != null && sr.flipX) ? -1f : 1f;
        Vector2 attackOrigin = hitPoint ? (Vector2)hitPoint.position : (Vector2)transform.position + Vector2.right * dir * hitOffset;

        // Calcular daño base aleatorio
        float damage = Random.Range(minDamage, maxDamage);

        // ¿Es crítico?
        bool isCritical = Random.value < criticalChance;
        if (isCritical)
        {
            damage *= criticalMultiplier;
            // Aquí puedes agregar feedback visual/sonoro de crítico
        }

        // Detectar enemigos y aplicar daño
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackOrigin, hitRadius, enemyMask);
        foreach (var hit in hits)
        {
            var health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
                // Puedes pasar isCritical si quieres efectos especiales
            }
        }
    }

    // Método para recibir daño y animar el hit
    public void OnHit()
    {
        // Evita que la animación de daño se repita si está en cooldown
        if (Time.time < nextHitTime) return;
        nextHitTime = Time.time + hitCooldown;
        anim.SetTrigger("Hit");
        // Aquí se puede reproducir un sonido o efecto de daño en el futuro
        // if (audioSource && hitSound) audioSource.PlayOneShot(hitSound);
        // if (hitEffect) Instantiate(hitEffect, transform.position, Quaternion.identity);
    }

    /* Gizmo para ver el área de golpe */
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        var sr = GetComponent<SpriteRenderer>();
        float dir = (sr != null && sr.flipX) ? -1f : 1f;
        Vector2 attackOrigin = hitPoint ? (Vector2)hitPoint.position : (Vector2)transform.position + new Vector2(dir * hitOffset, 0);
        Gizmos.DrawWireSphere(attackOrigin, hitRadius);
    }
}
