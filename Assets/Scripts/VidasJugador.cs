using UnityEngine;

public class VidasJugador : MonoBehaviour
{
    public string nombreMostrado = "Jugador 1";
    public bool esHumano = true;
    public int vidas = 3;
    public Transform puntoReaparicion;
    public HUDLuchador hud;

    [HideInInspector] public int puntos;
    [HideInInspector] public VidasJugador ultimoAtacante;

    Rigidbody2D rb;
    JugadorMovimiento movimiento;
    Sprite retrato;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movimiento = GetComponent<JugadorMovimiento>();

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) retrato = sr.sprite;   // primer frame = retrato del HUD
    }

    void Start()
    {
        ActualizarHUD();
    }

    public void Morir()
    {
        // El último que te disparó se lleva los puntos
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
        if (GameManager.instancia != null)
            GameManager.instancia.ComprobarFinPartida();
    }

    void ActualizarHUD()
    {
        if (hud != null)
            hud.Actualizar(nombreMostrado, vidas, puntos, retrato);
    }
}