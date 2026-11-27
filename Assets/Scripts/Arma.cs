using UnityEngine;

// Indica que la clase se puede serializar, es decir, se pueden editar desde el editor
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

/*
 *  Clase Arma, contiene la logica del arma, desde la tecla con la cual el jugador dispara hasta el control de la IA sobre
 *  el arma.
 * 
 */
public class Arma : MonoBehaviour
{
    public KeyCode tclDisparo = KeyCode.Space;
    public GameObject prefabBala;
    public Transform puntoDisparo;

    // ToolTip aparece cuando dejas el mouse en el editor sobre el boolean controladoPorIa
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
    HUDLuchador hud;

    public string NombreArma => actual.nombre;

    void Awake()
    {
        jugador = GetComponentInParent<JugadorMovimiento>();
        sr = GetComponent<SpriteRenderer>();
        puntoInicial = puntoDisparo.localPosition;
        VidasJugador vidas = GetComponentInParent<VidasJugador>();
        if (vidas != null) hud = vidas.hud;
        Equipar(armaInicial);
    }

    public void Equipar(DatosArma datos)
    {
        actual = datos;
        municionRestante = datos.municion;

        if (sr != null && datos.sprite != null)
            sr.sprite = datos.sprite;

        puntoDisparo.localPosition = datos.canon != Vector2.zero ? (Vector3)datos.canon : puntoInicial;

        NotificarHUD();
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

        // Si el arma tiene munición limitada, al acabarse se suelta y vuelve la inicial
        if (actual.municion > 0)
        {
            municionRestante--;
            if (municionRestante <= 0)
            {
                SoltarArma();
                Equipar(armaInicial);
            }
            else
            {
                NotificarHUD();
            }
        }
    }

    void NotificarHUD()
    {
        if (hud != null)
            hud.ActualizarArma(actual.sprite, actual.municion > 0 ? municionRestante : -1);
    }

    /// Crea una copia del arma que sale despedida girando y desaparece.
    void SoltarArma()
    {
        if (sr == null || sr.sprite == null) return;

        float dir = Mathf.Sign(jugador.transform.localScale.x);

        GameObject suelta = new GameObject("ArmaSoltada");
        suelta.transform.position = transform.position;
        suelta.transform.rotation = transform.rotation;
        suelta.transform.localScale = transform.lossyScale;   // mantiene hacia dónde miraba

        SpriteRenderer copia = suelta.AddComponent<SpriteRenderer>();
        copia.sprite = sr.sprite;
        copia.sortingLayerID = sr.sortingLayerID;
        copia.sortingOrder = sr.sortingOrder;

        // Sin collider: atraviesa el escenario mientras cae
        Rigidbody2D rb = suelta.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.velocity = new Vector2(-dir * 3f, 8f);   // hacia atrás y hacia arriba
        rb.angularVelocity = dir * 720f;            // dos vueltas por segundo

        suelta.AddComponent<Desvanecer>();
    }
}