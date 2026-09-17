using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnerJogo : MonoBehaviour
{
    [Header("Jogador")]
    [SerializeField] private Transform pontoSpawnJogador;

    [Header("NPC")]
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private Transform[] pontosSpawnNPC;

    private GameObject jogadorAtual;

    private List<GameObject> npcsAtuais =
        new List<GameObject>();

    void Start()
    {
        SpawnarTudo();
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;


        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetarArena();
        }
    }

    void SpawnarTudo()
    {
        SpawnarJogador();
        SpawnarNPCs();
    }

    void SpawnarJogador()
    {
        if (GameManagerJogo.Instance == null)
        {
            Debug.LogError(
                "GameManagerJogo não existe!"
            );

            return;
        }

        GameObject personagem =
            GameManagerJogo.Instance.PersonagemSelecionado;

        if (personagem == null)
        {
            Debug.LogError(
                "Nenhum personagem foi selecionado!"
            );

            return;
        }

        jogadorAtual = Instantiate(
            personagem,
            pontoSpawnJogador.position,
            pontoSpawnJogador.rotation
        );
    }

    void SpawnarNPCs()
    {
        if (npcPrefab == null)
        {
            Debug.LogWarning(
                "Nenhum prefab de NPC foi configurado."
            );

            return;
        }

        foreach (Transform ponto in pontosSpawnNPC)
        {
            if (ponto == null)
                continue;

            GameObject npc = Instantiate(
                npcPrefab,
                ponto.position,
                ponto.rotation
            );

            npcsAtuais.Add(npc);
        }
    }

    void ResetarArena()
    {
       
        if (jogadorAtual != null)
        {
            Destroy(jogadorAtual);
        }


        foreach (GameObject npc in npcsAtuais)
        {
            if (npc != null)
            {
                Destroy(npc);
            }
        }

        npcsAtuais.Clear();

   
        SpawnarTudo();
    }
}

