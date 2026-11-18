using UnityEngine;

[RequireComponent(typeof(JugadorMovimiento))]
public class BotIA : MonoBehaviour
{
    [Header("Comportamiento")]
    public float distanciaIdeal = 5f;        // distancia a la que intenta quedarse
    public float rangoDisparo = 12f;
    public float toleranciaAltura = 1f;      // diferencia de altura máxima para disparar
    public float tiempoReaccion = 0.25f;     // más alto = bot más lento y fácil
    [Range(0f, 1f)] public float agresividad = 0.8f;   // probabilidad de disparar cuando puede

    [Header("Mapa")]
    public LayerMask capaSuelo;
    public Transform centroMapa;             // a dónde vuelve si lo tiran
    public float alturaPeligro = -4f;        // por debajo de esta Y intenta recuperarse

    JugadorMovimiento mov;
    Arma arma;
    Collider2D col;
    Rigidbody2D rb;
    Transform objetivo;
    float siguienteDecision;

    float dir;
    bool saltar;
    bool bajar;
    bool disparar;

    void Awake()
    {
        mov = GetComponent<JugadorMovimiento>();
        arma = GetComponentInChildren<Arma>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Time.time >= siguienteDecision)
        {
            Decidir();
            // Un poco de aleatoriedad para que no parezca un robot
            siguienteDecision = Time.time + tiempoReaccion * Random.Range(0.7f, 1.3f);
        }

        mov.EntradaIA(dir, saltar, bajar);
        saltar = false;   // saltar y bajar son "pulsaciones" de un solo frame
        bajar = false;

        if (arma != null) arma.disparoIA = disparar;
    }

    void Decidir()
    {
        dir = 0f;
        disparar = false;

        // 1. Si lo han tirado fuera, intenta volver al centro con el doble salto
        bool cayendoAlVacio = rb.velocity.y < -2f && !HaySueloDebajo();
        if (transform.position.y < alturaPeligro || cayendoAlVacio)
        {
            dir = Mathf.Sign(centroMapa.position.x - transform.position.x);
            saltar = true;
            return;
        }

        BuscarObjetivo();
        if (objetivo == null) return;

        Vector2 delta = objetivo.position - transform.position;
        float distX = Mathf.Abs(delta.x);
        float lado = Mathf.Sign(delta.x);
        bool aSuAltura = Mathf.Abs(delta.y) < toleranciaAltura;

        // 2. Acercarse si está lejos, alejarse si está demasiado cerca
        if (distX > distanciaIdeal) dir = lado;
        else if (distX < distanciaIdeal * 0.5f && !aSuAltura) dir = -lado;

        // 3. No tirarse al vacío: solo salta el hueco si el objetivo está al otro lado
        if (dir != 0f && mov.EnSuelo && !HaySueloDelante(dir))
        {
            if (dir == lado && distX > 2f) saltar = true;
            else dir = 0f;
        }

        // 4. Saltar si el objetivo está más arriba o hay una pared delante
        if (mov.EnSuelo && delta.y > 1.5f && Random.value < 0.6f) saltar = true;
        if (dir != 0f && HayParedDelante(dir)) saltar = true;

        // 5. Bajar de la plataforma si el objetivo está debajo
        if (mov.EnSuelo && delta.y < -1.5f && distX < 3f) bajar = true;

        // 6. Disparar si está a su altura y en rango
        if (aSuAltura && distX < rangoDisparo)
        {
            if (dir == -lado) dir = 0f;   // no da la espalda para disparar
            mov.MirarHacia(lado);
            disparar = Random.value < agresividad;
        }
    }

    // Busca al humano vivo más cercano (si no hay, a cualquier otro luchador)
    void BuscarObjetivo()
    {
        objetivo = null;
        float mejorDistancia = float.MaxValue;
        bool mejorEsHumano = false;

        foreach (VidasJugador v in FindObjectsOfType<VidasJugador>())
        {
            if (v.gameObject == gameObject) continue;

            float d = Vector2.Distance(transform.position, v.transform.position);
            bool preferible = (v.esHumano && !mejorEsHumano) ||
                              (v.esHumano == mejorEsHumano && d < mejorDistancia);
            if (preferible)
            {
                objetivo = v.transform;
                mejorDistancia = d;
                mejorEsHumano = v.esHumano;
            }
        }
    }

    bool HaySueloDelante(float d)
    {
        Bounds b = col.bounds;
        Vector2 origen = new Vector2(b.center.x + d * (b.extents.x + 0.3f), b.min.y + 0.1f);
        return Physics2D.Raycast(origen, Vector2.down, 1.5f, capaSuelo);
    }

    bool HayParedDelante(float d)
    {
        Bounds b = col.bounds;
        return Physics2D.Raycast(b.center, new Vector2(d, 0f), b.extents.x + 0.3f, capaSuelo);
    }

    bool HaySueloDebajo()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, 20f, capaSuelo);
    }
}