using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCreator : MonoBehaviour
{
    public GameObject ballLauncher;
    private float velocidadMovimiento;
    private float efectoLanzamiento;
    private int _ball_id;
    public int ball_id
    {
        get { return _ball_id; }
        set { _ball_id = value; }
    }
    private string _ball_status = "En movimiento";
    public string ball_status
    {
        get { return _ball_status; }
        set
        {
            _ball_status = value;
            OnBallStatusChanged();
        }
    }

    private Vector3 puntoInicial;
    private Vector3 puntoFinal;
    private float distanciaRecorrida = 0f;
    private float distanciaTotal;
    private bool moverHaciaFinal = true;

    [HideInInspector]
    [System.NonSerialized]
    public int indiceLanzamiento;
    public bool useFixedVelocity; // Configurable: Fixed or Dynamic Velocity


    private BodyPartCollisionDetector bodyCollisionScript;
    private SliderTimeProgressiveDifficulty sliderTimeProgressive;
    private GameUtilsManager gameUtilsManager;
    [SerializeField] ParticleSystem sistemaParticulasCanon;
    ParticleSystem particlesSystem;
    int canonChoosed = 0;
    private Spawner spawner;

    int final_place = -1; // For specific final points

    void Start()
    {
        //Generar posicion inicial/es del lanzador
        generarCanons();

        // Elegir punto de tiro
        elegirPuntoDeTiro();

        bodyCollisionScript = FindObjectOfType<BodyPartCollisionDetector>();
        sliderTimeProgressive = FindObjectOfType<SliderTimeProgressiveDifficulty>();
        spawner = FindObjectOfType<Spawner>();
        gameUtilsManager = FindObjectOfType<GameUtilsManager>();

        // Calcular la distancia total entre los puntos
        distanciaTotal = Vector3.Distance(puntoInicial, puntoFinal);

        // Calcular velocidad de disparo de la pelota si no se ha fijado
        if (!useFixedVelocity)
            velocidadMovimiento = Random.Range(Constants_Handball.velocidadMinima, Constants_Handball.velocidadMaxima);

        //Calcular efecto del disparo
        efectoLanzamiento = Random.Range(Constants_Handball.efectoLanzamientoMinimo, Constants_Handball.efectoLanzamientoMaximo);
    }

    void Update()
    {
        var currentFrame = TimeManager.sharedFrame;

      //  if (currentFrame % 5 == 0)
       //     saveBallPositionData(currentFrame, TimeManager.sharedTime, transform.position);

        // Si el objeto esta en movimiento
        if (moverHaciaFinal)
        {
            gameObject.SetActive(true);
            float speed = useFixedVelocity ? Constants_Handball.velocidadLanzamientoFija : velocidadMovimiento;
            // Calcular la distancia recorrida
            distanciaRecorrida += speed * Time.deltaTime;

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
                Vector3 newPosition = Vector3.Lerp(puntoInicial, puntoFinal, t);

                if (Constants_Handball.isActiveCurvaLanzamiento)
                {
                    // Aplicar una curva sinusoidal a la posición Y para crear un efecto
                    float yOffset = Mathf.Sin(t * Mathf.PI) * efectoLanzamiento;
                    newPosition.y += yOffset;
                }
                transform.position = newPosition;

                if (Constants_Handball.isActiveRotacionPelota)
                {
                    // Calcular el ángulo de rotación deseado 
                    float rotationAngle = t * 360f;
                    transform.rotation = Quaternion.Euler(0f, rotationAngle, 0f);
                }
            }
        }
        else
        {
            gameUtilsManager.checkIfRemoveCanon(indiceLanzamiento);

            // Actualizar el estatus de la pelota
            this.ball_status = "Gol";

            // Destruir objeto y actualizar marcador de pelota no parada
            StartCoroutine(DestroyAfterFrames(3));

            IEnumerator DestroyAfterFrames(int frameCount)
            {
                for (int i = 0; i < frameCount; i++)
                {
                    yield return null;
                }
                Destroy(gameObject);

                if (bodyCollisionScript != null)
                    bodyCollisionScript.noStoppedBall();
            }
        }

        if (particlesSystem != null)
        {
            particlesSystem.transform.position = GameData.canon[canonChoosed].transform.Find("firePosition").position;
        }
    }

    private void OnBallStatusChanged()
    {
        saveBallPositionData(TimeManager.sharedFrame, TimeManager.sharedTime, transform.position);
    }

    void elegirPuntoDeTiro()
    {
        canonChoosed = Random.Range(0, GameData.canon.Length);
        Transform innerObjectTransform = GameData.canon[canonChoosed].transform.Find("handballBall");
        Transform firePosition = GameData.canon[canonChoosed].transform.Find("firePosition");
        HistoricalRegistry.id_puntoInicialLanzamiento.Add(GameData.canon[canonChoosed].GetComponent<CanonController>().shootPosition.ToString());
        HistoricalRegistry.tiempoDisparo.Add((System.DateTime.Now - HistoricalRegistry.InicioSerie).TotalSeconds.ToString().Replace(",", "."));

        if (innerObjectTransform != null)
        {
            puntoInicial = innerObjectTransform.position;
            HistoricalRegistry.puntoInicialLanzamiento.Add(puntoInicial);
        }

        if (firePosition != null)
            MostrarSistemaParticulasLanzamiento(firePosition.position);

        if (final_place == -1)
            final_place = getFinalPlace();

        //Punto de tiro
        generateFinalPoint();

        StartCoroutine(RotateTowardsTarget(canonChoosed));
    }

    int getFinalPlace()
    {
        if (!useFixedVelocity) return Random.Range(0, Constants_Handball.HandballShootPositions.Length);

        List<int> activeCanons = new List<int>();

        if (Constants_Handball.EscuadraDerechaCanonActivated) activeCanons.Add(0);
        if (Constants_Handball.BajoDerechaCanonActivated) activeCanons.Add(1);
        if (Constants_Handball.CentroDerechaCanonActivated) activeCanons.Add(2);
        if (Constants_Handball.CentroDerechaBajoCanonActivated) activeCanons.Add(3);
        if (Constants_Handball.CentroCanonActivated) activeCanons.Add(4);
        if (Constants_Handball.CentroBajoCanonActivated) activeCanons.Add(5);
        if (Constants_Handball.CentroIzquierdaCanonActivated) activeCanons.Add(6);
        if (Constants_Handball.CentroIzquierdaBajoCanonActivated) activeCanons.Add(7);
        if (Constants_Handball.EscuadraIzquierdaCanonActivated) activeCanons.Add(8);
        if (Constants_Handball.BajoIzquierdaCanonActivated) activeCanons.Add(9);

        if (activeCanons.Count > 0)
        {
            return activeCanons[Random.Range(0, activeCanons.Count)];
        }
        return -1;
    }

    private void generateFinalPoint()
    {
        switch (Constants_Handball.HandballType)
        {
            case HandballType.Handball:
                puntoFinal = (Vector3)Constants_Handball.HandballShootPositions.GetValue(final_place);
                puntoFinal.y += Random.Range(-Constants_Handball.MaxVariationY, Constants_Handball.MaxVariationZ);
                puntoFinal.z += Random.Range(-Constants_Handball.MaxVariationY, Constants_Handball.MaxVariationZ);
                break;
            case HandballType.AdaptedHandball:
                puntoFinal = (Vector3)Constants_Handball.AdaptedHandballShootPositions.GetValue(final_place);
                puntoFinal.y += Random.Range(-Constants_Handball.MaxVariationYInAdaptedHandball, Constants_Handball.MaxVariationZInAdaptedHandball);
                puntoFinal.z += Random.Range(-Constants_Handball.MaxVariationYInAdaptedHandball, Constants_Handball.MaxVariationZInAdaptedHandball);
                break;
        }
        HistoricalRegistry.id_puntoFinalLanzamiento.Add(Constants_Handball.nameShootPositions.GetValue(final_place).ToString());
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
            int numCanons = CountCanons();
            GameData.canon = new GameObject[numCanons];
            InitializeCanons();
            orientateCanons();
            GameData.posicionInicialGenerada = !GameData.posicionInicialGenerada;
        }
    }

    private int CountCanons()
    {
        int numCanons = 0;
        if (Constants_Handball.centralCanonActivated) numCanons++;
        if (Constants_Handball.pivotCanonActivated) numCanons++;
        if (Constants_Handball.latIzqCanonActivated) numCanons++;
        if (Constants_Handball.latDerCanonActivated) numCanons++;
        if (Constants_Handball.extIzqCanonActivated) numCanons++;
        if (Constants_Handball.extDerCanonActivated) numCanons++;
        return numCanons;
    }

    private void InitializeCanons()
    {
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

    public void saveBallPositionData(int frame, float time, Vector3 posicionBola)
    {
        // Vector3 referencePoint = new Vector3(4.107f, 0.1f, -7.15f);
        // posicionBola -= referencePoint; 
        string dataLine = string.Join(";", new string[]
        {
            frame.ToString(),
            time.ToString("f4").Replace(",", "."),
            ball_id.ToString(),
            ball_status,
            posicionBola.x.ToString("f4").Replace(",", "."),
            posicionBola.y.ToString("f4").Replace(",", "."),
            posicionBola.z.ToString("f4").Replace(",", ".")
        });

        HistoricalRegistry.posicionesBola.Add(dataLine);
    }
}
