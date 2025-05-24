using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
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
    private Animator animator;

    // Movement state
    private Vector2 moveInput;
    private bool isDashing;
    private float dashTimeLeft;
    private float dashCooldownLeft;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
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
            rb.linearVelocity = new Vector2(moveInput.x * dashSpeed, rb.linearVelocity.y);
            
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
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

            // Reducir el cooldown del dash
            if (dashCooldownLeft > 0)
            {
                dashCooldownLeft -= Time.fixedDeltaTime;
            }
        }

        // Actualizar la animación basada en el movimiento horizontal
        float horizontalValue = Mathf.Abs(moveInput.x);
        animator.SetFloat("Horizontal", horizontalValue);
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
        
        // Voltear el sprite basado en la dirección del movimiento
        if (moveInput.x != 0)
        {
            spriteRenderer.flipX = moveInput.x < 0;
        }
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
