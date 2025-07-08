using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScrollControl : MonoBehaviour
{
    // Variables
    private float frequency = 0.5f;
    private float speed = 0.5f;
    // Thresholds
    private float minFrequency = 2.4f;
    private float maxFrequency = 3.8f;
    private float minSpeed = 3.0f;
    private float maxSpeed = 9.0f;
    // UI elements
    public Scrollbar frequencyScrollbar;
    public Scrollbar speedScrollbar;
    public TextMeshProUGUI valueTextFrequency;
    public TextMeshProUGUI valueTextSpeed;

    void Start()
    {
        valueTextFrequency.text = minFrequency.ToString("F2");
        valueTextSpeed.text = minSpeed.ToString("F2");

        frequencyScrollbar.onValueChanged.AddListener(UpdateFrequency);
        speedScrollbar.onValueChanged.AddListener(UpdateSpeed);
    }

    void UpdateFrequency(float value)
    {
        frequency = Mathf.Lerp(minFrequency, maxFrequency, value);
        UpdateUIValue(valueTextFrequency, frequency);
    }

    void UpdateSpeed(float value)
    {
        speed = Mathf.Lerp(minSpeed, maxSpeed, value);
        UpdateUIValue(valueTextSpeed, speed);
    }

    void UpdateUIValue(TextMeshProUGUI text, float value)
    {
        if (text != null)
        {
            text.text = value.ToString("F2");
        }
    }
}