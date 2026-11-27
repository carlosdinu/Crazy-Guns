using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("Fin de partida")]
    public GameObject panelFin;
    public TMP_Text textoFin;
    public GameObject botonSiguiente;
    public string siguienteNivel = "";   // nombre de la escena del siguiente nivel

    [Header("Celebración")]
    public TMP_Text textoGrande;                 // "¡VICTORIA!" / "DERROTA"
    public float zoomCelebracion = 3f;           // cuanto más bajo, más cerca
    public float camaraLenta = 0.3f;             // velocidad del juego durante la celebración
    public float duracionCelebracion = 2.5f;     // segundos reales antes del panel

    VidasJugador[] luchadores;
    CamaraDinamica camara;
    bool terminada;

    VidasJugador[] vivos;
    VidasJugador ganador;
    bool victoria;

    [Header("Panel fin")]
    public TMP_Text textoReiniciar;
    public TMP_Text textoCondicion;

    [Header("Puntuacion")]
    string titulo;
    int eliminaciones;
    int vidasRestantes;
    int total;


    void Awake()
    {
        instancia = this;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    void Start()
    {
        luchadores = FindObjectsOfType<VidasJugador>();
        camara = Camera.main.GetComponent<CamaraDinamica>();
        panelFin.SetActive(false);
        if (textoGrande != null) textoGrande.gameObject.SetActive(false);
    }

    public void ComprobarFinPartida()
    {
        if (terminada) return;

        vivos = luchadores.Where(l => l.gameObject.activeSelf).ToArray();
        bool quedaHumano = vivos.Any(l => l.esHumano);

        if (vivos.Length > 1 && quedaHumano) return;   // la partida sigue

        terminada = true;
        ganador = vivos.Length == 1 ? vivos[0] : null;
        victoria = ganador != null && ganador.esHumano;

        // Puntuación: 100 por eliminación + 50 por vida restante (solo si ganas)
        VidasJugador jugador = luchadores.FirstOrDefault(l => l.esHumano);
        eliminaciones = jugador != null ? jugador.puntos / 100 : 0;
        vidasRestantes = victoria ? ganador.vidas : 0;
        total = eliminaciones * 100 + vidasRestantes * 50;

        // En la derrota se enfoca al enemigo que queda en pie
        VidasJugador enfocado = ganador != null ? ganador : vivos.FirstOrDefault();
        StartCoroutine(Celebracion(victoria, enfocado));
    }

    IEnumerator Celebracion(bool victoria, VidasJugador enfocado)
    {
        // Cámara lenta (la física también va más despacio para que siga siendo fluida)
        Time.timeScale = camaraLenta;
        Time.fixedDeltaTime = 0.02f * camaraLenta;

        if (camara != null && enfocado != null)
            camara.Enfocar(enfocado.transform, zoomCelebracion);

        if (textoGrande != null)
        {
            textoGrande.text = victoria ? "¡VICTORIA!" : "DERROTA";
            textoGrande.color = victoria ? new Color32(255, 213, 79, 255) : new Color32(255, 82, 82, 255);
            textoGrande.gameObject.SetActive(true);
            yield return AnimarEntrada(textoGrande);
        }

        yield return new WaitForSecondsRealtime(duracionCelebracion);

        if (textoGrande != null) textoGrande.gameObject.SetActive(false);
        botonSiguiente.SetActive(victoria && siguienteNivel != "");
        ComprobarPanelFin();
        Time.timeScale = 0f;   // congela el juego
    }

    // El texto entra muy grande y transparente y "cae" a su tamaño con un rebote
    IEnumerator AnimarEntrada(TMP_Text texto)
    {
        const float duracion = 0.5f;
        for (float t = 0f; t < duracion; t += Time.unscaledDeltaTime)
        {
            float k = t / duracion;
            texto.transform.localScale = Vector3.one * Mathf.LerpUnclamped(3f, 1f, RebotarSalida(k));
            texto.alpha = k;
            yield return null;
        }
        texto.transform.localScale = Vector3.one;
        texto.alpha = 1f;
    }

    static float RebotarSalida(float x)
    {
        const float c1 = 1.70158f, c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }

    public void Reintentar()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (SceneManager.GetActiveScene().name.Equals("Nivel5") && victoria)
        {
            SceneManager.LoadScene("Nivel1");
        } else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void SiguienteNivel()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        SceneManager.LoadScene(siguienteNivel);
    }

    public void ComprobarPanelFin()
    {
        panelFin.SetActive(true);

        if (SceneManager.GetActiveScene().name.Equals("Nivel5") && victoria)
        {
            textoReiniciar.text = "Volver al nivel 1";
            titulo = victoria ? "<size=150%><color=#FFD54F>¡Juego Completado!</color></size>" : "<size=150%><color=#FFD54F>¡DERROTA!</color></size>";

            textoFin.text = titulo + "\n" +
                        "<size=70%>Eliminaciones: " + eliminaciones + " × 100\n" +
                        "Vidas restantes: " + vidasRestantes + " × 50</size>\n" +
                        "<color=#90CAF9>Puntuación: " + total + "</color>";
        } else
        {
            titulo = victoria
            ? "<size=150%><color=#FFD54F>¡VICTORIA!</color></size>"
            : "<size=150%><color=#FF5252>DERROTA</color></size>";

            textoFin.text = titulo + "\n" +
                        "<size=70%>Eliminaciones: " + eliminaciones + " × 100\n" +
                        "Vidas restantes: " + vidasRestantes + " × 50</size>\n" +
                        "<color=#90CAF9>Puntuación: " + total + "</color>";
        }
    }
}