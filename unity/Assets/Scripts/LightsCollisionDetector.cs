using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LightsCollisionDetector : MonoBehaviour
{
    [SerializeField] private LightsModeController lightsModeController;
    //  [SerializeField] private GameObject textPrefab;
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private GameObject[] closerButtons;
    [SerializeField] protected ParticleSystem systemParticlesLight;
    private ParticleSystem particlesSystem;

    [SerializeField] protected Text touchedLights;

    protected AudioSource audioSource;
    [SerializeField] protected AudioClip touchedLightSound;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        touchedLights.text = GameData.n_luces.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        int lightID;

        GameObject[] lights = buttons;
        if (Constants_Handball.Difficulty == Difficulty.LightsReaction2)
        {
            lights = closerButtons;
        }

        if (other.CompareTag("LightSphere"))
        {
            Renderer renderer = other.GetComponent<MeshRenderer>();

            if (renderer.material.color == Color.green)
            {
                lightID = Array.IndexOf(lights, other.gameObject);
                UpdateReactionFile(lightID, "tocado");
                renderer.material.color = Color.red;

                ++GameData.n_luces;
                touchedLights.text = GameData.n_luces.ToString();

                ShowParticleEffect(other.gameObject);
                // ShowFloatingText(other.gameObject.transform);
                PlaySound();

                if (Constants_Handball.Difficulty == Difficulty.LightsReaction)
                {
                    StartCoroutine(WaitAndTurnOnLight());
                }
                else
                {
                    lightsModeController.turnOnOneLight();
                }
            }
        }
    }

    private void ShowParticleEffect(GameObject light)
    {
        particlesSystem = Instantiate(systemParticlesLight, light.transform.position, Quaternion.Euler(0, 90, 0));
        particlesSystem.Play();
    }

    /*  void ShowFloatingText(Transform t)
      {
          textPrefab.SetActive(true);
          Debug.Log("ShowFloatingText" + t.position);
          textPrefab.transform.position = t.position + new Vector3(0, 1, 0);
          Debug.Log("ShowFloatingText new" + textPrefab.transform.position);
          TMPro.TextMeshProUGUI textMeshPro = textPrefab.GetComponent<TMPro.TextMeshProUGUI>();
          if (textMeshPro == null)
          {
              Debug.LogError("TextMeshPro component is missing on textPrefab!");
              return;
          }
          textMeshPro.text = GameData.n_luces.ToString();

          StartCoroutine(MoveAndFadeText(textPrefab));
      }

      IEnumerator MoveAndFadeText(GameObject textObject)
      {
          float duration = 1f;
          float elapsed = 0f;
          Vector3 startPos = textObject.transform.position;
          Vector3 endPos = startPos + new Vector3(0, 1, 0);

          while (elapsed < duration)
          {
              elapsed += Time.deltaTime;
              float t = elapsed / duration;

              textObject.transform.position = Vector3.Lerp(startPos, endPos, t);

              yield return null;
          }

          textPrefab.SetActive(false);
      }
  */
    IEnumerator WaitAndTurnOnLight()
    {
        yield return new WaitForSeconds(1);
        lightsModeController.turnOnOneLight();
    }

    void PlaySound()
    {
        AudioSource.PlayClipAtPoint(touchedLightSound, transform.position, 0.5f);
    }

    public void UpdateReactionFile(int lightID, string status)
    {
        int frame = TimeManager.sharedFrame;
        float time = TimeManager.sharedTime;
        string dataLine = string.Join(";", new string[]
        {
            frame.ToString(),
            time.ToString("f4", CultureInfo.InvariantCulture),
            //time.ToString("f4").Replace(",", "."),
            lightID.ToString(),
            status,
        });

        HistoricalRegistry.tiemposReaccion.Add(dataLine);
    }

    public void ResetGameState()
    {
        GameData.t_serie = "";
        GameData.n_luces = 0;
        touchedLights.text = GameData.n_luces.ToString();
    }
}
