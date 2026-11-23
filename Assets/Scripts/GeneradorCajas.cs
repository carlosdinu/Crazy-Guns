using UnityEngine;

/// <summary>Hace aparecer cajas de armas cada cierto tiempo en una posición aleatoria.</summary>
public class GeneradorCajas : MonoBehaviour
{
    public GameObject prefabCaja;
    public float intervalo = 8f;
    public float limiteIzq = -10f;
    public float limiteDer = 10f;
    public float alturaAparicion = 10f;
    public int maxCajas = 2;

    float siguiente;

    void Start()
    {
        siguiente = Time.time + intervalo * 0.5f;
    }

    void Update()
    {
        if (Time.time < siguiente) return;
        siguiente = Time.time + intervalo;

        if (FindObjectsOfType<CajaArma>().Length >= maxCajas) return;

        Vector3 pos = new Vector3(Random.Range(limiteIzq, limiteDer), alturaAparicion, 0f);
        Instantiate(prefabCaja, pos, Quaternion.identity);
    }

    // Dibuja en la escena la zona donde aparecen las cajas
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(limiteIzq, alturaAparicion), new Vector3(limiteDer, alturaAparicion));
    }
}