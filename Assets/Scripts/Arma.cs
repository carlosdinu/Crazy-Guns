using UnityEngine;

[System.Serializable]
public class DatosArma
{
    public string nombre = "Pistola";
    public Sprite sprite;                // dibujo del arma
    public float cadencia = 0.25f;       // segundos entre disparos
    public float velocidadBala = 20f;
    public float empuje = 8f;            // empuje al rival
    public float retroceso = 2f;         // empuje hacia atrás al que dispara
    public int balasPorDisparo = 1;
    public float dispersion = 0f;        // grados de desviación de cada bala
    public int municion = 0;             // 0 = infinita
    public Vector2 canon;                // punta del cañón (0,0 = usar PuntoDisparo)
}

public class Arma : MonoBehaviour
{
    public KeyCode tclDisparo = KeyCode.Space;
    public GameObject prefabBala;
    public Transform puntoDisparo;

    [Tooltip("Si está marcado, dispara cuando BotIA lo ordena")]
    public bool controladoPorIA = false;
    [HideInInspector] public bool disparoIA;

    public DatosArma armaInicial = new DatosArma();

    DatosArma actual;
    int municionRestante;
    JugadorMovimiento jugador;
    SpriteRenderer sr;
    Vector3 puntoInicial;
    float siguienteDisparo;

    public string NombreArma => actual.nombre;

    void Awake()
    {
        jugador = GetComponentInParent<JugadorMovimiento>();
        sr = GetComponent<SpriteRenderer>();
        puntoInicial = puntoDisparo.localPosition;
        Equipar(armaInicial);
    }

    public void Equipar(DatosArma datos)
    {
        actual = datos;
        municionRestante = datos.municion;

        if (sr != null && datos.sprite != null)
            sr.sprite = datos.sprite;

        puntoDisparo.localPosition = datos.canon != Vector2.zero ? (Vector3)datos.canon : puntoInicial;
    }

    void Update()
    {
        bool quiereDisparar = controladoPorIA ? disparoIA : Input.GetKey(tclDisparo);

        if (quiereDisparar && Time.time >= siguienteDisparo)
        {
            Disparar();
            siguienteDisparo = Time.time + actual.cadencia;
        }
    }

    void Disparar()
    {
        float dir = Mathf.Sign(jugador.transform.localScale.x);

        for (int i = 0; i < actual.balasPorDisparo; i++)
        {
            float angulo = Random.Range(-actual.dispersion, actual.dispersion);
            Vector2 direccion = Quaternion.Euler(0f, 0f, angulo) * new Vector2(dir, 0f);

            GameObject bala = Instantiate(prefabBala, puntoDisparo.position, Quaternion.Euler(0f, 0f, angulo));
            bala.GetComponent<Bala>().Iniciar(direccion, actual.velocidadBala, actual.empuje, jugador.gameObject);
        }

        jugador.RecibirEmpuje(-dir * actual.retroceso);

        // Si el arma tiene munición limitada, al acabarse vuelve a la inicial
        if (actual.municion > 0)
        {
            municionRestante--;
            if (municionRestante <= 0)
                Equipar(armaInicial);
        }
    }
}