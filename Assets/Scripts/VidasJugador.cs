using UnityEngine;
using TMPro;

public class VidasJugador : MonoBehaviour
{
    public string nombreMostrado = "Jugador 1";
    public bool esHumano = true;
    public int vidas = 3;
    public Transform puntoReaparicion;
    public TextMeshProUGUI textoHUD;

    [HideInInspector] public int puntos;
    [HideInInspector] public VidasJugador ultimoAtacante;

    Rigidbody2D rb;
    JugadorMovimiento movimiento;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movimiento = GetComponent<JugadorMovimiento>();
    }

    void Start()
    {
        ActualizarHUD();
    }

    public void Morir()
    {
        if (ultimoAtacante != null && ultimoAtacante != this)
            ultimoAtacante.SumarPuntos(100);
        ultimoAtacante = null;

        vidas--;
        ActualizarHUD();

        if (vidas > 0)
            Reaparecer();
        else
            Eliminar();
    }

    public void SumarPuntos(int cantidad)
    {
        puntos += cantidad;
        ActualizarHUD();
    }

    void Reaparecer()
    {
        rb.velocity = Vector2.zero;
        rb.position = puntoReaparicion.position;
        transform.position = puntoReaparicion.position;
        movimiento.ReiniciarEmpuje();
    }

    void Eliminar()
    {
        gameObject.SetActive(false);
        GameManager.instancia.ComprobarFinPartida();
    }

    void ActualizarHUD()
    {
        if (textoHUD != null)
            textoHUD.text = nombreMostrado + "\nVidas: " + vidas + "\nPuntos: " + puntos;
    }
}