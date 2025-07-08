using System;
using System.Collections.Generic;
using UnityEngine;
public enum Difficulty
{
    Beginner,
    Intermediate,
    Expert,
    Progressive1,
    Progressive2,
    PerTime,
    LightsReaction,
    LightsReaction2
}

public enum Mode
{
    Default,
    FixedShoots
}

public enum HandballType
{
    Handball,
    AdaptedHandball
}
public enum HandsType
{
    Both,
    Left,
    Right
}
public enum Hand
{
    Left,
    Right
}

public static class GameData
{
    //Variables para datos finales
    public static int n_paradas = 0;
    public static int n_goles = 0;
    public static int n_lanzamientos = 0;
    public static string t_serie = "";
    public static int n_luces = 0;

    //Variable para canones generados
    public static bool posicionInicialGenerada = false;
    public static GameObject[] canon;
}

public static class KeysConfiguration
{
    public static string k_mode = "mode";
    public static string k_level = "level";
    public static string k_handballadapted = "k_handballadapted";
    public static string k_desplazamientoCanon = "k_desplazamientoCanon";
    public static string k_rotacionPelota = "k_rotacionPelota";
    public static string k_curvaLanzamiento = "k_curvaLanzamiento";
    public static string k_distintosTiposPelotas = "k_distintosTiposPelotas";
    public static string k_manosUtilizadas = "k_manosUtilizadas";
    public static string k_golesObjetivo = "golesObjetivo";
    public static string k_golesObjetivoIndex = "k_golesObjetivoIndex";
    public static string k_tiempoObjetivo = "tiempoObjetivo";
    public static string k_tiempoObjetivoIndex = "k_tiempoObjetivoIndex";

    public static string k_canonCentral = "k_canonCentral";
    public static string k_canonPivote = "k_canonPivote";
    public static string k_canonLatIzq = "k_canonLatIzq";
    public static string k_canonLatDer = "k_canonLatDer";
    public static string k_canonExtIzq = "k_canonExtIzq";
    public static string k_canonExtDer = "k_canonExtDer";

    public static string k_canonEscuadraDerecha = "k_canonEscuadraDerecha";
    public static string k_canonBajoDerecha = "k_canonBajoDerecha";
    public static string k_canonCentroDerecha = "k_canonCentroDerecha";
    public static string k_canonCentroDerechaBajo = "k_canonCentroDerechaBajo";
    public static string k_canonCentro = "k_canonCentro";
    public static string k_canonCentroBajo = "k_canonCentroBajo";
    public static string k_canonCentroIzquierda = "k_canonCentroIzquierda";
    public static string k_canonCentroIzquierdaBajo = "k_canonCentroIzquierdaBajo";
    public static string k_canonEscuadraIzquierda = "k_canonEscuadraIzquierda";
    public static string k_canonBajoIzquierda = "k_canonBajoIzquierda";

    public static string k_tipoBola = "tipoBola";
    public static string k_idEjercicio = "idEjercicio";
    public static string k_puntoInicialLanzamiento = "puntoInicialLanzamiento";
    public static string k_id_puntoInicialLanzamiento = "id_puntoInicialLanzamiento ";
    public static string k_puntoFinalLanzamiento = "puntoFinalLanzamiento";
    public static string k_id_puntoFinalLanzamiento = "id_puntoFinalLanzamiento";
    public static string k_paradoNoParado = "paradoNoParado";
    public static string k_manoDeLaParada = "manoDeLaParada";
    public static string k_tiempoDisparo = "tiempoDisparo";
    public static string k_tiempoParadaOGol = "tiempoParadaOGol";
}
public static class HistoricalRegistry
{
    public static List<string> tipoBola = new List<string>();
    public static List<string> idBola = new List<string>();
    public static List<Vector3> puntoInicialLanzamiento = new List<Vector3>();      //Coordenadas del canon de lanzamiento
    public static List<string> id_puntoInicialLanzamiento = new List<string>();     //Identificador del canon usando para lanzar
    public static List<Vector3> puntoFinalLanzamiento = new List<Vector3>();        //Coordenadas del punto final de lanzamiento
    public static List<string> id_puntoFinalLanzamiento = new List<string>();       //Identificador del punto final de lanzamiento
    public static List<string> paradoNoParado = new List<string>();
    public static List<string> manoDeLaParada = new List<string>();
    public static List<string> tiempoDisparo = new List<string>();                  // Momento en el que se realiza el disparo
    public static List<string> tiempoParadaOGol = new List<string>();                // Registro del momento en el que se realiza la parada o el gol
    public static DateTime InicioSerie;
    public static DateTime FinalSerie;
    public static List<string> posicionesBola = new List<string>();
    public static List<string> tiemposReaccion = new List<string>();
}

public static class Constants_Handball
{
    public static Difficulty Difficulty = Difficulty.Beginner;
    public static Mode Mode = Mode.Default;
    public static HandballType HandballType = HandballType.Handball;
    public static HandsType handsType = HandsType.Both;

    public static Vector3 centerGoalPoint = new Vector3(14.9f, 1.5f, 0f); // Punto localizacion porteria
    public static int identificador = 1;
    public static int idEjercicio = 1;

    /*Flags opciones adicionales (debe coincidar con la opcion inicial de configuracion adicional )****/
    public static bool isActiveDesplazamientoCanon = true;
    public static bool isActiveRotacionPelota = true;
    public static bool isActiveCurvaLanzamiento = false;
    public static bool isActiveDistintosTiposPelotas = true;
    /**************************************************************************************************/

    public static bool deleteCanonFlag = false;

    public static float tiempoCuentaAtras
    {
        get
        {
            switch (Difficulty)
            {
                case Difficulty.Beginner:
                    return 5f;
                case Difficulty.Intermediate:
                    return 5f;
                case Difficulty.Expert:
                    return 5f;
                case Difficulty.Progressive1:
                    return 5f;
                case Difficulty.Progressive2:
                    return 5f;
                default:
                    return 5f;
            }
        }
    }

    public static float velocidadLanzamientoFija;
    public static float frecuenciaLanzamientoFija;

    public static float velocidadMinima
    {
        get
        {
            switch (Difficulty)
            {
                case Difficulty.Beginner:
                    return 3f; // Velocidad m�nima disparo para nivel f�cil
                case Difficulty.Intermediate:
                    return 5f; // Velocidad m�nima disparo para nivel intermedio
                case Difficulty.Expert:
                    return 7f; // Velocidad m�nima disparo para nivel dif�cil
                case Difficulty.Progressive1:
                    return velocidadMinimaModificadaProgresiva; // Velocidad m�nima disparo para nivel progresiva
                case Difficulty.Progressive2:
                    return velocidadMinimaModificadaProgresiva2; // Velocidad m�nima disparo para nivel progresiva
                case Difficulty.PerTime:
                    return velocidadMinimaModificadaProgresiva;
                default:
                    return 5f;
            }
        }
    }

    public static float velocidadMaxima
    {
        get
        {
            switch (Difficulty)
            {
                case Difficulty.Beginner:
                    return 4f; // Velocidad m�xima disparo para nivel f�cil
                case Difficulty.Intermediate:
                    return 6f; // Velocidad m�xima disparo para nivel intermedio
                case Difficulty.Expert:
                    return 9f; // Velocidad m�xima disparo para nivel dif�cil
                case Difficulty.Progressive1:
                    return velocidadMaximaModificadaProgresiva; // Velocidad m�xima disparo para nivel progresiva
                case Difficulty.Progressive2:
                    return velocidadMaximaModificadaProgresiva2; // Velocidad m�xima disparo para nivel progresiva
                case Difficulty.PerTime:
                    return velocidadMaximaModificadaProgresiva;
                default:
                    return 6f;
            }
        }
    }

    public static int bolasALanzar
    {
        get
        {
            switch (Difficulty)
            {
                case Difficulty.Beginner:
                    return 5; // Bolas a lanzar para nivel f�cil
                case Difficulty.Intermediate:
                    return 10; // Bolas a lanzar para nivel intermedio
                case Difficulty.Expert:
                    return 15; // Bolas a lanzar para nivel dif�cil
                case Difficulty.Progressive1:
                    return bolasALanzarModificadaProgresiva; // Bolas a lanzar para nivel progresiva
                case Difficulty.Progressive2:
                    return bolasALanzarModificadaProgresiva2; // Bolas a lanzar para nivel progresiva
                case Difficulty.PerTime:
                    return bolasALanzarModificadaProgresiva;
                case Difficulty.LightsReaction:
                    return 0;
                case Difficulty.LightsReaction2:
                    return 0;
                default:
                    return 4;
            }
        }
    }

    public static float frecuenciaLanzamiento
    {
        get
        {
            switch (Difficulty)
            {
                case Difficulty.Beginner:
                    return 3.8f;
                case Difficulty.Intermediate:
                    return 3.2f;
                case Difficulty.Expert:
                    return 2.6f;
                case Difficulty.Progressive1:
                    return frecuencaLanzamientoModificadaProgresiva;
                case Difficulty.Progressive2:
                    return frecuencaLanzamientoModificadaProgresiva2;
                case Difficulty.PerTime:
                    return frecuencaLanzamientoModificadaProgresiva;
                default:
                    return 3.8f;
            }
        }
    }

    public static float efectoLanzamientoMinimo
    {
        get
        {
            switch (Difficulty)
            {
                case Difficulty.Beginner:
                    return 0.0f;
                case Difficulty.Intermediate:
                    return 0.1f;
                case Difficulty.Expert:
                    return 0.2f;
                case Difficulty.Progressive1:
                    return efectoLanzamientoMinimoModificadaProgresiva;
                case Difficulty.Progressive2:
                    return efectoLanzamientoMinimoModificadaProgresiva2;
                case Difficulty.PerTime:
                    return efectoLanzamientoMinimoModificadaProgresiva;
                default:
                    return 0.5f;
            }
        }
    }

    public static float efectoLanzamientoMaximo
    {
        get
        {
            switch (Difficulty)
            {
                case Difficulty.Beginner:
                    return 0.6f;
                case Difficulty.Intermediate:
                    return 1.1f;
                case Difficulty.Expert:
                    return 1.7f;
                case Difficulty.Progressive1:
                    return efectoLanzamientoMaximoModificadaProgresiva;
                case Difficulty.Progressive2:
                    return efectoLanzamientoMaximoModificadaProgresiva2;
                case Difficulty.PerTime:
                    return efectoLanzamientoMaximoModificadaProgresiva;
                default:
                    return 0.5f;
            }
        }
    }

    // Valores iniciales dificultad progresiva
    public static float velocidadMinimaInicialProgresiva = 5f;
    public static float velocidadMaximaInicialProgresiva = 6f;
    public static int bolasALanzarInicialProgresiva = 500;
    public static float frecuencaLanzamientoInicialProgresiva = 3.8f;
    public static float efectoLanzamientoMinimoInicialProgresiva = 0.0f;
    public static float efectoLanzamientoMaximoInicialProgresiva = 0.6f;

    // Valores modificados dificultad progresiva
    public static float velocidadMinimaModificadaProgresiva = 5f;
    public static float velocidadMaximaModificadaProgresiva = 6f;
    public static int bolasALanzarModificadaProgresiva = 500;
    public static float frecuencaLanzamientoModificadaProgresiva = 3.8f;
    public static float efectoLanzamientoMinimoModificadaProgresiva = 0.0f;
    public static float efectoLanzamientoMaximoModificadaProgresiva = 0.6f;

    //Frencuencia de variacion en dificultad progresiva
    public static float variacionMinimaProgresiva = 0.05f;
    public static float variacionMaximaProgresiva = 0.2f;

    // Valores iniciales dificultad progresiva2
    public static float velocidadMinimaInicialProgresiva2 = 5f; //Para porteros de balonmano 6, para pacientes 5
    public static float velocidadMaximaInicialProgresiva2 = 6f; //Para porteros de balonmano 7, para pacientes 6
    public static int bolasALanzarInicialProgresiva2 = 500; //Para porteros de balonmano 500, para pacientes 500
    public static float frecuencaLanzamientoInicialProgresiva2 = 3.3f; //Para porteros de balonmano 3.3, para pacientes 3.5
    public static float efectoLanzamientoMinimoInicialProgresiva2 = 0.0f; //Para porteros de balonmano 0.0, para pacientes 0.04
    public static float efectoLanzamientoMaximoInicialProgresiva2 = 0.6f; //Para porteros de balonmano 0.0, para pacientes 0.04

    // Valores modificados dificultad progresiva2
    public static float velocidadMinimaModificadaProgresiva2 = 5f; //Para porteros de balonmano 6, para pacientes 5
    public static float velocidadMaximaModificadaProgresiva2 = 6f; //Para porteros de balonmano 7, para pacientes 6
    public static int bolasALanzarModificadaProgresiva2 = 500; //Para porteros de balonmano 500, para pacientes 500
    public static float frecuencaLanzamientoModificadaProgresiva2 = 3.3f; //Para porteros de balonmano 3.3, para pacientes 3.3
    public static float efectoLanzamientoMinimoModificadaProgresiva2 = 0.0f; //Para porteros de balonmano 0.0, para pacientes 0.0
    public static float efectoLanzamientoMaximoModificadaProgresiva2 = 0.6f; //Para porteros de balonmano 0.0, para pacientes 0.0

    //Frencuencia de variacion en dificultad progresiva2
    public static float variacionMinimaProgresiva2 = 0.05f;
    public static float variacionMaximaProgresiva2 = 0.2f;
    public static float variacionMinimaProgresiva2bis = 0.05f; //Para porteros de balonmano 0.05, para pacientes 0.05
    public static float variacionMaximaProgresiva2bis = 0.13f; //Para porteros de balonmano 0.17, para pacientes 0.13

    // RANGO PORTERIA
    public static float Ymax //Altura de la porteria dependiente de balonmano adaptado
    {
        get
        {
            switch (HandballType)
            {
                case HandballType.Handball:
                    return 2.000f;
                case HandballType.AdaptedHandball:
                    return 1.600f;
                default:
                    return 2.000f;
            }
        }
    }

    public static float Ymin //Altura minima de lanzamiento dependiente de balonmano adaptado
    {
        get
        {
            switch (HandballType)
            {
                case HandballType.Handball:
                    return 1.000f;
                case HandballType.AdaptedHandball:
                    return 0.700f;
                default:
                    return 1.000f;
            }
        }
    }

    public static float Zmax = 1.5f;
    public static float Zmin = -1.5f;
    public static float Xminmax = 15.75f;

    //POSICIONES FINALES DE DISPARO PARA BALONMANO
    public static Vector3[] HandballShootPositions = new Vector3[]
        {
            new Vector3(Xminmax, 1.75f, 1.2f),  // EscuadraDerecha
            new Vector3(Xminmax, 1.25f, 1.2f),  // BajoDerecha
            new Vector3(Xminmax, 1.75f, 0.6f),  // CentroDerecha
            new Vector3(Xminmax, 1.25f, 0.6f),  // CentroDerechaBajo
            new Vector3(Xminmax, 1.75f, 0.0f),  // Centro
            new Vector3(Xminmax, 1.25f, 0.0f),  // CentroBajo
            new Vector3(Xminmax, 1.75f, -0.6f), // CentroIzquierda
            new Vector3(Xminmax, 1.25f, -0.6f), // CentroIzquierdaBajo
            new Vector3(Xminmax, 1.75f, -1.2f), // EscuadraIzquierda
            new Vector3(Xminmax, 1.25f, -1.2f), // BajoIzquierda
        };

    //POSICIONES FINALES DE DISPARO PARA BALONMANO ADAPTADO
    public static Vector3[] AdaptedHandballShootPositions = new Vector3[]
        {
            new Vector3(Xminmax, 1.375f, 0.77f),  // EscuadraDerecha
            new Vector3(Xminmax, 0.925f, 0.77f),  // BajoDerecha
            new Vector3(Xminmax, 1.375f, 0.25f),  // CentroDerecha
            new Vector3(Xminmax, 0.925f, 0.25f),  // CentroDerechaBajo
            new Vector3(Xminmax, 1.375f, 0.0f),  // Centro
            new Vector3(Xminmax, 0.925f, 0.0f),  // CentroBajo
            new Vector3(Xminmax, 1.375f, -0.25f), // CentroIzquierda
            new Vector3(Xminmax, 0.925f, -0.25f), // CentroIzquierdaBajo
            new Vector3(Xminmax, 1.375f, -0.77f), // EscuadraIzquierda
            new Vector3(Xminmax, 0.925f, -0.77f), // BajoIzquierda
        };

    public static string[] nameShootPositions = new string[]
    {
        "EscuadraDerecha",
        "BajoDerecha",
        "CentroDerecha",
        "CentroDerechaBajo",
        "Centro",
        "CentroBajo",
        "CentroIzquierda",
        "CentroIzquierdaBajo",
        "EscuadraIzquierda",
        "BajoIzquierda",
    };

    public static float MaxVariationY = 0.12f; //M�xima variaci�n del punto final de disparo en Y
    public static float MaxVariationZ = 0.18f; //M�xima variaci�n del punto final de disparo en Z
    public static float MaxVariationYInAdaptedHandball = 0.12f; //Maxima variacion del punto final de disparo en Y en balonmano adaptado
    public static float MaxVariationZInAdaptedHandball = 0.18f; //Maxima variacion del punto final de disparo en Z en balonmano adaptado

    //POSICIONES LANZADORES DE BOLAS
    public static Vector3 extIzqPosition = new Vector3(10.41f, 1.15f, -6.84f); // Posicion extremo izquierdo
    public static Vector3 latIzqPosition = new Vector3(7.72f, 1.15f, -4.55f);  // Posicion lateral izquierdo
    public static Vector3 centralPosition = new Vector3(4.5f, 1.15f, 0.0f); // Posicion central
    public static Vector3 pivotPosition = new Vector3(7.5f, 1.15f, 1.08f);    // Posici�n pivote
    public static Vector3 latDerPosition = new Vector3(8.81f, 1.15f, 6.01f);   // Posici�n lateral derecho
    public static Vector3 extDerPosition = new Vector3(12.11f, 1.15f, 7.55f);  // Posici�n extremo derecho

    //LANZADOR DE BOLAS ACTIVADO/DESACTIVADO
    public static bool centralCanonActivated = true; // Activado canon central
    public static bool pivotCanonActivated = false;  // Activado canon pivote
    public static bool latIzqCanonActivated = false; // Activado canon lateral izquierdo
    public static bool latDerCanonActivated = false; // Activado canon lateral derecho
    public static bool extIzqCanonActivated = false; // Activado canon extremo izquierdo
    public static bool extDerCanonActivated = false; // Activado canon extremo derecho

    //LANZADOR DE BOLAS POR POSICION DE LANZAMIENTO ACTIVADO/DESACTIVADO
    public static bool EscuadraDerechaCanonActivated = false;
    public static bool BajoDerechaCanonActivated = false;
    public static bool CentroDerechaCanonActivated = false;
    public static bool CentroDerechaBajoCanonActivated = false;
    public static bool CentroCanonActivated = true;
    public static bool CentroBajoCanonActivated = false;
    public static bool CentroIzquierdaCanonActivated = false;
    public static bool CentroIzquierdaBajoCanonActivated = false;
    public static bool EscuadraIzquierdaCanonActivated = false;
    public static bool BajoIzquierdaCanonActivated = false;

    //PROBABILIDAD TAMA�O PELOTAS EN DIFICULTAD PROGRESIVA 2
    public static float handballBallSize1Probability = 0.5f;
    public static float handballBallSize2Probability = 0.35f;
    public static float handballBallSize3Probability = 0.15f;

    public static int n_limitToHandballSizes = 10;
    public static int n_limit_goles_index = 2;
    public static int n_limit_goles = 15;

    public static DateTime seriesDate;

    public static int n_limit_time = 60;
    public static int n_limit_time_index = 0;
}