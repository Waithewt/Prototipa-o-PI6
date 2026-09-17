using UnityEngine;

public class BolaRebatida : MonoBehaviour
{
    [Header("Força")]
    [SerializeField] private float perdaForcaQuicar = 0.75f;
    [SerializeField] private float perdaForcaJogador = 0.35f;
    [SerializeField] private float velocidadeMaxima = 30f;

    [Header("Parada")]
    [SerializeField] private float velocidadeMinima = 0.5f;

    private Rigidbody2D rb;
    private Rebatedor dono;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Lancar(
        Vector2 direcao,
        float velocidade,
        float perdaForca
    )
    {
        rb.linearVelocity =
            direcao.normalized * velocidade;

        perdaForcaQuicar = perdaForca;

        LimitarVelocidade();
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

    public void DefinirDono(Rebatedor novoDono)
    {
        dono = novoDono;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (EhProjetil(other))
        {
            Physics2D.IgnoreCollision(
                GetComponent<Collider2D>(),
                other
            );

            return;
        }

        if (dono == null)
            return;

        if (other.transform.root == dono.transform.root)
        {
            dono.RecuperarBola();
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        LimitarVelocidade();

        if (rb.linearVelocity.magnitude < velocidadeMinima)
        {
            rb.linearVelocity = Vector2.zero;
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

            rb.linearVelocity *= perdaForcaQuicar;

            LimitarVelocidade();

            if (rb.linearVelocity.magnitude < velocidadeMinima)
            {
                rb.linearVelocity = Vector2.zero;
            }

            return;
        }


        JogadorMorte jogador =
            col.collider.GetComponentInParent<JogadorMorte>();

        if (jogador != null)
        {
        
            if (dono != null &&
                jogador.transform.root == dono.transform.root)
            {
                dono.RecuperarBola();
                Destroy(gameObject);
                return;
            }


            if (!jogador.Invulneravel)
            {
                jogador.Morrer();
            }

            rb.linearVelocity *= perdaForcaJogador;

            LimitarVelocidade();

            if (rb.linearVelocity.magnitude < velocidadeMinima)
            {
                rb.linearVelocity = Vector2.zero;
            }

            return;
        }


        rb.linearVelocity *= perdaForcaQuicar;

        LimitarVelocidade();

        if (rb.linearVelocity.magnitude < velocidadeMinima)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void LimitarVelocidade()
    {
        if (rb.linearVelocity.magnitude > velocidadeMaxima)
        {
            rb.linearVelocity =
                rb.linearVelocity.normalized *
                velocidadeMaxima;
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