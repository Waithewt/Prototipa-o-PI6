using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManagerJogo : MonoBehaviour
{
    public static GameManagerJogo Instance;

    private GameObject personagemSelecionado;

    public GameObject PersonagemSelecionado =>
        personagemSelecionado;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SelecionarPersonagem(GameObject personagem)
    {
        if (personagem == null)
        {
            Debug.LogError(
                "O prefab do personagem não foi configurado!"
            );

            return;
        }

        personagemSelecionado = personagem;

        SceneManager.LoadScene("Jogo");
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            if (SceneManager.GetActiveScene().name == "Jogo")
            {
                SceneManager.LoadScene("SelecaoPersonagem");
            }
        }
    }
}
