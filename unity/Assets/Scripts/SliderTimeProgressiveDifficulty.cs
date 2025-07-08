using UnityEngine;
using UnityEngine.UI;

public class SliderTimeProgressiveDifficulty : MonoBehaviour
{
    public Slider slider; // Referencia al objeto Slider
    public GameObject gameObjectRestantes;
    public Text labelTiempo;
    private float incrementRate = 0.4f; // Velocidad de incremento por segundo
    private Spawner spawner;
    [SerializeField] private LightsModeController lightsModeController;

    [HideInInspector]
    [System.NonSerialized]
    public bool isTimeEnd = false;
    private float tiempoActual = 0.0f; // El tiempo actual en segundos.

    private void Start()
    {
        // Inicializa el valor del Slider
        slider.value = slider.minValue;
        if (Constants_Handball.Difficulty == Difficulty.PerTime || Constants_Handball.Difficulty == Difficulty.LightsReaction || Constants_Handball.Difficulty == Difficulty.LightsReaction2)
        {
            labelTiempo.text = Constants_Handball.n_limit_time.ToString();
        }

        spawner = FindObjectOfType<Spawner>();
      //  lightsModeController = FindObjectOfType<LightsModeController>();
    }

    private void Update()
    {
        if (Constants_Handball.Difficulty == Difficulty.Progressive1)
        {
            // Incrementa el valor del Slider con el tiempo
            slider.value += incrementRate * Time.deltaTime;

            if (slider.value == slider.maxValue && !isTimeEnd)
            {
                isTimeEnd = true;
                finishProgressiveSerie();
            }
        }
        else if (Constants_Handball.Difficulty == Difficulty.Progressive2 || Constants_Handball.Difficulty == Difficulty.PerTime || Constants_Handball.Difficulty == Difficulty.LightsReaction || Constants_Handball.Difficulty == Difficulty.LightsReaction2)
        {
            updateTime();
        }
    }

    public void reduceTimeSliderWhenStopBall()
    {
        if (!isTimeEnd)
        {
            float newSliderValue = slider.value - 0.8f;
            if (GameData.n_lanzamientos >= 40)
            {
                newSliderValue = slider.value - 0.1f;
            }
            else if (GameData.n_lanzamientos >= 30)
            {
                newSliderValue = slider.value - 0.2f;
            }
            else if (GameData.n_lanzamientos >= 15)
            {
                newSliderValue = slider.value - 0.4f;
            }
            else
            {
                newSliderValue = slider.value - 0.8f;
            }

            if (newSliderValue <= 0.0f)
            {
                newSliderValue = 0.0f;
            }
            slider.value = newSliderValue;
        }
    }

    public void configureStateScoreboard()
    {

        if (Constants_Handball.Difficulty == Difficulty.Progressive1)
        {
            slider.gameObject.SetActive(true);
            gameObjectRestantes.SetActive(false);
            labelTiempo.gameObject.SetActive(false);
            slider.value = slider.minValue;
            isTimeEnd = false;
            tiempoActual = 0.0f;
        }
        else if (Constants_Handball.Difficulty == Difficulty.Progressive2)
        {
            slider.gameObject.SetActive(false);
            gameObjectRestantes.SetActive(false);
            labelTiempo.gameObject.SetActive(true);
            slider.value = slider.minValue;
            isTimeEnd = false;
            tiempoActual = 0.0f;
        }
        else if (Constants_Handball.Difficulty == Difficulty.PerTime)
        {
            slider.gameObject.SetActive(true);
            gameObjectRestantes.SetActive(false);
            labelTiempo.gameObject.SetActive(true);
            isTimeEnd = false;
            slider.value = slider.minValue;
            slider.maxValue = Constants_Handball.n_limit_time;
        }
        else if (Constants_Handball.Difficulty == Difficulty.LightsReaction || Constants_Handball.Difficulty == Difficulty.LightsReaction2)
        {
            slider.gameObject.SetActive(true);
            gameObjectRestantes.SetActive(false);
            labelTiempo.gameObject.SetActive(true);
            isTimeEnd = false;
            slider.value = slider.minValue;
            slider.maxValue = Constants_Handball.n_limit_time;
        }
        else
        {
            slider.gameObject.SetActive(false);
            gameObjectRestantes.SetActive(true);
            labelTiempo.gameObject.SetActive(false);
        }
    }

    private void finishProgressiveSerie()
    {
        if (spawner != null)
        {
            spawner.cancelSerieInProgressiveSerie();
        }
    }

    private void updateTime()
    {
        if (Constants_Handball.Difficulty == Difficulty.Progressive2)
        {
            HandleProgressive2Time();
        }
        else if (Constants_Handball.Difficulty == Difficulty.PerTime || Constants_Handball.Difficulty == Difficulty.LightsReaction || Constants_Handball.Difficulty == Difficulty.LightsReaction2)
        {
            HandlePerTimeAndLightsReaction();
        }
    }
    private void UpdateTimeDisplay(int minutos, int segundos)
    {
        // Formatea el tiempo en "mm:ss".
        GameData.t_serie = string.Format("{0:00}:{1:00}", minutos, segundos);
        // Actualiza el texto del Label.
        labelTiempo.text = GameData.t_serie;
    }

    private void HandleProgressive2Time()
    {
        tiempoActual += Time.deltaTime;

        int minutos = Mathf.FloorToInt(tiempoActual / 60);
        int segundos = Mathf.FloorToInt(tiempoActual % 60);

        UpdateTimeDisplay(minutos, segundos);
    }

    private void HandlePerTimeAndLightsReaction()
    {
        if (!isTimeEnd)
        {
            slider.value += Time.deltaTime;

            float remainingTime = slider.maxValue - slider.value;
            int minutos = Mathf.FloorToInt(remainingTime / 60);
            int segundos = Mathf.FloorToInt(remainingTime % 60);

            UpdateTimeDisplay(minutos, segundos);

            if (slider.value >= slider.maxValue && !isTimeEnd)
            {
                isTimeEnd = true;
                FinishSeries();
            }
        }
    }

    private void FinishSeries()
    {
        int minutos = Mathf.FloorToInt(slider.maxValue / 60);
        int segundos = Mathf.FloorToInt(slider.maxValue % 60);

        GameData.t_serie = string.Format("{0:00}:{1:00}", minutos, segundos);

        if (Constants_Handball.Difficulty == Difficulty.PerTime)
        {
            finishProgressiveSerie();
        }
        else
        {
            lightsModeController.finishSeries();
        }
    }
}