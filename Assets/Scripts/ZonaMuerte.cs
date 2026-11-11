using UnityEngine;

public class ZonaMuerte : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D otro)
    {
        VidasJugador v = otro.GetComponent<VidasJugador>();
        if (v != null)
            v.Morir();
    }
}