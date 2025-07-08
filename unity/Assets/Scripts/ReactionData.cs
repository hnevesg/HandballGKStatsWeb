using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;

public class ReactionData : MonoBehaviour
{
    string _trackingPath;
    string _userPath;
    string _filePath;
    bool _setHead;

    private static readonly string _filename = "ReactionSpeed";
    private const string headerLine = "Frame;Tiempo;ID_Luz;Estatus;\n";

    public ReactionData(string trackingPath, string userPath)
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
        string reactionData = Path.Combine(_userPath, RehabConstants.DirReactionSpeed);
        _filePath = Path.Combine(reactionData, _filename + Constants_Handball.seriesDate.ToString(RehabConstants.DateFormat)
                                                        + RehabConstants.ExtensionCsv);
        if (!Directory.Exists(reactionData))
        {
            try
            {
                //create new user path
                Directory.CreateDirectory(reactionData);
            }
            catch (Exception e)
            {
                Debug.LogError("Error creating user directory to save reaction speed data: " + e.Message);
            }
        }
    }

    public void WriteData()
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

            var tiemposReaccionArray = HistoricalRegistry.tiemposReaccion.ToArray();
            string tiemposReaccionStr = string.Join("\n", tiemposReaccionArray);

            file.WriteLine(tiemposReaccionStr + "\n");
        }
        catch (IOException e)
        {
            Debug.LogError("Error writing reaction speed data: " + e.Message);
        }
    }
}