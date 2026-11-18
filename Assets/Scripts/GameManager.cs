using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    public GameObject panelFin;
    public TextMeshProUGUI textoFin;
    public GameObject botonSiguiente;
    public string siguienteNivel = "";   // nombre de la escena del nivel 2

    VidasJugador[] luchadores;
    bool terminada;

    void Awake()
    {
        instancia = this;
        Time.timeScale = 1f;
    }

    void Start()
    {
        luchadores = FindObjectsOfType<VidasJugador>();
        panelFin.SetActive(false);
    }

    public void ComprobarFinPartida()
    {
        if (terminada) return;

        VidasJugador[] vivos = luchadores.Where(l => l.gameObject.activeSelf).ToArray();
        bool quedaHumano = vivos.Any(l => l.esHumano);

        if (vivos.Length > 1 && quedaHumano) return;   // la partida sigue

        terminada = true;
        VidasJugador ganador = vivos.Length == 1 ? vivos[0] : null;
        bool victoria = ganador != null && ganador.esHumano;

        if (victoria)
        {
            int total = ganador.puntos + ganador.vidas * 50;
            textoFin.text = "¡VICTORIA!\n" + ganador.nombreMostrado + " gana\nPuntuación: " + total;
        }
        else
        {
            VidasJugador humano = luchadores.FirstOrDefault(l => l.esHumano);
            int total = humano != null ? humano.puntos : 0;
            textoFin.text = "DERROTA\nPuntuación: " + total;
        }

        botonSiguiente.SetActive(victoria && siguienteNivel != "");
        panelFin.SetActive(true);
        Time.timeScale = 0f;   // congela el juego
    }

    public void Reintentar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SiguienteNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(siguienteNivel);
    }
}