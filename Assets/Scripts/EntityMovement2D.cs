using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class EntityMovement2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundRadius = 0.2f;
    [SerializeField] LayerMask groundLayer;

    [Header("Dash")]
    [SerializeField] float dashForce = 18f;
    [SerializeField] float dashDuration = 0.18f;
    [SerializeField] float dashCooldown = 0.7f;
    [SerializeField] TrailRenderer dashTrail;

    [Header("Stamina")]
    public float maxStamina = 100f; // Para que StaminaBar.cs pueda leerlo
    [HideInInspector] public float currentStamina; // Para que StaminaBar.cs pueda leerlo, oculto del inspector

    [Header("Dash Walls")]
    [SerializeField] LayerMask wallLayer; // Asigna la capa de muros en el inspector

    /* ─── Runtime ─── */
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;

    Vector2 moveInput;
    public bool isGrounded;

    /* Bloqueo (lo usa Combat2D y la IA) */
    public bool IsBlocking { get; private set; }

    bool isDashing = false;
    float dashTimer = 0f;
    float nextDashTime = 0f; // Este es el tiempo cuando el dash estará disponible de nuevo (fin del cooldown)
    Collider2D col;

    public SpriteRenderer SpriteRenderer { get; private set; }

    /* ------------------------ */
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        SpriteRenderer = sr;
        if (dashTrail) dashTrail.emitting = false;
        currentStamina = maxStamina; // Iniciar con stamina llena
        // Debug.Log($\"{gameObject.name} - EntityMovement2D Awake: moveSpeed = {moveSpeed}, Rigidbody2D is {(rb != null ? \"found\" : \"NOT FOUND\")}\"); // DEBUG
        // if (rb != null) Debug.Log($\"{gameObject.name} - EntityMovement2D Awake: Rigidbody BodyType = {rb.bodyType}\"); // DEBUG
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        UpdateSpriteDirection();
        UpdateAnimator();
        HandleDash();
        RegenerateStamina(); // Añadir regeneración de stamina
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
            // Debug.Log($\"{gameObject.name} - EntityMovement2D FixedUpdate: moveInput.x = {moveInput.x}, moveSpeed = {moveSpeed}, Calculated X Velocity = {moveInput.x * moveSpeed}, Current Y Velocity = {rb.linearVelocity.y}\"); // DEBUG
        }
        // else // DEBUG
        // { // DEBUG
            // Debug.Log($\"{gameObject.name} - EntityMovement2D FixedUpdate: Currently Dashing, X Velocity = {rb.linearVelocity.x}\"); // DEBUG
        // } // DEBUG
    }

    /* Helpers */
    void UpdateSpriteDirection()
    {
        if (moveInput.x != 0)
            sr.flipX = moveInput.x < 0;
    }

    void UpdateAnimator()
    {
        anim.SetFloat("Speed", Mathf.Abs(moveInput.x));
        anim.SetBool("Grounded", isGrounded);
        anim.SetBool("Blocking", IsBlocking);
        anim.SetFloat("YVelocity", rb.linearVelocity.y);
        // Podrías añadir un parámetro al animator para la stamina si quieres animaciones basadas en ella
        // anim.SetFloat("Stamina", currentStamina / maxStamina);
    }

    void HandleDash()
    {
        if (isDashing)
        {
            dashTimer += Time.deltaTime;
            // Debug.Log($\"{gameObject.name} - HandleDash: isDashing = true, dashTimer = {dashTimer}\"); // DEBUG - Puede ser muy verboso
            if (dashTimer >= dashDuration)
            {
                EndDash();
            }
        }
    }

    /* ------------- Input callbacks ------------- */
    public void OnMove(InputValue v)
    {
        moveInput = v.Get<Vector2>();
        // Debug.Log($\"{gameObject.name} - EntityMovement2D OnMove: moveInput = {moveInput}\"); // DEBUG
    }

    public void OnJump(InputValue v)
    {
        if (v.isPressed && isGrounded && !IsBlocking)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            anim.SetTrigger("Jump");
        }
    }

    public void OnBlock(InputValue v) => IsBlocking = v.isPressed;

    public void OnDash(InputValue v)
    {
        // Debug.Log($\"{gameObject.name} - OnDash called. isPressed: {v.isPressed}, isDashing: {isDashing}, Time.time: {Time.time}, nextDashTime: {nextDashTime}, IsBlocking: {IsBlocking}, currentStamina: {currentStamina}\"); // DEBUG
        // Ahora también verificamos si tenemos suficiente stamina (prácticamente llena)
        // y si el cooldown ha pasado (Time.time >= nextDashTime)
        if (v.isPressed && !isDashing && Time.time >= nextDashTime && !IsBlocking && currentStamina >= maxStamina * 0.99f) // Usamos 99% para evitar problemas de flotantes
        {
            StartDash();
        }
    }

    void StartDash()
    {
        // Detectar si hay muro en la dirección del dash
        float dir = sr.flipX ? -1f : 1f;
        Vector2 dashOrigin = transform.position;
        float dashDistance = dashForce * dashDuration * 1.1f; // Ajusta el factor si es necesario
        RaycastHit2D hit = Physics2D.Raycast(dashOrigin, Vector2.right * dir, dashDistance, wallLayer);
        if (hit.collider != null)
        {
            // Hay un muro, ajusta la distancia máxima del dash
            float allowedDistance = hit.distance - 0.1f; // Deja un pequeño margen
            rb.linearVelocity = new Vector2(dir * (allowedDistance / dashDuration), 0f);
        }
        else
        {
            rb.linearVelocity = new Vector2(dir * dashForce, 0f);
        }
        isDashing = true;
        dashTimer = 0f;
        nextDashTime = Time.time + dashCooldown;
        currentStamina = 0f;
        anim.SetTrigger("Dash");
        if (dashTrail) dashTrail.emitting = true;
        if (col) col.enabled = false; // Permite atravesar enemigos
        // Debug.Log($"{gameObject.name} - StartDash: Set velocity to ({rb.linearVelocity.x}, 0). isDashing = {isDashing}");
    }

    void EndDash()
    {
        // Debug.Log($\"{gameObject.name} - EndDash called.\"); // DEBUG
        isDashing = false;
        if (dashTrail) dashTrail.emitting = false;
        if (col) col.enabled = true;
        // Opcional: detener el impulso horizontal al terminar el dash
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    /* ----- API que puede usar la IA ------ */
    public void SetMoveInput(Vector2 input) { moveInput = input; UpdateSpriteDirection(); }
    public void TryJump() { if (isGrounded) rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); }
    public void SetBlocking(bool state) => IsBlocking = state;

    // Método público para dash de IA
    public void DashAI(int direction)
    {
        // La IA también necesita stamina y respetar el cooldown
        if (!isDashing && Time.time >= nextDashTime && !IsBlocking && currentStamina >= maxStamina * 0.99f)
        {
            isDashing = true;
            dashTimer = 0f;
            nextDashTime = Time.time + dashCooldown; // Marcar cuándo termina el cooldown
            currentStamina = 0f; // Consumir toda la stamina

            anim.SetTrigger("Dash");
            if (dashTrail) dashTrail.emitting = true;
            if (col) col.enabled = false;
            rb.linearVelocity = new Vector2(direction * dashForce, 0f);
        }
    }

    void RegenerateStamina()
    {
        if (currentStamina < maxStamina)
        {
            // Si el dash está en cooldown (nextDashTime es un tiempo futuro)
            if (Time.time < nextDashTime)
            {
                // Calculamos cuánto tiempo ha pasado desde que comenzó el cooldown del dash actual
                float timeSinceDashCooldownStarted = Time.time - (nextDashTime - dashCooldown);
                timeSinceDashCooldownStarted = Mathf.Max(0, timeSinceDashCooldownStarted); // Asegurar que no sea negativo

                if (dashCooldown > 0)
                {
                    // Interpolar la stamina desde 0 hasta maxStamina durante la duración del cooldown
                    currentStamina = (timeSinceDashCooldownStarted / dashCooldown) * maxStamina;
                }
                else // Si el cooldown es 0 (o negativo, aunque no debería), la stamina se llena instantáneamente
                {
                    currentStamina = maxStamina;
                }
                currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); // Asegurar que no exceda los límites
            }
            else // El cooldown ha terminado
            {
                currentStamina = maxStamina; // Rellenar la stamina completamente
            }
        }
    }
}
