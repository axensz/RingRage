using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Combat2D : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] float attackDamage = 25f;         // Daño base recomendado
    [SerializeField] float attackRate = 1.0f;          // Tiempo entre ataques (cooldown)
    [SerializeField] Transform hitPoint;
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

        // Aquí se puede reproducir un sonido de ataque en el futuro
        // if (audioSource && attackSound) audioSource.PlayOneShot(attackSound);

        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPoint.position, hitRadius, enemyMask);
        foreach (var h in hits)
        {
            if (h.TryGetComponent<Health>(out var hp))
            {
                bool killed = hp.TakeDamage(attackDamage);
                // Aquí se puede reproducir un sonido o efecto de impacto en el futuro
                // if (audioSource && hitSound) audioSource.PlayOneShot(hitSound);
                // if (hitEffect) Instantiate(hitEffect, h.transform.position, Quaternion.identity);
                /* Score sólo si somos el Player */
                if (CompareTag("Player")) ScoreManager.Add(10);
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
        if (hitPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitPoint.position, hitRadius);
    }
}
