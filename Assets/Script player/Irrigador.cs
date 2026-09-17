using UnityEngine;
using UnityEngine.InputSystem;

public class Irrigador : MonoBehaviour
{
    [Header("Munição")]
    [SerializeField] private int municaoMaxima = 4;

    [Header("Prego")]
    [SerializeField] private GameObject pregoPrefab;
    [SerializeField] private float distanciaSpawn = 0.6f;
    [SerializeField] private float forcaTiro = 16f;

    [Header("Ímã")]
    [SerializeField] private float raioImã = 8f;
    [SerializeField] private float forcaImã = 8f;
    [SerializeField] private float anguloImã = 30f;

    private PlayerInput playerInput;
    private InputAction mirar;
    private InputAction habilidade;

    private MiraJogador mira;
    private Rigidbody2D rb;

    private int municaoAtual;

    private PregoIrrigador pregoAlvo;


    private bool segurandoHabilidade;

    public int MunicaoAtual => municaoAtual;
    public int MunicaoMaxima => municaoMaxima;

    void Awake()
    {
        municaoAtual = municaoMaxima;

        playerInput = GetComponent<PlayerInput>();

        mirar = playerInput.actions["Mirar"];
        habilidade = playerInput.actions["Habilidade"];

        mira = GetComponent<MiraJogador>();
        rb = GetComponent<Rigidbody2D>();

        mirar.canceled += OnPararDeMirar;

        habilidade.started += OnHabilidadeComecar;
        habilidade.canceled += OnHabilidadeSoltar;
    }

    void Update()
    {
       
        if (municaoAtual != 0)
        {
            pregoAlvo = null;
            return;
        }

        if (mira == null)
            return;

        if (!mira.EstaMirando)
        {
            pregoAlvo = null;
            return;
        }

        ProcurarPrego();
    }

    void FixedUpdate()
    {
        // Ímã normal
        if (municaoAtual != 0)
            return;

        if (pregoAlvo == null)
            return;

        PuxarParaPrego();
    }



    void OnPararDeMirar(InputAction.CallbackContext ctx)
    {
        if (municaoAtual <= 0)
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
                "O Irrigador não possui um componente MiraJogador!"
            );

            return;
        }

        if (pregoPrefab == null)
        {
            Debug.LogError(
                "O prefab do prego não foi configurado no Irrigador!"
            );

            return;
        }

        Vector2 direcao = mira.DirecaoMira;

        if (direcao == Vector2.zero)
            return;

        Vector2 posicao =
            (Vector2)transform.position +
            direcao * distanciaSpawn;

        GameObject prego = Instantiate(
            pregoPrefab,
            posicao,
            Quaternion.identity
        );

        PregoIrrigador pregoScript =
            prego.GetComponent<PregoIrrigador>();

        if (pregoScript == null)
        {
            Debug.LogError(
                "O prefab do prego não possui o componente PregoIrrigador!"
            );

            Destroy(prego);
            return;
        }

        pregoScript.Ativar(
            posicao,
            direcao,
            forcaTiro
        );

        pregoScript.DefinirDono(this);

        municaoAtual--;
    }



    void ProcurarPrego()
    {
        Vector2 direcaoMira = mira.DirecaoMira;

        if (direcaoMira == Vector2.zero)
        {
            pregoAlvo = null;
            return;
        }

        Collider2D[] objetos = Physics2D.OverlapCircleAll(
            transform.position,
            raioImã
        );

        PregoIrrigador melhorPrego = null;
        float menorAngulo = anguloImã;

        foreach (Collider2D objeto in objetos)
        {
            PregoIrrigador prego =
                objeto.GetComponentInParent<PregoIrrigador>();

            if (prego == null)
                continue;

            if (!prego.Fincado)
                continue;

            Vector2 direcaoParaPrego =
                (Vector2)prego.transform.position -
                (Vector2)transform.position;

            if (direcaoParaPrego.sqrMagnitude <= 0.01f)
                continue;

            float angulo = Vector2.Angle(
                direcaoMira,
                direcaoParaPrego.normalized
            );

            if (angulo <= menorAngulo)
            {
                menorAngulo = angulo;
                melhorPrego = prego;
            }
        }

        pregoAlvo = melhorPrego;
    }

    void PuxarParaPrego()
    {
        if (pregoAlvo == null)
            return;

        Vector2 direcao =
            ((Vector2)pregoAlvo.transform.position -
            (Vector2)transform.position).normalized;

        rb.linearVelocity = direcao * forcaImã;
    }


    void OnHabilidadeComecar(InputAction.CallbackContext ctx)
    {
        if (municaoAtual > municaoMaxima - 2)
            return;

        segurandoHabilidade = true;
    }

    void OnHabilidadeSoltar(InputAction.CallbackContext ctx)
    {
        if (!segurandoHabilidade)
            return;

        segurandoHabilidade = false;

        if (municaoAtual > municaoMaxima - 2)
            return;

        AtivarHabilidade();
    }

    void AtivarHabilidade()
    {
        Vector2 direcaoMira = ObterDirecaoHabilidade();

        if (direcaoMira == Vector2.zero)
            return;

        Collider2D[] objetos = Physics2D.OverlapCircleAll(
            transform.position,
            raioImã
        );

        int quantidade = 0;

        foreach (Collider2D objeto in objetos)
        {
            PregoIrrigador prego =
                objeto.GetComponentInParent<PregoIrrigador>();

            if (prego == null)
                continue;

            if (!prego.Fincado)
                continue;

            Vector2 direcaoParaPrego =
                (Vector2)prego.transform.position -
                (Vector2)transform.position;

            if (direcaoParaPrego.sqrMagnitude <= 0.01f)
                continue;

            float angulo = Vector2.Angle(
                direcaoMira,
                direcaoParaPrego.normalized
            );

            if (angulo <= anguloImã)
            {
                quantidade++;
            }
        }

        if (quantidade == 0)
            return;

        PregoIrrigador[] pregosPuxados =
            new PregoIrrigador[quantidade];

        int indice = 0;

        foreach (Collider2D objeto in objetos)
        {
            PregoIrrigador prego =
                objeto.GetComponentInParent<PregoIrrigador>();

            if (prego == null)
                continue;

            if (!prego.Fincado)
                continue;

            Vector2 direcaoParaPrego =
                (Vector2)prego.transform.position -
                (Vector2)transform.position;

            if (direcaoParaPrego.sqrMagnitude <= 0.01f)
                continue;

            float angulo = Vector2.Angle(
                direcaoMira,
                direcaoParaPrego.normalized
            );

            if (angulo <= anguloImã)
            {
                pregosPuxados[indice] = prego;
                indice++;
            }
        }

        foreach (PregoIrrigador prego in pregosPuxados)
        {
            if (prego == null)
                continue;

            prego.IniciarPuxao(this);
        }
    }

    Vector2 ObterDirecaoHabilidade()
    {
        if (Mouse.current == null)
            return Vector2.zero;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );

        mousePos.z = 0f;

        Vector2 direcao =
            (Vector2)mousePos -
            (Vector2)transform.position;

        if (direcao.sqrMagnitude <= 0.01f)
            return Vector2.zero;

        return direcao.normalized;
    }

 

    public void ColetarPrego(PregoIrrigador prego)
    {
        if (prego == null)
            return;

        if (municaoAtual >= municaoMaxima)
            return;

        municaoAtual++;

        Destroy(prego.gameObject);

        pregoAlvo = null;

        rb.linearVelocity = Vector2.zero;
    }

  

    void OnDisable()
    {
        mirar.canceled -= OnPararDeMirar;

        habilidade.started -= OnHabilidadeComecar;
        habilidade.canceled -= OnHabilidadeSoltar;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            raioImã
        );
    }
}