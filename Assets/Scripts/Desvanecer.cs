using UnityEngine;

/// <summary>
/// Hace que un sprite desaparezca poco a poco y después destruye el objeto.
/// Se usa para el arma que se suelta al quedarse sin munición.
/// </summary>
public class Desvanecer : MonoBehaviour
{
    public float duracion = 1.2f;

    SpriteRenderer sr;
    float inicio;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        inicio = Time.time;
    }

    void Update()
    {
        float t = (Time.time - inicio) / duracion;

        // Visible del todo la primera mitad; luego se va desvaneciendo
        Color c = sr.color;
        c.a = Mathf.Clamp01(2f - 2f * t);
        sr.color = c;

        if (t >= 1f)
            Destroy(gameObject);
    }
}