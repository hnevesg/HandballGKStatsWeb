using Tracking;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BodyPartCollisionDetector : MonoBehaviour
{
    protected bool isTouchingObject = false;
    protected GameObject currentTouchedObject = null;

    [SerializeField] protected Text Paradas;
    [SerializeField] protected Text NoParadas;
    [SerializeField] protected AudioClip goalSound;
    [SerializeField] protected AudioClip stop1Sound;
    [SerializeField] protected AudioClip stop2Sound;
    [SerializeField] protected AudioClip stop3Sound;
    [SerializeField] protected ParticleSystem sistemaParticulasParada;
    private static SliderTimeProgressiveDifficulty sliderTimeProgressiveDifficulty;
    private GameUtilsManager gameUtilsManager;      // Referencia al controlador util del juego
    private Spawner spawner;

    protected static int paradas = 0;
    protected static int noParadas = 0;
    protected AudioSource audioSource;

    [SerializeField]
    protected enum BodyPart
    {
        RightHand,
        LeftHand,
        Head,
        Trunk,
        RightUpperarm,
        LeftUpperarm,
        RightForearm,
        LeftForearm
    }
    [SerializeField] protected BodyPart bodyPart;
    
    private TrackingDataExecutorController trackingDataExecutorController;

    // protected abstract string GetBodyPartName();

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spawner = FindObjectOfType<Spawner>();
        gameUtilsManager = FindObjectOfType<GameUtilsManager>();
        trackingDataExecutorController = FindObjectOfType<TrackingDataExecutorController>();
    }

    public void OnTriggerEnter(Collider other)
    {
        // Comprobar si el objeto tocado es la bola
        if (other.CompareTag("HandballBall"))
        {
            isTouchingObject = true;
            currentTouchedObject = other.gameObject;

            if (isTouchingObject)
            {
                handleTouch();
                generateSaveSound();
              //  updateTrackingFile();

                other.enabled = false;
                isTouchingObject = false;
                currentTouchedObject.SetActive(false);

                BallCreator ballScript = currentTouchedObject.GetComponent<BallCreator>();
                if (ballScript != null)
                {
                    ballScript.ball_status = "Parada";
                    //Comprobar si es ultima bola para eliminar canon
                    gameUtilsManager.checkIfRemoveCanon(ballScript.indiceLanzamiento);

                    //Mostrar sistema de particulas
                    MostrarSistemaParticulasParada(ballScript.transform.position);
                }

                StartCoroutine(DestroyAfterFrames(3, currentTouchedObject));

                UpdateTimeIfNecesaryProgressiveDifficulty();
                UpdateTimeIfNecesaryProgressive2Difficulty();
                UpdateTimeIfNecesaryPerTimeDifficulty();

            }
        }
    }

            IEnumerator DestroyAfterFrames(int frameCount, GameObject gameObject)
            {
                for (int i = 0; i < frameCount; i++)
                {
                    yield return null;
                }
                Destroy(gameObject);
                gameObject = null;
            }

    public void handleTouch()
    {
        ++paradas;
        GameData.n_paradas++;
        HistoricalRegistry.paradoNoParado.Add("Parada");
        HistoricalRegistry.manoDeLaParada.Add(bodyPart.ToString());
        HistoricalRegistry.tiempoParadaOGol.Add((System.DateTime.Now - HistoricalRegistry.InicioSerie).TotalSeconds.ToString().Replace(",", "."));
        Paradas.text = paradas.ToString();
    }

    public void generateSaveSound()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(stop1Sound);
                }
                break;

            case 1:
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(stop2Sound);
                }
                break;

            case 2:
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(stop3Sound);
                }
                break;

            default:
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(stop1Sound);
                }
                break;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentTouchedObject)
        {
            isTouchingObject = false;
            currentTouchedObject = null;
        }
    }

    public void noStoppedBall()
    {
        ++noParadas;
        GameData.n_goles++;
        HistoricalRegistry.paradoNoParado.Add("Gol recibido");
        HistoricalRegistry.manoDeLaParada.Add("No parada");
        HistoricalRegistry.tiempoParadaOGol.Add((System.DateTime.Now - HistoricalRegistry.InicioSerie).TotalSeconds.ToString().Replace(",", "."));
        NoParadas.text = noParadas.ToString();

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(goalSound);
        }

        UpdateTimeIfNecesaryProgressive2Difficulty();
    }

    //Reiniciar valores
    public void reinitStopAndGoals()
    {
        paradas = 0;
        noParadas = 0;
        GameData.n_paradas = 0;
        GameData.n_goles = 0;
        GameData.n_lanzamientos = 0;
        GameData.t_serie = "";
        NoParadas.text = noParadas.ToString();
        Paradas.text = paradas.ToString();

        //Reinicar valores dificultad progresiva si es necesario
        Utils.reinitProgressiveValuesIfNeccesary();
    }
    private void MostrarSistemaParticulasParada(Vector3 position)
    {
        // Crea una instancia del sistema de part�culas en la posici�n del evento.
        ParticleSystem particulasInstanciadas = Instantiate(sistemaParticulasParada, position, Quaternion.identity);

        // Inicia el sistema de part�culas.
        particulasInstanciadas.Play();
    }

    //M�todo para controlar si se para una bola en dificultad progresiva, descontar tiempo en el timer
    private void UpdateTimeIfNecesaryProgressiveDifficulty()
    {
        if (Constants_Handball.Difficulty == Difficulty.Progressive1)
        {
            if (sliderTimeProgressiveDifficulty == null)
            {
                sliderTimeProgressiveDifficulty = FindObjectOfType<SliderTimeProgressiveDifficulty>();
                if (sliderTimeProgressiveDifficulty != null)
                {
                    sliderTimeProgressiveDifficulty.reduceTimeSliderWhenStopBall();
                }
            }
            else
            {
                sliderTimeProgressiveDifficulty.reduceTimeSliderWhenStopBall();
            }

            //Modificar el tiempo de frencuencia de lanzamiento tras parada del portero
            spawner.modifyBallSpawn();
        }
    }

    private void UpdateTimeIfNecesaryProgressive2Difficulty()
    {
        if (Constants_Handball.Difficulty == Difficulty.Progressive2)
        {
            spawner.modifyBallSpawn2();
        }
    }

    private void UpdateTimeIfNecesaryPerTimeDifficulty()
    {
        if (Constants_Handball.Difficulty == Difficulty.PerTime)
        {
            spawner.modifyBallSpawnPerTime();
        }
    }

    private void updateTrackingFile()
    {
        trackingDataExecutorController.ProcessFrame(TimeManager.sharedFrame, true);
    }
}

