using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    public GameObject handballBall;      // Bola de balonmano generada de tama�o 1
    public GameObject handballBallSize2; // Bola de balonmano generada de tama�o 2
    public GameObject handballBallSize3; // Bola de balonmano generada de tama�o 3

    [SerializeField] Text countdownTextRestBalls;
    [SerializeField] AudioClip CanonShootSound;

    private int bolasRestantes = 0;                 //Bolas restantes en la serie de lanzamientos
    private AudioSource audioSource;
    private GameObject handballBallInstance;        // Referencia de la bola de balonmano generada
    public CanvasController canvasController;      // Referencia al canvas de las interfaces UI
    private GameUtilsManager gameUtilsManager;      // Referencia al controlador util del juego

    private float tiempoInicio;                     // Variable para almacenar el tiempo de inicio
    private float tiempoFin;                        // Variable para almacenar el tiempo de finalizaci�n

    private bool _isFixedShoots;

    [HideInInspector]
    [System.NonSerialized]
    public bool lastShootProgressive = false;       // Variable para gestionar el ultimo lanzamiento de dificultad progresiva
    private bool allowShootProgressive2 = true;
    private TrackingDataExecutorController trackingDataExecutorController;
    void Start()
    {
        //        canvasController = FindObjectOfType<CanvasController>(); 
        audioSource = GetComponent<AudioSource>();
        gameUtilsManager = FindObjectOfType<GameUtilsManager>();
        trackingDataExecutorController = FindObjectOfType<TrackingDataExecutorController>(); 
    }

    void GenerarObjeto()
    {
        bolasRestantes--;

        if (bolasRestantes < 0)                     //Si el contador de bolsa lanzadas llega a 0 se cancela el lanzamiento de bolas, en caso contrario se genera un nuevo lanzamiento
        {
            if (lastShootProgressive)               // En dificultad progresiva generar un �ltimo lanzamiento antes de finalizar la serie
            {
               // generateShoot();
                lastShootProgressive = false;
            }
            else
            {
                CancelInvoke("GenerarObjeto");      // En caso general cancelar la repetecion de la funcion y mostrar el menu
                Constants_Handball.deleteCanonFlag = true;
                gameUtilsManager.checkIfRemoveCanon();
                canvasController.showMenu();
            }

        }
        else
        {
            generateShoot();
        }
    }

    /* Iniciar la generecion de lanzamiento */
    public void startBallSpawn(bool isFixedShoots)
    {
        _isFixedShoots = isFixedShoots;
        HistoricalRegistry.InicioSerie = System.DateTime.Now;                            // Registrar inicio del entrenamiento
        allowShootProgressive2 = true;                                                   // Permitir lanzamientos en dificultad progresiva 2
        bolasRestantes = Constants_Handball.bolasALanzar;                                // Definir la cantidad de bolas a lanzar
        InvokeRepeating("GenerarObjeto", 0f, Constants_Handball.frecuenciaLanzamiento);  // Llamar a la funci�n GenerarObjeto cada X segundos dependiendo de la dificultad
    }

    /* Generacion del lanzamiento, instancimiento de la bola */
    private void generateShoot()
    {
        if ((allowShootProgressive2 && Constants_Handball.Difficulty == Difficulty.Progressive2) || (Constants_Handball.Difficulty != Difficulty.Progressive2))
        {
            allowShootProgressive2 = false;
            GameData.n_lanzamientos++;
            countdownTextRestBalls.text = bolasRestantes.ToString();                       // Actualizar texto con las bolas restantes                
            HistoricalRegistry.idBola.Add(GameData.n_lanzamientos.ToString());             // Registrar el id de la bola
            handballBallInstance = generateHandBall();                                      // Instancia la pelota

            if (handballBallInstance != null)                                               // Verifica si la instancia es v�lida
            {
                BallCreator ballScript = handballBallInstance.GetComponent<BallCreator>();  // Acceder al componente
                if (ballScript != null)
                {
                    ballScript.ball_id = GameData.n_lanzamientos;
                    ballScript.indiceLanzamiento = bolasRestantes;                          // Asignar a la pelota su indice correspondiente
                    ballScript.useFixedVelocity = _isFixedShoots;                              // Asignar si el lanzamiento es fijo o no
                    trackingDataExecutorController.SetCurrentBall(ballScript);
                }

                BallCreatorHandTracking ballScriptHandTracking = handballBallInstance.GetComponent<BallCreatorHandTracking>();  // Acceder al componente
                if (ballScriptHandTracking != null)
                {
                    ballScriptHandTracking.indiceLanzamiento = bolasRestantes;                                                  // Asignar a la pelota su indice correspondiente
                }

            }

            if (!audioSource.isPlaying)                                                     // Sonido disparo lanzador
            {
                audioSource.volume = 0.125f;
                audioSource.PlayOneShot(CanonShootSound);
            }
            tiempoInicio = Time.time;
        }
    }


    /*Generacion del tipo de bola. En el caso general se genera una pelota de tama�o estandar. En dificultad progresiva 2, bolas de distintos tama�os*/
    private GameObject generateHandBall()
    {
        if (Constants_Handball.Difficulty != Difficulty.Progressive2)
        {
            HistoricalRegistry.tipoBola.Add("1");
            return Instantiate(handballBall);
        }
        else
        {
            if (Constants_Handball.isActiveDistintosTiposPelotas && GameData.n_lanzamientos >= Constants_Handball.n_limitToHandballSizes)
            {
                switch (GenerarOpcionPelota())
                {
                    case 0:
                        HistoricalRegistry.tipoBola.Add("1");
                        return Instantiate(handballBall);
                    case 1:
                        HistoricalRegistry.tipoBola.Add("2");
                        return Instantiate(handballBallSize2);
                    case 2:
                        HistoricalRegistry.tipoBola.Add("3");
                        return Instantiate(handballBallSize3);
                    default:
                        HistoricalRegistry.tipoBola.Add("1");
                        return Instantiate(handballBall);
                }
            }
            else
            {
                HistoricalRegistry.tipoBola.Add("1");
                return Instantiate(handballBall);
            }
        }
    }

    /* Modificar frencuencia de disparo y velocidad en dificultad progresiva */
    public void modifyBallSpawn()
    {
        if (lastShootProgressive)                                   // Si es el ultmo disparo se cancela la serie
        {
            CancelInvoke("GenerarObjeto");
            gameUtilsManager.checkIfRemoveCanon();
            canvasController.showMenu();
        }
        else
        {
            CancelInvoke("GenerarObjeto");                          // Si no es el ultimo disparo se cancela la repeticion y se inicializa una nueva

            tiempoFin = Time.time;                                  //Obtener tiempo transcurrido desde el ultimo lanzamiento hasta el momento de la modificaci�n del tiempo transcurrido entre disparos
            float tiempoTranscurrido = tiempoFin - tiempoInicio;
            tiempoInicio = 0.0f;
            tiempoFin = 0.0f;

            /* Calcular nuevos datos para frencuencia de lanzamiento y velocidad de lanzamiento */
            Constants_Handball.frecuencaLanzamientoModificadaProgresiva = Constants_Handball.frecuencaLanzamientoModificadaProgresiva - Random.Range(Constants_Handball.variacionMinimaProgresiva, Constants_Handball.variacionMaximaProgresiva);
            Constants_Handball.velocidadMinimaModificadaProgresiva = Constants_Handball.velocidadMinimaModificadaProgresiva + Random.Range(Constants_Handball.variacionMinimaProgresiva, Constants_Handball.variacionMaximaProgresiva);
            Constants_Handball.velocidadMaximaModificadaProgresiva = Constants_Handball.velocidadMaximaModificadaProgresiva + Random.Range(Constants_Handball.variacionMinimaProgresiva, Constants_Handball.variacionMaximaProgresiva);

            if (Constants_Handball.frecuencaLanzamientoModificadaProgresiva <= 1.0f)
            {
                Constants_Handball.frecuencaLanzamientoModificadaProgresiva = 1.0f;
            }

            if (tiempoTranscurrido <= 1.0f)
            {
                tiempoTranscurrido = 1.0f;
            }

            float t_espera = Constants_Handball.frecuenciaLanzamiento - tiempoTranscurrido;

            if (t_espera < 0.2f)
            {
                t_espera = 0.2f;
            }

            // Llamar a la funci�n GenerarObjeto con la reduccion de tiempo de la dificultad progresiva
            InvokeRepeating("GenerarObjeto", (t_espera), Constants_Handball.frecuenciaLanzamiento);
        }
    }

    public void modifyBallSpawn2()
    {
        if (GameData.n_goles == Constants_Handball.n_limit_goles)     // Si es el ultmo disparo se cancela la serie
        {
            CancelInvoke("GenerarObjeto");
            Constants_Handball.deleteCanonFlag = true;
            gameUtilsManager.checkIfRemoveCanon();
            canvasController.showMenu();
        }
        else
        {
            CancelInvoke("GenerarObjeto");                          // Si no es el ultimo disparo se cancela la repeticion y se inicializa una nueva

            tiempoFin = Time.time;                                  //Obtener tiempo transcurrido desde el ultimo lanzamiento hasta el momento de la modificaci�n del tiempo transcurrido entre disparos
            float tiempoTranscurrido = tiempoFin - tiempoInicio;
            tiempoInicio = 0.0f;
            tiempoFin = 0.0f;

            /* Calcular nuevos datos para frencuencia de lanzamiento y velocidad de lanzamiento */
            Constants_Handball.frecuencaLanzamientoModificadaProgresiva2 = Constants_Handball.frecuencaLanzamientoModificadaProgresiva2 - Random.Range(Constants_Handball.variacionMinimaProgresiva2, Constants_Handball.variacionMaximaProgresiva2);
            Constants_Handball.velocidadMinimaModificadaProgresiva2 = Constants_Handball.velocidadMinimaModificadaProgresiva2 + Random.Range(Constants_Handball.variacionMinimaProgresiva2bis, Constants_Handball.variacionMinimaProgresiva2bis);
            Constants_Handball.velocidadMaximaModificadaProgresiva2 = Constants_Handball.velocidadMaximaModificadaProgresiva2 + Random.Range(Constants_Handball.variacionMinimaProgresiva2bis, Constants_Handball.variacionMinimaProgresiva2bis);

            if (Constants_Handball.frecuencaLanzamientoModificadaProgresiva2 <= 0.7f)
            {
                Constants_Handball.frecuencaLanzamientoModificadaProgresiva2 = 0.7f;
            }

            if (tiempoTranscurrido <= 1.0f)
            {
                tiempoTranscurrido = 1.0f;
            }

            float t_espera = Constants_Handball.frecuenciaLanzamiento - tiempoTranscurrido;

            if (t_espera < 0.2f)
            {
                t_espera = 0.2f;
            }
            // Llamar a la funci�n GenerarObjeto con la reduccion de tiempo de la dificultad progresiva
            InvokeRepeating("GenerarObjeto", t_espera, Constants_Handball.frecuenciaLanzamiento);
            allowShootProgressive2 = true;
        }

    }

    /* Modificar frencuencia de disparo y velocidad en dificultad progresiva */
    public void modifyBallSpawnPerTime()
    {
        if (lastShootProgressive)                                   // Si es el ultmo disparo se cancela la serie
        {
            CancelInvoke("GenerarObjeto");
            gameUtilsManager.checkIfRemoveCanon();
            canvasController.showMenu();
        }
        else
        {
            CancelInvoke("GenerarObjeto");                          // Si no es el ultimo disparo se cancela la repeticion y se inicializa una nueva

            float difficultyMultiplier = 0;
            if (Constants_Handball.n_limit_time == 60)
            {
                difficultyMultiplier = 3.5f;
            }
            else if (Constants_Handball.n_limit_time == 120)
            {
                difficultyMultiplier = 2.5f;
            }
            else if (Constants_Handball.n_limit_time == 180)
            {
                difficultyMultiplier = 2f;
            }

            /* Calcular nuevos datos para frencuencia de lanzamiento y velocidad de lanzamiento */
            Constants_Handball.frecuencaLanzamientoModificadaProgresiva -= Random.Range(Constants_Handball.variacionMinimaProgresiva, Constants_Handball.variacionMaximaProgresiva) + difficultyMultiplier;
            Constants_Handball.velocidadMinimaModificadaProgresiva += Random.Range(Constants_Handball.variacionMinimaProgresiva, Constants_Handball.variacionMaximaProgresiva) * difficultyMultiplier;
            Constants_Handball.velocidadMaximaModificadaProgresiva += Random.Range(Constants_Handball.variacionMinimaProgresiva, Constants_Handball.variacionMaximaProgresiva) * difficultyMultiplier;

            if (Constants_Handball.frecuencaLanzamientoModificadaProgresiva <= 1.0f)
            {
                Constants_Handball.frecuencaLanzamientoModificadaProgresiva = 1.0f;
            }


            /*if (tiempoTranscurrido <= 1.0f)
            {
                tiempoTranscurrido = 1.0f;
            }*/

            float t_espera = Constants_Handball.frecuencaLanzamientoModificadaProgresiva - (difficultyMultiplier * GameData.n_lanzamientos / Constants_Handball.bolasALanzar);

            if (t_espera < 0.2f)
            {
                t_espera = 0.2f;
            }

            // Llamar a la funci�n GenerarObjeto con la reduccion de tiempo de la dificultad progresiva
            InvokeRepeating("GenerarObjeto", (t_espera), Constants_Handball.frecuenciaLanzamiento);
        }
    }

    /* Cancelar serie de disparos en dificultad progresiva */
    public void cancelSerieInProgressiveSerie()
    {
        bolasRestantes = -1;
        lastShootProgressive = true;
    }

    private int GenerarOpcionPelota()
    {
        // Generar un n�mero aleatorio entre 0 y 1.
        float randomValue = Random.value;

        float probabilidadOpcion1 = Constants_Handball.handballBallSize1Probability;                                             // 50% Probabilidad pelota normal
        float probabilidadOpcion2 = Constants_Handball.handballBallSize1Probability + Constants_Handball.handballBallSize2Probability;    // 35% Probabilidad pelota media
        float probabilidadOpcion3 = probabilidadOpcion2 + Constants_Handball.handballBallSize3Probability;                       // 15% Probabilidad pelota peque�a

        if (randomValue < probabilidadOpcion1)
        {
            return 0;
        }
        else if (randomValue < probabilidadOpcion2)
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }

}