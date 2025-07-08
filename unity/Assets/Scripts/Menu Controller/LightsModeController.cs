using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightsModeController : MonoBehaviour
{

    public CanvasController canvasController;
    [SerializeField] private Text MarcadorLuces;
    [SerializeField] SliderTimeProgressiveDifficulty sliderTimeProgressiveDifficulty;
    [SerializeField] GameObject lights;
    [SerializeField] GameObject[] buttons;
    [SerializeField] GameObject closerLights;
    [SerializeField] GameObject[] closerButtons;
    private LightsCollisionDetector lightsCollisionDetector;

    void Start()
    {
        lightsCollisionDetector = FindObjectOfType<LightsCollisionDetector>();
    }

    public void finishSeries()
    {
        lights.SetActive(false);
        closerLights.SetActive(false);
        ResetLights();
        MarcadorLuces.text = "0";
        canvasController.showMenuLights();
    }

    public int turnOnOneLight()
    {
        int lightID = Random.Range(0, buttons.Length);
        GameObject selectedLight = buttons[lightID];

        if (Constants_Handball.Difficulty == Difficulty.LightsReaction2)
        {
            selectedLight = closerButtons[lightID];
        }

        Renderer renderer = selectedLight.GetComponent<MeshRenderer>();
        renderer.material.color = Color.green;
        lightsCollisionDetector.UpdateReactionFile(lightID, "encendido");

        return lightID;
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
}
