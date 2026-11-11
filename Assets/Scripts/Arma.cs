using UnityEngine;

public class Arma : MonoBehaviour
{
    public KeyCode tclDisparo = KeyCode.Space;
    public GameObject prefabBala;
    public Transform puntoDisparo;

    public float cadencia = 0.25f;     // segundos entre disparos
    public float velocidadBala = 20f;
    public float empuje = 8f;          // empuje al rival
    public float retroceso = 2f;       // empuje hacia atrás al que dispara

    JugadorMovimiento jugador;
    float siguienteDisparo;

    void Awake()
    {
        jugador = GetComponentInParent<JugadorMovimiento>();
    }

    void Update()
    {
        if (Input.GetKey(tclDisparo) && Time.time >= siguienteDisparo)
        {
            Disparar();
            siguienteDisparo = Time.time + cadencia;
        }
    }

    void Disparar()
    {
        float dir = Mathf.Sign(jugador.transform.localScale.x);
        GameObject bala = Instantiate(prefabBala, puntoDisparo.position, Quaternion.identity);
        bala.GetComponent<Bala>().Iniciar(new Vector2(dir, 0f), velocidadBala, empuje, jugador.gameObject);
        jugador.RecibirEmpuje(-dir * retroceso);
    }
}