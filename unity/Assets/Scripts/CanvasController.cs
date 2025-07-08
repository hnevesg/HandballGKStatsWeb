using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tracking;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    private float currentTime = 0f;
    private string currentTimeText = "";
    private AudioSource audioSource;
    [SerializeField] private MenuController menuController;
    [SerializeField] private MenuControllerFixedShoots menuControllerFixedShoots;

    [SerializeField] GameObject marcador;
    [SerializeField] GameObject menuCanvas;
    [SerializeField] GameObject menuInitialSection;
    [SerializeField] GameObject menuFixedShootsMainSection;
    [SerializeField] GameObject menuRetrySection;
    [SerializeField] GameObject menuOptionsSection;
    [SerializeField] Text countdownTextLeft;
    [SerializeField] Text countdownTextRight;
    [SerializeField] Text countdownTextRestBalls;
    [SerializeField] Text resultsTxt;
    [SerializeField] TMPro.TMP_Text resultsTxtTMP;  // Menu con HandTracking
    [SerializeField] AudioClip timerSound;
    [SerializeField] AudioClip preGameMusic;
    [SerializeField] AudioClip GameMusic1;
    [SerializeField] AudioClip GameMusic2;
    [SerializeField] AudioClip GameMusic3;
    [SerializeField] AudioClip GameMusic4;
    [SerializeField] AudioClip GameMusic5;
    [SerializeField] AudioClip reactionGameMusic;
    private bool enabledCounterdown = false;
    private bool _isFixedShoots;
    private Spawner spawner;
    private BodyPartCollisionDetector bodyPartCollisionDetector;
    private LightsCollisionDetector lightsCollisionDetector;
    private static SliderTimeProgressiveDifficulty sliderTimeProgressiveDifficulty;
    private GameUtilsManager gameUtilsManager;

    //Historical
    private Historical historial;
    [SerializeField] private SOGameConfiguration gameConfig;

    [SerializeField] GameObject lights;
    [SerializeField] GameObject[] buttons;
    [SerializeField] GameObject closerLights;
    [SerializeField] GameObject[] closerButtons;
    [SerializeField] private LightsModeController lightsModeController;

    void Start()
    {
        // Marcador y contadores se inicializa oculto
        marcador.SetActive(false);
        countdownTextLeft.gameObject.SetActive(false);
        countdownTextRight.gameObject.SetActive(false);
        menuRetrySection.SetActive(false);

        currentTime = Constants_Handball.tiempoCuentaAtras;
        audioSource = GetComponent<AudioSource>();
        spawner = FindObjectOfType<Spawner>();
        bodyPartCollisionDetector = FindObjectOfType<BodyPartCollisionDetector>();
        lightsCollisionDetector = FindObjectOfType<LightsCollisionDetector>();
        gameUtilsManager = FindObjectOfType<GameUtilsManager>();
        // lightsModeController = FindObjectOfType<LightsModeController>();

        //Iniciar musica de fondo en prejuego
        if (!audioSource.isPlaying)
        {
            audioSource.volume = 0.075f;
            audioSource.PlayOneShot(preGameMusic);
            audioSource.clip = preGameMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enabledCounterdown)
        {
            currentTime -= 1 * Time.deltaTime;
            currentTimeText = currentTime.ToString("0");
            countdownTextLeft.text = currentTimeText;
            countdownTextRight.text = currentTimeText;

            if (currentTimeText == "3" && currentTime > 0)
            {
                // Reproducir sonido cuando quedan 3 segundos
                if (!audioSource.isPlaying)
                {
                    audioSource.volume = 1f;
                    audioSource.PlayOneShot(timerSound);
                }
            }

            if (currentTime <= 0)
            {
                currentTime = 0;
                enabledCounterdown = false;
            }
        }
    }

    //Ocultar marcador y contadores cuando termine la cuenta atras
    private void OcultarCuentaAtras()
    {
        marcador.SetActive(true);
        countdownTextLeft.gameObject.SetActive(false);
        countdownTextRight.gameObject.SetActive(false);
        countdownTextRestBalls.text = Constants_Handball.bolasALanzar.ToString();                            //Inicializar marcador de bolas a lanzar
        if (sliderTimeProgressiveDifficulty == null)
        {
            sliderTimeProgressiveDifficulty = FindObjectOfType<SliderTimeProgressiveDifficulty>();
        }
        sliderTimeProgressiveDifficulty.configureStateScoreboard();                                         //Configurar el tipo de marcador a mostrar

        if (Constants_Handball.Difficulty == Difficulty.LightsReaction || Constants_Handball.Difficulty == Difficulty.LightsReaction2)
        {
            SetChildrenVisibility(marcador, false);
            ResetLights();
            GameData.n_luces = 0;
            StartLights();
        }
        else
        {
            SetChildrenVisibility(marcador, true);
            StartCoroutine(startSpawner());
        }
    }

    //Preparar la escena para el juego iniciando la cuenta atras
    public void ComenzarCuentaAtras(bool isFixedShoots)
    {
        menuCanvas.SetActive(false);

        _isFixedShoots = isFixedShoots;

        //Parar musica si esta reproduciendose
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.volume = 1f;
        }

        //Mostrar contadores
        countdownTextLeft.gameObject.SetActive(true);
        countdownTextRight.gameObject.SetActive(true);

        if (Constants_Handball.Difficulty == Difficulty.LightsReaction2)
        {
            closerLights.SetActive(true);
        }
        else if (Constants_Handball.Difficulty == Difficulty.LightsReaction)
        {
            lights.SetActive(true);
        }

        //Ocultar cuenta atras tras 5 segundos
        Invoke("OcultarCuentaAtras", Constants_Handball.tiempoCuentaAtras);

        //Habilitar cuenta atras
        enabledCounterdown = true;

        //Reiniciar contador de paradas y goles
        if (bodyPartCollisionDetector != null)
        {
            bodyPartCollisionDetector.reinitStopAndGoals();
        }
        else if (lightsCollisionDetector != null)
        {
            lightsCollisionDetector.ResetGameState();
        }
        else
        {
            gameUtilsManager.reinitValuesMarcador();
        }
    }

    private IEnumerator startSpawner()
    {
        // Esperar 1 segundo antes de que comience el juego
        yield return new WaitForSeconds(1f);

        //Comenzar la generacion de lanzamientos aleatorios
        spawner.startBallSpawn(_isFixedShoots);

        //Iniciar musica de juego
        if (!audioSource.isPlaying)
        {
            audioSource.volume = 0.125f;
            switch (Constants_Handball.Difficulty)
            {
                case Difficulty.Beginner:
                    audioSource.clip = GameMusic1;
                    break;
                case Difficulty.Intermediate:
                    audioSource.clip = GameMusic2;
                    break;
                case Difficulty.Expert:
                    audioSource.volume = 0.075f;
                    audioSource.clip = GameMusic4;
                    break;
                case Difficulty.Progressive1:
                    audioSource.clip = GameMusic3;
                    break;
                case Difficulty.Progressive2:
                    audioSource.clip = GameMusic5;
                    break;
                default:
                    audioSource.clip = GameMusic2;
                    break;
            }
            audioSource.loop = true;
            audioSource.Play();

            // Comienza captura de cinematicas
           // gameConfig.gameStarts = false;
            //TrackingDataWriter.Instance.SetHeaderControllerHGTrainer();
            //TrackingDataWriter.Instance.SetPathWritter(gameConfig.userPath);
            gameConfig.gameStarts = true;
            gameConfig.gameStopped = false;
        }
    }

    private void StartLights()
    {
        if (Constants_Handball.Difficulty == Difficulty.LightsReaction2)
        {
            closerLights.SetActive(true);
        }
        else
        {
            lights.SetActive(true);
        }

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        audioSource.PlayOneShot(reactionGameMusic);
        audioSource.volume = 0.3f;
        audioSource.clip = reactionGameMusic;
        audioSource.loop = true;
        audioSource.Play();

        gameConfig.gameStarts = false;
        TrackingDataWriter.Instance.SetHeaderRayBothHand();
        TrackingDataWriter.Instance.SetPathWritter(gameConfig.userPath);
        gameConfig.gameStarts = true;
        gameConfig.gameStopped = false;

        HistoricalRegistry.InicioSerie = System.DateTime.Now;
        lightsModeController.turnOnOneLight();
    }

    private void ResetLights()
    {
        GameObject[] lights = buttons;

        if (Constants_Handball.Difficulty == Difficulty.LightsReaction2)
        {
            lights = closerButtons;
        }

        foreach (var light in lights)
        {
            Renderer renderer = light.GetComponent<MeshRenderer>();
            renderer.material.color = Color.red;
        }
    }

    public void OnClickRetryBtn()
    {
        if (_isFixedShoots)
        {
            menuControllerFixedShoots.StartBtn();
        }
        else
        {
            menuController.StartBtn();
        }
    }

    public void OnClickMainSectionBtn()
    {
        menuRetrySection.SetActive(false);

        if (_isFixedShoots)
        {
            menuFixedShootsMainSection.SetActive(true);
        }
        else
        {
            menuInitialSection.SetActive(true);
        }
    }

    void SetChildrenVisibility(GameObject marcador, bool state)
    {
        marcador.transform.Find("Image/BackgroundLeft")?.gameObject.SetActive(state);
        marcador.transform.Find("Image/BackgroundRight")?.gameObject.SetActive(state);
        marcador.transform.Find("Image/BackgroundCenter")?.gameObject.SetActive(!state);
    }

    public void showMenu()
    {
        HistoricalRegistry.FinalSerie = System.DateTime.Now; //Registrar final de la serie

        // Mostrar menu de reintento y ocultar el de inicio
        menuInitialSection.SetActive(false);
        menuFixedShootsMainSection.SetActive(false);
        menuOptionsSection.SetActive(false);
        menuRetrySection.SetActive(true);

        //Mostrar resultados
        if (resultsTxt)
        {
            resultsTxt.text = "";
            resultsTxt.text = "\nIdentificador: " + (Constants_Handball.identificador) +
                                "\nModo: " + (Constants_Handball.Difficulty) +
                                "\nParadas: " + (GameData.n_paradas) +
                                "\nGoles recibidos: " + (GameData.n_goles) +
                                "\nLanzamientos totales: " + (GameData.n_lanzamientos);


            if (GameData.t_serie != "")
            {
                resultsTxt.text += "\nTiempo: " + GameData.t_serie;
            }
        }

        if (resultsTxtTMP)
        {
            resultsTxtTMP.text = "";
            resultsTxtTMP.text = "\nIdentificador: " + (Constants_Handball.identificador) +
                            "\nModo: " + (Constants_Handball.Difficulty) +
                            "\nParadas: " + (GameData.n_paradas) +
                            "\nGoles recibidos: " + (GameData.n_goles) +
                            "\nLanzamientos totales: " + (GameData.n_lanzamientos);


            if (GameData.t_serie != "")
            {
                resultsTxtTMP.text += "\nTiempo: " + GameData.t_serie;
            }
        }


        AddDataToHistorical();
        Utils.reinitHistorialValues();

        // Marcador y contadores se ocultan
        marcador.SetActive(false);
        countdownTextLeft.gameObject.SetActive(false);
        countdownTextRight.gameObject.SetActive(false);

        //Se reinicializa el contador
        currentTime = Constants_Handball.tiempoCuentaAtras;

        menuCanvas.SetActive(true);

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.volume = 0.075f;
            audioSource.clip = preGameMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
    private void AddDataToHistorical()
    {
        gameConfig.gameStopped = true;

        historial = new Historical(gameConfig.gameName)
        {
            time = (float)(HistoricalRegistry.FinalSerie - HistoricalRegistry.InicioSerie).TotalSeconds,
            userId = gameConfig.gameConfiguration.userId,
            //  autogrip = gameConfig.gameConfiguration.autogrip,
            //   handR = gameConfig.gameConfiguration.handInteraction == GameConfiguration.HandInteraction.Both,
            //level = gameConfig.gameConfiguration.level
        };

        historial.paradas = GameData.n_paradas;
        historial.goles = GameData.n_goles;
        historial.lanzamientoGenerados = GameData.n_lanzamientos;
        historial.n_luces = GameData.n_luces;
        historial.TrackingFile = TrackingDataWriter.Instance.filePath;

        HistoricalWritter historicalW = new HistoricalWritter(gameConfig.userPath);
        // historial._hmdPosition = gameConfig.calibrationConfiguration.hmdInitialPosition;
        //historial._hmdRotation = gameConfig.calibrationConfiguration.hmdInitialRotation;
        historial.AddOrUpdateGameOption(KeysConfiguration.k_mode, Constants_Handball.Mode.ToString());
        historial.AddOrUpdateGameOption(KeysConfiguration.k_level, Constants_Handball.Difficulty.ToString());
        historial.AddOrUpdateGameOption(KeysConfiguration.k_handballadapted, Constants_Handball.HandballType.ToString());
        historial.AddOrUpdateGameOption(KeysConfiguration.k_idEjercicio, Constants_Handball.idEjercicio.ToString());

        if (Constants_Handball.Difficulty == Difficulty.Progressive2)
        {
            historial.AddOrUpdateGameOption(KeysConfiguration.k_golesObjetivo, Constants_Handball.n_limit_goles.ToString());
        }
        else
        {
            historial.AddOrUpdateGameOption(KeysConfiguration.k_golesObjetivo, "");
        }

        if (Constants_Handball.Difficulty == Difficulty.PerTime)
        {
            historial.AddOrUpdateGameOption(KeysConfiguration.k_tiempoObjetivo, Constants_Handball.n_limit_time.ToString());
        }
        else
        {
            historial.AddOrUpdateGameOption(KeysConfiguration.k_tiempoObjetivo, "");
        }

        /*        StringBuilder sb = new StringBuilder();

                for (int i = 0; i < HistoricalRegistry.tipoBola.Count; i++)
                {
                    sb.Append($"[{HistoricalRegistry.tipoBola[i]}");
                    sb.Append($",{HistoricalRegistry.puntoInicialLanzamiento[i]}");
                    sb.Append($",{HistoricalRegistry.id_puntoInicialLanzamiento[i]}");
                    sb.Append($",{HistoricalRegistry.puntoFinalLanzamiento[i]}");
                    sb.Append($",{HistoricalRegistry.id_puntoFinalLanzamiento[i]}");
                    sb.Append($",{HistoricalRegistry.paradoNoParado[i]}");
                    sb.Append($",{HistoricalRegistry.tiempoDisparo[i]}");
                    sb.Append($",{HistoricalRegistry.tiempoParadaOGol[i]}]");
                }
        */
        StringBuilder sb = new StringBuilder();

        int countBola = HistoricalRegistry.tipoBola.Count;

        // convertir las listas a array
        var tipoBolaArray = HistoricalRegistry.tipoBola.ToArray();
        var idBolaArray = HistoricalRegistry.idBola.ToArray();
        Vector3[] puntoInicialLanzamientoArray = HistoricalRegistry.puntoInicialLanzamiento.ToArray();
        var id_puntoInicialLanzamientoArray = HistoricalRegistry.id_puntoInicialLanzamiento.ToArray();
        Vector3[] puntoFinalLanzamientoArray = HistoricalRegistry.puntoFinalLanzamiento.ToArray();
        var id_puntoFinalLanzamientoArray = HistoricalRegistry.id_puntoFinalLanzamiento.ToArray();
        var paradoNoParadoArray = HistoricalRegistry.paradoNoParado.ToArray();
        var manoDeLaParadaArray = HistoricalRegistry.manoDeLaParada.ToArray();
        var tiempoDisparoArray = HistoricalRegistry.tiempoDisparo.ToArray();
        var tiempoParadaOGolArray = HistoricalRegistry.tiempoParadaOGol.ToArray();


        //modifica la cadena vac�a por "None"
        manoDeLaParadaArray = manoDeLaParadaArray.Select(item => string.IsNullOrEmpty(item) ? "None" : item).ToArray();

        //separador formado por comas
        string manoDeLaParadaStr = string.Join(",", manoDeLaParadaArray);
        string tipoBolaStr = string.Join(",", tipoBolaArray);
        string idBolaStr = string.Join(",", idBolaArray);
        string puntoInicialLanzamientoStr = string.Join(",", puntoInicialLanzamientoArray);
        string id_puntoInicialLanzamientoStr = string.Join(",", id_puntoInicialLanzamientoArray);
        string puntoFinalLanzamientoStr = string.Join(",", puntoFinalLanzamientoArray);
        string id_puntoFinalLanzamientoStr = string.Join(",", id_puntoFinalLanzamientoArray);
        string paradoNoParadoStr = string.Join(",", paradoNoParadoArray);
        string tiempoDisparoStr = string.Join(",", tiempoDisparoArray);
        string tiempoParadaOGolStr = string.Join(",", tiempoParadaOGolArray);

        historial.AddOrUpdateGameOption("Tipo_bola", tipoBolaStr);
        historial.AddOrUpdateGameOption("Id_bola", idBolaStr);
        historial.AddOrUpdateGameOption("Punto_inicial_bola", puntoInicialLanzamientoStr);
        historial.AddOrUpdateGameOption("Id_punto_inicial_bola", id_puntoInicialLanzamientoStr);
        historial.AddOrUpdateGameOption("Punto_final_bola", puntoFinalLanzamientoStr);
        historial.AddOrUpdateGameOption("Id_punto_final_bola", id_puntoFinalLanzamientoStr);
        historial.AddOrUpdateGameOption("Parada_bola", paradoNoParadoStr);
        historial.AddOrUpdateGameOption("Mano_Parada", manoDeLaParadaStr);
        historial.AddOrUpdateGameOption("Tiempo_disparo", tiempoDisparoStr);
        historial.AddOrUpdateGameOption("Tiempo_parada_o_gol", tiempoParadaOGolStr);

        historicalW.WriteHistorical(historial);

   //     FiabilityData fiabilityFile = new FiabilityData(TrackingDataWriter.Instance.filePath, gameConfig.userPath);
     //   fiabilityFile.WriteBallPositionData();
    }

    public void showMenuLights()
    {
        HistoricalRegistry.FinalSerie = System.DateTime.Now; //Registrar final de la serie

        // Mostrar menu de reintento y ocultar el de inicio
        menuInitialSection.SetActive(false);
        menuFixedShootsMainSection.SetActive(false);
        menuOptionsSection.SetActive(false);
        menuRetrySection.SetActive(true);

        //Mostrar resultados
        if (resultsTxt)
        {

            resultsTxt.text = "";
            resultsTxt.text = "\nIdentificador: " + (Constants_Handball.identificador) +
                                "\nModo: " + (Constants_Handball.Difficulty) +
                                "\nNº de luces: " + (GameData.n_luces);



            if (GameData.t_serie != "")
            {
                resultsTxt.text += "\nTiempo: " + GameData.t_serie;
            }
        }

        if (resultsTxtTMP)
        {

            resultsTxt.text = "";
            resultsTxt.text = "\nIdentificador: " + (Constants_Handball.identificador) +
                                "\nModo: " + (Constants_Handball.Difficulty) +
                                "\nNº de luces: " + (GameData.n_luces);


            if (GameData.t_serie != "")
            {
                resultsTxtTMP.text += "\nTiempo: " + GameData.t_serie;
            }
        }

        AddLightsToHistorical();
        Utils.reinitHistorialValues();

        // Marcador y contadores se ocultan
        marcador.SetActive(false);
        countdownTextLeft.gameObject.SetActive(false);
        countdownTextRight.gameObject.SetActive(false);

        //Se reinicializa el contador
        currentTime = Constants_Handball.tiempoCuentaAtras;

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.volume = 0.075f;
            audioSource.clip = preGameMusic;
            audioSource.loop = true;
            audioSource.Play();
        }

        menuCanvas.SetActive(true);
    }
    
    private void AddLightsToHistorical()
    {
        gameConfig.gameStopped = true;

        historial = new Historical(gameConfig.gameName)
        {
            time = (float)(HistoricalRegistry.FinalSerie - HistoricalRegistry.InicioSerie).TotalSeconds,
            userId = gameConfig.gameConfiguration.userId,
        };

        historial.paradas = GameData.n_paradas;
        historial.goles = GameData.n_goles;
        historial.lanzamientoGenerados = GameData.n_lanzamientos;
        historial.n_luces = GameData.n_luces;
        historial.TrackingFile = TrackingDataWriter.Instance.filePath;

        HistoricalWritter historicalW = new HistoricalWritter(gameConfig.userPath);

        historial.AddOrUpdateGameOption(KeysConfiguration.k_mode, Constants_Handball.Mode.ToString());
        historial.AddOrUpdateGameOption(KeysConfiguration.k_level, Constants_Handball.Difficulty.ToString());
        historial.AddOrUpdateGameOption(KeysConfiguration.k_handballadapted, Constants_Handball.HandballType.ToString());
        historial.AddOrUpdateGameOption(KeysConfiguration.k_idEjercicio, Constants_Handball.idEjercicio.ToString());


        historial.AddOrUpdateGameOption(KeysConfiguration.k_golesObjetivo, "");
        historial.AddOrUpdateGameOption(KeysConfiguration.k_tiempoObjetivo, Constants_Handball.n_limit_time.ToString());

        StringBuilder sb = new StringBuilder();

        historial.AddOrUpdateGameOption("Tipo_bola", "");
        historial.AddOrUpdateGameOption("Id_bola", "");
        historial.AddOrUpdateGameOption("Punto_inicial_bola", "");
        historial.AddOrUpdateGameOption("Id_punto_inicial_bola", "");
        historial.AddOrUpdateGameOption("Punto_final_bola", "");
        historial.AddOrUpdateGameOption("Id_punto_final_bola", "");
        historial.AddOrUpdateGameOption("Parada_bola", "");
        historial.AddOrUpdateGameOption("Mano_Parada", "");
        historial.AddOrUpdateGameOption("Tiempo_disparo", "");
        historial.AddOrUpdateGameOption("Tiempo_parada_o_gol", "");

        historicalW.WriteHistorical(historial);

        ReactionData reactionFile = new ReactionData(TrackingDataWriter.Instance.filePath, gameConfig.userPath);
        reactionFile.WriteData();
    }
}