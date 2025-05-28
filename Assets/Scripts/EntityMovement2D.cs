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
    float nextDashTime = 0f;
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
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        UpdateSpriteDirection();
        UpdateAnimator();
        HandleDash();
    }

    void FixedUpdate()
    {
        if (!isDashing)
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
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
    }

    void HandleDash()
    {
        if (isDashing)
        {
            dashTimer += Time.deltaTime;
            if (dashTimer >= dashDuration)
            {
                EndDash();
            }
        }
    }

    /* ------------- Input callbacks ------------- */
    public void OnMove(InputValue v) => moveInput = v.Get<Vector2>();

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
        if (v.isPressed && !isDashing && Time.time >= nextDashTime && !IsBlocking)
        {
            StartDash();
        }
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = 0f;
        nextDashTime = Time.time + dashCooldown;
        anim.SetTrigger("Dash");
        if (dashTrail) dashTrail.emitting = true;
        if (col) col.enabled = false; // Permite atravesar enemigos
        // Aplica impulso en la dirección actual
        float dir = sr.flipX ? -1f : 1f;
        rb.linearVelocity = new Vector2(dir * dashForce, 0f);
    }

    void EndDash()
    {
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
        if (!isDashing && Time.time >= nextDashTime && !IsBlocking)
        {
            isDashing = true;
            dashTimer = 0f;
            nextDashTime = Time.time + dashCooldown;
            anim.SetTrigger("Dash");
            if (dashTrail) dashTrail.emitting = true;
            if (col) col.enabled = false;
            rb.linearVelocity = new Vector2(direction * dashForce, 0f);
        }
    }
}
