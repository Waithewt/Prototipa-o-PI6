using UnityEngine;

public class PregoIrrigador : MonoBehaviour
{
    [Header("Ponta do Prego")]
    [SerializeField] private Transform ponta;

    private Rigidbody2D rb;

    private bool fincado;
    private bool puxadoPelaHabilidade;

    private Vector2 direcaoTiro;

    private Irrigador dono;

    public bool Fincado => fincado;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Ativar(
        Vector2 posicao,
        Vector2 direcao,
        float forca
    )
    {
        transform.position = posicao;

        fincado = false;
        puxadoPelaHabilidade = false;

        direcaoTiro = direcao.normalized;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;

        rb.linearVelocity =
            direcaoTiro * forca;

        rb.angularVelocity = 0f;

        AtualizarRotacao(direcaoTiro);

        gameObject.SetActive(true);
    }

    public void DefinirDono(Irrigador jogador)
    {
        dono = jogador;
    }

    void Update()
    {
        if (puxadoPelaHabilidade)
        {
            if (dono == null)
                return;

            Vector2 direcao =
                (
                    (Vector2)dono.transform.position -
                    (Vector2)transform.position
                ).normalized;

            rb.linearVelocity =
                direcao * 18f;

            if (direcao.sqrMagnitude > 0.01f)
            {
                AtualizarRotacao(direcao);
            }

            return;
        }

        if (fincado)
            return;

        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            AtualizarRotacao(
                rb.linearVelocity.normalized
            );
        }
    }

    void AtualizarRotacao(Vector2 direcao)
    {
        float angulo =
            Mathf.Atan2(
                direcao.y,
                direcao.x
            ) * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(
                0f,
                0f,
                angulo + 90f
            );
    }

    public void IniciarPuxao(Irrigador jogador)
    {
        if (jogador == null)
            return;

        dono = jogador;

        fincado = false;
        puxadoPelaHabilidade = true;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
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

     

        if (puxadoPelaHabilidade)
        {
         
            Irrigador jogador =
                col.collider.GetComponentInParent<Irrigador>();

            if (jogador != null && jogador == dono)
            {
                jogador.ColetarPrego(this);
                return;
            }

         
            NPCMorte npc =
                col.collider.GetComponentInParent<NPCMorte>();

            if (npc != null)
            {
                npc.ReceberDano();
                return;
            }

           
            return;
        }

      

        if (fincado)
        {
            Irrigador jogador =
                col.collider.GetComponentInParent<Irrigador>();

            if (jogador != null)
            {
                jogador.ColetarPrego(this);
            }

            return;
        }

        if (col.contactCount == 0)
            return;

  
        NPCMorte npcNormal =
            col.collider.GetComponentInParent<NPCMorte>();

        if (npcNormal != null)
        {
            npcNormal.ReceberDano();
            return;
        }

     

        fincado = true;

        Vector2 velocidadeAntesDoImpacto =
            rb.linearVelocity;

        if (velocidadeAntesDoImpacto.sqrMagnitude > 0.01f)
        {
            AtualizarRotacao(
                velocidadeAntesDoImpacto.normalized
            );
        }

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.bodyType = RigidbodyType2D.Kinematic;

        ContactPoint2D contato =
            col.GetContact(0);

        if (ponta != null)
        {
            Vector2 diferenca =
                (Vector2)transform.position -
                (Vector2)ponta.position;

            transform.position =
                contato.point + diferenca;
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