using UnityEngine;
using UnityEngine.InputSystem;

public class Fumaceira : MonoBehaviour
{
    [Header("Munição")]
    [SerializeField] private int municaoMaxima = 4;
    [SerializeField] private float tempoRecargaMunicao = 1.5f;

    [Header("Tiro")]
    [SerializeField] private GameObject projetilPrefab;
    [SerializeField] private float distanciaSpawn = 0.6f;
    [SerializeField] private float velocidadeProjetil = 16f;

    [Header("Habilidade")]
    [SerializeField] private float forcaPuloHabilidade = 7f;
    [SerializeField] private float forcaHorizontalHabilidade = 5f;
    [SerializeField] private int cargasMaximas = 2;

    [Header("Hitbox da Habilidade")]
    [SerializeField] private GameObject hitboxHabilidade;
    [SerializeField] private float tempoHitbox = 0.1f;

    [Header("Recuperação da Habilidade")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Movimento")]
    [SerializeField] private Scriptdemovimento movimento;

    private PlayerInput playerInput;

    private InputAction mirar;
    private InputAction habilidade;

    private MiraJogador mira;

    private int municaoAtual;
    private float tempoAgachada;

    private int cargasAtuais;
    private bool estavaNoChao;

    private Rigidbody2D rb;

    public int MunicaoAtual => municaoAtual;
    public int MunicaoMaxima => municaoMaxima;
    public int CargasAtuais => cargasAtuais;

    void Awake()
    {
        municaoAtual = municaoMaxima;
        cargasAtuais = cargasMaximas;

        playerInput = GetComponent<PlayerInput>();

        mirar = playerInput.actions["Mirar"];
        habilidade = playerInput.actions["Habilidade"];

        mira = GetComponent<MiraJogador>();
        rb = GetComponent<Rigidbody2D>();

        mirar.canceled += OnPararDeMirar;
        habilidade.performed += OnHabilidade;

        if (hitboxHabilidade != null)
            hitboxHabilidade.SetActive(false);
    }

    void Update()
    {
        AtualizarRecarga();
        AtualizarCargas();
    }

    void OnPararDeMirar(InputAction.CallbackContext ctx)
    {
        if (movimento != null && movimento.IsCrouching)
            return;

        Atirar();
    }

    void Atirar()
    {
        if (municaoAtual <= 0)
            return;

        if (mira == null)
        {
            Debug.LogError(
                "A Fumaceira não possui um componente MiraJogador!"
            );

            return;
        }

        if (projetilPrefab == null)
        {
            Debug.LogError(
                "O prefab do projétil não foi configurado na Fumaceira!"
            );

            return;
        }

        Vector2 direcao = mira.DirecaoMira;

        if (direcao == Vector2.zero)
            return;

        Vector2 posicao =
            (Vector2)transform.position +
            direcao * distanciaSpawn;

        GameObject projetil = Instantiate(
            projetilPrefab,
            posicao,
            Quaternion.identity
        );

        ProjetilFumaceira projetilFumaceira =
            projetil.GetComponent<ProjetilFumaceira>();

        if (projetilFumaceira == null)
        {
            Debug.LogError(
                "O prefab do projétil não possui o componente ProjetilFumaceira!"
            );

            Destroy(projetil);
            return;
        }

        projetilFumaceira.Ativar(
            posicao,
            direcao,
            velocidadeProjetil
        );

        municaoAtual--;
    }

    void AtualizarRecarga()
    {
        if (movimento == null)
            return;

        if (!movimento.IsCrouching)
        {
            tempoAgachada = 0f;
            return;
        }

        if (municaoAtual >= municaoMaxima)
        {
            tempoAgachada = 0f;
            return;
        }

        tempoAgachada += Time.deltaTime;

        if (tempoAgachada >= tempoRecargaMunicao)
        {
            municaoAtual++;
            tempoAgachada = 0f;
        }
    }

    void OnHabilidade(InputAction.CallbackContext ctx)
    {
        if (cargasAtuais <= 0)
            return;

        if (movimento == null)
            return;

        if (movimento.IsCrouching)
            return;

        if (movimento.TocandoParede)
            return;

        UsarHabilidade();
    }

    void UsarHabilidade()
    {
        rb.linearVelocity = new Vector2(
            movimento.UltimaDirecao * forcaHorizontalHabilidade,
            forcaPuloHabilidade
        );

        cargasAtuais--;

        if (hitboxHabilidade != null)
        {
            hitboxHabilidade.SetActive(true);

            CancelInvoke(nameof(DesativarHitbox));

            Invoke(
                nameof(DesativarHitbox),
                tempoHitbox
            );
        }
    }

    void DesativarHitbox()
    {
        if (hitboxHabilidade != null)
            hitboxHabilidade.SetActive(false);
    }

    void AtualizarCargas()
    {
        if (groundCheck == null)
            return;

        bool estaNoChao = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            groundDistance,
            groundLayer
        );

        if (estaNoChao && !estavaNoChao)
        {
            cargasAtuais = cargasMaximas;
        }

        estavaNoChao = estaNoChao;
    }

    void OnDisable()
    {
        mirar.canceled -= OnPararDeMirar;
        habilidade.performed -= OnHabilidade;

        CancelInvoke(nameof(DesativarHitbox));

        if (hitboxHabilidade != null)
            hitboxHabilidade.SetActive(false);
    }
}