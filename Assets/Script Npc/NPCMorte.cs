using UnityEngine;

public class NPCMorte : MonoBehaviour
{
    public void ReceberDano()
    {
        Morrer();
    }

    public void Morrer()
    {
        Destroy(gameObject);
    }
}