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
    [SerializeField] int pointsPerKill = 10; // Puntos otorgados por derrotar a un enemigo
    [Header("Tiempo para puntos extra")]
    [SerializeField] int maxBonusPoints = 20; // Máximo de puntos extra por rapidez
    [SerializeField] float bonusTimeLimit = 30f; // Si matas antes de este tiempo, recibes más puntos
    [SerializeField] float minBonusTime = 5f; // Si matas antes de este tiempo, recibes el máximo de puntos extra
    TimerUI timerUI;

    [Header("Damage Feedback")]
    [SerializeField] float hitCooldown = 0.3f;         // Tiempo mínimo entre animaciones de daño

    float nextAttackTime;
    float nextHitTime;
    Animator anim;
    EntityMovement2D movement;

    // Combo system
    int comboCount = 0;
    float lastHitTime = -10f;
    [SerializeField] float comboResetTime = 1.5f; // Tiempo máximo entre golpes para mantener el combo

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
        timerUI = FindFirstObjectByType<TimerUI>();
        Debug.Log($"{gameObject.name} - Combat2D Awake. pointsPerKill: {pointsPerKill}"); // DEBUG
    }

    /* Input */
    public void OnAttack(UnityEngine.InputSystem.InputValue v)
    {
        Debug.Log($"{gameObject.name} - OnAttack called. Time: {Time.time}, nextAttackTime: {nextAttackTime}, IsBlocking: {movement.IsBlocking}"); // DEBUG
        if (Time.time < nextAttackTime || movement.IsBlocking) return;
        DoAttack();
    }

    void DoAttack()
    {
        Debug.Log($"{gameObject.name} - DoAttack called."); // DEBUG
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
        Debug.Log($"{gameObject.name} - DoAttack: Found {hits.Length} colliders in OverlapCircleAll."); // DEBUG
        foreach (var hit in hits)
        {
            Debug.Log($"{gameObject.name} - DoAttack: Checking hit on {hit.gameObject.name}"); // DEBUG
            var health = hit.GetComponent<Health>();
            if (health != null)
            {
                Debug.Log($"{gameObject.name} - DoAttack: {hit.gameObject.name} has Health component. Current HP before damage: {health.CurrentHP}"); // DEBUG
                bool enemyDied = health.TakeDamage(damage);

                // Combo logic
                if (Time.time - lastHitTime <= comboResetTime)
                {
                    comboCount++;
                }
                else
                {
                    comboCount = 1;
                }
                lastHitTime = Time.time;

                int points = 1 + (comboCount - 1); // 1 punto base + extra por combo
                ScoreManager.Add(points);
                Debug.Log($"{gameObject.name} - DoAttack: Suma {points} puntos por combo (comboCount={comboCount}) a ScoreManager."); // DEBUG

                if (enemyDied)
                {
                    // Calcular puntos extra por rapidez
                    int bonus = 0;
                    if (timerUI != null)
                    {
                        float t = timerUI.GetElapsedTime();
                        if (t <= minBonusTime)
                            bonus = maxBonusPoints;
                        else if (t <= bonusTimeLimit)
                            bonus = Mathf.RoundToInt(Mathf.Lerp(maxBonusPoints, 0, (t-minBonusTime)/(bonusTimeLimit-minBonusTime)));
                        // Si t > bonusTimeLimit, bonus = 0
                    }
                    int totalPoints = pointsPerKill + bonus;
                    Debug.Log($"{gameObject.name} - DoAttack: Enemy {hit.gameObject.name} died. Adding {totalPoints} points (base {pointsPerKill} + bonus {bonus})."); // DEBUG
                    ScoreManager.Add(totalPoints); // Puntos extra por matar rápido
                    comboCount = 0; // Reinicia combo al matar
                }
            }
            else
            {
                Debug.Log($"{gameObject.name} - DoAttack: {hit.gameObject.name} does NOT have Health component."); // DEBUG
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
