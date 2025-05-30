using UnityEngine;
using UnityEngine.Events;
using TMPro;

[System.Serializable] public class DeathEvent : UnityEvent { }

public class Health : MonoBehaviour
{

    [SerializeField] TMP_Text currentVida;

    [SerializeField] float maxHP = 100f;
    public float MaxHP => maxHP; // Make maxHP readable publicly
    public DeathEvent OnDeath;

    [SerializeField] private float currentHP;
    public float CurrentHP => currentHP;
    Animator anim;
    EntityMovement2D move;
    private bool isDead = false; // <-- AÑADIDO

    void Awake()
    {
        currentHP = maxHP;
        anim = GetComponent<Animator>();
        move = GetComponent<EntityMovement2D>();
    }

    public bool TakeDamage(float dmg)
    {
        if (isDead) return false; // <-- AÑADIDO: No tomar daño si ya está muerto

        /* Si estamos bloqueando, reducimos o anulamos daño */
        if (move != null && move.IsBlocking) dmg *= 0.3f;

        currentHP -= dmg;

        // Solo actualiza el texto si currentVida está asignado
        if (currentVida != null) 
        {
            currentVida.text = $"Health:{currentHP}";
        }

        // Llama a la animación de daño a través de Combat2D (mejor manejo de triggers y cooldown)
        var combat = GetComponent<Combat2D>();
        if (combat != null) combat.OnHit();
        // Si no hay Combat2D, fallback a animación directa
        else if (anim != null) anim.SetTrigger("Hit");

        if (currentHP <= 0 && !isDead) // <-- MODIFICADO: asegurar que no esté ya muerto
        {
            Die();
        }
        return currentHP <= 0;
    }

    void Die()
    {
        if (isDead) return; // <-- AÑADIDO: Salir si ya se llamó a Die()
        isDead = true; // <-- AÑADIDO: Marcar como muerto primero

        if (anim != null) anim.SetTrigger("Death"); // <-- Añadido null check por si acaso
        OnDeath?.Invoke();
        /* Desactivar colisiones y scripts de movimiento/combat */
        foreach (var c in GetComponents<MonoBehaviour>()) c.enabled = false;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        Destroy(gameObject, 2f); // se limpia en 2 s
    }
}
