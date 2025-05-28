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

    /* ─── Runtime ─── */
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;

    Vector2 moveInput;
    public bool isGrounded;

    /* Bloqueo (lo usa Combat2D y la IA) */
    public bool IsBlocking { get; private set; }

    /* ------------------------ */
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        UpdateSpriteDirection();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
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

    /* ----- API que puede usar la IA ------ */
    public void SetMoveInput(Vector2 input) { moveInput = input; UpdateSpriteDirection(); }
    public void TryJump() { if (isGrounded) rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); }
    public void SetBlocking(bool state) => IsBlocking = state;
}
