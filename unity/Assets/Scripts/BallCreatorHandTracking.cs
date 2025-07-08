using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BallCreatorHandTracking : MonoBehaviour
{
    public GameObject ballLauncher;
    private float velocidadMovimiento;
    private float efectoLanzamiento;

    private Vector3 puntoInicial;
    private Vector3 puntoFinal;
    private float distanciaRecorrida = 0f;
    private float distanciaTotal;
    private bool moverHaciaFinal = true;
    private AudioSource audioSource;

    [HideInInspector]
    [System.NonSerialized]
    public int indiceLanzamiento;

    private SliderTimeProgressiveDifficulty sliderTimeProgressive;
    private GameUtilsManager gameUtilsManager;      // Referencia al controlador util del juego
    [SerializeField] ParticleSystem sistemaParticulasCanon; 
    ParticleSystem particlesSystem;
    int canonChoosed = 0;
    private Spawner spawner;
    

    void Start()
    {
        //Generar posicion inicial/es del lanzador
        generarCanons();

        // Elegir punto de tiro
        elegirPuntoDeTiro();

        sliderTimeProgressive = FindObjectOfType<SliderTimeProgressiveDifficulty>();
        spawner = FindObjectOfType<Spawner>();
        audioSource = GetComponent<AudioSource>();
        gameUtilsManager = FindObjectOfType<GameUtilsManager>();

        // Calcular la distancia total entre los puntos
        distanciaTotal = Vector3.Distance(puntoInicial, puntoFinal);

        // Calcular velocidad de disparo de la pelota
        velocidadMovimiento = Random.Range(Constants_Handball.velocidadMinima, Constants_Handball.velocidadMaxima);

        //Calcular efecto del disparo
        efectoLanzamiento = Random.Range(Constants_Handball.efectoLanzamientoMinimo, Constants_Handball.efectoLanzamientoMaximo);
    }

    void Update()
    {
        // Si el objeto esta en movimiento
        if (moverHaciaFinal)
        {
            gameObject.SetActive(true);
            // Calcular la distancia recorrida
            distanciaRecorrida += velocidadMovimiento * Time.deltaTime;

            // Si la distancia recorrida supera la distancia total, generar nuevas posiciones
            if (distanciaRecorrida >= distanciaTotal)
            {
                moverHaciaFinal = false;
                distanciaRecorrida = 0f;
                transform.position = new Vector3(19, -2, 0);
            }
            else
            {
                float t = distanciaRecorrida / distanciaTotal;

                if (Constants_Handball.isActiveCurvaLanzamiento)
                {
                    // Aplicar una curva sinusoidal a la posición Y para crear un efecto
                    float yOffset = Mathf.Sin(t * Mathf.PI) * efectoLanzamiento;
                    Vector3 newPosition = Vector3.Lerp(puntoInicial, puntoFinal, t);
                    newPosition.y += yOffset;
                    transform.position = newPosition;
                }
                else
                {
                    Vector3 newPosition = Vector3.Lerp(puntoInicial, puntoFinal, t);
                    transform.position = newPosition;
                }

                if (Constants_Handball.isActiveRotacionPelota)
                {
                    // Calcular el ángulo de rotación deseado 
                    float rotationAngle = t * 360f;
                    transform.rotation = Quaternion.Euler(0f, rotationAngle, 0f); // Aplica la rotación
                }
            }
        }
        else
        {
            gameUtilsManager.checkIfRemoveCanon(indiceLanzamiento);

            //Destruir objeto y actualizar marcador de pelota no parada
            noStoppedBall();
            Destroy(gameObject);
        }
        if (particlesSystem != null)
        {
            particlesSystem.transform.position = GameData.canon[canonChoosed].transform.Find("firePosition").position;
        }
    }
    void elegirPuntoDeTiro()
    {
        Transform innerObjectTransform;
        canonChoosed = Random.Range(0, GameData.canon.Length);
        innerObjectTransform = GameData.canon[canonChoosed].transform.Find("handballBall");
        Transform firePosition = GameData.canon[canonChoosed].transform.Find("firePosition");
        HistoricalRegistry.id_puntoInicialLanzamiento.Add(GameData.canon[canonChoosed].GetComponent<CanonController>().shootPosition.ToString());
        HistoricalRegistry.tiempoDisparo.Add((System.DateTime.Now - HistoricalRegistry.InicioSerie).TotalSeconds.ToString().Replace(",", "."));

        if (innerObjectTransform != null)
        {
            puntoInicial = innerObjectTransform.position;
            HistoricalRegistry.puntoInicialLanzamiento.Add(puntoInicial);
        }

        if (firePosition != null)
        {
            MostrarSistemaParticulasLanzamiento(firePosition.position);
        }

        //Punto de tiro
        generateFinalPoint();

        StartCoroutine(RotateTowardsTarget(canonChoosed));
    }

    private void generateFinalPoint()
    {
        int opcionCanon = 0;
        switch (Constants_Handball.HandballType)
        {
            case HandballType.Handball:
                opcionCanon = Random.Range(0, Constants_Handball.HandballShootPositions.Length);
                puntoFinal = (Vector3)Constants_Handball.HandballShootPositions.GetValue(opcionCanon);
                puntoFinal.y = puntoFinal.y + Random.Range(-Constants_Handball.MaxVariationY, Constants_Handball.MaxVariationZ);
                puntoFinal.z = puntoFinal.z + Random.Range(-Constants_Handball.MaxVariationY, Constants_Handball.MaxVariationZ);
                break;
            case HandballType.AdaptedHandball:
                opcionCanon = Random.Range(0, Constants_Handball.AdaptedHandballShootPositions.Length);
                puntoFinal = (Vector3)Constants_Handball.AdaptedHandballShootPositions.GetValue(opcionCanon);
                puntoFinal.y = puntoFinal.y + Random.Range(-Constants_Handball.MaxVariationYInAdaptedHandball, Constants_Handball.MaxVariationZInAdaptedHandball);
                puntoFinal.z = puntoFinal.z + Random.Range(-Constants_Handball.MaxVariationYInAdaptedHandball, Constants_Handball.MaxVariationZInAdaptedHandball);
                break;
            default:
                opcionCanon = Random.Range(0, Constants_Handball.HandballShootPositions.Length);
                puntoFinal = (Vector3)Constants_Handball.HandballShootPositions.GetValue(opcionCanon);
                puntoFinal.y = puntoFinal.y + Random.Range(-Constants_Handball.MaxVariationY, Constants_Handball.MaxVariationZ);
                puntoFinal.z = puntoFinal.z + Random.Range(-Constants_Handball.MaxVariationY, Constants_Handball.MaxVariationZ);
                break;
        }
        HistoricalRegistry.id_puntoFinalLanzamiento.Add(Constants_Handball.nameShootPositions.GetValue(opcionCanon).ToString());
        HistoricalRegistry.puntoFinalLanzamiento.Add(puntoFinal);
    }

    private IEnumerator RotateTowardsTarget(int index)
    {
        Vector3 dir = puntoFinal - GameData.canon[index].transform.position;
        Quaternion target = Quaternion.LookRotation(dir);

        while (Quaternion.Angle(transform.rotation, target) > 0.01f)
        {
            if (GameData.canon[index] != null)
            {
                GameData.canon[index].transform.rotation = Quaternion.RotateTowards(GameData.canon[index].transform.rotation, target, 80f * Time.deltaTime);
            }
            yield return null;
        }
    }
    private void MostrarSistemaParticulasLanzamiento(Vector3 position)
    {
        // Crea una instancia del sistema de partículas en la posición del evento.
        particlesSystem = Instantiate(sistemaParticulasCanon, position, Quaternion.identity);
        // Inicia el sistema de partículas.
        particlesSystem.Play();
    }


    private void generarCanons()
    {
        //Generar una unica posición inicial
        if (!GameData.posicionInicialGenerada)
        {
            int numCanons = 0;
            if (Constants_Handball.centralCanonActivated) numCanons++;
            if (Constants_Handball.pivotCanonActivated) numCanons++;
            if (Constants_Handball.latIzqCanonActivated) numCanons++;
            if (Constants_Handball.latDerCanonActivated) numCanons++;
            if (Constants_Handball.extIzqCanonActivated) numCanons++;
            if (Constants_Handball.extDerCanonActivated) numCanons++;

            GameData.canon = new GameObject[numCanons];

            int indexArrayCanon = 0;

            if (Constants_Handball.centralCanonActivated)
            {
                GameData.canon[indexArrayCanon] = Instantiate(ballLauncher, Constants_Handball.centralPosition, Quaternion.identity);
                GameData.canon[indexArrayCanon].GetComponent<CanonController>().shootPosition = ShootPositions.Central;
                indexArrayCanon++;
            }

            if (Constants_Handball.pivotCanonActivated)
            {
                GameData.canon[indexArrayCanon] = Instantiate(ballLauncher, Constants_Handball.pivotPosition, Quaternion.identity);
                GameData.canon[indexArrayCanon].GetComponent<CanonController>().shootPosition = ShootPositions.Pivote;
                indexArrayCanon++;
            }

            if (Constants_Handball.latIzqCanonActivated)
            {
                GameData.canon[indexArrayCanon] = Instantiate(ballLauncher, Constants_Handball.latIzqPosition, Quaternion.identity);
                GameData.canon[indexArrayCanon].GetComponent<CanonController>().shootPosition = ShootPositions.LateralIzquierdo;
                indexArrayCanon++;
            }

            if (Constants_Handball.latDerCanonActivated)
            {
                GameData.canon[indexArrayCanon] = Instantiate(ballLauncher, Constants_Handball.latDerPosition, Quaternion.identity);
                GameData.canon[indexArrayCanon].GetComponent<CanonController>().shootPosition = ShootPositions.LateralDerecho;
                indexArrayCanon++;
            }

            if (Constants_Handball.extIzqCanonActivated)
            {
                GameData.canon[indexArrayCanon] = Instantiate(ballLauncher, Constants_Handball.extIzqPosition, Quaternion.identity);
                GameData.canon[indexArrayCanon].GetComponent<CanonController>().shootPosition = ShootPositions.ExtremoIzquierdo;
                indexArrayCanon++;
            }

            if (Constants_Handball.extDerCanonActivated)
            {
                GameData.canon[indexArrayCanon] = Instantiate(ballLauncher, Constants_Handball.extDerPosition, Quaternion.identity);
                GameData.canon[indexArrayCanon].GetComponent<CanonController>().shootPosition = ShootPositions.ExtremoDerecho;
            }

            orientateCanons();
            GameData.posicionInicialGenerada = !GameData.posicionInicialGenerada;
        }
    }

    private void orientateCanons()
    {
        for (int i = 0; i < GameData.canon.Length; i++)
        {
            Vector3 dir = Constants_Handball.centerGoalPoint - GameData.canon[i].transform.position;
            Quaternion target = Quaternion.LookRotation(dir);
            GameData.canon[i].transform.rotation = target;
        }
    }

    /*************************************************/
    /* CODIGO ADICIONAL PARA GESTION DE HAND TRACKING*/
    /*************************************************/
    private bool isTouchingObject = false;
    private bool ballIncremented = false;
    //[SerializeField] Hand hand;

    [SerializeField] ParticleSystem sistemaParticulasParada;
    private static SliderTimeProgressiveDifficulty sliderTimeProgressiveDifficulty;

    private void OnTriggerEnter(Collider other)
    {
        // Comprobar si el objeto tocado es la mano
        if (other.CompareTag("Untagged"))
        {
            isTouchingObject = true;

            if (isTouchingObject && !ballIncremented)
            {
                ballIncremented = true;
                GameData.n_paradas++;
                HistoricalRegistry.paradoNoParado.Add("Parada");
                HistoricalRegistry.manoDeLaParada.Add("Mano parada");
                HistoricalRegistry.tiempoParadaOGol.Add((System.DateTime.Now - HistoricalRegistry.InicioSerie).TotalSeconds.ToString());
                gameUtilsManager.updateParadasMarcador(GameData.n_paradas.ToString());

                //Generar sonido para la parada
                gameUtilsManager.reproduceStopSound();

                isTouchingObject = false;
                gameObject.SetActive(false);

                gameUtilsManager.checkIfRemoveCanon(indiceLanzamiento);

                //Mostrar sistema de particulas
                MostrarSistemaParticulasParada(transform.position);
                
                UpdateTimeIfNecesaryProgressiveDifficulty();
                UpdateTimeIfNecesaryProgressive2Difficulty();
                UpdateTimeIfNecesaryPerTimeDifficulty();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // Comprobar si el objeto que deja de tocar es la esfera
        /*if (other.gameObject == currentTouchedObject)
        {
            isTouchingObject = false;
            currentTouchedObject = null;
        }*/
    }

    public void noStoppedBall()
    {
        GameData.n_goles++;
        HistoricalRegistry.paradoNoParado.Add("Gol recibido");
        HistoricalRegistry.manoDeLaParada.Add("No parada");
        HistoricalRegistry.tiempoParadaOGol.Add((System.DateTime.Now - HistoricalRegistry.InicioSerie).TotalSeconds.ToString());
        gameUtilsManager.updateGolesMarcador(GameData.n_goles.ToString());
        gameUtilsManager.reproduceGoalSound();

        UpdateTimeIfNecesaryProgressive2Difficulty();
    }
    private void MostrarSistemaParticulasParada(Vector3 position)
    {
        // Crea una instancia del sistema de partículas en la posición del evento.
        ParticleSystem particulasInstanciadas = Instantiate(sistemaParticulasParada, position, Quaternion.identity);

        // Inicia el sistema de partículas.
        particulasInstanciadas.Play();
    }

    //Método para controlar si se para una bola en dificultad progresiva, descontar tiempo en el timer
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
            spawner.modifyBallSpawn();
        }
    }
}