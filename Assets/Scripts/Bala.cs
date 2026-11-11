using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bala : MonoBehaviour
{
    public float tiempoVida = 2f;

    Vector2 direccion;
    float empuje;
    GameObject duenio;

    // Lo llama el arma justo después de crear la bala
    public void Iniciar(Vector2 dir, float velocidad, float fuerzaEmpuje, GameObject quienDispara)
    {
        direccion = dir;
        empuje = fuerzaEmpuje;
        duenio = quienDispara;

        GetComponent<Rigidbody2D>().velocity = dir * velocidad;

        if (dir.x < 0)
        {
            Vector3 e = transform.localScale;
            e.x *= -1;
            transform.localScale = e;
        }
        Destroy(gameObject, tiempoVida);
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.gameObject == duenio) return;   // no se empuja a sí mismo

        JugadorMovimiento j = otro.GetComponent<JugadorMovimiento>();
        if (j != null)
        {
            j.RecibirEmpuje(direccion.x * empuje);
            Destroy(gameObject);
        }
    }
}