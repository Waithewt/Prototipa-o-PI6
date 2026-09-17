using UnityEngine;

public class ProjecNPC : MonoBehaviour
{
    [SerializeField] private float vida = 5f;

    public bool Inimigo = false;

    private Rigidbody2D rb;
    private float timer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= vida)
        {
            Destroy(gameObject);
            return;
        }

        if (rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            float angulo = Mathf.Atan2(
                rb.linearVelocity.y,
                rb.linearVelocity.x
            ) * Mathf.Rad2Deg;

            transform.rotation =
                Quaternion.Euler(
                    0f,
                    0f,
                    angulo
                );
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {    
        if (EhProjetil(col.collider))
        {
            Physics2D.IgnoreCollision(
                GetComponent<Collider2D>(),
                col.collider
            );

            return;
        }

        JogadorMorte morte =
            col.collider.GetComponentInParent<JogadorMorte>();

        if (morte != null)
        {
            if (morte.Invulneravel)
            {
                Physics2D.IgnoreCollision(
                    GetComponent<Collider2D>(),
                    col.collider
                );

                return;
            }

            morte.Morrer();

            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }

    bool EhProjetil(Collider2D col)
    {
        return
            col.GetComponentInParent<ProjecNPC>() != null ||
            col.GetComponentInParent<Projec>() != null ||
            col.GetComponentInParent<ProjetilFumaceira>() != null ||
            col.GetComponentInParent<ProjetilRebatel>() != null ||
            col.GetComponentInParent<BolaRebatida>() != null ||
            col.GetComponentInParent<PregoIrrigador>() != null;
    }
}