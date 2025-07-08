using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonController : MonoBehaviour
{
    public float speed = 0.0f;  // Velocidad de movimiento horizontal
    public float speed_rest = 0.0f;  // Resto de velocidades
    public float range = 0.0f;  // Distancia total que el objeto recorrerá de izquierda a derecha

    private Vector3 initialPosition;
    
    [HideInInspector]
    [System.NonSerialized]
    public ShootPositions shootPosition; 

    private void Start()
    {
        if (Constants_Handball.isActiveDesplazamientoCanon)
        {
            switch (shootPosition)
            {
                case ShootPositions.Central:
                    break;
                case ShootPositions.Pivote:
                    break;
                case ShootPositions.LateralDerecho:
                    transform.position = new Vector3(8.68f, 1.15f, 4.6f);
                    break;
                case ShootPositions.LateralIzquierdo:
                    transform.position = new Vector3(7.63f, 1.15f, -4.17f);
                    break;
                case ShootPositions.ExtremoIzquierdo:
                    transform.position = new Vector3(11.56f, 1.15f, -6.69f);
                    break;
                case ShootPositions.ExtremoDerecho:
                    transform.position = new Vector3(11.24f, 1.15f, 7.07f);
                    break;
            }
        }
        initialPosition = transform.position;
        speed = Random.Range(0.8f, 1.4f);
        speed_rest = Random.Range(0.8f, 1.0f);
        range = Random.Range(1.5f, 2.5f);
    }

    private void Update()
    {
        if (Constants_Handball.isActiveDesplazamientoCanon)
        {
            switch (shootPosition)
            {
                case ShootPositions.Central:
                    transform.position = new Vector3(transform.position.x, transform.position.y, initialPosition.z + Mathf.PingPong(Time.time * speed, range) - (range / 2));
                    //Debug.Log("Time:"+Time.time / speed + " Position: " + transform.position.z);
                    break;
                case ShootPositions.Pivote:
                    transform.position = new Vector3(transform.position.x, transform.position.y, initialPosition.z + Mathf.PingPong(Time.time * speed, range) - (range / 2));
                    break;
                case ShootPositions.LateralDerecho:
                    float time = Mathf.PingPong(Time.time * speed_rest, 1.0f);
                    transform.position = Vector3.Lerp(new Vector3(8.68f, 1.15f, 2.7f), new Vector3(7.51f, 1.15f, 5.94f), time);
                    break;
                case ShootPositions.LateralIzquierdo:
                    float time1 = Mathf.PingPong(Time.time * speed_rest, 1.0f);
                    transform.position = Vector3.Lerp(new Vector3(7.63f, 1.15f, -1.20f), new Vector3(6.81f, 1.15f, -4.00f), time1); 
                    break;
                case ShootPositions.ExtremoIzquierdo:
                    float time2 = Mathf.PingPong(Time.time * speed, range);
                    transform.position = Vector3.Lerp(new Vector3(11.56f, 1.15f, -6.50f), new Vector3(9.32f, 1.15f, -5.70f), time2); 
                    break;
                case ShootPositions.ExtremoDerecho:
                    float time3 = Mathf.PingPong(Time.time * speed, range);
                    transform.position = Vector3.Lerp(new Vector3(11.24f, 1.15f, 7.07f), new Vector3(13.51f, 1.15f, 8.37f), time3); 
                    break;
            }
        }
    }
}
