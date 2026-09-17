using UnityEngine;
using UnityEngine.UI;

public class SelecaoPersonagem : MonoBehaviour
{
    [Header("Botões")]
    [SerializeField] private Button botaoCowboy;
    [SerializeField] private Button botaoIrrigador;
    [SerializeField] private Button botaoRebatedor;
    [SerializeField] private Button botaoFumaceira;

    [Header("Prefabs")]
    [SerializeField] private GameObject prefabCowboy;
    [SerializeField] private GameObject prefabIrrigador;
    [SerializeField] private GameObject prefabRebatedor;
    [SerializeField] private GameObject prefabFumaceira;

    void Start()
    {
        if (GameManagerJogo.Instance == null)
        {
            Debug.LogError(
                "GameManagerJogo não existe!"
            );

            return;
        }

        botaoCowboy.onClick.AddListener(
            SelecionarCowboy
        );

        botaoIrrigador.onClick.AddListener(
            SelecionarIrrigador
        );

        botaoRebatedor.onClick.AddListener(
            SelecionarRebatedor
        );

        botaoFumaceira.onClick.AddListener(
            SelecionarFumaceira
        );
    }

    void SelecionarCowboy()
    {
        GameManagerJogo.Instance.SelecionarPersonagem(
            prefabCowboy
        );
    }

    void SelecionarIrrigador()
    {
        GameManagerJogo.Instance.SelecionarPersonagem(
            prefabIrrigador
        );
    }

    void SelecionarRebatedor()
    {
        GameManagerJogo.Instance.SelecionarPersonagem(
            prefabRebatedor
        );
    }

    void SelecionarFumaceira()
    {
        GameManagerJogo.Instance.SelecionarPersonagem(
            prefabFumaceira
        );
    }
}
