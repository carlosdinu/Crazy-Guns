using UnityEngine;

/// <summary>
/// Fondo con efecto parallax: se mueve una fracción del movimiento de la cámara
/// (cuanto más lejos está la capa, más se acerca "efectoParallax" a 1) y además
/// puede desplazarse solo, como nubes. Se repite sin fin si se indica el tamaño del patrón.
/// </summary>
[DefaultExecutionOrder(100)]   // se ejecuta después de la cámara para evitar temblores
public class CapaParallax : MonoBehaviour
{
    [Range(0f, 1f)]
    public float efectoParallax = 0.85f;     // 0 = fija en el mundo, 1 = pegada a la cámara
    public Vector2 velocidadAuto = new Vector2(0.3f, 0f);   // movimiento propio
    public float anchoRepeticion = 4f;       // ancho del patrón (0 = no repetir)
    public float altoRepeticion = 0f;        // alto del patrón (0 = no repetir)

    Transform cam;
    Vector3 posInicial;
    Vector3 camInicial;
    Vector2 recorridoAuto;

    void Start()
    {
        cam = Camera.main.transform;
        posInicial = transform.position;
        camInicial = cam.position;
    }

    void LateUpdate()
    {
        recorridoAuto += velocidadAuto * Time.deltaTime;
        Vector3 deltaCam = cam.position - camInicial;

        float x = posInicial.x + deltaCam.x * efectoParallax + recorridoAuto.x;
        float y = posInicial.y + deltaCam.y * efectoParallax + recorridoAuto.y;

        // Se recoloca a saltos del tamaño del patrón para que nunca se le vea el borde
        if (anchoRepeticion > 0f)
            x += Mathf.Round((cam.position.x - x) / anchoRepeticion) * anchoRepeticion;
        if (altoRepeticion > 0f)
            y += Mathf.Round((cam.position.y - y) / altoRepeticion) * altoRepeticion;

        transform.position = new Vector3(x, y, posInicial.z);
    }
}