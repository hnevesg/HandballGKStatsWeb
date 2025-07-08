using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebaController : MonoBehaviour
{
    [SerializeField] ParticleSystem sistemaParticulasParada;
    ParticleSystem particlesSystem;
    private GameUtilsManager gameUtilsManager;
    // Start is called before the first frame update
    void Start()
    {
        gameUtilsManager = FindObjectOfType<GameUtilsManager>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameUtilsManager == null)
        {
            gameUtilsManager = FindObjectOfType<GameUtilsManager>();
            gameUtilsManager.updateDebug(other.tag);
        }
        else
        {
            gameUtilsManager.updateDebug(other.tag);
        }
        particlesSystem = Instantiate(sistemaParticulasParada, new Vector3(12.07f, 1.15f, 1.7f), Quaternion.identity);
        // Inicia el sistema de part�culas.
        particlesSystem.Play();
    }
}
