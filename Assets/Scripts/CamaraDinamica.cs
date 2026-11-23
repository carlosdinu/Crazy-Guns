using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CamaraDinamica : MonoBehaviour
{
    [Header("Encuadre")]
    public float margen = 2f;            // espacio extra alrededor de los luchadores
    public float zoomMinimo = 5f;        // lo más cerca que se acerca
    public float zoomMaximo = 9f;        // lo más lejos que se aleja
    public Vector2 desplazamiento = new Vector2(0f, 1f);   // sube un poco el encuadre
    public float suavizado = 0.35f;      // más alto = más lenta y suave

    [Header("Límites del mapa")]
    public bool limitar = true;
    public Vector2 limiteMin = new Vector2(-16f, -7f);
    public Vector2 limiteMax = new Vector2(16f, 10f);

    Camera cam;
    VidasJugador[] luchadores;
    Vector3 velocidadPos;
    float velocidadZoom;

    Transform enfoque;
    float zoomEnfoque;

    void Start()
    {
        cam = GetComponent<Camera>();
        luchadores = FindObjectsOfType<VidasJugador>();
    }

    void LateUpdate()
    {
        // Modo enfoque: zoom sobre un luchador. Usa tiempo real para funcionar en cámara lenta.
        if (enfoque != null)
        {
            Vector3 destino = enfoque.position + (Vector3)(desplazamiento * 0.5f);
            destino.z = transform.position.z;
            transform.position = Vector3.SmoothDamp(transform.position, destino, ref velocidadPos,
                                                    0.25f, Mathf.Infinity, Time.unscaledDeltaTime);
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoomEnfoque, ref velocidadZoom,
                                                    0.25f, Mathf.Infinity, Time.unscaledDeltaTime);
            return;
        }

        // Caja que contiene a todos los luchadores vivos
        bool hayAlguno = false;
        Bounds caja = new Bounds();

        foreach (VidasJugador l in luchadores)
        {
            if (l == null || !l.gameObject.activeInHierarchy) continue;

            Vector3 p = l.transform.position;
            if (limitar)   // el que cae al vacío no arrastra la cámara
            {
                p.x = Mathf.Clamp(p.x, limiteMin.x, limiteMax.x);
                p.y = Mathf.Clamp(p.y, limiteMin.y, limiteMax.y);
            }

            if (!hayAlguno) { caja = new Bounds(p, Vector3.zero); hayAlguno = true; }
            else caja.Encapsulate(p);
        }
        if (!hayAlguno) return;

        // Zoom necesario para que quepan todos (en alto y en ancho)
        float zoom = Mathf.Max(caja.extents.y + margen, (caja.extents.x + margen) / cam.aspect);
        zoom = Mathf.Clamp(zoom, zoomMinimo, zoomMaximo);

        Vector3 objetivo = caja.center + (Vector3)desplazamiento;
        if (limitar)
        {
            objetivo.x = LimitarCentro(objetivo.x, limiteMin.x, limiteMax.x, zoom * cam.aspect);
            objetivo.y = LimitarCentro(objetivo.y, limiteMin.y, limiteMax.y, zoom);
        }
        objetivo.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position, objetivo, ref velocidadPos, suavizado);
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoom, ref velocidadZoom, suavizado);
    }

    // Mantiene la vista dentro de los límites; si el mapa es más pequeño que la vista, la centra
    static float LimitarCentro(float valor, float min, float max, float mitadVista)
    {
        if (max - min <= mitadVista * 2f) return (min + max) * 0.5f;
        return Mathf.Clamp(valor, min + mitadVista, max - mitadVista);
    }

    /// Deja de seguir a todos y hace zoom sobre un solo luchador (fin de partida).
    public void Enfocar(Transform objetivo, float zoom)
    {
        enfoque = objetivo;
        zoomEnfoque = zoom;
    }

    // Dibuja los límites del mapa en la escena
    void OnDrawGizmosSelected()
    {
        if (!limitar) return;
        Gizmos.color = Color.cyan;
        Vector3 centro = (limiteMin + limiteMax) * 0.5f;
        Gizmos.DrawWireCube(centro, limiteMax - limiteMin);
    }
}