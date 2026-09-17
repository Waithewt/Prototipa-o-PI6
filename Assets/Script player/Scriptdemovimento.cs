using UnityEngine;
using UnityEngine.InputSystem;

public class Scriptdemovimento : MonoBehaviour
{
    private PlayerInput playerinput;
    private InputAction mover;
    private InputAction pular;
    private InputAction abaixar;
    private InputAction mirar;
    private InputAction habilidade;
    private float pulotimer;

    [SerializeField] private float velocidade = 5f;
    [SerializeField] private float forcaPulo = 5f;
    [SerializeField] private float forcaWallJump = 4f;
    [SerializeField] private float velocidadeDeslizando = 3f;
    [SerializeField] private float tempoPreso = 0.5f;
    [SerializeField] private float wallDistance = 0.1f;
    [SerializeField] private float groundDistance = 0.15f;
    [SerializeField] private float wallJumpCooldown = 0.15f;
    [SerializeField] private float colliderHalfWidth = 0.5f;
    [SerializeField] private float pulochaograce = 0.1f;
    [SerializeField] private float crouchHeightFactor = 0.5f;

    [SerializeField] private LayerMask wall;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private MiraJogador mira;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool tocandoEsquerda;
    private bool tocandoDireita;
    private float tempopreso;
    private bool estavatocandoparede;
    private float wallJumpTimer;

    // Crouch
    private bool isCrouching;
    private CapsuleCollider2D capsule;
    private SpriteRenderer spriteRenderer;
    private Vector2 colliderSizeOriginal;
    private Vector2 colliderOffsetOriginal;
    private Vector3 localScaleOriginal;
    private float deltaCrouch;
    private float ultimaDirecao = 1f;
    private bool movimentoBloqueado;
    public float UltimaDirecao => ultimaDirecao;
    public bool MovimentoBloqueado => movimentoBloqueado;
    public bool TocandoParede => tocandoEsquerda || tocandoDireita;
    public bool EstaEmWallJump => wallJumpTimer > 0f;

    public bool IsCrouching => isCrouching;

    void Awake()
    {
        playerinput = GetComponent<PlayerInput>();
        mover = playerinput.actions["Andar"];
        pular = playerinput.actions["Pular"];
        abaixar = playerinput.actions["Abaixar"];
        mirar = playerinput.actions["Mirar"];
        habilidade = playerinput.actions["Habilidade"];

        rb = GetComponent<Rigidbody2D>();
        pular.performed += OnJump;

        capsule = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        colliderSizeOriginal = capsule.size;
        colliderOffsetOriginal = capsule.offset;
        localScaleOriginal = transform.localScale;
        deltaCrouch = colliderSizeOriginal.y * (1f - crouchHeightFactor);
    }

    void Update()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundDistance, groundLayer);

        float dir = mover.ReadValue<Vector2>().x;

        if (dir > 0f)
            ultimaDirecao = 1f;
        else if (dir < 0f)
            ultimaDirecao = -1f;

       

        bool querAbaixar = abaixar.ReadValue<float>() > 0f;

        if (querAbaixar && isGrounded && !isCrouching)
            Crouch();
        else if (!querAbaixar && isCrouching)
            Stand();

        Vector2 origem = transform.position;
        tocandoDireita = Physics2D.Raycast(origem + Vector2.right * colliderHalfWidth, Vector2.right, wallDistance, wall);
        tocandoEsquerda = Physics2D.Raycast(origem + Vector2.left * colliderHalfWidth, Vector2.left, wallDistance, wall);


        if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
            return;
        }

        if (movimentoBloqueado)
            return;

        bool estatocandoparede = !isGrounded && pulotimer <= 0f && (tocandoDireita || tocandoEsquerda);

        if (estatocandoparede && !estavatocandoparede)
        {
            tempopreso = tempoPreso;
        }

        if (estatocandoparede && tempopreso > 0f)
        {
            rb.linearVelocity = new Vector2(0f, 0f);
            tempopreso -= Time.deltaTime;
        }
        else if (estatocandoparede && tempopreso <= 0f)
        {
            bool empurrandoParaFora = (tocandoDireita && dir < 0f) || (tocandoEsquerda && dir > 0f);

            if (empurrandoParaFora)
            {
                rb.linearVelocity = new Vector2(dir * velocidade, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(0f, -velocidadeDeslizando);
            }
        }
        else
        {
            float velHorizontal = isCrouching ? 0f : dir * velocidade;
            rb.linearVelocity = new Vector2(velHorizontal, rb.linearVelocity.y);
        }

        estavatocandoparede = estatocandoparede;

        if (pulotimer > 0f)
        {
            pulotimer -= Time.deltaTime;
        }
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        if (isCrouching) return;

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);
            pulotimer = pulochaograce;
        }
        else if (tocandoDireita)
        {
            rb.linearVelocity = new Vector2(-forcaWallJump, forcaPulo);
            tempopreso = 0f;
            wallJumpTimer = wallJumpCooldown;
        }
        else if (tocandoEsquerda)
        {
            rb.linearVelocity = new Vector2(forcaWallJump, forcaPulo);
            tempopreso = 0f;
            wallJumpTimer = wallJumpCooldown;
        }
    }

    void Crouch()
    {
        isCrouching = true;

        if (mira != null) mira.CancelarMira();

        float novaAltura = colliderSizeOriginal.y * crouchHeightFactor;

      
        rb.position = (Vector2)rb.position - new Vector2(0f, deltaCrouch / 2f);

        capsule.size = new Vector2(colliderSizeOriginal.x, novaAltura);
        capsule.offset = new Vector2(colliderOffsetOriginal.x, colliderOffsetOriginal.y - deltaCrouch / 2f);

        transform.localScale = new Vector3(localScaleOriginal.x, localScaleOriginal.y * crouchHeightFactor, localScaleOriginal.z);

        mirar.Disable();
        habilidade.Disable();
    }

    void Stand()
    {
        isCrouching = false;

  
        rb.position = (Vector2)rb.position + new Vector2(0f, deltaCrouch / 2f);

        capsule.size = colliderSizeOriginal;
        capsule.offset = colliderOffsetOriginal;

        transform.localScale = localScaleOriginal;

        mirar.Enable();
        habilidade.Enable();
    }

    void OnDisable()
    {
        pular.performed -= OnJump;
    }
    public void BloquearMovimento(bool bloquear)
    {
        movimentoBloqueado = bloquear;
    }
}