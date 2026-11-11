using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class JugadorMovimiento : MonoBehaviour
{
    public KeyCode tclIzq = KeyCode.A;
    public KeyCode tclDer = KeyCode.D;
    public KeyCode tclSalto = KeyCode.W;
    public KeyCode tclBajar = KeyCode.S;

    public float velocidad = 7f;
    public float fuerzaSalto = 12f;
    public int saltosMaximos = 2;

    public LayerMask capaSuelo;

    public float frenadoEmpuje = 15f;
    float empujeX;

    Rigidbody2D rb;
    Collider2D col;
    Animator anim;
    float direccion;
    int saltosRestantes;
    bool enSuelo;
    bool quiereSaltar;
    bool mirandoDerecha = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        direccion = 0f;
        if (Input.GetKey(tclIzq)) direccion -= 1f;
        if (Input.GetKey(tclDer)) direccion += 1f;
        if (Input.GetKeyDown(tclSalto)) quiereSaltar = true;
        if (Input.GetKeyDown(tclBajar) && enSuelo)
            IntentarBajar();

        if ((direccion > 0 && !mirandoDerecha) || (direccion < 0 && mirandoDerecha))
            Girar();

        if (anim != null)
        {
            anim.SetFloat("VelocidadX", Mathf.Abs(direccion));
            anim.SetFloat("VelocidadY", rb.velocity.y);
            anim.SetBool("EnSuelo", enSuelo);
        }
    }

    void FixedUpdate()
    {
        Bounds b = col.bounds;
        Vector2 pies = new Vector2(b.center.x, b.min.y);
        Vector2 tamano = new Vector2(b.size.x * 0.9f, 0.1f);
        enSuelo = Physics2D.OverlapBox(pies, tamano, 0f, capaSuelo);

        if (enSuelo && rb.velocity.y <= 0.01f)
            saltosRestantes = saltosMaximos;

        rb.velocity = new Vector2(direccion * velocidad + empujeX, rb.velocity.y);
        empujeX = Mathf.MoveTowards(empujeX, 0f, frenadoEmpuje * Time.fixedDeltaTime);

        if (quiereSaltar && saltosRestantes > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);
            saltosRestantes--;
        }
        quiereSaltar = false;
    }

    void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    void OnDrawGizmosSelected()
    {
        Collider2D c = GetComponent<Collider2D>();
        if (c == null) return;
        Bounds b = c.bounds;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(b.center.x, b.min.y), new Vector3(b.size.x * 0.9f, 0.1f));
    }

    public void RecibirEmpuje(float fuerza)
    {
        empujeX += fuerza;
    }

    public void ReiniciarEmpuje()
    {
        empujeX = 0f;
    }

    void IntentarBajar()
    {
        Bounds b = col.bounds;
        Collider2D debajo = Physics2D.OverlapBox(
            new Vector2(b.center.x, b.min.y), new Vector2(b.size.x * 0.9f, 0.1f), 0f, capaSuelo);

        if (debajo != null && debajo.GetComponent<PlatformEffector2D>() != null)
            StartCoroutine(AtravesarPlataforma(debajo));
    }

    IEnumerator AtravesarPlataforma(Collider2D plataforma)
    {
        Physics2D.IgnoreCollision(col, plataforma, true);
        yield return new WaitForSeconds(0.4f);
        Physics2D.IgnoreCollision(col, plataforma, false);
    }
}