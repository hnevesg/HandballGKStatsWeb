using System.Net.Sockets;
using System.IO;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;

public class CSV_TCPUploader : MonoBehaviour
{
    private string serverIP = "192.168.43.173"; 
    private int serverPort = 12345;

    [SerializeField] public SOGameConfiguration gameConfigSO;

    void Start()
    {
        Connect();
    }

    private void Connect()
    {
        Debug.Log("Inicio del proceso.");
        string directoryPath = gameConfigSO.userPath;
        string filePath = Path.Combine(directoryPath, "Historical.csv");

        try
        {
            using (TcpClient client = new TcpClient(serverIP, serverPort))
            using (NetworkStream stream = client.GetStream())
            {
                Debug.Log("Conexión establecida con el servidor.");
                //string message = "Hola, soy un mensaje desde Unity.";
                //byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
                string csvContent = File.ReadAllText(filePath);
                byte[] data = System.Text.Encoding.ASCII.GetBytes(csvContent);

                stream.Write(data, 0, data.Length);
                Debug.Log("Mensaje enviado al servidor.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error al conectar: " + ex.Message);
        }
        Debug.Log("Proceso finalizado.");
    }
}