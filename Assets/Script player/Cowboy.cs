using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Cowboy : MonoBehaviour
{
    [Header("Munição")]
    [SerializeField] private int municaoMaxima = 6;
    [SerializeField] private float tempoRecarga = 1.5f;

    [Header("Tiro")]
    [SerializeField] private GameObject projecPrefab;
    [SerializeField] private int tamanhoPool = 10;
    [SerializeField] private float forcaTiro = 13f;
    [SerializeField] private float distanciaSpawn = 0.5f;

    [Header("Habilidade")]
    [SerializeField] private Scriptdemovimento movimento;
    [SerializeField] private float velocidadeRolamento = 12f;
    [SerializeField] private float duracaoRolamento = 0.4f;

    private int municaoAtual;
    private bool recarregando;
    private bool rolando;

    private PlayerInput playerInput;
    private InputAction mirar;
    private InputAction habilidade;

    private MiraJogador mira;
    private JogadorMorte jogadorMorte;

    private Queue<GameObject> pool =
        new Queue<GameObject>();

    public int MunicaoAtual => municaoAtual;
    public int MunicaoMaxima => municaoMaxima;
    public bool Recarregando => recarregando;
    public bool Rolando => rolando;

    void Awake()
    {
        municaoAtual = municaoMaxima;

        playerInput = GetComponent<PlayerInput>();

        mirar = playerInput.actions["Mirar"];
        habilidade = playerInput.actions["Habilidade"];

        mira = GetComponent<MiraJogador>();

        mirar.canceled += OnPararDeMirar;
        habilidade.performed += OnHabilidade;

        jogadorMorte = GetComponent<JogadorMorte>();

        CriarPool();
    }

    void CriarPool()
    {
        if (projecPrefab == null)
        {
            Debug.LogError(
                "O prefab do projétil não foi configurado no Cowboy!"
            );

            return;
        }

        for (int i = 0; i < tamanhoPool; i++)
        {
            GameObject obj = Instantiate(
                projecPrefab,
                transform.position,
                Quaternion.identity
            );

            obj.SetActive(false);
            obj.transform.SetParent(transform);

            pool.Enqueue(obj);
        }
    }

    void OnPararDeMirar(InputAction.CallbackContext ctx)
    {
        Atirar();
    }

    void Atirar()
    {
        if (!PodeAtirar())
            return;

        if (mira == null)
        {
            Debug.LogError(
                "O Cowboy não possui um componente MiraJogador!"
            );

            return;
        }

        Vector2 direcao = mira.DirecaoMira;

        if (direcao == Vector2.zero)
            return;

        GameObject projec = PegarDoPool();

        if (projec == null)
            return;

        Vector2 posSpawn =
            (Vector2)transform.position +
            direcao * distanciaSpawn;

        Projec projetil =
            projec.GetComponent<Projec>();

        if (projetil == null)
        {
            Debug.LogError(
                "O prefab do projétil não possui o componente Projec!"
            );

            DevolverParaPool(projec);
            return;
        }

        projetil.Ativar(
            posSpawn,
            direcao * forcaTiro,
            this
        );

        ConsumirMunicao();
    }

    GameObject PegarDoPool()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();

            obj.transform.SetParent(null);
            obj.SetActive(true);

            return obj;
        }

        if (projecPrefab == null)
            return null;

        return Instantiate(
            projecPrefab,
            transform.position,
            Quaternion.identity
        );
    }

    public void DevolverParaPool(GameObject obj)
    {
        if (obj == null)
            return;

        obj.SetActive(false);
        obj.transform.SetParent(transform);

        pool.Enqueue(obj);
    }

    public bool PodeAtirar()
    {
        return municaoAtual > 0 &&
               !recarregando &&
               !rolando;
    }

    public void ConsumirMunicao()
    {
        if (municaoAtual <= 0 || recarregando)
            return;

        municaoAtual--;

        if (municaoAtual == 0)
        {
            IniciarRecarga();
        }
    }

    private void IniciarRecarga()
    {
        if (recarregando)
            return;

        recarregando = true;

        Invoke(
            nameof(TerminarRecarga),
            tempoRecarga
        );
    }

    private void TerminarRecarga()
    {
        municaoAtual = municaoMaxima;
        recarregando = false;
    }

    void OnHabilidade(InputAction.CallbackContext ctx)
    {
        if (movimento == null)
            return;

        if (rolando)
            return;

        if (movimento.TocandoParede)
            return;

        if (movimento.EstaEmWallJump)
            return;

        IniciarRolamento();
    }

    void IniciarRolamento()
    {
        gameObject.layer =
            LayerMask.NameToLayer("PlayerRolando");

        rolando = true;

        if (jogadorMorte != null)
            jogadorMorte.DefinirInvulnerabilidade(true);

        movimento.BloquearMovimento(true);

        float direcao =
            movimento.UltimaDirecao;

        Rigidbody2D rb =
            GetComponent<Rigidbody2D>();

        rb.linearVelocity = new Vector2(
            direcao * velocidadeRolamento,
            rb.linearVelocity.y
        );

        Invoke(
            nameof(TerminarRolamento),
            duracaoRolamento
        );
    }

    void TerminarRolamento()
    {
        gameObject.layer =
            LayerMask.NameToLayer("Player");

        rolando = false;

        if (jogadorMorte != null)
            jogadorMorte.DefinirInvulnerabilidade(false);

        movimento.BloquearMovimento(false);
    }

    void OnDisable()
    {
        mirar.canceled -= OnPararDeMirar;
        habilidade.performed -= OnHabilidade;

        CancelInvoke(nameof(TerminarRolamento));
        CancelInvoke(nameof(TerminarRecarga));
    }
}