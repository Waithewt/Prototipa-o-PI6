using UnityEngine;

public class ProjetilFumaceira : MonoBehaviour
{
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Ativar(
        Vector2 posicao,
        Vector2 direcao,
        float velocidade
    )
    {
        transform.position = posicao;

        rb.linearVelocity =
            direcao.normalized * velocidade;

        rb.angularVelocity = 0f;

        gameObject.SetActive(true);
    }

    void Update()
    {
        if (rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            float angulo =
                Mathf.Atan2(
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

        NPCMorte npc =
            col.collider.GetComponentInParent<NPCMorte>();

        if (npc != null)
        {
            npc.ReceberDano();

            gameObject.SetActive(false);
            return;
        }

       
        gameObject.SetActive(false);
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