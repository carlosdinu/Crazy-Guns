using UnityEngine;

/// <summary>
/// Power-up: caja que cae del cielo, se posa en el suelo y da un arma
/// aleatoria al primer luchador que la toca. Parpadea y desaparece si nadie la coge.
/// </summary>
public class CajaArma : MonoBehaviour
{
    public DatosArma[] armasPosibles;
    public float velocidadCaida = 4f;
    public LayerMask capaSuelo;
    public float tiempoVida = 10f;
    public float alturaLimite = -15f;    // si cae al vacío, se destruye

    bool enSuelo;
    float tiempoAparicion;
    SpriteRenderer sr;
    Collider2D col;

    void Start()
    {
        tiempoAparicion = Time.time;
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        // Cae hasta tocar el suelo
        if (!enSuelo)
        {
            float paso = velocidadCaida * Time.deltaTime;
            Vector2 base_ = new Vector2(col.bounds.center.x, col.bounds.min.y);
            RaycastHit2D suelo = Physics2D.Raycast(base_, Vector2.down, paso + 0.02f, capaSuelo);

            if (suelo)
            {
                transform.position += Vector3.down * suelo.distance;
                enSuelo = true;
            }
            else
            {
                transform.position += Vector3.down * paso;
            }
        }

        // Parpadea los últimos 3 segundos y luego desaparece
        float vida = Time.time - tiempoAparicion;
        if (vida > tiempoVida - 3f)
            sr.enabled = Mathf.FloorToInt(vida * 8f) % 2 == 0;

        if (vida > tiempoVida || transform.position.y < alturaLimite)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        Arma arma = otro.GetComponentInChildren<Arma>();
        if (arma == null || armasPosibles.Length == 0) return;

        arma.Equipar(armasPosibles[Random.Range(0, armasPosibles.Length)]);
        Destroy(gameObject);
    }
}