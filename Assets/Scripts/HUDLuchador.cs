using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Panel del HUD de un luchador: retrato, nombre, corazones de vida y puntos.
/// Cuando se pierde una vida, el corazón que se vacía hace un "latido".
/// </summary>
public class HUDLuchador : MonoBehaviour
{
    public Image retrato;
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoPuntos;
    public Image[] corazones;
    public Sprite corazonLleno;
    public Sprite corazonVacio;
    public Image iconoArma;
    public TMP_Text textoMunicion;

    int vidasMostradas = -1;

    public void Actualizar(string nombre, int vidas, int puntos, Sprite cara)
    {
        textoNombre.text = nombre;
        textoPuntos.text = puntos.ToString();

        if (retrato != null && cara != null)
            retrato.sprite = cara;

        for (int i = 0; i < corazones.Length; i++)
            corazones[i].sprite = i < vidas ? corazonLleno : corazonVacio;

        // Si ha perdido una vida, el corazón que se acaba de vaciar late
        if (vidasMostradas > vidas && vidas >= 0 && vidas < corazones.Length)
            StartCoroutine(Latido(corazones[vidas].transform));

        // Eliminado: retrato en gris
        if (retrato != null)
            retrato.color = vidas > 0 ? Color.white : new Color(0.4f, 0.4f, 0.4f, 1f);

        vidasMostradas = vidas;
    }

    public void ActualizarArma(Sprite icono, int municion)
    {
        if (iconoArma != null)
        {
            iconoArma.sprite = icono;
            iconoArma.enabled = icono != null;
        }

        if (textoMunicion != null)
        {
            textoMunicion.text = municion < 0 ? "∞" : municion.ToString();
            bool pocaMunicion = municion >= 0 && municion <= 3;
            textoMunicion.color = pocaMunicion ? new Color32(255, 82, 82, 255) : Color.white;
        }
    }

    IEnumerator Latido(Transform corazon)
    {
        const float duracion = 0.35f;
        for (float t = 0f; t < duracion; t += Time.unscaledDeltaTime)
        {
            float escala = 1f + 0.6f * Mathf.Sin(t / duracion * Mathf.PI);
            corazon.localScale = Vector3.one * escala;
            yield return null;
        }
        corazon.localScale = Vector3.one;
    }
}