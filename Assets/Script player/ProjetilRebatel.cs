using UnityEngine;

public class ProjetilRebatel : MonoBehaviour
{
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Rebater(
        Vector2 direcaoRebatida,
        float forcaMinima
    )
    {
        Vector2 velocidade = rb.linearVelocity;

        if (velocidade.magnitude < forcaMinima)
        {
            rb.linearVelocity =
                direcaoRebatida.normalized *
                forcaMinima;

            return;
        }

        rb.linearVelocity = -velocidade;
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
            return;
        }
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