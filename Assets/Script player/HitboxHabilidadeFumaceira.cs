using UnityEngine;

public class HitboxHabilidadeFumaceira : MonoBehaviour
{
    [SerializeField] private float forcaKnockback = 15f;
    [SerializeField] private float forcaVertical = 4f;

    private Fumaceira fumaceira;

    void Awake()
    {
        fumaceira = GetComponentInParent<Fumaceira>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (fumaceira == null)
            return;

        Rigidbody2D alvoRb =
            col.GetComponentInParent<Rigidbody2D>();

        if (alvoRb == null)
            return;

        if (alvoRb.gameObject == fumaceira.gameObject)
            return;

        Vector2 direcao =
            (alvoRb.position - (Vector2)fumaceira.transform.position);

        if (direcao == Vector2.zero)
            direcao = Vector2.right * fumaceira.GetComponent<Scriptdemovimento>().UltimaDirecao;

        direcao.Normalize();

        Vector2 knockback =
            direcao * forcaKnockback;

        knockback.y += forcaVertical;

        alvoRb.linearVelocity = knockback;
    }
}