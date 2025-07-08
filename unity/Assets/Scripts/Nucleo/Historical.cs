using System;
using System.Collections.Generic;
using System.Numerics;

[Serializable]
public class Historical
{
    public string userId;
    public string game;
    public string date;
    //public int level;
    public float time;
    public int lanzamientoGenerados;
    public int paradas;
    public int n_luces;
    //num blocks moved with error
    public int goles;
    private string _separator = ";";
    private string _trackingFile ;
    //HMD initial position and rotation
    public UnityEngine.Vector3 _hmdPosition;
    public UnityEngine.Vector3   _hmdRotation;
    private Dictionary<string, string> gameOptions = new();

    private string _dataHeader =
        "USER;GAME;DATE;TIME;LANZAMIENTOS_GENERADOS;PARADAS;GOLES;N_LUCES;TRACKING_FILE";

    private string _dataHeaderDictionary;
    private string _dictionaryValue;
    public Historical(string game)
    {
        this.game = game;
        date = Constants_Handball.seriesDate.ToString("yyyy-MM-dd HH:mm:ss");;
        //level = 1;
        lanzamientoGenerados = 0;
        this._trackingFile = "";
        gameOptions = new Dictionary<string, string>();
       // _hmdPosition = new UnityEngine.Vector3();
        //_hmdRotation = new UnityEngine.Vector3();
    }

    public string Game
    {
        get => game;
        set => game = value;
    }

    public string Date
    {
        get => date;
        set => date = value;
    }

  /*  public int Level
    {
        get => level;
        set => level = value;
    }
*/
    public float Time
    {
        get => time;
        set => time = value;
    }


  
    public int Paradas
    {
        get => paradas;
        set => paradas = value;
    }

    public int Goles
    {
        get => goles;
        set => goles = value;
    }

    public string UserId
    {
        get => userId;
        set => userId = value;
    }

    public string TrackingFile
    {
        get => _trackingFile;
        set => _trackingFile = value;
    }

    public int LanzamientosGenerados
    {
        get => lanzamientoGenerados;
        set => lanzamientoGenerados = value;
    }

    public int N_Luces
    {
        get => n_luces;
        set => n_luces = value;
    }

    public override string ToString()

    {
        SetDictionaryHeaderAndValue();
        return userId + _separator + game + _separator + date + _separator
               + time.ToString().Replace(",", ".") + _separator  + lanzamientoGenerados.ToString()  + _separator 
               + paradas.ToString() + _separator 
               + goles.ToString() + _separator 
               + n_luces.ToString() + _separator
               + _trackingFile.ToString() //+ _separator
            /*  + _hmdPosition.x.ToString("f4").Replace(",", ".") + _separator
               + _hmdPosition.y.ToString("f4").Replace(",", ".") + _separator
               + _hmdPosition.z.ToString("f4").Replace(",", ".") + _separator
               + _hmdRotation.x.ToString("f4").Replace(",", ".") + _separator
               + _hmdRotation.y.ToString("f4").Replace(",", ".") + _separator
               + _hmdRotation.z.ToString("f4").Replace(",", ".") 
            */
               + _dictionaryValue ;
    }
    
    public void AddOrUpdateGameOption(string key, string value )
    {
        
        if (gameOptions != null)
        {
            if (gameOptions.ContainsKey(key))
            {
                //update
                gameOptions[key] = value;
            }
            else
            {
                //add new object position
                gameOptions.Add(key, value);
            }
        }
    }

    public void SetDictionaryHeaderAndValue()
    {
        _dataHeaderDictionary = "";
        _dictionaryValue = "";
        if (gameOptions != null)
        {
            foreach (KeyValuePair<string, string> kvp in gameOptions)
            {
                _dataHeaderDictionary += _separator + kvp.Key.ToUpper() ;
                _dictionaryValue += _separator + kvp.Value.ToString();
            }

        }
    }

    public String GetDataHeader()
    {
        SetDictionaryHeaderAndValue();
        return _dataHeader + _dataHeaderDictionary;
    }
}