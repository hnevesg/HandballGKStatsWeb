using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUtilsManager : MonoBehaviour
{
    private Spawner spawner;
    [SerializeField] Text Paradas;
    [SerializeField] Text NoParadas;
    [SerializeField] Text Luces;
    [SerializeField] Text debug;
    private AudioSource audioSource;
    [SerializeField] AudioClip goalSound;
    [SerializeField] AudioClip stop1Sound;
    [SerializeField] AudioClip stop2Sound;
    [SerializeField] AudioClip stop3Sound;
    // Start is called before the first frame update
    void Start()
    {
        spawner = FindObjectOfType<Spawner>(); 
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void checkIfRemoveCanon(int indiceLanzamiento = -1)
    {
        if (!(Constants_Handball.Difficulty == Difficulty.Progressive1))
        {
            if (Constants_Handball.Difficulty == Difficulty.Progressive2 && Constants_Handball.deleteCanonFlag)
            {
                // Destruir todos los objetos en la matriz
                for (int i = 0; i < GameData.canon.Length; i++)
                {
                    Destroy(GameData.canon[i]);
                }
                Constants_Handball.deleteCanonFlag = false;
                GameData.posicionInicialGenerada = false;
            }
            //Es la ultima pelota lanzada en la serie
            else if (indiceLanzamiento <= 0)
            {
                // Destruir todos los objetos en la matriz
                for (int i = 0; i < GameData.canon.Length; i++)
                {
                    Destroy(GameData.canon[i]);
                }
                Constants_Handball.deleteCanonFlag = false;
                GameData.posicionInicialGenerada = false;
            }
        }
        else
        {
            if (Constants_Handball.Difficulty == Difficulty.Progressive1 && Constants_Handball.deleteCanonFlag)
            {
                // Destruir todos los objetos en la matriz
                for (int i = 0; i < GameData.canon.Length; i++)
                {
                    Destroy(GameData.canon[i]);
                }
                Constants_Handball.deleteCanonFlag = false;
                spawner.lastShootProgressive = false;
                GameData.posicionInicialGenerada = false;
            }
        }
    }

    public void updateParadasMarcador(string paradas)
    {
        Paradas.text = paradas;
    }
    public void updateGolesMarcador(string goles)
    {
        NoParadas.text = goles;
    }

    public void reinitValuesMarcador()
    {
        GameData.n_paradas = 0;
        GameData.n_goles = 0;
        GameData.n_luces = 0;
        GameData.n_lanzamientos = 0;
        GameData.t_serie = "";
        NoParadas.text = "0";
        Paradas.text = "0";
        Luces.text = "0";

        //Reinicar valores dificultad progresiva si es necesario
        Utils.reinitProgressiveValuesIfNeccesary();
    }

    public void updateDebug(string text)
    {
        //debug.text = text;
    }
    public void reproduceStopSound()
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

    public void reproduceGoalSound()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(goalSound);
        }
    }

}
