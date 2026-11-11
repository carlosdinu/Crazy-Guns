using UnityEngine;

public class VidasJugador : MonoBehaviour
{
    public int vidas = 3;
    public Transform puntoReaparicion;

    Rigidbody2D rb;
    JugadorMovimiento movimiento;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movimiento = GetComponent<JugadorMovimiento>();
    }

    public void Morir()
    {
        vidas--;
        Debug.Log(name + " ha caído. Vidas restantes: " + vidas);

        if (vidas > 0)
            Reaparecer();
        else
            Eliminar();
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
        Debug.Log(name + " ha sido eliminado");
        gameObject.SetActive(false);
    }
}