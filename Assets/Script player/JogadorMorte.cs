using UnityEngine;

public class JogadorMorte : MonoBehaviour
{
    [SerializeField] private GameObject explosaoMorte;

    private bool morto;
    private bool invulneravel;

    public bool Invulneravel => invulneravel;

    public void DefinirInvulnerabilidade(bool valor)
    {
        invulneravel = valor;
    }

    public void Morrer()
    {
        if (morto || invulneravel)
            return;

        morto = true;

        if (explosaoMorte != null)
            Instantiate(explosaoMorte, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}