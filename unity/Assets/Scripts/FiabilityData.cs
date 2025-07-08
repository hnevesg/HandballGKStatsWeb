using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;

public class FiabilityData : MonoBehaviour
{
    string _trackingPath;
    string _userPath;
    string _filePath;
    bool _setHead;
    // string _posicionesBolaStr; 
    //public Vector3 posicionRHand;
    //public Vector3 posicionLHand;
    List<string> posicionesBola = new List<string>();

    private static readonly string _filename = "BallTracking";
    private const string headerLine = "Frame;Tiempo;ID_Bola;Estatus;Posicion_Bola_x;Posicion_Bola_y;Posicion_Bola_z\n";

    List<string> rhandPositionData = new List<string>();
    List<string> lhandPositionData = new List<string>();

    public FiabilityData(string trackingPath, string userPath)
    {
        this._trackingPath = trackingPath;
        this._userPath = userPath;
    }

    private void SetUserPath()
    {
        if (_userPath == "")
        {
            var dirParent = Directory.GetParent(Application.persistentDataPath);
            if (dirParent != null)
            {
                var parentPath = dirParent.ToString();
                _userPath = Path.Combine(parentPath, RehabConstants.DirBbt, RehabConstants.DefaultUser);
            }
        }
    }

    public void SetPathWritter()
    {
        SetUserPath();
        _setHead = true;
        string trackingData = Path.Combine(_userPath, RehabConstants.DirCompareFiability);
        _filePath = Path.Combine(trackingData, _filename + Constants_Handball.seriesDate.ToString(RehabConstants.DateFormat)
                                                        + RehabConstants.ExtensionCsv);
        if (!Directory.Exists(trackingData))
        {
            try
            {
                //create new user path
                Directory.CreateDirectory(trackingData);
            }
            catch (Exception e)
            {
                Debug.LogError("Error creating user directory to save data tracking: " + e.Message);
            }
        }
    }

    List<string> extractRHandData()
    {
        using (var reader = new StreamReader(_trackingPath))
        {
            string headerLine = reader.ReadLine();
            string[] headers = headerLine.Split(';');

            int rHandPosXIndex = Array.IndexOf(headers, "RHandPosition_x");
            int rHandPosYIndex = Array.IndexOf(headers, "RHandPosition_y");
            int rHandPosZIndex = Array.IndexOf(headers, "RHandPosition_z");

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(';');

                string rHandPosition = $"({values[rHandPosXIndex].Replace(",", ".")},{values[rHandPosYIndex].Replace(",", ".")},{values[rHandPosZIndex].Replace(",", ".")})";
                rhandPositionData.Add($"{rHandPosition}");
            }
        }
        return rhandPositionData;
    }

    List<string> extractLHandData()
    {
        using (var reader = new StreamReader(_trackingPath))
        {
            string headerLine = reader.ReadLine();
            string[] headers = headerLine.Split(';');

            int lHandPosXIndex = Array.IndexOf(headers, "LHandPosition_x");
            int lHandPosYIndex = Array.IndexOf(headers, "LHandPosition_y");
            int lHandPosZIndex = Array.IndexOf(headers, "LHandPosition_z");

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(';');

                string lHandPosition = $"({values[lHandPosXIndex].Replace(",", ".")},{values[lHandPosYIndex].Replace(",", ".")},{values[lHandPosZIndex].Replace(",", ".")})";

                lhandPositionData.Add($"{lHandPosition}");
            }
        }
        return lhandPositionData;
    }

    public void WriteBallPositionData()
    {
        SetPathWritter();
        try
        {
            using StreamWriter file = new StreamWriter(_filePath, true, Encoding.UTF8);

            if (_setHead)
            {
                //write header first time
                file.Write(headerLine);
                _setHead = false;
            }

            /* 
              var manoDeLaParadaArray = HistoricalRegistry.manoDeLaParada.ToArray();

              file.Write(manoDeLaParadaStr + ";");
              file.Write(_posicionesBolaStr + ";");
            
            var manoDeLaParadaArray = HistoricalRegistry.manoDeLaParada.ToArray();
            var tiempoParadaOGolArray = HistoricalRegistry.tiempoParadaOGol.ToArray();
            */
            var posicionesBolaArray = HistoricalRegistry.posicionesBola.ToArray();
            string posicionesBolaStr = string.Join("\n", posicionesBolaArray);
            /* string manoDeLaParadaStr = string.Join(",", manoDeLaParadaArray);
             string posicionesRHandStr = string.Join(",", extractRHandData());
             string posicionesLHandStr = string.Join(",", extractLHandData());
             string tiempoParadaOGolStr = string.Join(",", tiempoParadaOGolArray);
            */

            file.WriteLine(posicionesBolaStr + "\n");
            /* file.Write("Mano_Derecha: ");
             file.WriteLine(posicionesRHandStr + ";");
             file.Write("Mano_Izquierda: ");
             file.WriteLine(posicionesLHandStr);
             file.Write("Mano_Parada: ");
             file.WriteLine(manoDeLaParadaStr);
             file.Write("Tiempo parada o gol: ");
             file.WriteLine(tiempoParadaOGolStr);*/
        }
        catch (IOException e)
        {
            Debug.LogError("Error writing ball position data: " + e.Message);
        }
    }
}