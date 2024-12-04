using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using UnityEngine.Profiling;

public class Fps_cntr : MonoBehaviour
{
    //objetos de texto
    [SerializeField]
    public TextMeshProUGUI FPS;
    public TextMeshProUGUI FPSAvg;
    public TextMeshProUGUI frameTime;
    public TextMeshProUGUI FPSMax;
    public TextMeshProUGUI FPSMin;
    public TextMeshProUGUI FPSUnoP;
    public TextMeshProUGUI FPSCeroUnoP;
    public TextMeshProUGUI lblAuto;
    public TextMeshProUGUI Memoria;
    public TMP_InputField logText;


    public short fpsActual { get; private set; } = 0;
    public short mediaFps { get; private set; } = 0;
    private short fpsCeroUnoP = 0;
    private short fpsUnoP = 0;

    private List<float> listaFps = new List<float>();
    private short[] muestreoFps;
    private short[] muestreoFpsOrdenado;
    private short conteoMuesFps = 0;
    private short indexSample = 0;
    private short capMuestreoFps = 1024;
    private short muestrasUnP = 10;
    private short muestrasCeroUnP = 1;
    private long memoriaTotal;
    private float memoriaTotalMB;
    private float deltaTime = 0f;
    private float unescaledDeltaTime = 0f;
    private float fpsMax = 0f;
    private float fpsMin = 0f;
    private float fTimeActual = 0f;
    private float siguienteUpdate = 0.0f;
    private float tasaUpdate = 0.5f;
    private int tasaRefresco = 4;
    private float fpsSuavizado;
    private float fTimeSuavizado;
    private float tiempoEspera;
    int contadorAutos = 0;
    private string contenidoLog = "";

    readonly FrameTiming[] m_FrameTimings = new FrameTiming[1];


    // Start is called before the first frame update
    private IEnumerator Start()
    {

        init();
        QualitySettings.vSyncCount = 1;
        muestreoFps = new short[capMuestreoFps];
        muestreoFpsOrdenado = new short[capMuestreoFps];



        while (true)
        {
            //Conteo de fps actual
            fpsActual = (short)(Mathf.RoundToInt(1f / unescaledDeltaTime));
            //Conteo del Frame time del CPU
            fTimeActual = (float)m_FrameTimings[0].cpuFrameTime;
            //Tiempo transcurrido para la actualizacion de datos
            yield return new WaitForSeconds(0.1f);
        }


    }

    // Update is called once per frame
    private void Update()
    {
        CalcularMinMax();
        capturarFtiming();
        ContadorLabelUpdate();
        deltaTime += Time.unscaledDeltaTime;
        unescaledDeltaTime = Time.unscaledDeltaTime;

        // actualiza la media de fps
        uint mediaFpsAñadida = 0;
        indexSample++;

        //Debug.Log(indexSample);
        if (indexSample >= capMuestreoFps) indexSample = 0;

        muestreoFps[indexSample] = fpsActual;

        if (conteoMuesFps < capMuestreoFps)
        {
            conteoMuesFps++;
        }

        for (int i = 0; i < conteoMuesFps; i++)
        {
            mediaFpsAñadida += (uint)muestreoFps[i];
        }
        mediaFps = (short)((float)mediaFpsAñadida / (float)conteoMuesFps);


        //calcular el porcentaje de 0.1% y 1%
        if (Time.unscaledTime >= siguienteUpdate)
        {
            siguienteUpdate += tasaUpdate;

        }

        // 0.1% y 1% 
        muestreoFps.CopyTo(muestreoFpsOrdenado, 0);

        //ordena los fps para encontrar los porcentiles
        Array.Sort(muestreoFpsOrdenado, (x, y) => x.CompareTo(y));

        bool ceroComaUnoPCalculado = false; //0.1%

        uint fpsTotalAñadido = 0;

        short muesAtravesDeUnoP = conteoMuesFps < muestrasUnP ? conteoMuesFps : muestrasUnP;
        short muesAtravesDeCeroUnoP = conteoMuesFps < muestrasCeroUnP ? conteoMuesFps : muestrasCeroUnP;


        short muesParaComenzar = (short)(capMuestreoFps - conteoMuesFps);

        for (short i = muesParaComenzar; i < muesParaComenzar + muesAtravesDeUnoP; i++)
        {
            fpsTotalAñadido += (ushort)muestreoFpsOrdenado[i];

            if (!ceroComaUnoPCalculado && i >= muesAtravesDeCeroUnoP - 1)
            {
                ceroComaUnoPCalculado = true;

                fpsCeroUnoP = (short)((float)fpsTotalAñadido / (float)muestrasCeroUnP);
            }
        }

        fpsUnoP = (short)((float)fpsTotalAñadido / (float)muestrasUnP);

        //Calcula el total de memoria utilizada
        memoriaTotal = Profiler.GetTotalAllocatedMemoryLong();
        memoriaTotalMB = memoriaTotal / (1024f * 1024f);


        //el texto se actualizara por el valor dado en tasaRefresco por segundo
        if (deltaTime > 1f / tasaRefresco)
        {
            FPS.text = $"FPS: {Mathf.Round(fpsActual)}";
            frameTime.text = $"{fTimeActual:0.0} ms";
            FPSAvg.text = $"avg: {mediaFps}";
            FPSMax.text = $"max: {fpsMax}";
            FPSMin.text = $"min: {fpsMin}";
            FPSUnoP.text = $"1% bajo: {fpsUnoP}";
            FPSCeroUnoP.text = $"0.1% bajo: {fpsCeroUnoP}";
            Memoria.text = $"Memoria: {memoriaTotalMB:F2} MB";
        }

        //actualiza el log

        if(contadorAutos != 0 && contadorAutos % 50 == 0)
        {
            agregarDataLog();
        }

    }


    //calcular minimo y maximo de fps
    private void CalcularMinMax()
    {
        //inicializa los valores minimo y maximo con el primer valor del arreglo
        short max = muestreoFps[0];
        short min = muestreoFps[0];

        //recorre el arreglo para encontrar el minimo y el maximo
        foreach (short fps in muestreoFps)
        {
            if (fps > max)
            {
                fpsMax = fps;
            }
            if (fps < min)
            {
                fpsMin = fps;
            }
        }
    }

    private void agregarDataLog()
    {
        string nuevoRegistro =
            $"===========LOG============\n" +
            $"Objetos = {contadorAutos}\n" +
            $"FPS = {fpsActual}\n"+
            $"Frametime = {fTimeActual:0.0}\n" +
            $"FPS avg = {mediaFps}\n" +
            $"FPS max = {fpsMax}\n" +
            $"FPS min = {fpsMin}\n" +
            $"1% = {fpsUnoP}\n" +
            $"0.1% = {fpsCeroUnoP}\n" +
            $"Memoria = {memoriaTotalMB:F2}MB\n" +
            $"==========================";

        contenidoLog = nuevoRegistro;
        logText.text = contenidoLog;
    }



    public void ParametrosActualizados()
    {
        muestrasUnP = (short)(capMuestreoFps / 100);
        muestrasCeroUnP = (short)(capMuestreoFps / 1000);
    }

    private void ContadorLabelUpdate()
    {

        contadorAutos = GameObject.FindGameObjectsWithTag("Auto").Length / 5;

        if (lblAuto != null)
        {
            lblAuto.text = $"{contadorAutos}";
        }
    }

    private void init()
    {
        muestreoFps = new short[capMuestreoFps];
    }

    private void capturarFtiming()
    {
        FrameTimingManager.CaptureFrameTimings();
        FrameTimingManager.GetLatestTimings((uint)m_FrameTimings.Length, m_FrameTimings);
    }

}