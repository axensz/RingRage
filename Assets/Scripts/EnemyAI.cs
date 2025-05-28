using UnityEngine;

[RequireComponent(typeof(EntityMovement2D))]
[RequireComponent(typeof(Combat2D))]
public class EnemyAI : MonoBehaviour
{
    public Transform target;

    [Header("Distancias")]
    [SerializeField] float detectRange = 10f;
    [SerializeField] float attackRange = 1.2f;

    [Header("Ataque")]
    [SerializeField] float attackInterval = 1.2f; // Cooldown entre ataques
    [SerializeField] float tellTime = 0.4f;       // Tiempo de "aviso" antes de atacar

    [Header("Bloqueo Aleatorio")]
    [SerializeField] float blockChance = 0.2f;
    [SerializeField] float blockDuration = 0.7f;

    [Header("Stun")]
    [SerializeField] float stunDuration = 0.8f;

    [Header("Dash IA")]
    [SerializeField] float dashHealthThreshold = 30f; // Vida baja para dash defensivo
    [SerializeField] float dashDistanceMin = 1.5f;   // Distancia mínima para dash
    [SerializeField] float dashCooldownAI = 2.0f;    // Cooldown entre dashes IA
    float nextAIDashTime = 0f;

    // [Header("Sonido")]
    // [SerializeField] AudioClip attackSound;
    // [SerializeField] AudioClip tellSound;
    // [SerializeField] AudioClip stunSound;
    // [SerializeField] AudioSource audioSource;
    // TODO: Agregar sonidos en el futuro

    enum State { Idle, Chase, Tell, Attack, Cooldown, Stunned, Dead }
    State currentState = State.Idle;

    EntityMovement2D movement;
    Combat2D combat;
    float stateTimer;
    float blockTimer;
    bool isDead = false;

    void Awake()
    {
        movement = GetComponent<EntityMovement2D>();
        combat = GetComponent<Combat2D>();

        if (TryGetComponent<Health>(out var hp))
        {
            hp.OnDeath.AddListener(() => { isDead = true; currentState = State.Dead; enabled = false; });
        }
    }

    void Update()
    {
        if (isDead || target == null) return;

        Vector2 toTarget = target.position - transform.position;
        float dist = toTarget.magnitude;
        float dx = toTarget.x;
        float dy = toTarget.y;

        // Dash defensivo si la vida es baja y el jugador está cerca
        var health = GetComponent<Health>();
        if (health != null && health.CurrentHP < dashHealthThreshold && dist < dashDistanceMin && Time.time >= nextAIDashTime)
        {
            int dashDir = dx > 0 ? -1 : 1; // Dash en dirección opuesta al player
            movement.DashAI(dashDir);
            nextAIDashTime = Time.time + dashCooldownAI;
        }

        // Bloqueo aleatorio
        if (blockTimer > 0)
        {
            blockTimer -= Time.deltaTime;
            movement.SetBlocking(true);
        }
        else
        {
            movement.SetBlocking(false);
        }

        switch (currentState)
        {
            case State.Idle:
                movement.SetMoveInput(Vector2.zero);
                if (dist <= detectRange)
                    currentState = State.Chase;
                break;

            case State.Chase:
                if (dist > detectRange)
                {
                    movement.SetMoveInput(Vector2.zero);
                    currentState = State.Idle;
                }
                else if (dist > attackRange)
                {
                    float dirX = Mathf.Sign(dx);
                    movement.SetMoveInput(new Vector2(dirX, 0));
                }
                else
                {
                    movement.SetMoveInput(Vector2.zero);
                    stateTimer = 0;
                    currentState = State.Tell;
                }
                break;

            case State.Tell:
                // Aquí se puede reproducir un sonido de "tell" en el futuro
                // if (audioSource && tellSound) audioSource.PlayOneShot(tellSound);
                stateTimer += Time.deltaTime;
                if (stateTimer >= tellTime)
                {
                    stateTimer = 0;
                    currentState = State.Attack;
                }
                break;

            case State.Attack:
                // Aquí se puede reproducir un sonido de ataque en el futuro
                // if (audioSource && attackSound) audioSource.PlayOneShot(attackSound);
                combat.OnAttack(new UnityEngine.InputSystem.InputValue());
                stateTimer = 0;
                currentState = State.Cooldown;
                break;

            case State.Cooldown:
                stateTimer += Time.deltaTime;
                if (stateTimer >= attackInterval)
                {
                    stateTimer = 0;
                    currentState = State.Chase;
                }
                break;

            case State.Stunned:
                // Aquí se puede reproducir un sonido de stun en el futuro
                // if (audioSource && stunSound) audioSource.PlayOneShot(stunSound);
                stateTimer += Time.deltaTime;
                if (stateTimer >= stunDuration)
                {
                    stateTimer = 0;
                    currentState = State.Chase;
                }
                break;

            case State.Dead:
                movement.SetMoveInput(Vector2.zero);
                break;
        }
    }

    // Llamada opcional desde Health para bloquear tras daño
    public void TryBlockOnDamage()
    {
        if (Random.value < blockChance)
            blockTimer = blockDuration;
    }

    // Llamada opcional para aturdir al enemigo
    public void Stun()
    {
        currentState = State.Stunned;
        stateTimer = 0;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green; Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
