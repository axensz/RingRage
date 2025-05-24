using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class EntityMovement2D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    // Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // Movement state
    private Vector2 moveInput;
    private bool isDashing;
    private float dashTimeLeft;
    private float dashCooldownLeft;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Update sprite direction
        UpdateSpriteDirection();
    }

    private void FixedUpdate()
    {
        // Apply movement
        Move();
    }

    private void Move()
    {
        if (isDashing)
        {
            // Durante el dash, mantener la velocidad máxima en la dirección actual
            Vector2 dashVelocity = moveInput.normalized * dashSpeed;
            rb.linearVelocity = dashVelocity;
            
            // Reducir el tiempo de dash
            dashTimeLeft -= Time.fixedDeltaTime;
            if (dashTimeLeft <= 0)
            {
                isDashing = false;
                dashCooldownLeft = dashCooldown;
            }
        }
        else
        {
            // Movimiento normal
            Vector2 velocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
            rb.linearVelocity = velocity;

            // Reducir el cooldown del dash
            if (dashCooldownLeft > 0)
            {
                dashCooldownLeft -= Time.fixedDeltaTime;
            }
        }
    }

    private void UpdateSpriteDirection()
    {
        if (moveInput.x != 0)
        {
            spriteRenderer.flipX = moveInput.x < 0;
        }
    }

    // New Input System callbacks
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && !isDashing && dashCooldownLeft <= 0)
        {
            // Iniciar el dash
            isDashing = true;
            dashTimeLeft = dashDuration;
        }
    }
}
