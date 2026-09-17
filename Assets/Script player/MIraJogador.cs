using UnityEngine;
using UnityEngine.InputSystem;

public class MiraJogador : MonoBehaviour
{
    [Header("Mira")]
    [SerializeField] private Transform aimTransform;
    [SerializeField] private GameObject reticula;
    [SerializeField] private float raioMaximoMira = 3f;
    [SerializeField] private float velocidadeRotacao = 10f;

    [Header("Movimento")]
    [SerializeField] private Scriptdemovimento movimento;

    private PlayerInput playerInput;
    private InputAction mirar;

    private bool estaMirando;
    private Vector2 direcaoMira = Vector2.zero;

    public bool EstaMirando => estaMirando;
    public Vector2 DirecaoMira => direcaoMira;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        mirar = playerInput.actions["Mirar"];

        mirar.performed += OnMirarLigar;
        mirar.canceled += OnMirarDesligar;
    }

    void Update()
    {
        if (!estaMirando)
            return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );

        mousePos.z = 0f;

        Vector2 direcao =
            (Vector2)mousePos -
            (Vector2)transform.position;

        if (direcao.magnitude > raioMaximoMira)
        {
            direcao =
                direcao.normalized *
                raioMaximoMira;
        }

        if (direcao != Vector2.zero)
        {
            direcaoMira = direcao.normalized;
        }

        if (reticula != null)
        {
            reticula.transform.position =
                (Vector2)transform.position + direcao;
        }

        if (aimTransform != null && direcao != Vector2.zero)
        {
            float angulo =
                Mathf.Atan2(direcao.y, direcao.x) *
                Mathf.Rad2Deg;

            aimTransform.rotation = Quaternion.RotateTowards(
                aimTransform.rotation,
                Quaternion.Euler(0f, 0f, angulo),
                velocidadeRotacao * Time.deltaTime
            );
        }
    }

    void OnMirarLigar(InputAction.CallbackContext ctx)
    {
        if (movimento != null && movimento.IsCrouching)
            return;

        estaMirando = true;

        if (reticula != null)
            reticula.SetActive(true);

        Cursor.visible = false;
    }

    void OnMirarDesligar(InputAction.CallbackContext ctx)
    {
        if (!estaMirando)
            return;

        estaMirando = false;

        if (reticula != null)
            reticula.SetActive(false);

        Cursor.visible = true;
    }

    public void CancelarMira()
    {
        if (!estaMirando)
            return;

        estaMirando = false;

        if (reticula != null)
            reticula.SetActive(false);

        Cursor.visible = true;
    }

    void OnDisable()
    {
        mirar.performed -= OnMirarLigar;
        mirar.canceled -= OnMirarDesligar;
    }
}