using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Spawner_autos : MonoBehaviour
{
    public GameObject prefabAuto;
    public float intervaloSpawn = 0.2f;
    public float velMinima = 4f;
    public float velMaxima = 6f;
    public float rotacionMin = 0f;
    public float rotacionMax = 360f;
    public TextMeshProUGUI contadorAutosText;

    private float nextSpawnTime;
    private int contadorAutos = 0;



    // Start is called before the first frame update
    void Start()
    {
        //ecuentra el objeto de texto en la escena
        GameObject textoObj = GameObject.Find("AutosNumero");
        if (textoObj != null)
        {
            contadorAutosText = textoObj.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.Log("no se encontro objeto de texto");
        }


        //configura el tiempo inicial para el primer spawn
        nextSpawnTime = intervaloSpawn;

        //Actualiza el label al iniciar
        contadorAutosUpdate();

    }

    // Update is called once per frame
    void Update()
    {
        //temporizador para crear un nuevo auto
        nextSpawnTime -= Time.unscaledDeltaTime;
        float fps = (short)(Mathf.RoundToInt(1f / Time.unscaledDeltaTime));

        if (fps > 30)
        {
            if (nextSpawnTime <= 0f)
            {
                GenerarAuto();
                //restablece el temporizador
                nextSpawnTime = intervaloSpawn;
            }
        }
        else 
        {
            Debug.Log("ya no se generan más objetos por limite de fps");
        }
        
    }

    private void GenerarAuto()
    {
        //genera el auto en la posicion del spawner
        GameObject nuevoAuto = Instantiate(prefabAuto, transform.position, Quaternion.identity);

        //asigna una rotacion aleatoria
        float rotacionAleatoria = Random.Range(rotacionMin, rotacionMax);
        nuevoAuto.transform.rotation = Quaternion.Euler(0f, 0f, rotacionAleatoria);

        //asigna una velocidad aleatoria al script del auto
        auto_mov autoMov = nuevoAuto.GetComponent<auto_mov>();

        if (autoMov != null)
        {
            autoMov.velocidad = Random.Range(velMinima, velMaxima);
        }

        //color aleatorio al gener un nuevo auto
        SpriteRenderer spriteRenderer = nuevoAuto.GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null) 
        {
            float tonoAleatorio = Random.Range(0f, 1f);
            float saturacion = 0.38f;
            float brillo = 1f;
            Color colorAleatorio = Color.HSVToRGB(tonoAleatorio, saturacion, brillo);
            spriteRenderer.color = colorAleatorio;
        }

    }

    private void contadorAutosUpdate()
    {
        if (contadorAutosText != null)
        {
            contadorAutosText.text = $"{contadorAutos}";
        }
    }
}
