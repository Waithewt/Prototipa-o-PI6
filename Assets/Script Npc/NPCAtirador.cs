using UnityEngine;

public class NPCAtirador : MonoBehaviour
{
    [SerializeField] private GameObject projecPrefab;
    [SerializeField] private float forcaTiro = 8f;
    [SerializeField] private float cooldownTiro = 1.5f;
    [SerializeField] private Transform pontoDisparo;
    [SerializeField] private Vector2 direcaoTiro = Vector2.right; 

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= cooldownTiro)
        {
            timer = 0f;
            Atirar();
        }
    }

    void Atirar()
    {
        GameObject projec = Instantiate(projecPrefab, pontoDisparo.position, Quaternion.identity);
        Rigidbody2D rb = projec.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direcaoTiro * forcaTiro;


        projec.GetComponent<ProjecNPC>().Inimigo = true;
    }
}