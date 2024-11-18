using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class auto_mov : MonoBehaviour
{
    public float velocidad = 5f;
    public float velocidadReducidaLateral = 1f;
    public float velocidadReducidaTrasera = 2f;
    public float tiempoSuavizado = 0.5f;

    public float velocidadActual;
    public float velocidadObjetivo;

    //limites de la pantalla 
    //coordenadas x
    private float limiteDerecho;
    private float limiteIzquierdo;
    //coordenadas y
    private float limiteSuperior;
    private float limiteInferior;

    Vector2 nuevaPosicion;


    // Start is called before the first frame update
    void Start()
    {
        //configurar la velocidad actual
        velocidadActual = velocidad;
        velocidadObjetivo = velocidad;


        //coordenadas de los bordes de la pantalla
        limiteDerecho = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        limiteIzquierdo = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        limiteSuperior = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
        limiteInferior = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
    }

    // Update is called once per frame
    void Update()
    {
        LimitePantalla();

        //suaviza la velocidad del auto cuando este cambia de velocidad
        velocidadActual = Mathf.Lerp(velocidadActual, velocidadObjetivo, Time.deltaTime / tiempoSuavizado);

        //mueve el auto
        transform.Translate(Vector2.up * velocidadActual * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Auto"))
        {
            switch (collision.gameObject.name)
            {
                case "ZonaTrasera":
                    //Debug.Log("Entro a la zona trasera");
                    velocidadObjetivo = velocidadReducidaTrasera;
                    break;
                case "ZonaDerecha":
                    //Debug.Log("Entro a la zona Derecha");
                    velocidadObjetivo = velocidadReducidaLateral;
                    break;
                case "ZonaIzquierda":
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Auto"))
        {
            //Debug.Log("auto salio del trigger");
            velocidadObjetivo = velocidad;
        }
    }

    private void LimitePantalla()
    {
        //si el auto supera el limite derecho
        if (transform.position.x > limiteDerecho)
        {
            nuevaPosicion = transform.position;
            nuevaPosicion.x = limiteIzquierdo;
            transform.position = nuevaPosicion;
        }
        //si el auto supera el limite izquierdo
        if (transform.position.x < limiteIzquierdo)
        {
            nuevaPosicion = transform.position;
            nuevaPosicion.x = limiteDerecho;
            transform.position = nuevaPosicion;
        }


        //si el auto supera el limite superior
        if (transform.position.y > limiteSuperior)
        {
            nuevaPosicion = transform.position;
            nuevaPosicion.y = limiteInferior;
            transform.position = nuevaPosicion;
        }
        //si el auto supera el limite inferior
        if (transform.position.y < limiteInferior)
        {
            nuevaPosicion = transform.position;
            nuevaPosicion.y = limiteSuperior;
            transform.position = nuevaPosicion;
        }
    }
}
