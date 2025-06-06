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

    void Awake()
    {
        currentHP = maxHP;
        anim = GetComponent<Animator>();
        move = GetComponent<EntityMovement2D>();

    }

    public bool TakeDamage(float dmg)
    {
        if (currentHP <= 0) return true; // Ya está muerto, no procesar más daño

        /* Si estamos bloqueando, reducimos o anulamos daño */
        if (move != null && move.IsBlocking) dmg *= 0.3f;

        float hpBeforeDamage = currentHP; // DEBUG
        currentHP -= dmg;
        Debug.Log($"{gameObject.name} - TakeDamage: HP was {hpBeforeDamage}, damage taken = {dmg}, current HP now = {currentHP}"); // DEBUG

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

        if (currentHP <= 0) Die();
        return currentHP <= 0;
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} - Die: HP is {currentHP}. Character is dying. Calling OnDeath.Destroying in 2s."); // DEBUG
        anim.SetTrigger("Death");
        OnDeath?.Invoke();
        /* Desactivar colisiones y scripts de movimiento/combat */
        foreach (var c in GetComponents<MonoBehaviour>()) c.enabled = false;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        Destroy(gameObject, 2f); // se limpia en 2 s
    }
}
