using UnityEngine;

public class Projec : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool ativo;
    private Cowboy cowboy;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Ativar(
        Vector2 pos,
        Vector2 vel,
        Cowboy dono
    )
    {
        transform.position = pos;

        rb.linearVelocity = vel;
        rb.angularVelocity = 0f;

        cowboy = dono;
        ativo = true;

        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!ativo)
            return;

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

            Desativar();
            return;
        }

     
        Desativar();
    }

    void Desativar()
    {
        ativo = false;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        if (cowboy != null)
        {
            cowboy.DevolverParaPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
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