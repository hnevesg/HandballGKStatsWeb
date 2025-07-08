using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;


public class HistoricalWritter
{
    private static readonly string _filename = "Historical.csv";




    private string _directoryName;
    private readonly string _filePath;


    public HistoricalWritter(string userPath)
    {
        _filePath = Path.Combine(userPath, _filename);
    }


    public void WriteHistorical(Historical historicalData)
    {
        bool exits = File.Exists(_filePath);
        //set game configuration options from dictionary to header and toString
        try
        {
            using StreamWriter file = new StreamWriter(_filePath, true, Encoding.UTF8);
            /*if (!exits)
            {
                //file.WriteLine(historicalData.GetDataHeader());
            }*/
            
            file.WriteLine(historicalData.GetDataHeader());
            file.WriteLine(historicalData + "\n");
        }
        catch (Exception e)
        {
            Debug.LogError("Error writting historical " + _filePath.ToString() + "\n" + e.Message);
        }
    }
}