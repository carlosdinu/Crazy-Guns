using UnityEngine;

/// <summary>
/// Peligro del mapa (sierra, pinchos...). Al tocarlo lanza al luchador
/// hacia arriba y en dirección contraria, como un golpe muy fuerte.
/// </summary>
public class Peligro : MonoBehaviour
{
    public float empujeHorizontal = 14f;
    public float impulsoVertical = 10f;

    void OnTriggerEnter2D(Collider2D otro)
    {
        JugadorMovimiento j = otro.GetComponent<JugadorMovimiento>();
        if (j == null) return;

        float lado = Mathf.Sign(otro.transform.position.x - transform.position.x);
        j.RecibirEmpuje(lado * empujeHorizontal);

        Rigidbody2D rb = otro.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(rb.velocity.x, impulsoVertical);

        // Si cae después del golpe, nadie se lleva los puntos
        VidasJugador v = otro.GetComponent<VidasJugador>();
        if (v != null) v.ultimoAtacante = null;
    }
}