using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rebatedor : MonoBehaviour
{
    [Header("Rebatida")]
    [SerializeField] private GameObject bolaPrefab;
    [SerializeField] private float distanciaSpawn = 0.6f;

    [Header("Passiva - Força das Rebatidas")]
    [SerializeField] private float velocidadeRebatida1 = 12f;
    [SerializeField] private float velocidadeRebatida2 = 20f;
    [SerializeField] private float velocidadeRebatida3 = 30f;

    [SerializeField] private float perdaForcaQuicar1 = 0.70f;
    [SerializeField] private float perdaForcaQuicar2 = 0.85f;
    [SerializeField] private float perdaForcaQuicar3 = 0.95f;

    [Header("Mira Normal")]
    [SerializeField] private Transform mira;
    [SerializeField] private float raioMaximoMira = 3f;

    [Header("Habilidade")]
    [SerializeField] private float alcanceHabilidade = 1.5f;
    [SerializeField] private float anguloHabilidade = 70f;
    [SerializeField] private float forcaMinimaRebatida = 8f;
    [SerializeField] private float forcaKnockback = 3f;
    private PlayerInput playerInput;

    private InputAction mirar;
    private InputAction habilidade;

    private bool estaMirando;
    private bool estaMirandoHabilidade;

    private Vector2 direcaoMira;
    private Vector2 direcaoMiraHabilidade;


    private GameObject bolaAtual;

    private int nivelRebatida = 1;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        mirar = playerInput.actions["Mirar"];

        mirar.performed += OnMirar;
        mirar.canceled += OnPararDeMirar;

        habilidade = playerInput.actions["Habilidade"];

        habilidade.started += OnHabilidadeComecar;
        habilidade.canceled += OnHabilidadeTerminar;
    }

    void Update()
    {
        if (estaMirando)
        {
            AtualizarMiraNormal();
        }

        if (estaMirandoHabilidade)
        {
            AtualizarMiraHabilidade();
        }
    }

    void AtualizarMiraNormal()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );

        mousePos.z = 0f;

        Vector2 direcao =
            (Vector2)mousePos - (Vector2)transform.position;

        if (direcao.magnitude > raioMaximoMira)
        {
            direcao = direcao.normalized * raioMaximoMira;
        }

        direcaoMira = direcao.normalized;

        if (mira != null)
        {
            mira.position =
                (Vector2)transform.position + direcao;
        }
    }

    void OnMirar(InputAction.CallbackContext ctx)
    {
        if (bolaAtual != null)
            return;

        if (estaMirandoHabilidade)
            return;

        estaMirando = true;

        if (mira != null)
            mira.gameObject.SetActive(true);
    }

    void OnPararDeMirar(InputAction.CallbackContext ctx)
    {
        if (!estaMirando)
            return;

        estaMirando = false;

        if (mira != null)
            mira.gameObject.SetActive(false);

        Rebatida();
    }

    void Rebatida()
    {
        if (bolaAtual != null)
            return;

        if (bolaPrefab == null)
        {
            Debug.LogError("Bola da rebatida não foi configurada!");
            return;
        }

        if (direcaoMira == Vector2.zero)
            return;

        Vector2 posicao =
            (Vector2)transform.position +
            direcaoMira * distanciaSpawn;

        bolaAtual = Instantiate(
            bolaPrefab,
            posicao,
            Quaternion.identity
        );

        BolaRebatida bola =
            bolaAtual.GetComponent<BolaRebatida>();

        if (bola == null)
        {
            Debug.LogError(
                "O prefab da bola não possui o componente BolaRebatida!"
            );

            Destroy(bolaAtual);
            bolaAtual = null;

            return;
        }

        bola.DefinirDono(this);

        float velocidade;
        float perdaForcaQuicar;

        if (nivelRebatida == 1)
        {
            velocidade = velocidadeRebatida1;
            perdaForcaQuicar = perdaForcaQuicar1;
        }
        else if (nivelRebatida == 2)
        {
            velocidade = velocidadeRebatida2;
            perdaForcaQuicar = perdaForcaQuicar2;
        }
        else
        {
            velocidade = velocidadeRebatida3;
            perdaForcaQuicar = perdaForcaQuicar3;
        }

        bola.Lancar(
            direcaoMira,
            velocidade,
            perdaForcaQuicar
        );

        nivelRebatida++;

        if (nivelRebatida > 3)
        {
            nivelRebatida = 1;
        }
    }

    void OnHabilidadeComecar(InputAction.CallbackContext ctx)
    {
        if (estaMirando)
            return;

        estaMirandoHabilidade = true;
    }

    void OnHabilidadeTerminar(InputAction.CallbackContext ctx)
    {
        if (!estaMirandoHabilidade)
            return;

        estaMirandoHabilidade = false;

        UsarHabilidade();
    }

    void AtualizarMiraHabilidade()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );

        mousePos.z = 0f;

        Vector2 direcao =
            (Vector2)mousePos - (Vector2)transform.position;

        if (direcao == Vector2.zero)
            return;

        direcaoMiraHabilidade = direcao.normalized;
    }

    void UsarHabilidade()
    {
        if (direcaoMiraHabilidade == Vector2.zero)
            return;

        Collider2D[] objetos = Physics2D.OverlapCircleAll(
            transform.position,
            alcanceHabilidade
        );

        foreach (Collider2D objeto in objetos)
        {
            Vector2 direcaoObjeto =
                (Vector2)objeto.transform.position -
                (Vector2)transform.position;

            if (direcaoObjeto == Vector2.zero)
                continue;

            float angulo = Vector2.Angle(
                direcaoMiraHabilidade,
                direcaoObjeto.normalized
            );

            if (angulo > anguloHabilidade / 2f)
                continue;

      
            BolaRebatida bola =
                objeto.GetComponentInParent<BolaRebatida>();

            if (bola != null)
            {
                bola.Rebater(
                    direcaoMiraHabilidade,
                    forcaMinimaRebatida
                );

                continue;
            }

        
            ProjetilRebatel projetil =
                objeto.GetComponentInParent<ProjetilRebatel>();

            if (projetil != null)
            {
                projetil.Rebater(
                    direcaoMiraHabilidade,
                    forcaMinimaRebatida
                );

                continue;
            }

        
            JogadorMorte jogador =
                objeto.GetComponentInParent<JogadorMorte>();

            NPCMorte npc =
                objeto.GetComponentInParent<NPCMorte>();

            if (jogador != null || npc != null)
            {
                Rigidbody2D alvoRb =
                    objeto.GetComponentInParent<Rigidbody2D>();

                if (alvoRb != null)
                {
                    Vector2 direcaoKnockback =
                        (
                            (Vector2)alvoRb.transform.position -
                            (Vector2)transform.position
                        ).normalized;

                    Vector2 knockback =
                        new Vector2(
                            direcaoKnockback.x * forcaKnockback,
                            forcaKnockback * 0.6f
                        );

                    alvoRb.AddForce(
                        knockback,
                        ForceMode2D.Impulse
                    );
                }
            }
        }
    }

    public void RecuperarBola()
    {
        if (bolaAtual == null)
            return;

        bolaAtual = null;
    }

    void OnDrawGizmosSelected()
    {
        Vector2 direcao = direcaoMiraHabilidade;

        if (direcao == Vector2.zero)
            direcao = Vector2.right;

        Vector3 origem = transform.position;

        float meioAngulo =
            anguloHabilidade / 2f;

        Vector2 direcaoEsquerda =
            Quaternion.Euler(
                0f,
                0f,
                meioAngulo
            ) * direcao;

        Vector2 direcaoDireita =
            Quaternion.Euler(
                0f,
                0f,
                -meioAngulo
            ) * direcao;

        Vector3 pontoEsquerdo =
            origem +
            (Vector3)(
                direcaoEsquerda.normalized *
                alcanceHabilidade
            );

        Vector3 pontoDireito =
            origem +
            (Vector3)(
                direcaoDireita.normalized *
                alcanceHabilidade
            );

        Gizmos.DrawLine(
            origem,
            pontoEsquerdo
        );

        Gizmos.DrawLine(
            origem,
            pontoDireito
        );

        int segmentos = 30;

        Vector3 pontoAnterior =
            pontoEsquerdo;

        for (int i = 1; i <= segmentos; i++)
        {
            float angulo =
                meioAngulo -
                (
                    anguloHabilidade *
                    i /
                    segmentos
                );

            Vector2 direcaoAtual =
                Quaternion.Euler(
                    0f,
                    0f,
                    angulo
                ) * direcao;

            Vector3 pontoAtual =
                origem +
                (Vector3)(
                    direcaoAtual.normalized *
                    alcanceHabilidade
                );

            Gizmos.DrawLine(
                pontoAnterior,
                pontoAtual
            );

            pontoAnterior = pontoAtual;
        }
    }

    void OnDisable()
    {
        mirar.performed -= OnMirar;
        mirar.canceled -= OnPararDeMirar;

        habilidade.started -= OnHabilidadeComecar;
        habilidade.canceled -= OnHabilidadeTerminar;
    }
}

