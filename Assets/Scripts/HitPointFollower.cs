using UnityEngine;

public class HitPointFollower : MonoBehaviour
{
    [SerializeField] private SpriteRenderer parentSprite; // Asigna el SpriteRenderer del personaje en el inspector
    [SerializeField] private float offsetX = 0.7f;        // Distancia al personaje (ajusta según tu juego)

    void Update()
    {
        if (parentSprite == null) return;
        float dir = parentSprite.flipX ? -1f : 1f;
        transform.localPosition = new Vector3(Mathf.Abs(offsetX) * dir, transform.localPosition.y, transform.localPosition.z);
    }
} 