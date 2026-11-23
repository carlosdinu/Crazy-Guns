using UnityEngine;

/// <summary>Mueve el objeto de ida y vuelta entre dos puntos.</summary>
public class MovimientoPatrulla : MonoBehaviour
{
    public Transform puntoA;
    public Transform puntoB;
    public float velocidad = 3f;
    public float velocidadGiro = 360f;   // grados por segundo (0 = no gira)

    Transform destino;

    void Start()
    {
        transform.position = puntoA.position;
        destino = puntoB;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, destino.position, velocidad * Time.deltaTime);
        transform.Rotate(0f, 0f, -velocidadGiro * Time.deltaTime);

        if (Vector3.Distance(transform.position, destino.position) < 0.01f)
            destino = destino == puntoA ? puntoB : puntoA;
    }
}